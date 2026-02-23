using k8s.Frontman;
using k8s.Frontman.Features.Install;
using k8s.Frontman.Features.Providers;
using k8s.Frontman.Features.Releases;
using k8s.Operator;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(ko => {
    ko.AddServerHeader = false;
    ko.ListenAnyIP(Frontman.WebPort);
    ko.ListenAnyIP(Frontman.ManagmentPort);
});

builder.Services.AddResponseCaching();
builder.Services.AddResponseCompression();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Add(ReleaseJsonSerializerContext.Default);
});

builder.Services.AddOperator(x =>
{
    x.Name = "k8s-frontman";
    x.Container.Registry = "ghcr.io";
    x.Container.Image = x.Name;

    x.WithDeployment();
    x.WithService();
});

var app = builder.Build();

app.UseWhen(x => x.Connection.LocalPort == Frontman.WebPort, c =>
{
    app.UseResponseCaching();
    app.UseResponseCompression();
    c.UseMiddleware<ReleaseMiddleware>();
});

app.MapWhen(x => x.Connection.LocalPort == Frontman.ManagmentPort, c =>
{
    c.UseRouting();
    c.UseEndpoints(ep =>
    {
        ep.MapGet("/api/releases", GetReleases.Handle);
    });
});

app.MapProvider();
app.MapRelease();

await app.RunOperatorAsync();

