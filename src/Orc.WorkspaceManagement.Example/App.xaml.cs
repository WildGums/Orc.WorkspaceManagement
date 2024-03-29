﻿namespace Orc.WorkspaceManagement.Example;

using System.Globalization;
using System.Windows;
using Catel.IoC;
using Catel.Logging;
using Catel.Services;
using Orchestra;
using Orchestra.Services;
using Orchestra.Views;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private static readonly ILog Log = LogManager.GetCurrentClassLogger();

    protected override async void OnStartup(StartupEventArgs e)
    {
#if DEBUG
        LogManager.AddDebugListener(true);
#endif

        var languageService = ServiceLocator.Default.ResolveRequiredType<ILanguageService>();

        // Note: it's best to use .CurrentUICulture in actual apps since it will use the preferred language
        // of the user. But in order to demo multilingual features for devs (who mostly have en-US as .CurrentUICulture),
        // we use .CurrentCulture for the sake of the demo
        languageService.PreferredCulture = CultureInfo.CurrentCulture;
        languageService.FallbackCulture = new CultureInfo("en-US");

        Log.Info("Starting application");

        this.ApplyTheme();

        Log.Info("Calling base.OnStartup");

        var serviceLocator = ServiceLocator.Default;
        var shellService = serviceLocator.ResolveRequiredType<IShellService>();
        await shellService.CreateAsync<ShellWindow>();
    }
}
