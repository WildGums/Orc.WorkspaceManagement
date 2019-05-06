// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CancelWorkspaceEventArgs.cs" company="WildGums">
//   Copyright (c) 2008 - 2019 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orc.WorkspaceManagement
{
    using System.ComponentModel;

    public class CancelWorkspaceEventArgs : CancelEventArgs
    {
        public CancelWorkspaceEventArgs(IWorkspace workspace)
        {
            Workspace = workspace;
        }

        public IWorkspace Workspace { get; private set; }
    }
}
