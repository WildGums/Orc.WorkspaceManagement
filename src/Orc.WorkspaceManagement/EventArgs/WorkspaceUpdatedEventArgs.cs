namespace Orc.WorkspaceManagement
{
    using System;
    using Catel;

    public class WorkspaceUpdatedEventArgs : EventArgs
    {
        public WorkspaceUpdatedEventArgs(IWorkspace? oldWorkspace, IWorkspace? newWorkspace)
        {
            OldWorkspace = oldWorkspace;
            NewWorkspace = newWorkspace;
        }

        public IWorkspace? OldWorkspace { get; private set; }

        public IWorkspace? NewWorkspace { get; private set; }

        public bool IsRefresh
        {
            get
            {
                if (OldWorkspace is null || NewWorkspace is null)
                {
                    return false;
                }

                return ObjectHelper.AreEqual(OldWorkspace, NewWorkspace);
            }
        }
    }
}
