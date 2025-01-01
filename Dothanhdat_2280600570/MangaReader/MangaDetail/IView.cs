namespace MangaReader.MangaDetail;

public interface IView
{
    void ShowLoadingManga();
    void ShowErrorPanel(string msg);
    void ShowMangaContent(Manga manga);
    void ShowOverview(string title, int chapterNumber, string description, string coverUrl);
    void ShowChapter(string chapterUrl);
}