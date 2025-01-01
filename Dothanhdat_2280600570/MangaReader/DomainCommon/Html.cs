using System.Web;

namespace MangaReader.DomainCommon;

public class Html
{
    public static string Decode(string str)
    {
        var tmp = str.Trim().Replace('\r', '\n').Replace('\n', ' ');
        return HttpUtility.HtmlDecode(tmp);
    }
}