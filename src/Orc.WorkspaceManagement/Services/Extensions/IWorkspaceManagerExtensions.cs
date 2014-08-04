// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWorkspaceManagerExtensions.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement.Services
{
    using Catel;
    using Models;

    public static class IWorkspaceManagerExtensions
    {
        public static TWorkspace GetWorkspace<TWorkspace>(this IWorkspaceManager workspaceManager)
            where TWorkspace : IWorkspace
        {
            Argument.IsNotNull("workspaceManager", workspaceManager);

            return (TWorkspace)workspaceManager.CurrentWorkspace;
        }
    }
}