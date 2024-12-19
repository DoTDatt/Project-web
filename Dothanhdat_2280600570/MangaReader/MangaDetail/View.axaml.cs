using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using MangaReader.DomainCommon;

namespace MangaReader.MangaDetail;

public partial class View : Window, IView
{

    private readonly Http? http;
    private readonly Presenter? presenter;
    
    public View()
    {
        InitializeComponent();
    }

    public View(string mangaUrl, Http http) : this()
    {
        Console.WriteLine(mangaUrl);
        this.http = http;
        var domain = new Domain(mangaUrl, http);
        presenter = new Presenter(domain, this);
        this.ErrorPanel.RetryButton.Click += (sender, args) => presenter.LoadManga();
    }

    public void ShowLoadingManga()
    {
        this.MangaContent.IsVisible = false;
        this.ErrorPanel.IsVisible = false;
        this.LoadingProgressBar.IsVisible = true;
        this.Title = "Loading...";
    }

    public void ShowErrorPanel(string msg)
    {
        this.LoadingProgressBar.IsVisible = false;
        this.MangaContent.IsVisible = false;
        this.ErrorPanel.MessageTextBlock.Text = msg;
        this.ErrorPanel.IsVisible = true;
        this.Title = "Error";
    }

    public void ShowOverview(string title, int chapterNumber, string description, string coverUrl)
    {
        this.MainPanel.Content = new Overview(this.http!, title, chapterNumber, description, coverUrl);
    }

    public void ShowMangaContent(Manga manga)
    {
        this.LoadingProgressBar.IsVisible = false;
        this.ErrorPanel.IsVisible = false;
        this.MangaContent.IsVisible = true;

        this.ListBox.Items.Clear();
        this.ListBox.Items.Add("Overview");
        foreach (var chapter in manga.Chapters)
        {
            var tb = new TextBlock { Text = chapter.Title, TextWrapping = TextWrapping.Wrap};
            this.ListBox.Items.Add(tb);
        }

        this.ListBox.SelectedIndex = 0;

        this.MainPanel.Content = 
            new Overview(this.http!, manga.Title, manga.Chapters.Count, manga.Description, manga.CoverUrl);
        this.Title = manga.Title;
    }

    private void ListBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is ListBox lb)
        {
            presenter?.OnListBoxItemSelected(lb.SelectedIndex);
        }
    }
    
    
}