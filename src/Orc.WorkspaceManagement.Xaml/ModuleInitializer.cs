﻿using Catel.IoC;
using Catel.Services;
using Orc.WorkspaceManagement.Behaviors;

/// <summary>
/// Used by the ModuleInit. All code inside the Initialize method is ran as soon as the assembly is loaded.
/// </summary>
public static class ModuleInitializer
{
    /// <summary>
    /// Initializes the module.
    /// </summary>
    public static void Initialize()
    {
        var serviceLocator = ServiceLocator.Default;

        var languageService = serviceLocator.ResolveType<ILanguageService>();
        languageService.RegisterLanguageSource(new LanguageResourceSource("Orc.WorkspaceManagement.Xaml", "Orc.WorkspaceManagement.Properties", "Resources"));
    }
}