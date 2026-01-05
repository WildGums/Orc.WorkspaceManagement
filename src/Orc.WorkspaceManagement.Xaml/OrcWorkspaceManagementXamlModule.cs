namespace Orc.WorkspaceManagement
{
    using Catel.Services;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Core module which allows the registration of default services in the service collection.
    /// </summary>
    public static class OrcWorkspaceManagementXamlModule
    {
        public static IServiceCollection AddOrcWorkspaceManagementXaml(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<ILanguageSource>(new LanguageResourceSource("Orc.WorkspaceManagement.Xaml", "Orc.WorkspaceManagement.Properties", "Resources"));

            return serviceCollection;
        }
    }
}
