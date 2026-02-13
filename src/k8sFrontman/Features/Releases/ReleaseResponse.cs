namespace k8s.Frontman.Features.Releases;

public record ReleaseResponse(string Name, string Url, string CurrentVersion, string PreviousVersion);
