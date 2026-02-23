using k8s.Operator;

namespace k8s.Frontman.Features.Releases;

public static class ReleaseFeature
{
    extension<T>(T app)
        where T : IHost
    {
        public void MapRelease()
        {
            app.AddReconciler<V1Release>(ReleaseReconciler.ReconcileAsync);
        }
    }
}
