using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MangaReader.DomainCommon;
using System;
namespace MangaReader.MangaList;

public class Presenter
{
    private readonly Domain domain;
    private IView? view;
    private CancellationTokenSource? cts;
    private Task? task;
    private int currentPageIndex = 1;
    private int totalPageNumber = 0;
    private bool isLoading;

    public Presenter(Domain domain)
    {
        this.domain = domain;
    }

    public void AttachView(IView view)
    {
        if (this.view != null)
            return;
        this.view = view;
        this.Load();
    }

    private void ShowLoading()
    {
        view?.SetLoadingVisible(true);
        view?.SetErrorPanelVisible(false);
        view?.SetMainContentVisible(false);
    }

    private void ShowError(string errorMessage)
    {
        view?.SetLoadingVisible(false);
        view?.SetMainContentVisible(false);
        view?.SetErrorMessage(errorMessage);
        view?.SetErrorPanelVisible(true);
    }

    private void ShowNoManga()
    {
        view?.SetTotalMangaNumber("No manga");
        view?.SetCurrentPageButtonContent("No page");
        view?.SetCurrentPageButtonEnabled(false);
        view?.SetFirstButtonAndPrevButtonEnabled(false);
        view?.SetLastButtonAndNextButtonEnabled(false);
        view?.SetListBoxContent(Enumerable.Empty<Item>());
        view?.SetLoadingVisible(false);
        view?.SetMainContentVisible(true);
        view?.SetErrorPanelVisible(false);
    }

    private void ShowMangaList(MangaList list)
    {
        view?.SetTotalMangaNumber(list.TotalMangaNumber + "mangas");
        view?.SetFirstButtonAndPrevButtonEnabled(currentPageIndex > 1);
        view?.SetCurrentPageButtonContent("page" + currentPageIndex+ " of " + list.TotalPageNumber);
        view?.SetCurrentPageButtonEnabled(true);
        view?.SetNumericUpDownValue(currentPageIndex);
        view?.SetNumericUpDownMaximum(list.TotalPageNumber);
        view?.SetLastButtonAndNextButtonEnabled(currentPageIndex < list.TotalPageNumber);
        view?.SetListBoxContent(
            list.CurrentPage.Select(manga =>new Item
            (
                title : manga.Title,
                lastChapter : manga.LastChapter+"chapters",
                description : manga.Description   
            ))
        );
        view?.SetMainContentVisible(true);
        view?.SetErrorPanelVisible(false);
        view?.SetLoadingVisible(false);
    }

    public async void Load()
    {
        if(isLoading) return;
        isLoading = true;
        this.ShowLoading();
        if (cts != null)
        {
            cts.Cancel();
            if (task != null)
            {
                try
                {
                    await task;
                }
            catch (OperationCanceledException){}
                task = null;
            }
            cts = null;
        }
        MangaList? list = null;
        string? errorMessage = null;
        try
        {
            list = await domain.LoadMangaList(currentPageIndex);
        }
        catch (NetworkException ex)
        {
            errorMessage = "Network error: " + ex.Message;
        }
        catch (ParseException)
        {
            errorMessage = "Oops! Something went wrong.";
        }

        if (list == null)
        {
            this.ShowError(errorMessage!);
        }
        else if (list.TotalMangaNumber <= 0 || list.TotalPageNumber <= 0)
        {
            this.ShowNoManga();
        }
        else
        {
            totalPageNumber=list.TotalPageNumber;
            this.ShowMangaList(list);
            cts = new CancellationTokenSource();
            var coverUrls = list.CurrentPage.Select(manga => manga.CoverUrl);
            task = this.LoadCovers(coverUrls, cts.Token);
        }
        isLoading = false;
    }

    public void ApplyFilter()
    {
        currentPageIndex = 1;
        this.Load();
    }

    private async Task LoadCovers(IEnumerable<string> urls, CancellationToken token)
    {
        var index = -1;
        foreach (var url in urls)
        {
            index++;
            byte[]? bytes;
            try
            {
                bytes = await domain.LoadBytes(url, token);
            }
            catch (NetworkException)
            {
                bytes = null;
            }
            if(token.IsCancellationRequested) break;
            view?.SetCover(index, bytes);
        }
    }

    public void GoNextPage()
    {
        if(isLoading || currentPageIndex >= totalPageNumber) return;
        currentPageIndex++;
        view?.SetNumericUpDownValue(currentPageIndex);
        this.Load();
    }

    public void GoPrevPage()
    {
        if (isLoading || currentPageIndex <= 1) return;
        currentPageIndex--;
        view?.SetNumericUpDownValue(currentPageIndex);
        this.Load();
    }

    public void GoFirstPage()
    {
        if( isLoading || currentPageIndex <=1) return;
        currentPageIndex = 1;
        view?.SetNumericUpDownValue(currentPageIndex);
        this.Load();
    }

    public void GoLastPage()
    {
        if(isLoading || currentPageIndex >= totalPageNumber) return;
        currentPageIndex = totalPageNumber;
        view?.SetNumericUpDownValue(currentPageIndex);
        this.Load();
    }

    public void GoSpecificPage()
    {
        if(isLoading || view == null)  return;
        view.HideFlyout();
        var pageIndex = view.GetNumericUpDownValue();
        if(pageIndex < 1 || pageIndex > totalPageNumber) return;
        currentPageIndex = pageIndex;
        this.Load();
    }
}