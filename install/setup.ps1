#!/usr/bin/env pwsh
# k8s-frontman installation script for Windows
#
# Usage: iwr -useb https://raw.githubusercontent.com/pmdevers/k8s-frontman/main/install/setup.ps1 | iex
#        or: .\setup.ps1 [-BinDir <path>] [-Version <version>] [-GitHubToken <token>]
#
# Parameters:
#   -BinDir       - Installation directory (default: $env:LOCALAPPDATA\k8s-frontman)
#   -Version      - Specific version to install (e.g., 1.0.0)
#   -GitHubToken  - GitHub token for private repo access (optional)
#

[CmdletBinding()]
param(
    [string]$BinDir = "$env:LOCALAPPDATA\k8s-frontman",
    [string]$Version = "",
    [string]$GitHubToken = ""
)

$ErrorActionPreference = 'Stop'
$ProgressPreference = 'SilentlyContinue'

$GitHubRepo = "pmdevers/k8s-frontman"
$TempDir = $null

# Helper functions for logs
function Write-Info {
    param([string]$Message)
    Write-Host "[INFO] $Message" -ForegroundColor Cyan
}

function Write-Warn {
    param([string]$Message)
    Write-Host "[WARN] $Message" -ForegroundColor Yellow
}

function Write-Fatal {
    param([string]$Message)
    Write-Host "[ERROR] $Message" -ForegroundColor Red
    exit 1
}

# Verify OS and architecture
function Test-SystemRequirements {
    Write-Info "Verifying system requirements"
    
    # Verify OS
    if (-not $IsWindows -and $PSVersionTable.PSVersion.Major -ge 6) {
        Write-Fatal "This script is for Windows. Use setup.sh for Unix-like systems."
    }
    
    # Get architecture
    $arch = $env:PROCESSOR_ARCHITECTURE
    switch ($arch) {
        "AMD64" { $script:Arch = "x64" }
        "ARM64" { $script:Arch = "arm64" }
        default { Write-Fatal "Unsupported architecture: $arch" }
    }
    
    $script:OS = "windows"
    Write-Info "Detected: Windows $script:Arch"
}

# Create temporary directory and setup cleanup
function Initialize-TempDirectory {
    $script:TempDir = Join-Path $env:TEMP "k8s-frontman-install-$(Get-Random)"
    New-Item -ItemType Directory -Path $script:TempDir -Force | Out-Null
    Write-Info "Created temporary directory: $script:TempDir"
}

# Cleanup temporary files
function Remove-TempDirectory {
    if ($script:TempDir -and (Test-Path $script:TempDir)) {
        Remove-Item -Path $script:TempDir -Recurse -Force -ErrorAction SilentlyContinue
        Write-Info "Cleaned up temporary directory"
    }
}

# Get release version from GitHub
function Get-ReleaseVersion {
    if ($Version) {
        $suffixUrl = "tags/v$Version"
        $script:Version = $Version
    } else {
        $suffixUrl = "latest"
    }
    
    $metadataUrl = "https://api.github.com/repos/$GitHubRepo/releases/$suffixUrl"
    Write-Info "Downloading metadata from $metadataUrl"
    
    try {
        $headers = @{
            'User-Agent' = 'k8s-frontman-installer'
        }
        
        if ($GitHubToken) {
            $headers['Authorization'] = "token $GitHubToken"
        }
        
        $response = Invoke-RestMethod -Uri $metadataUrl -Headers $headers
        $script:Version = $response.tag_name -replace '^v', ''
        
        if (-not $script:Version) {
            Write-Fatal "Unable to determine release version"
        }
        
        Write-Info "Using version $script:Version"
    } catch {
        Write-Fatal "Failed to fetch release metadata: $_"
    }
}

