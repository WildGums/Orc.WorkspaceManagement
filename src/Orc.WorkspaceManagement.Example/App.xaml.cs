namespace Orc.WorkspaceManagement.Example;

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using Catel;
using Catel.IoC;
using Catel.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orc.WorkspaceManagement.Example.Services;
using Orc.WorkspaceManagement.Example.WorkspaceManagement;
using Orchestra;
using Orchestra.Views;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
#pragma warning disable IDISP006 // Implement IDisposable
    private readonly IHost _host;
#pragma warning restore IDISP006 // Implement IDisposable

    public App()
    {
        var hostBuilder = new HostBuilder()
            .ConfigureServices((hostContext, services) =>
            {
                services.AddCatelCore();
                services.AddCatelMvvm();
                services.AddOrcAutomation();
                services.AddOrcControls();
                services.AddOrcFileSystem();
                services.AddOrcLogViewer();
                services.AddOrcSerializationJson();
                services.AddOrcSystemInfo();
                services.AddOrcTheming();
                services.AddOrcWorkspaceManagement();
                services.AddOrcWorkspaceManagementXaml();
                services.AddOrchestraCore();
                services.AddOrchestraShellRibbonFluent();

                services.AddSingleton<IWorkspacesStorageService, ExampleWorkspacesStorageService>();
                services.AddSingleton<IRibbonService, RibbonService>();
                services.AddSingleton<IApplicationInitializationService, ApplicationInitializationService>();

                // Initializers
                services.AddSingleton<IWorkspaceInitializer, WorkspaceInitializer>();

                services.AddLogging(x =>
                {
                    x.AddConsole();
                    x.AddDebug();
                });
            });

        _host = hostBuilder.Build();

        IoCContainer.ServiceProvider = _host.Services;
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var serviceProvider = IoCContainer.ServiceProvider;

        serviceProvider.CreateTypesThatMustBeConstructedAtStartup();

        var languageService = serviceProvider.GetRequiredService<ILanguageService>();

        // Note: it's best to use .CurrentUICulture in actual apps since it will use the preferred language
        // of the user. But in order to demo multilingual features for devs (who mostly have en-US as .CurrentUICulture),
        // we use .CurrentCulture for the sake of the demo
        languageService.PreferredCulture = CultureInfo.CurrentCulture;
        languageService.FallbackCulture = new CultureInfo("en-US");

        this.ApplyTheme();

        Orc.Theming.FontImage.RegisterFont("FontAwesome", new FontFamily(new Uri("pack://application:,,,/Orc.WorkspaceManagement.Example;component/Resources/Fonts/", UriKind.RelativeOrAbsolute), "./#FontAwesome"));

        Orc.Theming.FontImage.DefaultBrush = new SolidColorBrush(Color.FromArgb(255, 87, 87, 87));
        Orc.Theming.FontImage.DefaultFontFamily = "FontAwesome";

        var shellService = serviceProvider.GetRequiredService<IShellService>();
        await shellService.CreateAsync<ShellWindow>();
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        using (_host)
        {
            await _host.StopAsync();
        }

        base.OnExit(e);
    }
}
