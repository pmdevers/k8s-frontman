using Frontman.Operator.Helpers;

namespace Frontman.Operator.Metadata;

[AttributeUsage(AttributeTargets.Assembly)]
public class DockerImageAttribute(string registery, string repository, string tag) : Attribute
{
    public static DockerImageAttribute Default => new("ghcr.io", "operator/operator", "latest");

    public string Registery { get; set; } = registery;
    public string Repository { get; set; } = repository;
    public string Tag { get; set; } = tag;
    public string GetImage() => $"{Registery}/{Repository}:{Tag}";

    public override string ToString()
        => DebuggerHelpers.GetDebugText("DockerImage", GetImage());
}
