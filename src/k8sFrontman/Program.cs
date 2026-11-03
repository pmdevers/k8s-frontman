using k8s.Frontman.Configuration;
using k8s.Frontman.Infrastructure;
using k8s.Operator;

var builder = WebApplication.CreateBuilder(args);

builder
    .AddLoggerConfigs();

using var loggerFactory = LoggerFactory.Create(config => config.AddConsole());
var startupLogger = loggerFactory.CreateLogger<Program>();

builder
    .AddOptionConfigs(startupLogger)
    .AddServiceConfigs(startupLogger)
    .AddInfrastructure(startupLogger);

var app = builder.Build();

app.UseAppMiddleware();

await app.RunOperatorAsync();

