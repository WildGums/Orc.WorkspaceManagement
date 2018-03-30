// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkspaceException.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement
{
    using System;

    public class WorkspaceException : Exception
    {
        public WorkspaceException(IWorkspace workspace)
            : this(workspace, string.Empty)
        {
            
        }

        public WorkspaceException(IWorkspace workspace, string message)
            : base(message)
        {
            Workspace = workspace;
        }

        public IWorkspace Workspace { get; private set; }
    }
}