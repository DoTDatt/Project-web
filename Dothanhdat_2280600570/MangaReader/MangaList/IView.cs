using System.Collections.Generic;

namespace MangaReader.MangaList;

public class Item
{
    public Item(string title, string lastChapter, string description)
    {
        Title = title;
        LastChapter= lastChapter;
        Description = description;
    }
    public string Title { get;  }
    public string LastChapter { get;  }
    public string Description { get;  }
    public string ToolTip => this.Title+" - " + this.Description;

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

}