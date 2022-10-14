namespace Orc.WorkspaceManagement
{
    using System;
    using System.Runtime.Serialization;

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

        public IWorkspaceManager WorkspaceManager { get; private set; }
    }
}
