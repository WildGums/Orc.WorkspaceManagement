namespace Orc.WorkspaceManagement.Automation;

using System.Collections.Generic;

public class WorkspaceViewGroupItem 
{
    public string? GroupName { get; set; }
    public IReadOnlyList<WorkspaceViewItem>? Items { get; set; }
}
