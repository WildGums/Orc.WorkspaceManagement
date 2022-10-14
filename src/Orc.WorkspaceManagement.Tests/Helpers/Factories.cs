namespace Orc.WorkspaceManagement.Test
{
    using Catel.IoC;
    using Catel.Services;
    using Moq;

    public static class Factories
    {
        public static class WorkspaceManager
        {
            public static IWorkspaceManager WithEmptyInitializer(IWorkspacesStorageService workspacesStorageService = null)
            {
                if (workspacesStorageService is null)
                {
                    workspacesStorageService = Mock.Of<IWorkspacesStorageService>();
                }

                var serviceLocator = ServiceLocator.Default;
                var emptyWorkspaceInitializer = new EmptyWorkspaceInitializer();
                var appDataService = serviceLocator.ResolveRequiredType<IAppDataService>();

                return new WorkspaceManagement.WorkspaceManager(emptyWorkspaceInitializer, workspacesStorageService, serviceLocator, appDataService);
            }
        }
    }
}
