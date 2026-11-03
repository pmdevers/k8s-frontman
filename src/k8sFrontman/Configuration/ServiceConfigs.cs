using k8s.Frontman.Domain.Entities;
using k8s.Operator;

namespace k8s.Frontman.Configuration;

public static class ServiceConfigs
{
    public static WebApplicationBuilder AddServiceConfigs(this WebApplicationBuilder builder, ILogger logger)
    {
        builder.Services.AddResponseCaching();
        builder.Services.AddResponseCompression();

        builder.Services.AddOperator(x =>
        {
            x.Namespace = "frontman-system";
            x.JsonSerializerContext = AppJsonSerializerContext.Default;

            x.WithDeployment();
            x.WithService();

            x.Install.Resources.Add(typeof(V1Provider));
            x.Install.Resources.Add(typeof(V1Release));
        });

        logger.LogInformation("Registered service configuration.");

        return builder;
    }
}
