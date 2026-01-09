using Frontman.Domain;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Physical;
using Microsoft.Extensions.Primitives;

namespace Frontman.Features.Serve;

public class VersionedFileProvider : IFileProvider
{
    private readonly string _contentRoot;
    private readonly ReleaseService _releaseService;

    public VersionedFileProvider(string contentRoot, ReleaseService releaseService)
    {
        _contentRoot = contentRoot ?? throw new ArgumentNullException(nameof(contentRoot));
        _releaseService = releaseService ?? throw new ArgumentNullException(nameof(releaseService));
    }

    public IDirectoryContents GetDirectoryContents(string subpath)
    {
        return NotFoundDirectoryContents.Singleton;
    }

    public IFileInfo GetFileInfo(string subpath)
    {
        if (string.IsNullOrEmpty(subpath) || subpath == "/")
        {
            return new NotFoundFileInfo(subpath);
        }

        // Parse the path to extract the app segment
        var segments = subpath.TrimStart('/').Split('/', StringSplitOptions.RemoveEmptyEntries);
        
        if (segments.Length < 2)
        {
            return new NotFoundFileInfo(subpath);
        }

        var appSegment = segments[0];
        var relativePath = string.Join("/", segments.Skip(1));

        // Get the release information for the app segment
        var release = _releaseService.GetRelease(appSegment);
        
        if (release == null || string.IsNullOrEmpty(release.Version))
        {
            return new NotFoundFileInfo(subpath);
        }

        // Build the versioned path: /app1/v1.0.0/index.html
        var versionedPath = Path.Combine(_contentRoot, appSegment, release.Version, relativePath);
        
        if (File.Exists(versionedPath))
        {
            var fileInfo = new FileInfo(versionedPath);
            return new PhysicalFileInfo(fileInfo);
        }

        return new NotFoundFileInfo(subpath);
    }

    public IChangeToken Watch(string filter)
    {
        return NullChangeToken.Singleton;
    }
}
