// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Factories.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement.Test
{
    using Catel.IoC;
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

                return new WorkspaceManagement.WorkspaceManager(emptyWorkspaceInitializer, workspacesStorageService, serviceLocator);
            }
        }
    }
}
