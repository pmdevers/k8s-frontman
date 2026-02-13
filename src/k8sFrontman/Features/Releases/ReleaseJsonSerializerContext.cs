using System.Text.Json.Serialization;

namespace k8s.Frontman.Features.Releases;

[JsonSerializable(typeof(List<ReleaseResponse>))]
[JsonSerializable(typeof(ReleaseResponse))]
internal partial class ReleaseJsonSerializerContext : JsonSerializerContext
{
}
