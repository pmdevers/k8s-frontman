using k8s.Frontman.Features.Install;
using k8s.Frontman.Features.Providers;
using k8s.Frontman.Features.Releases;
using k8s.Operator;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddResponseCaching();
builder.Services.AddResponseCompression();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Add(ReleaseJsonSerializerContext.Default);
});

builder.WebHost.ConfigureKestrel(options =>
{
    options.AddServerHeader = false;
});

builder.Services.AddOperator(x =>
{
    x.Name = "k8s-frontman";
    x.Container.Registry = "ghcr.io";

    x.WithDeployment();
    x.WithService();
});

var app = builder.Build();

app.UseResponseCaching();
app.UseResponseCompression();

app.MapProvider();
app.MapRelease();

await app.RunOperatorAsync();

