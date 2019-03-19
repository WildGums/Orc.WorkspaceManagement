namespace Orc.WorkspaceManagement
{
    using System.Diagnostics;
    using Catel.Collections;

    [DebuggerDisplay("{Title}")]
    public class WorkspaceGroup
    {
        public WorkspaceGroup()
        {
            Workspaces = new FastObservableCollection<IWorkspace>();
        }

        public string Title { get; set; }

        public FastObservableCollection<IWorkspace> Workspaces { get; protected internal set; }
    }
}
