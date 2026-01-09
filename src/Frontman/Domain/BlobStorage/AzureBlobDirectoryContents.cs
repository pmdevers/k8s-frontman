using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Frontman.Features.Releases;
using Frontman.Operator;
using Microsoft.Extensions.FileProviders;

namespace Frontman.Domain.BlobStorage;

internal class AzureBlobDirectoryContents(BlobContainerClient containerClient, string prefix) : IDirectoryContents
{
    private readonly string _prefix = prefix.TrimEnd('/');

    public bool Exists
    {
        get
        {
            try
            {
                var options = new GetBlobsOptions() { Prefix = _prefix };
                var blobs = containerClient.GetBlobs(options).Take(1);
                return blobs.Any();
            }
            catch
            {
                return false;
            }
        }
    }

    public IEnumerator<IFileInfo> GetEnumerator()
    {
        var prefix = string.IsNullOrEmpty(_prefix) ? "" : $"{_prefix}/";
        var options = new GetBlobsOptions() { Prefix = _prefix };

        var blobs = containerClient.GetBlobs(options);

        foreach (var blobItem in blobs)
        {
            var blobClient = containerClient.GetBlobClient(blobItem.Name);
            yield return new AzureBlobFileInfo(blobClient, blobItem.Name);
        }
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
