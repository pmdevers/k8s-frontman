#!/usr/bin/env bash
# k8s-frontman installation script
#
# Usage: curl -fsSL https://raw.githubusercontent.com/pmdevers/k8s-frontman/main/install/setup.sh | bash
#        or: ./setup.sh [BIN_DIR]
#
# Environment variables:
#   VERSION - Specific version to install (e.g., VERSION=1.0.0)
#   GITHUB_TOKEN - GitHub token for private repo access (optional)
#   BIN_DIR - Installation directory (default: /usr/local/bin)
#
set -e

DEFAULT_BIN_DIR="/usr/local/bin"
BIN_DIR=${1:-"${DEFAULT_BIN_DIR}"}
GITHUB_REPO="pmdevers/k8s-frontman"

# Helper functions for logs
info() {
    echo '[INFO] ' "$@"
}

warn() {
    echo '[WARN] ' "$@" >&2
}

fatal() {
    echo '[ERROR] ' "$@" >&2
    exit 1
}

# Set os, fatal if operating system not supported
setup_verify_os() {
    if [[ -z "${OS}" ]]; then
        OS=$(uname)
    fi
    case ${OS} in
        Darwin)
            OS=darwin
            ;;
        Linux)
            OS=linux
            ;;
        *)
            fatal "Unsupported operating system ${OS}"
    esac
}

# Set arch, fatal if architecture not supported
setup_verify_arch() {
    if [[ -z "${ARCH}" ]]; then
        ARCH=$(uname -m)
    fi
    case ${ARCH} in
        arm|armv6l|armv7l)
            ARCH=arm
            ;;
        arm64|aarch64|armv8l)
            ARCH=arm64
            ;;
        amd64)
            ARCH=amd64
            ;;
        x86_64)
            ARCH=amd64
            ;;
        *)
            fatal "Unsupported architecture ${ARCH}"
    esac
}

# Verify existence of downloader executable
verify_downloader() {
    # Return failure if it doesn't exist or is no executable
    [[ -x "$(command -v "$1")" ]] || return 1

    # Set verified executable as our downloader program and return success
    DOWNLOADER=$1
    return 0
}

# Create temporary directory and cleanup when done
setup_tmp() {
    TMP_DIR=$(mktemp -d -t k8s-frontman-install.XXXXXXXXXX)
    TMP_METADATA="${TMP_DIR}/k8s-frontman.json"
    TMP_HASH="${TMP_DIR}/k8s-frontman.hash"
    TMP_BIN="${TMP_DIR}/k8s-frontman.tar.gz"
    cleanup() {
        local code=$?
        set +e
        trap - EXIT
        rm -rf "${TMP_DIR}"
        exit ${code}
    }
    trap cleanup INT EXIT
}

# Find version from Github metadata
get_release_version() {
    if [[ -n "${VERSION}" ]]; then
      SUFFIX_URL="tags/v${VERSION}"
    else
      SUFFIX_URL="latest"
    fi

    METADATA_URL="https://api.github.com/repos/${GITHUB_REPO}/releases/${SUFFIX_URL}"

    info "Downloading metadata ${METADATA_URL}"
    download "${TMP_METADATA}" "${METADATA_URL}"

    VERSION=$(grep '"tag_name":' "${TMP_METADATA}" | sed -E 's/.*"([^"]+)".*/\1/' | cut -c 2-)
    if [[ -n "${VERSION}" ]]; then
        info "Using ${VERSION} as release"
    else
        fatal "Unable to determine release version"
    fi
}

# Download from file from URL
download() {
    [[ $# -eq 2 ]] || fatal 'download needs exactly 2 arguments'

    case $DOWNLOADER in
        curl)
            if [[ -n "${GITHUB_TOKEN}" ]]; then
                curl -H "Authorization: token ${GITHUB_TOKEN}" -o "$1" -sfL "$2"
            else
                curl -o "$1" -sfL "$2"
            fi
            ;;
        wget)
            if [[ -n "${GITHUB_TOKEN}" ]]; then
                wget --header="Authorization: token ${GITHUB_TOKEN}" -qO "$1" "$2"
            else
                wget -qO "$1" "$2"
            fi
            ;;
        *)
            fatal "Incorrect executable '${DOWNLOADER}'"
            ;;
    esac

    # Abort if download command failed
    [[ $? -eq 0 ]] || fatal 'Download failed'
}