# Download file from URL
function Get-FileFromUrl {
    param(
        [string]$Url,
        [string]$OutputPath
    )
    
    try {
        $headers = @{
            'User-Agent' = 'k8s-frontman-installer'
        }
        
        if ($GitHubToken) {
            $headers['Authorization'] = "token $GitHubToken"
        }
        
        Invoke-WebRequest -Uri $Url -OutFile $OutputPath -Headers $headers
    } catch {
        Write-Fatal "Download failed from ${Url}: $_"
    }
}

# Download checksum file (contains checksums for all platforms)
function Get-Checksum {
    $hashUrl = "https://github.com/$GitHubRepo/releases/download/v$script:Version/k8s-frontman_${script:Version}_checksums.txt"
    $hashFile = Join-Path $script:TempDir "checksums.txt"
    
    Write-Info "Downloading checksums file from $hashUrl"
    Get-FileFromUrl -Url $hashUrl -OutputPath $hashFile
    
    # Extract expected hash for this platform from the checksums file
    $checksumContent = Get-Content $hashFile
    $binaryFileName = "k8s-frontman_${script:Version}_${script:OS}_${script:Arch}.exe"
    $expectedHashLine = $checksumContent | Where-Object { $_ -match [regex]::Escape($binaryFileName) }
    
    if (-not $expectedHashLine) {
        Write-Fatal "Checksum not found for $binaryFileName in checksums file"
    }
    
    $script:BinaryFileName = $binaryFileName
    $script:ExpectedHash = ($expectedHashLine -split '\s+')[0]
    Write-Info "Found checksum for $binaryFileName"
    Write-Info "Expected SHA256: $script:ExpectedHash"
}

# Download binary
function Get-Binary {
    $binUrl = "https://github.com/$GitHubRepo/releases/download/v$script:Version/$script:BinaryFileName"
    $script:BinaryPath = Join-Path $script:TempDir $script:BinaryFileName
    
    Write-Info "Downloading binary from $binUrl"
    Get-FileFromUrl -Url $binUrl -OutputPath $script:BinaryPath
}

# Verify downloaded binary checksum
function Test-BinaryChecksum {
    Write-Info "Verifying binary checksum"
    
    $actualHash = (Get-FileHash -Path $script:BinaryPath -Algorithm SHA256).Hash.ToLower()
    
    if ($actualHash -ne $script:ExpectedHash.ToLower()) {
        Write-Fatal "Checksum mismatch! Expected: $script:ExpectedHash, Got: $actualHash"
    }
    
    Write-Info "Checksum verified successfully"
}

# Install binary
function Install-Binary {
    Write-Info "Installing k8s-frontman to $BinDir"
    
    # Create installation directory if it doesn't exist
    if (-not (Test-Path $BinDir)) {
        New-Item -ItemType Directory -Path $BinDir -Force | Out-Null
    }
    
    # Install binary
    $targetPath = Join-Path $BinDir "k8s-frontman.exe"
    Copy-Item -Path $script:BinaryPath -Destination $targetPath -Force
    
    Write-Info "k8s-frontman installed successfully to $targetPath"
    
    # Add to PATH if not already present
    $userPath = [Environment]::GetEnvironmentVariable("Path", "User")
    if ($userPath -notlike "*$BinDir*") {
        Write-Info "Adding $BinDir to user PATH"
        [Environment]::SetEnvironmentVariable("Path", "$userPath;$BinDir", "User")
        Write-Warn "Please restart your terminal for PATH changes to take effect"
    }
    
    Write-Info "Installation complete! Run 'k8s-frontman --version' to verify."
}

# Main installation process
try {
    Write-Info "Starting k8s-frontman installation"
    
    Test-SystemRequirements
    Initialize-TempDirectory
    Get-ReleaseVersion
    Get-Checksum
    Get-Binary
    Test-BinaryChecksum
    Install-Binary
    
    Write-Info "Installation completed successfully!"
} catch {
    Write-Fatal "Installation failed: $_"
} finally {
    Remove-TempDirectory
}
