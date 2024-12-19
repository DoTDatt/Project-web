using System.Collections.Generic;

namespace MangaReader.MangaList;

public class Item
{
    // public string Title { get; init; } = null!;
    // public string LastChapter { get; init; } = null!;
    // public string Description { get; init; } = null!;

    public string Title { get; init; } = null!;
    public string ChapterNumber { get; init; } = null!;
    public string Description { get; init; } = null!;

    public Item(string title, string chapterNumber, string description)
    {
        Title = title;
        ChapterNumber = chapterNumber;
        Description = description;
    }

    // public Item()
    // {
    //     throw new System.NotImplementedException();
    // }

    public string ToolTip => this.Title + " - " + this.Description;
}

public interface IView
{
    void SetLoadingVisible(bool value);
    void SetErrorPanelVisible(bool value);
    void SetMainContentVisible(bool value);

    void SetTotalMangaNumber(string text);
    void SetCurrentPageButtonContent(string content);
    void SetCurrentPageButtonEnabled(bool value);

    void SetNumericUpDownMaximum(int value);
    void SetNumericUpDownValue(int value);
    int GetNumericUpDownValue();
    void SetListBoxContent(IEnumerable<Item> items);
    void SetCover(int index, byte[]? bytes);

    void SetFirstButtonAndPrevButtonEnabled(bool value);
    void SetLastButtonAndNextButtonEnabled(bool value);

    void HideFlyout();

    void SetErrorMessage(string text);

    string? GetFilterText();

    void OpenMangaDetail(string mangaUrl);
}