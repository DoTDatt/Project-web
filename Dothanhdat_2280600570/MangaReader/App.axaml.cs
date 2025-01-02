using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using MangaReader.DomainCommon;

namespace MangaReader;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            const string baseUrl = "https://apptruyen247.com";
            const string dbFile = "MangaReader.db";
            var http = new Http();
            
            desktop.MainWindow = new MangaList.View(baseUrl, http, dbFile);
        }

        base.OnFrameworkInitializationCompleted();
    }
}