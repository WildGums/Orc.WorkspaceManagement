namespace Orc.WorkspaceManagement.Test;

using Catel.Services;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Orc.WorkspaceManagement.Tests;

public static class Factories
{
    public static class WorkspaceManager
    {
        public static IWorkspaceManager WithEmptyInitializer(IWorkspacesStorageService workspacesStorageService = null)
        {
            var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

            serviceCollection.AddSingleton<IWorkspaceInitializer, EmptyWorkspaceInitializer>();

#pragma warning disable IDISP001 // Dispose created
            var serviceProvider = serviceCollection.BuildServiceProvider();
#pragma warning restore IDISP001 // Dispose created

            if (workspacesStorageService is null)
            {
                workspacesStorageService = Mock.Of<IWorkspacesStorageService>();
            }

            var workspaceManager = ActivatorUtilities.CreateInstance<WorkspaceManagement.WorkspaceManager>(serviceProvider, workspacesStorageService);
            return workspaceManager;
        }
    }
}
