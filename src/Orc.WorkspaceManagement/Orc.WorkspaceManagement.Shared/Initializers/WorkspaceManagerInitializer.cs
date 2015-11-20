// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkspaceManagerInitializer.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET40 || SL5
#define USE_TASKEX
#endif

namespace Orc.WorkspaceManagement
{
    using System.Threading.Tasks;
    using Catel;
    using Catel.IoC;
    using Catel.IO;
    using Catel.Threading;

    public class WorkspaceManagerInitializer : IWorkspaceManagerInitializer
    {
        private readonly IWorkspaceProviderLocator _workspaceProviderLocator;
        protected readonly IServiceLocator ServiceLocator;

        public WorkspaceManagerInitializer(IWorkspaceProviderLocator workspaceProviderLocator, IServiceLocator serviceLocator)
        {           
            Argument.IsNotNull(() => workspaceProviderLocator);
            Argument.IsNotNull(() => serviceLocator);

            _workspaceProviderLocator = workspaceProviderLocator;
            ServiceLocator = serviceLocator;
        }

        public async Task InitializeAsync(IWorkspaceManager workspaceManager)
        {
            await InitializeWorkspacesAsync(workspaceManager);

            await InitializeProvidersAsnc(workspaceManager);
        }

        protected virtual async Task InitializeWorkspacesAsync(IWorkspaceManager workspaceManager)
        {
            Argument.IsNotNull(() => workspaceManager);

            var workspacesStorageService = ServiceLocator.ResolveType<IWorkspacesStorageService>(workspaceManager.Tag);

            var baseDirectory = workspaceManager.BaseDirectory;
            var workspaces = workspacesStorageService.LoadWorkspaces(baseDirectory);

            foreach (var workspace in workspaces)
            {
                await workspaceManager.AddAsync(workspace);
            }
        }

        protected virtual async Task InitializeProvidersAsnc(IWorkspaceManager workspaceManager)
        {
            Argument.IsNotNull(() => workspaceManager);

#if USE_TASKEX
            var workspaceProviders = await TaskShim.WhenAll(_workspaceProviderLocator.ResolveAllProviders(workspaceManager.Tag));
#else
            var workspaceProviders = await Task.WhenAll(_workspaceProviderLocator.ResolveAllProviders(workspaceManager.Tag));
#endif

            foreach (var workspaceProvider in workspaceProviders)
            {
                workspaceManager.AddProvider(workspaceProvider);
            }
        }

        protected virtual Task InitializeWatchersAsync(IWorkspaceManager workspaceManager)
        {
            Argument.IsNotNull(() => workspaceManager);

            return TaskHelper.Completed;
        }
    }
}