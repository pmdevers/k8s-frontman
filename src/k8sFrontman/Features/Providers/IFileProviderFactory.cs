using Microsoft.Extensions.FileProviders;

namespace k8s.Frontman.Features.Providers;

public interface IFileProviderFactory
{
    IFileProvider Create();
}
