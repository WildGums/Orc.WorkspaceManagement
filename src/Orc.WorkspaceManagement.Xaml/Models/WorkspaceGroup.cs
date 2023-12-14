namespace Orc.WorkspaceManagement;

using System.Collections.Generic;
using System.Diagnostics;

[DebuggerDisplay("{Title}")]
public class WorkspaceGroup
{
    public WorkspaceGroup(string? title, IEnumerable<IWorkspace> workspaces)
    {
        Title = title;
        Workspaces = new List<IWorkspace>();

        if (workspaces is not null)
        {
            Workspaces.AddRange(workspaces);
        }
    }

    public string? Title { get; private set; }

    public List<IWorkspace> Workspaces { get; private set; }
}
