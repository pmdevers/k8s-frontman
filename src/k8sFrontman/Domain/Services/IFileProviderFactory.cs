using Microsoft.Extensions.FileProviders;

namespace k8s.Frontman.Domain.Services;

public interface IFileProviderFactory
{
    IFileProvider Create();
}
