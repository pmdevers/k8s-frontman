using Frontman.Domain;

namespace Frontman.Features.Serve;

public static class ServeRelease
{
    public static IApplicationBuilder UseVersionedStaticFiles(this IApplicationBuilder app, string contentRoot)
    {
        var releaseService = app.ApplicationServices.GetRequiredService<ReleaseService>();

        app.

        return app;
    }
}
