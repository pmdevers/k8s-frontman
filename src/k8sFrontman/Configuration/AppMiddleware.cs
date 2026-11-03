using k8s.Frontman.Features;

namespace k8s.Frontman.Configuration;

public static class AppMiddleware
{
    extension(WebApplication app)
    {
        public WebApplication UseAppMiddleware()
        {
            app.UseResponseCaching();
            app.UseResponseCompression();

            app.MapApi();

            return app;
        }
    }
}
