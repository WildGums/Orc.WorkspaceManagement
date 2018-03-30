// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkspaceEventArgs.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement
{
    using System;

    public class WorkspaceEventArgs : EventArgs
    {
        public WorkspaceEventArgs(IWorkspace workspace)
        {
            Workspace = workspace;
        }

        public IWorkspace Workspace { get; private set; }
    }
}