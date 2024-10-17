namespace Orc.WorkspaceManagement.Automation;

using System.Collections.Generic;
using System.Windows.Automation;
using Orc.Automation;
using Orc.Automation.Controls;

[AutomatedControl(Class = typeof(Views.WorkspacesView))]
public class WorkspacesView(AutomationElement element)
    : FrameworkElement<WorkspaceViewModel, WorkspaceViewMap>(element)
{
    public IReadOnlyList<WorkspaceViewGroupItem>? GroupItems
        => Map.GroupList?.GetGroupItems();
}
