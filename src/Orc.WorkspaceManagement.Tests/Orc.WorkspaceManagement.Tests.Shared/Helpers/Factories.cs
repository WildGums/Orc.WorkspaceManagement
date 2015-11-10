// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Factories.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement.Test
{
    using Moq;

    public static class Factories
    {
        public static class WorkspaceManager
        {
            public static IWorkspaceManager WithEmptyInitializer(IWorkspacesStorageService workspacesStorageService = null)
            {
                if (workspacesStorageService == null)
                {
                    workspacesStorageService = Mock.Of<IWorkspacesStorageService>();
                }

                var workspaceProviderLocator = new WorkspaceProviderLocator();
                return new WorkspaceManagement.WorkspaceManager(new EmptyWorkspaceInitializer(), workspaceProviderLocator, workspacesStorageService,
                    new WorkspaceManagerInitializer(workspacesStorageService, workspaceProviderLocator));
            }
        }
    }
}