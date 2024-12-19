using System;

namespace MangaReader.DomainCommon;

public class Url
{
    public static string Combine(string baseUrl, string path)
    {
        var tmp = new Uri(baseUrl);
        return new Uri(tmp, path).ToString();
    }
}