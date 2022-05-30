namespace Orc.WorkspaceManagement.Automation
{
    using System.Collections.Generic;
    using System.Windows.Automation;
    using Orc.Automation;
    using Orc.Automation.Controls;

    [AutomatedControl(Class = typeof(Views.WorkspacesView))]
    public class WorkspacesView : FrameworkElement<WorkspaceViewModel, WorkspaceViewMap>
    {
        public WorkspacesView(AutomationElement element) 
            : base(element)
        {
        }

        public IReadOnlyList<WorkspaceViewGroupItem> GroupItems
            => Map.GroupList.GetGroupItems();
    }
}
