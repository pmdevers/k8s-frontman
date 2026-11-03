using k8s.Frontman.Domain.Entities;
using k8s.Operator.Generation;
using k8s.Operator.Reconciler;

namespace k8s.Frontman.Features.Releases;

public static class ReleaseReconciler
{
    public static Task<IReconcileResult> ReconcileAsync(ReconcileContext context)
    {
        var resource = context.GetResource<V1Release>();
        var informer = context.GetInformer<V1Release>();
        var providers = context.GetInformer<V1Provider>();

        var current = informer.Indexer.Get(resource);
        var provider = providers.Indexer.Get(resource.Spec.Provider, resource.Metadata.NamespaceProperty);

        if (provider is null)
        {
            return Task.FromResult(ReconcileResult.Success<V1Release>(x =>
            {
                UpdateLabels(x, context.Configuration.Name, resource.Spec.Provider);
                x.WithStatus(x =>
                {
                    x.Message = $"Provider '{resource.Spec.Provider}' not found.";
                });
            }));
        }

        if (!provider.Status!.Versions.Contains(resource.Spec.Version))
        {
            return Task.FromResult(ReconcileResult.Requeue<V1Release>(TimeSpan.FromSeconds(30), x =>
            {
                UpdateLabels(x, context.Configuration.Name, resource.Spec.Provider);
                x.WithStatus(x =>
                {
                    x.Message = $"Version '{resource.Spec.Version}' not found.";
                    if (current?.Status?.CurrentVersion != resource.Spec.Version)
                    {
                        x.PreviousVersion = current?.Status?.CurrentVersion ?? string.Empty;
                    }
                });
            }));
        }

        return Task.FromResult(ReconcileResult.Success<V1Release>(x =>
        {
            x.WithStatus(x =>
            {
                x.Message = string.Empty;
                x.CurrentVersion = resource.Spec.Version;
                if (current?.Status?.CurrentVersion != resource.Spec.Version)
                {
                    x.PreviousVersion = current?.Status?.CurrentVersion ?? string.Empty;
                }
            });
        }));
    }

    private static void UpdateLabels(IObjectBuilder<V1Release> x, string name, string provider)
    {
        x.WithLabel("managed-by", name);
        x.WithLabel("processed", "true");
        x.WithLabel($"{V1Release.KubeGroup}/provider", provider);
    }
}
