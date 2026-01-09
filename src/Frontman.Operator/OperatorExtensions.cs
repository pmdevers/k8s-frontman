using Frontman.Operator.Builder;
using Frontman.Operator.Commands;
using Frontman.Operator.Metadata;
using k8s;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;

namespace Frontman.Operator;

public static class OperatorExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddOperator(Action<OperatorBuilder>? configure = null)
        {
            services.AddSingleton(sp =>
            {
                var ds = new CommandDatasource(sp);

                ds.Add<HelpCommand>();
                ds.Add<OperatorCommand>();
                ds.Add<InstallCommand>();
                ds.Add<VersionCommand>();

                return ds;
            });

            services.AddSingleton(sp =>
            {
                var operatorName = Assembly.GetEntryAssembly()?.GetCustomAttribute<OperatorNameAttribute>()
                    ?? OperatorNameAttribute.Default;

                var dockerImage = Assembly.GetEntryAssembly()?.GetCustomAttribute<DockerImageAttribute>()
                    ?? DockerImageAttribute.Default;

                var ns = Assembly.GetEntryAssembly()?.GetCustomAttribute<NamespaceAttribute>()
                    ?? NamespaceAttribute.Default;

                return new EventWatcherDatasource(sp, [operatorName, dockerImage, ns]);
            });

            services.AddSingleton<IKubernetes>(x =>
            {
                KubernetesClientConfiguration config;

                if (KubernetesClientConfiguration.IsInCluster())
                {
                    config = KubernetesClientConfiguration.InClusterConfig();
                }
                else
                {
                    config = KubernetesClientConfiguration.BuildConfigFromConfigFile();
                }
                return new Kubernetes(config);
            });

            services.AddHostedService<OperatorService>();

            return services;
        }
    }
}

public static class WebApplicationExtensions
{
    extension(IApplicationBuilder app)
    {
        public ConventionBuilder<ControllerBuilder> MapController<T>()
            where T : IOperatorController
        {
            var datasource = app.ApplicationServices.GetRequiredService<EventWatcherDatasource>();
            return datasource.Add<T>();
        }
    }

    extension(IHost app)
    {

        public Task RunOperatorAsync()
        {
            var args = Environment.GetCommandLineArgs().Skip(1).ToArray();
            var commandDatasource = app.Services.GetRequiredService<CommandDatasource>();
            var command = commandDatasource.GetCommands(app)
                .FirstOrDefault(Filter)?.Command;

            bool Filter(CommandInfo command)
            {
                var arg = command.Metadata.OfType<OperatorArgumentAttribute>().First().Argument;
                return args.FirstOrDefault() == arg || args.FirstOrDefault() == $"--{arg}";
            }

            if (command == null)
            {
                return app.RunAsync();
            }

            return command.RunAsync(args);
        }
    }
}

public class OperatorBuilder
{

}
