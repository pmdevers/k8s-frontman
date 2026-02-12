#k8s-frontman cli installation

Binaries for macOS and Linux AMD64 are available for download on the 
[release page](https://github.com/pmdevers/k8s-frontman/releases).

To install the latest release run:

```bash
curl -s https://raw.githubusercontent.com/pmdevers/k8s-frontman/main/install/setup.sh | sudo bash
```

The install script does the following:
* attempts to detect your OS
* downloads and unpacks the release tar file in a temporary directory
* copies the k8s-frontman binary to `/usr/local/bin`
* removes the temporary directory