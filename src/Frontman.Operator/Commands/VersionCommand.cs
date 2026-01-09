using Frontman.Operator.Builder;
using Frontman.Operator.Generation;
using Frontman.Operator.Metadata;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Frontman.Operator.Commands;

[OperatorArgument("version", Description = "Shows the version", Order = 998)]
internal class VersionCommand(IHost app) : IOperatorCommand
{
    public Task RunAsync(string[] args)
    {
        var watcher = app.Services.GetRequiredService<EventWatcherDatasource>();
        var name = watcher.Metadata.TryGetValue<OperatorNameAttribute, string>(x => x.OperatorName);
        var version = watcher.Metadata.TryGetValue<DockerImageAttribute, string>(x => x.Tag);

        Console.WriteLine($"{name} version {version}.");

        return Task.CompletedTask;
    }
}
