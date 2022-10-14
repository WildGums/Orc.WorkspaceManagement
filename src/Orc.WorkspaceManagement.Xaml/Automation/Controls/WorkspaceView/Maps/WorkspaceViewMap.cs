namespace Orc.WorkspaceManagement.Automation
{
    using System.Windows.Automation;
    using Orc.Automation;

    public class WorkspaceViewMap : AutomationBase
    {
        public WorkspaceViewMap(AutomationElement element) 
            : base(element)
        {
        }

        public WorkspaceViewGroupList? GroupList => By.One<WorkspaceViewGroupList>();
    }
}
