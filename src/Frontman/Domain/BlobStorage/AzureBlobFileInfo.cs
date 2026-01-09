using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.FileProviders;

namespace Frontman.Domain.BlobStorage;

internal class AzureBlobFileInfo(BlobClient blobClient, string name) : IFileInfo
{
    private readonly string _name = Path.GetFileName(name);
    private BlobProperties? _properties;

    public bool Exists
    {
        get
        {
            try
            {
                EnsureProperties();
                return _properties != null;
            }
            catch
            {
                return false;
            }
        }
    }

    public long Length
    {
        get
        {
            EnsureProperties();
            return _properties?.ContentLength ?? -1;
        }
    }

    public string? PhysicalPath => null;

    public string Name => _name;

    public DateTimeOffset LastModified
    {
        get
        {
            EnsureProperties();
            return _properties?.LastModified ?? DateTimeOffset.MinValue;
        }
    }

    public bool IsDirectory => false;

    public Stream CreateReadStream()
    {
        if (!Exists)
        {
            throw new FileNotFoundException($"Blob '{blobClient.Name}' not found.");
        }

        return blobClient.OpenRead();
    }

    private void EnsureProperties()
    {
        if (_properties == null)
        {
            try
            {
                _properties = blobClient.GetProperties().Value;
            }
            catch (Azure.RequestFailedException ex) when (ex.Status == 404)
            {
                _properties = null;
            }
        }
    }
}
