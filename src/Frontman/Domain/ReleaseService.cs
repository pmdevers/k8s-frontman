namespace Frontman.Domain;

public class ReleaseService
{
    private readonly Dictionary<string, Release> _releases = [];

    public Release? FindByName(string name)
    {
        return _releases.Values.FirstOrDefault(x => x.Name == name);
    }


    public Release? FindByUrl(string url)
    {
        return _releases.TryGetValue(url, out var release) ? release : null;
    }
    public void Add(Release release)
    {
        _releases[release.Url] = release;
    }

    public void Remove(string url)
    {
        _releases.Remove(url);
    }
}
