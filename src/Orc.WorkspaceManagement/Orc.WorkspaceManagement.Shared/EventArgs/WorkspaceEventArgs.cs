// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkspaceEventArgs.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
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