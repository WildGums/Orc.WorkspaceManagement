namespace Orc.WorkspaceManagement.Automation
{
    using Orc.Automation;

    [ActiveAutomationModel]
    public class WorkspaceViewModel : ControlModel
    {
        public WorkspaceViewModel(AutomationElementAccessor accessor)
            : base(accessor)
        {
        }

        public object Scope { get; set; }
        public bool HasRefreshButton { get; set; }
    }
}
