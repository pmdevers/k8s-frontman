using k8s.Frontman.Domain.Entities;
using k8s.Frontman.Features.Providers;
using k8s.Frontman.Features.Releases;
using k8s.Operator;

namespace k8s.Frontman.Features;

public static class ApplicationFeatures
{
    extension<T>(T app)
        where T : IHost, IApplicationBuilder, IEndpointRouteBuilder
    {
        public void MapApi()
        {
            app.UseMiddleware<ReleaseMiddleware>();

            app.MapGet("/releases", GetReleases.Handle);


            app.AddReconciler<V1Release>(ReleaseReconciler.ReconcileAsync);
            app.AddReconciler<V1Provider>(ProviderReconciler.ReconcileAsync);
        }
    }
}
