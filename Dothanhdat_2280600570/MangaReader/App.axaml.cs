using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using MangaReader.DomainCommon;
using MangaReader.MangaList;

namespace MangaReader;

public class App : Application
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
            var http = new Http();
            var domain = new MangaList.Domain(baseUrl,http);
            var presenter = new MangaList.Presenter(domain);
            var view = new MangaList.View(presenter);
            presenter.AttachView(view);
            desktop.MainWindow = view;
        }

        base.OnFrameworkInitializationCompleted();
    }
}