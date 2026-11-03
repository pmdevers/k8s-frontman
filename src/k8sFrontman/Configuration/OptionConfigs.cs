namespace k8s.Frontman.Configuration;

public static class OptionConfigs
{
    public static WebApplicationBuilder AddOptionConfigs(this WebApplicationBuilder builder, ILogger logger)
    {
        builder.Services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
        });

        logger.LogInformation("Configured JSON serialization options.");

        return builder;
    }
}
