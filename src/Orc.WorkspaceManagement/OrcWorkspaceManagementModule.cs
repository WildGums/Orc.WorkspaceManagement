namespace Orc.WorkspaceManagement
{
    using Catel.Services;
    using Catel.ThirdPartyNotices;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    /// <summary>
    /// Core module which allows the registration of default services in the service collection.
    /// </summary>
    public static class OrcWorkspaceManagementModule
    {
        public static IServiceCollection AddOrcWorkspaceManagement(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddSingleton<IWorkspaceManager, WorkspaceManager>();
            serviceCollection.TryAddSingleton<IWorkspaceInitializer, EmptyWorkspaceInitializer>();
            serviceCollection.TryAddSingleton<IWorkspacesStorageService, WorkspacesStorageService>();

            serviceCollection.AddSingleton<ILanguageSource>(new LanguageResourceSource("Orc.WorkspaceManagement", "Orc.WorkspaceManagement.Properties", "Resources"));

            serviceCollection.AddSingleton<IThirdPartyNotice>((x) => new LibraryThirdPartyNotice("Orc.WorkspaceManagement", "https://github.com/wildgums/orc.workspacemanagement"));

            return serviceCollection;
        }
    }
}
