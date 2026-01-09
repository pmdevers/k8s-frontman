using Microsoft.Extensions.FileProviders;

namespace Frontman.Domain;

public class Release(string name, string url, string version, IFileProvider fileProvider)
{
    public string Name { get; } = name;
    public string Url { get; } = url;
    public string Version { get; } = version;
    public IFileProvider FileProvider { get; } = fileProvider;
}
