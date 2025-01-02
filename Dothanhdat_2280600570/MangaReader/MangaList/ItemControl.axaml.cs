using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

namespace MangaReader.MangaList;

public partial class ItemControl : UserControl
{

    private Geometry GetResouce(string name)
    {
        if (this.TryGetResource(name, out var res))
        {
            return (res as Geometry)!;
        }

        return null!;
    }

    public bool IsFavorites
    {
        set => this.FavoritesIcon.Data = this.GetResouce(value ? "star-solid" : "star-regular");
    }

    public string Title
    {
        init => this.TitleTextBlock.Text = value;
    }

    public string ChapterNumber
    {
        init => this.ChapterNumberTextBlock.Text = value;
    }

    public string CoverToolTip
    {
        init => ToolTip.SetTip(this.CoverBorder, value);
    }
    
    public ItemControl()
    {
        InitializeComponent();
    }
}