# Version comparison
# Returns 0 on '=', 1 on '>', and 2 on '<'.
# Ref: https://stackoverflow.com/a/4025065
vercomp () {
    if [[ $1 == $2 ]]
    then
        return 0
    fi
    local IFS=.
    local i ver1=($1) ver2=($2)
    # fill empty fields in ver1 with zeros
    for ((i=${#ver1[@]}; i<${#ver2[@]}; i++))
    do
        ver1[i]=0
    done
    for ((i=0; i<${#ver1[@]}; i++))
    do
        if [[ -z ${ver2[i]} ]]
        then
            # fill empty fields in ver2 with zeros
            ver2[i]=0
        fi
        if ((10#${ver1[i]} > 10#${ver2[i]}))
        then
            return 1
        fi
        if ((10#${ver1[i]} < 10#${ver2[i]}))
        then
            return 2
        fi
    done
    return 0
}

# Download hash from Github URL
download_hash() {
    HASH_URL="https://github.com/${GITHUB_REPO}/releases/download/v${VERSION}/k8s-frontman_${VERSION}_checksums.txt"
    info "Downloading hash ${HASH_URL}"
    download "${TMP_HASH}" "${HASH_URL}"
    HASH_EXPECTED=$(grep " k8s-frontman_${VERSION}_${OS}_${ARCH}.tar.gz$" "${TMP_HASH}")
    HASH_EXPECTED=${HASH_EXPECTED%%[[:blank:]]*}
}

# Download binary from Github URL
download_binary() {
    BIN_URL="https://github.com/${GITHUB_REPO}/releases/download/v${VERSION}/k8s-frontman_${VERSION}_${OS}_${ARCH}.tar.gz"
    info "Downloading binary ${BIN_URL}"
    download "${TMP_BIN}" "${BIN_URL}"
}

compute_sha256sum() {
  cmd=$(command -v sha256sum || command -v shasum)
  case $(basename "$cmd") in
    sha256sum)
      sha256sum "$1" | cut -f 1 -d ' '
      ;;
    shasum)
      shasum -a 256 "$1" | cut -f 1 -d ' '
      ;;
    *)
      fatal "Can not find sha256sum or shasum to compute checksum"
      ;;
  esac
}

# Verify downloaded binary hash
verify_binary() {
    info "Verifying binary download"
    HASH_BIN=$(compute_sha256sum "${TMP_BIN}")
    HASH_BIN=${HASH_BIN%%[[:blank:]]*}
    if [[ "${HASH_EXPECTED}" != "${HASH_BIN}" ]]; then
        fatal "Download sha256 does not match ${HASH_EXPECTED}, got ${HASH_BIN}"
    fi
}

# Setup permissions and move binary
setup_binary() {
    info "Installing k8s-frontman to ${BIN_DIR}/k8s-frontman"
    tar -xzf "${TMP_BIN}" -C "${TMP_DIR}"
    
    if [[ ! -f "${TMP_DIR}/k8s-frontman" ]]; then
        fatal "Failed to extract k8s-frontman binary from archive"
    fi
    
    chmod 755 "${TMP_DIR}/k8s-frontman"
    
    if [[ -w "${BIN_DIR}" ]]; then
        mv -f "${TMP_DIR}/k8s-frontman" "${BIN_DIR}"
    else
        sudo mv -f "${TMP_DIR}/k8s-frontman" "${BIN_DIR}"
    fi
}

# Run the install process
{
    setup_verify_os
    setup_verify_arch
    verify_downloader curl || verify_downloader wget || fatal 'Can not find curl or wget for downloading files'
    setup_tmp
    get_release_version
    download_hash
    download_binary
    verify_binary
    setup_binary
}