﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Factories.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
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
                if (workspacesStorageService == null)
                {
                    workspacesStorageService = Mock.Of<IWorkspacesStorageService>();
                }

                var workspaceProviderLocator = new WorkspaceProviderLocator();
                var serviceLocator = ServiceLocator.Default;
                var emptyWorkspaceInitializer = new EmptyWorkspaceInitializer();
                var workspaceManagerInitializer = new WorkspaceManagerInitializer(workspaceProviderLocator, serviceLocator);

                return new WorkspaceManagement.WorkspaceManager(emptyWorkspaceInitializer, workspaceProviderLocator, workspacesStorageService,
                    workspaceManagerInitializer, serviceLocator);
            }
        }
    }
}