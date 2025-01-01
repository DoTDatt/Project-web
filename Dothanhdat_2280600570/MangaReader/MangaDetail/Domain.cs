using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using MangaReader.DomainCommon;

namespace MangaReader.MangaDetail;

public class Chapter
{
    public string Title { get; }
    public string Url { get; }

    public Chapter(string title, string url)
    {
        Title = title;
        Url = url;
    }
}

public class Manga
{
    public string Title { get; }
    public string Description { get; }
    public string CoverUrl { get; }
    public List<Chapter> Chapters { get; }

    public Manga(string title, string description, string coverUrl, List<Chapter> chapters)
    {
        Title = title;
        Description = description;
        CoverUrl = coverUrl;
        Chapters = chapters;
    }
}

public class Domain
{
    private readonly string mangaUrl;
    private readonly Http http;

    public Domain(string mangaUrl, Http http)
    {
        this.mangaUrl = mangaUrl;
        this.http = http;
    }

    public async Task<Manga> LoadManga(CancellationToken token)
    {
        var html = await http.GetStringAsync(mangaUrl, token);
        var doc = new HtmlDocument();
        doc.LoadHtml(html);
        var docNode = doc.DocumentNode;
        var tNode = docNode.QuerySelector("head > title");
        var metaNodes = docNode.QuerySelectorAll("head > meta").ToArray();

        string ParseTitle()
        {
            string title = Html.Decode(tNode.InnerText.Trim());
            Console.WriteLine(title);
            return title;
        }

        string ParseDescription()
        {
            var desMeta = metaNodes.First(meta => meta.Attributes["name"]?.Value == "description");
            var desc = Html.Decode(desMeta.Attributes["content"].Value);
            Console.WriteLine(desc);
            return desc;
        }

        string ParseCoverUrl()
        {
            var imgMeta = metaNodes.First(meta => meta.Attributes["property"]?.Value == "og:image");
            var img = Html.Decode(imgMeta.Attributes["content"].Value);
            Console.WriteLine(img);
            return img;
        }

        List<Chapter> ParseChapters()
        {
            var chapterNode = docNode.QuerySelector("body > main > section.py-8 > div.xl-container > div").FirstChild.FirstChild.ChildNodes[2];
            var ul = chapterNode.QuerySelector("ul");
            var list = new List<Chapter>();
            foreach (var li in ul.QuerySelectorAll("li"))
            {
                var title = Html.Decode(li.QuerySelector("a > p").InnerText.Trim());
                var url = Url.Combine(mangaUrl, li.QuerySelector("a").Attributes["href"].Value);
                var chapter = new Chapter(title, url);
                Console.WriteLine($"Add chapter: {title}:{url}");
                list.Add(chapter);
            }
            Console.WriteLine("Finished loading chapters");
            return list;
        }

        try
        {
            return new Manga(
                title: ParseTitle(),
                description: ParseDescription(),
                coverUrl: ParseCoverUrl(),
                chapters: ParseChapters()
            );
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new ParseException();
        }
    }
}