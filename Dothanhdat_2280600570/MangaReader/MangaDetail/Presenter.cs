
using System;
using System.Threading;
using MangaReader.DomainCommon;

namespace MangaReader.MangaDetail;

internal enum State
{
    Loading,
    Success,
    Error,
    Disposed
}
public class Presenter : IDisposable
{
    private readonly Domain domain;
    private readonly IView view;

    // private bool isLoadingManga;
    private State state;
    private Manga? manga;
    private CancellationTokenSource? cts;

    public Presenter(Domain domain, IView view)
    {
        this.domain = domain;
        this.view = view;
        this.LoadManga();
    }

    private async void LoadManga()
    {
        state = State.Loading;
        view.ShowLoadingManga();
        manga = null;
        cts = new CancellationTokenSource();
        try
        {
            manga = await domain.LoadManga(cts.Token);
            state = State.Success;
            view.ShowMangaContent(manga);
        }
        catch (NetworkException ex)
        {
            state = State.Error;
        }
        catch (ParseException)
        {
            state = State.Error;
            view.ShowErrorPanel("Oop! Something went wrong.");
        }
        catch (OperationCanceledException)
        {
            state = State.Disposed;
        }
    }

    public void OnListBoxItemSelected(int selectedIndex)
    {
        if (state != State.Success || selectedIndex < 0) return;
        if (selectedIndex == 0)
        {
            view.ShowOverview(manga!.Title, manga.Chapters.Count, manga.Description, manga.CoverUrl);
            return;
        }
        
        var chapterIndex = selectedIndex - 1;
        var chapterUrl = manga!.Chapters[chapterIndex].Url;
        view.ShowChapter(chapterUrl);
    }

    public void Retry()
    {
        if (state == State.Error)
        {
            this.LoadManga();
        }
    }

    public void Dispose()
    {
        cts?.Cancel();
        cts?.Dispose();
        state = State.Disposed;
    }
}

