// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkspaceProviderEventArgs.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
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