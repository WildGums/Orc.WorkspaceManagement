// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkspaceProviderEventArgs.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement
{
    using System;

    public class WorkspaceProviderEventArgs : EventArgs
    {
        public WorkspaceProviderEventArgs(IWorkspaceProvider workspaceProvider)
        {
            WorkspaceProvider = workspaceProvider;
        }

        public IWorkspaceProvider WorkspaceProvider { get; private set; }
    }
}