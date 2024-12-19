using MangaReader.DomainCommon;

namespace MangaReader.MangaDetail;

public class Presenter
{
    private readonly Domain domain;
    private readonly IView view;

    private bool isLoadingManga;
    private Manga? manga;

    public Presenter(Domain domain, IView view)
    {
        this.domain = domain;
        this.view = view;
        this.LoadManga();
    }

    public async void LoadManga()
    {
        if (isLoadingManga) return;
        isLoadingManga = true;
        view.ShowLoadingManga();
        manga = null;
        try
        {
            manga = await domain.LoadManga();
            view.ShowMangaContent(manga);
        }
        catch (NetworkException ex)
        {
            view.ShowErrorPanel(ex.Message);
        }
        catch (ParseException)
        {
            view.ShowErrorPanel("Oop! Something went wrong.");
        }
        isLoadingManga = false;
    }

    public void OnListBoxItemSelected(int selectedIndex)
    {
        if (isLoadingManga || manga == null) return;
        if (selectedIndex < 0 || selectedIndex >= manga.Chapters.Count) return;
        if (selectedIndex == 0)
        {
            view.ShowOverview(manga.Title, manga.Chapters.Count, manga.Description, manga.CoverUrl);
        }
        // view.ShowOverview(manga.Title, manga.Chapters.Count, manga.Description, manga.CoverUrl);
    }
}

