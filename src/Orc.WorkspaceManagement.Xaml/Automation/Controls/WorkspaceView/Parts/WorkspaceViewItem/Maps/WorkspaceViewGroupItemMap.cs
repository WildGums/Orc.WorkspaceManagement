namespace Orc.WorkspaceManagement.Automation
{
    using System.Windows.Automation;
    using Orc.Automation;
    using Orc.Automation.Controls;

    public class WorkspaceViewItemMap : AutomationBase
    {
        public WorkspaceViewItemMap(AutomationElement element)
            : base(element)
        {
        }

        public Text Title => By.One<Text>();
        public Button EditWorkspaceButton => By.Id("EditWorkspaceButton").One<Button>();
        public Button RemoveWorkspaceButton => By.Id("RemoveWorkspaceButton").One<Button>();
        public Button RefreshWorkspaceButton => By.Id("RefreshWorkspaceButton").One<Button>();
    }
}
