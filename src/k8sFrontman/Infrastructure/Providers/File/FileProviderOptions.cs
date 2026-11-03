using k8s.Frontman.Domain.Services;
using Microsoft.Extensions.FileProviders;

namespace k8s.Frontman.Infrastructure.Providers.File;

public class FileProviderOptions : IFileProviderFactory
{
    public string Path { get; set; } = string.Empty;

    public IFileProvider Create()
        => new PhysicalFileProvider(Path);
}
