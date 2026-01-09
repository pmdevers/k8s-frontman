using Azure.Storage.Blobs;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace Frontman.Domain.BlobStorage;

public class AzureBlobFileProvider : IFileProvider
{
    private readonly BlobContainerClient _containerClient;
    private readonly string _basePath;

    public AzureBlobFileProvider(string connectionString, string containerName, string basePath = "")
    {
        _containerClient = new BlobContainerClient(connectionString, containerName);
        _basePath = basePath.TrimStart('/').TrimEnd('/');
    }

    public IDirectoryContents GetDirectoryContents(string subpath)
    {
        var normalizedPath = NormalizePath(subpath);
        return new AzureBlobDirectoryContents(_containerClient, normalizedPath);
    }

    public IFileInfo GetFileInfo(string subpath)
    {
        var normalizedPath = NormalizePath(subpath);
        var blobClient = _containerClient.GetBlobClient(normalizedPath);

        return new AzureBlobFileInfo(blobClient, normalizedPath);
    }

    public IChangeToken Watch(string filter)
    {
        // Azure Blob Storage doesn't support change notifications
        // Return a token that never changes
        return NullChangeToken.Singleton;
    }

    private string NormalizePath(string subpath)
    {
        var normalized = subpath.TrimStart('/');

        if (!string.IsNullOrEmpty(_basePath))
        {
            normalized = string.IsNullOrEmpty(normalized)
                ? _basePath
                : $"{_basePath}/{normalized}";
        }

        return normalized;
    }
}
