﻿namespace Orc.WorkspaceManagement.Example.Services;

using System;
using Orchestra.Services;
using System.Threading.Tasks;
using System.Windows.Media;
using Catel.IoC;
using WorkspaceManagement;

public class ApplicationInitializationService : ApplicationInitializationServiceBase
{
    private readonly IServiceLocator _serviceLocator;

    public ApplicationInitializationService(IServiceLocator serviceLocator)
    {
        ArgumentNullException.ThrowIfNull(serviceLocator);

        _serviceLocator = serviceLocator;
    }

    public override Task InitializeBeforeCreatingShellAsync()
    {
        InitializeFonts();
        RegisterTypes();

        return Task.CompletedTask;
    }

    private void InitializeFonts()
    {
        Orc.Theming.FontImage.RegisterFont("FontAwesome", new FontFamily(new Uri("pack://application:,,,/Orc.WorkspaceManagement.Example;component/Resources/Fonts/", UriKind.RelativeOrAbsolute), "./#FontAwesome"));

        Orc.Theming.FontImage.DefaultBrush = new SolidColorBrush(Color.FromArgb(255, 87, 87, 87));
        Orc.Theming.FontImage.DefaultFontFamily = "FontAwesome";
    }

    private void RegisterTypes()
    {
        _serviceLocator.RegisterType<IWorkspaceInitializer, WorkspaceInitializer>();
    }
}
