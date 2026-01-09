using Frontman.Domain;
using Frontman.Features.Releases;
using Frontman.Features.Serve;
using Frontman.Operator;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddOperator();

builder.Services.AddSingleton<ReleaseService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapReleases();
app.MapServe();

await app.RunOperatorAsync();
