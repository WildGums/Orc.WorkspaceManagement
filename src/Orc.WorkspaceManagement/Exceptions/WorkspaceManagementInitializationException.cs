// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkspaceManagementInitializationException.cs" company="WildGums">
//   Copyright (c) 2008 - 2018 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class WorkspaceManagementInitializationException : Exception
    {
        public WorkspaceManagementInitializationException(IWorkspaceManager workspaceManager)
            : base("Unable to initialize WorkspaceManager. Probably initialization was canceled.")
        {
            WorkspaceManager = workspaceManager;
        }

        public WorkspaceManagementInitializationException(IWorkspaceManager workspaceManager, string message)
            : base(message)
        {
            WorkspaceManager = workspaceManager;
        }

        public WorkspaceManagementInitializationException(IWorkspaceManager workspaceManager, string message, Exception innerException)
            : base(message, innerException)
        {
            WorkspaceManager = workspaceManager;
        }

        protected WorkspaceManagementInitializationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public IWorkspaceManager WorkspaceManager { get; private set; }
    }
}
