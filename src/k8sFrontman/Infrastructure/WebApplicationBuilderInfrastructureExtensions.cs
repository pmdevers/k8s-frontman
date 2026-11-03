namespace k8s.Frontman.Infrastructure;

public static class WebApplicationBuilderInfrastructureExtensions
{
    public static WebApplicationBuilder AddInfrastructure(this WebApplicationBuilder builder, ILogger logger)
    {
        logger.LogInformation("Infrastructure registration completed.");
        return builder;
    }
}
