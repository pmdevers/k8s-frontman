using k8s.Operator;

namespace k8s.Frontman.Features.Providers;

public static class ProviderFeature
{
    extension<T>(T app)
        where T : IHost, IApplicationBuilder
    {
        public void MapProvider()
        {
            app.ReconcilerFor<Provider>(ProviderReconciler.ReconcileAsync);
        }
    }
}
