using k8s.Frontman.Domain.Entities;
using k8s.Operator.Generation;
using k8s.Operator.Reconciler;

namespace k8s.Frontman.Features.Providers;

public static partial class ProviderReconciler
{
    public static async Task<IReconcileResult> ReconcileAsync(ReconcileContext context)
    {
        var resource = context.GetResource<V1Provider>();
        var fileprovider = resource.GetFileProvider();

        var delay = ResyncIntervalAttribute.ParseDuration(resource.Spec.Interval);

        if (fileprovider is not null)
        {
            var dirs = fileprovider.GetDirectoryContents("")
                .Where(x => x.IsDirectory)
                .Select(x => x.Name).ToList();

            return ReconcileResult.Requeue<V1Provider>(delay, x =>
            {
                x.WithLabel("managed-by", context.Configuration.Name);
                x.WithLabel("processed", "true");
                x.WithStatus(x =>
                {
                    x.NumberOfReleases = dirs.Count;
                    x.Versions = [.. dirs.TakeLast(10)];
                });
            });
        }


        return ReconcileResult.Requeue<V1Provider>(delay);
    }
}
