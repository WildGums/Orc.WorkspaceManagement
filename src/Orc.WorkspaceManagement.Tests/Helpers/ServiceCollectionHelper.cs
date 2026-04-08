namespace Orc.WorkspaceManagement.Tests;

using Catel;
using Microsoft.Extensions.DependencyInjection;
using Orc.FileSystem;
using Orc.Theming;
using Orc.WorkspaceManagement;

internal static class ServiceCollectionHelper
{
    public static IServiceCollection CreateServiceCollection()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddLogging();
        serviceCollection.AddCatelCore();
        serviceCollection.AddCatelMvvm();
        serviceCollection.AddOrcFileSystem();
        serviceCollection.AddOrcTheming();
        serviceCollection.AddOrcWorkspaceManagement();
        serviceCollection.AddOrcWorkspaceManagementXaml();

        return serviceCollection;
    }
}
