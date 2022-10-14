using Catel.IoC;
using Catel.Services;
using Orc.WorkspaceManagement;

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

        serviceLocator.RegisterType<IWorkspaceManager, WorkspaceManager>();
        serviceLocator.RegisterType<IWorkspaceInitializer, EmptyWorkspaceInitializer>();
        serviceLocator.RegisterType<IWorkspacesStorageService, WorkspacesStorageService>();

        var languageService = serviceLocator.ResolveRequiredType<ILanguageService>();
        languageService.RegisterLanguageSource(new LanguageResourceSource("Orc.WorkspaceManagement", "Orc.WorkspaceManagement.Properties", "Resources"));
    }
}
