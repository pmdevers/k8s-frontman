# k8s-frontman

Manager for hosting versioned static content within Kubernetes

![License](https://img.shields.io/github/license/pmdevers/k8s-frontman)
![Release](https://img.shields.io/github/v/release/pmdevers/k8s-frontman)

## Overview

k8s-frontman is a Kubernetes operator that simplifies the deployment and management of versioned static content. It provides a declarative approach to hosting and serving static files from multiple sources, with built-in support for various storage providers.

## Features

- 🚀 **Kubernetes Native** - Deploy as a native Kubernetes operator
- 📦 **Multiple Storage Providers** - Support for Azure Blob Storage, File System, and more
- 🔄 **Version Management** - Manage multiple versions of your static content
- 🎯 **Declarative Configuration** - Define your content using Kubernetes Custom Resources
- ⚡ **High Performance** - Built-in response caching and compression
- 🐳 **Container Ready** - Available as a Docker image on GitHub Container Registry

## Quick Start

### Prerequisites

- Kubernetes cluster (v1.20+)
- kubectl configured to communicate with your cluster

### Installation

**Using kubectl:**
```bash
kubectl apply -f https://github.com/pmdevers/k8s-frontman/releases/latest/download/install.yaml
```

**Using Helm (if available):**
```bash
helm repo add k8s-frontman https://pmdevers.github.io/k8s-frontman
helm install k8s-frontman k8s-frontman/k8s-frontman
```

### CLI Installation

For local development and management, install the k8s-frontman CLI:

**Linux/macOS:**
```bash
curl -s https://raw.githubusercontent.com/pmdevers/k8s-frontman/main/install/setup.sh | sudo bash
```

**Windows (PowerShell):**
```powershell
iwr -useb https://raw.githubusercontent.com/pmdevers/k8s-frontman/main/install/setup.ps1 | iex
```

For detailed installation options, see the [install README](install/README.md).

## Usage

### Defining a Provider

Create a provider to specify where your static content is stored:

```yaml
apiVersion: frontman.k8s.io/v1
kind: Provider
metadata:
  name: my-static-content
spec:
  type: azureBlob
  azureBlob:
    connectionString: <your-connection-string>
    containerName: static-files
```

### Creating a Release

Create a release to serve specific versions of your content:

```yaml
apiVersion: frontman.k8s.io/v1
kind: Release
metadata:
  name: my-app-v1
spec:
  provider: my-static-content
  version: "1.0.0"
  path: /app
```

### Accessing Your Content

Once deployed, your static content will be available at the configured path through the k8s-frontman service.

## Configuration

Configuration is done through Kubernetes Custom Resources. For detailed configuration options, see the [documentation](docs/introduction.md).

### Supported Providers

- **Azure Blob Storage** - Store content in Azure Blob Storage
- **File System** - Use local file system storage
- More providers coming soon!

## Development

### Building from Source

```bash
# Clone the repository
git clone https://github.com/pmdevers/k8s-frontman.git
cd k8s-frontman

# Build the project
dotnet build

# Run locally
dotnet run --project src/k8sFrontman
```

### Running Tests

```bash
dotnet test
```

## Documentation

For more information, see:

- [Getting Started Guide](docs/getting-started.md)
- [Introduction](docs/introduction.md)
- [Installation Guide](install/README.md)

## Contributing

Contributions are welcome! Please see our [Code of Conduct](CODE_OF_CONDUCT.md) for details.

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Acknowledgments

Built with:
- [.NET 10.0](https://dotnet.microsoft.com/)
- [k8sOperator](https://github.com/pmdevers/k8sOperator) - Kubernetes Operator framework for .NET

---

Made with ❤️ by [Patrick Evers](https://github.com/pmdevers)
