# k8s-frontman CLI Installation

Binaries for Windows, macOS, and Linux AMD64 are available for download on the 
[release page](https://github.com/pmdevers/k8s-frontman/releases).

## Linux and macOS Installation

To install the latest release run:

```bash
curl -s https://raw.githubusercontent.com/pmdevers/k8s-frontman/main/install/setup.sh | sudo bash
```

The install script does the following:
* attempts to detect your OS
* downloads and unpacks the release tar file in a temporary directory
* copies the k8s-frontman binary to `/usr/local/bin`
* removes the temporary directory

## Windows Installation

To install the latest release on Windows, run in PowerShell:

```powershell
iwr -useb https://raw.githubusercontent.com/pmdevers/k8s-frontman/main/install/setup.ps1 | iex
```

Or download and run the script manually:

```powershell
.\setup.ps1
```

The PowerShell install script does the following:
* detects your Windows architecture (AMD64 or ARM64)
* downloads and unpacks the release archive in a temporary directory
* copies the k8s-frontman binary to `%LOCALAPPDATA%\k8s-frontman`
* adds the installation directory to your user PATH
* removes the temporary directory

### Optional Parameters

Both scripts support optional parameters:

**Bash (Linux/macOS):**
```bash
# Install specific version
VERSION=1.0.0 ./setup.sh

# Install to custom directory
./setup.sh /custom/path

# Use GitHub token for private repo
GITHUB_TOKEN=your_token ./setup.sh
```

**PowerShell (Windows):**
```powershell
# Install specific version
.\setup.ps1 -Version 1.0.0

# Install to custom directory
.\setup.ps1 -BinDir "C:\custom\path"

# Use GitHub token for private repo
.\setup.ps1 -GitHubToken "your_token"
```
