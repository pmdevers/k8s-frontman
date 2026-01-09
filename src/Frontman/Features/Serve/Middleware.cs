using Frontman.Domain;
using Microsoft.AspNetCore.StaticFiles;

namespace Frontman.Features.Serve;

public static class ServeRelease
{
    extension(IApplicationBuilder app)
    {
        public IApplicationBuilder MapServe()
        {
            app.UseMiddleware<ReleaseMiddleware>();
            return app;
        }
    }
}

public class ReleaseMiddleware(RequestDelegate next, ReleaseService releaseService)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var segments = (context.Request.Path.Value ?? string.Empty)
            .Split('/', StringSplitOptions.RemoveEmptyEntries)
            .ToList();

        var releaseName = segments.FirstOrDefault() ?? string.Empty;
        var release = releaseService.FindByUrl(releaseName);

        if (release == null)
        {
            await next(context);
            return;
        }

        var newPath = string.Join('/', ["", release.Version, .. segments.Skip(1)]);

        var fileProvider = release.FileProvider;
        var fileInfo = fileProvider.GetFileInfo(newPath);

        if (!fileInfo.Exists)
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            await context.Response.WriteAsync("File not found");
            return;
        }

        var contentTypeProvider = new FileExtensionContentTypeProvider();
        if (!contentTypeProvider.TryGetContentType(fileInfo.Name, out var contentType))
        {
            contentType = "application/octet-stream";
        }
        context.Response.ContentType = contentType;
        context.Response.ContentLength = fileInfo.Length;

        using var stream = fileInfo.CreateReadStream();
        await stream.CopyToAsync(context.Response.Body);
    }
}

