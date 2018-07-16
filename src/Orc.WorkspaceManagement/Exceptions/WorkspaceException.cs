// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkspaceException.cs" company="WildGums">
//   Copyright (c) 2008 - 2018 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
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

        public WorkspaceException(IWorkspace workspace, string message, Exception innerException)
            : base(message, innerException)
        {
            Workspace = workspace;
        }

        protected WorkspaceException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public IWorkspace Workspace { get; }
    }
}
