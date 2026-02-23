using k8s.Frontman.Features.Providers;
using k8s.Frontman.Features.Releases;
using k8s.Models;
using k8s.Operator.Configuration;
using k8s.Operator.Generation;
using System.Collections.Frozen;

namespace k8s.Frontman.Features.Install;

public static class InstallFeature
{
    extension(OperatorConfiguration config)
    {
        public OperatorConfiguration WithDeployment()
        {
            config.Install.Resources.Add(typeof(V1Provider));
            config.Install.Resources.Add(typeof(V1Release));

            config.Install.ConfigureDeployment = deployment =>
            {
                deployment.Spec.Template.Spec.Containers[0].Ports ??= [];
                deployment.Spec.Template.Spec.Containers[0].Ports.Add(new()
                {
                    ContainerPort = Frontman.WebPort,
                    Name = nameof(Frontman.WebPort).ToLowerInvariant(),
                });
                deployment.Spec.Template.Spec.Containers[0].Ports.Add(new()
                {
                    ContainerPort = Frontman.ManagmentPort,
                    Name = nameof(Frontman.ManagmentPort).ToLowerInvariant(),
                });
            };
            return config;
        }

        public OperatorConfiguration WithService()
        {
            config.Install.AdditionalObjects.Add(
                KubernetesObjectBuilder.Create<V1Service>()
                    .WithName(config.Name)
                    .WithLabel("operator", config.Name)
                    .WithSpec(s =>
                    {
                        s.WithSelector("operator", config.Name);
                        s.WithPort(nameof(Frontman.WebPort).ToLowerInvariant(), Frontman.WebPort, Frontman.WebPort);
                        s.WithPort(nameof(Frontman.ManagmentPort).ToLowerInvariant(), Frontman.ManagmentPort, Frontman.ManagmentPort);
                    }).Build()
            );

            return config;
        }
    }
}
