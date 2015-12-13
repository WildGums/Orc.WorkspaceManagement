// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkspaceEventArgs.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
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