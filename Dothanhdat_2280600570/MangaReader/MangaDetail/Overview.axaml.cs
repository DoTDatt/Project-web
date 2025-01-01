using System;
using System.IO;
using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using MangaReader.DomainCommon;

namespace MangaReader.MangaDetail;

public partial class Overview : UserControl, IDisposable
{
    private readonly CancellationTokenSource cts = new();
    public Overview()
    {
        InitializeComponent();
    }

    public Overview(Http http, string title, int chapterNumber, string description, string coverUrl) : this()
    {
        this.TitleTextBlock.Text = title;
        if (chapterNumber == 0)
        {
            this.ChapterNumberTextBlock.Text = "this manga is banned";
            this.ChapterNumberTextBlock.Foreground = Brushes.White;
            this.ChapterNumberTextBlock.Background = Brushes.DeepPink;
            this.ChapterNumberTextBlock.Padding = new Thickness(5);
        }
        else
        {
            this.ChapterNumberTextBlock.Text = chapterNumber + " chapters";
        }

        this.DescriptionTextBlock.Text = description;
        this.LoadCover(http, coverUrl);
    }

    private async void LoadCover(Http http, string url)
    {
        var token = cts.Token;
        try
        {
            var data = await http.GetBytesAsync(url, token);
            using var stream = new MemoryStream(data);
            this.CoverImage.Source = new Bitmap(stream);
        }
        catch (Exception)
        {
            this.CoverImage.Source = null;
            this.Border.Background = Brushes.DeepPink;
        }
    }

    public void Dispose()
    {
        cts.Cancel();
        cts.Dispose();
        ViewCommon.Utils.DisposeImageSource(this.CoverImage);
    }
    
}