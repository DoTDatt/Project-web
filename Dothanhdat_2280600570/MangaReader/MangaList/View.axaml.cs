using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using MangaReader.DomainCommon;

namespace MangaReader.MangaList;

public partial class View : Window, IView
{

    // private  Presenter? presenter;
    private  readonly Presenter? presenter;
    private readonly Http? http;
    
    private readonly List<ItemControl> itemControls = new();
    
    public View()
    {
        InitializeComponent();
    }
    
    // public void SetPresenter(Presenter presenter)
    public View(string baseUrl, Http http, string dbFile) : this()
    {
        // this.presenter = presenter;
        // this.ErrorPanel.RetryButton.Click += (sender, args) => this.presenter?.Load();

        this.http = http;
        // var domain = new Domain(baseUrl, http);
        var db = new Db(dbFile);
        var domain = new Domain(baseUrl, http, db);
        presenter = new Presenter(domain, this);
        this.ErrorPanel.RetryButton.Click += (sender, args) => presenter.Load();
    }

    public void SetLoadingVisible(bool value)
    {
        this.LoadingProgressBar.IsVisible = value;
    }

    public void SetErrorPanelVisible(bool value)
    {
        this.ErrorPanel.IsVisible = value;
    }
    // ---------- 2 ---------
    public void SetMainContentVisible(bool value)
    {
        this.MainContent.IsVisible = value;
    }

    public void SetTotalMangaNumber(string text)
    {
        this.TotalMangaNumberLabel.Content = text;
    }

    public void SetCurrentPageButtonContent(string content)
    {
        this.CurrentPageButton.Content = content;
    }

    public void SetCurrentPageButtonEnabled(bool value)
    {
        this.CurrentPageButton.IsEnabled = value;
    }

    public void SetNumericUpDownMaximum(int value)
    {
        this.NumericUpDown.Maximum = value;
    }

    public void SetNumericUpDownValue(int value)
    {
        this.NumericUpDown.Value = value;
    }

    public int GetNumericUpDownValue()
    {
        return (int)this.NumericUpDown.Value!;
    }

    public void SetListBoxContent(IEnumerable<Item> items)
    {
        this.MangaListBox.Items.Clear();
        foreach (var itemControl in itemControls)
        {
            ViewCommon.Utils.DisposeImageSource(itemControl.CoverImage);
        }
        itemControls.Clear();
        var index = -1;
        foreach (var item in items)
        {
            // var itemControl = new ItemControl();
            // itemControl.TitleTextBlock.Text = item.Title;
            // itemControl.ChapterNumberTextBlock.Text = item.ChapterNumber;
            // ToolTip.SetTip(itemControl.CoverBorder, item.ToolTip);
            // itemControls.Add(itemControl);
            // this.MangaListBox.Items.Add(new ListBoxItem { Content = itemControl});
            index++;
            var itemControl = new ItemControl
            {
                Title = item.Title,
                ChapterNumber = item.ChapterNumber,
                CoverToolTip = item.ToolTip,
                IsFavorites = item.IsFavorites
            };
            var i = index;
            itemControl.FavoritesButton.Click += (s, e) => presenter?.ToggleFavoritesManga(i);
            itemControls.Add(itemControl);
            this.MangaListBox.Items.Add(new ListBoxItem{Content = itemControl});
        }
    }

    public void SetCover(int index, byte[]? bytes)
    {
        var errorCoverBackground = Brushes.DeepPink;
        var itemControl = itemControls[index];
        if (bytes == null)
        {
            itemControl.CoverBorder.Background = errorCoverBackground;
            return;
        }

        using var ms = new MemoryStream(bytes);
        try
        {
            itemControl.CoverImage.Source = new Bitmap(ms);
        }
        catch (Exception)
        {
            itemControl.CoverBorder.Background = errorCoverBackground;
        }
    }

    public void SetFirstButtonAndPrevButtonEnabled(bool value)
    {
        this.FirstButton.IsEnabled = value;
        this.PrevButton.IsEnabled = value;
    }

    public void SetLastButtonAndNextButtonEnabled(bool value)
    {
        this.LastButton.IsEnabled = value;
        this.NextButton.IsEnabled = value;
    }

    public void HideFlyout()
    {
        this.CurrentPageButton.Flyout?.Hide();
    }

    public void SetErrorMessage(string text)
    {
        this.ErrorPanel.MessageTextBlock.Text = text;
    }

    public string? GetFilterText()
    {
        return this.MyTextBox.Text;
    }

    public void OpenMangaDetail(string mangaUrl)
    {
        var window = new MangaDetail.View(mangaUrl, http!);
        window.Show(owner:this);
    }


    public void SetFavoritesMangas(IEnumerable<string> mangaTitles)
    {
        this.FavoritesMenuItem.Items.Clear();
        var index = -1;
        foreach (var title in mangaTitles)
        {
            index++;
            var item = new MenuItem { Header = title };
            var i = index;
            item.Click += (s, e) => presenter?.SelectFavoritesManga(i);
            this.FavoritesMenuItem.Items.Add(item);
        }
    }

    public void UpdateFavoritesManga(int index, bool value)
    {
        itemControls[index].IsFavorites = value;
    }
    
    private void MyListBox_OnDoubleTapped(object? sender, TappedEventArgs e)
    {
        presenter?.SelectManga(this.MangaListBox.SelectedIndex);
    }

    private void MyListBox_OnkeyUp(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            presenter?.SelectManga(this.MangaListBox.SelectedIndex);
        }
    }
    // -----------------------------------------------------------------------
    // private void RefreshButton_OnClick(object? sender, RoutedEventArgs e)
    // {
    //     presenter?.Load();
    // }

    private void PrevButton_OnClick(object? sender, RoutedEventArgs e)
    {
        presenter?.GoPrevPage();
    }

    private void NextButton_OnClick(object? sender, RoutedEventArgs e)
    {
        presenter?.GoNextPage();
    }

    private void FirstButton_OnClick(object? sender, RoutedEventArgs e)
    {
        presenter?.GoFirstPage();
    }

    private void LastButton_OnClick(object? sender, RoutedEventArgs e)
    {
        presenter?.GoLastPage();
    }

    private void GoButton_OnClick(object? sender, RoutedEventArgs e)
    {
        presenter?.GoSpecificPage();
    }

    private void NumericUpDown_OnKeyUp(object? sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter) return;
        presenter?.GoSpecificPage();
    }

    private void MyClearButtom_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.MyTextBox.Text == null) return;
        else
        {
            this.MyTextBox.Text = "";
        }
    }

    private void MyApplyButton_OnClick(object? sender, RoutedEventArgs e)
    {
        presenter?.ApplyFilter();
    }

    private void OnkeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            presenter?.ApplyFilter();
        }
    }
    
}