namespace Orc.WorkspaceManagement.Automation
{
    using System.Windows.Automation.Peers;
    using Orc.Automation;

    public class WorkspaceWindowPeer : AutomationControlPeerBase<Views.WorkspaceWindow>
    {
        public WorkspaceWindowPeer(Views.WorkspaceWindow owner) 
            : base(owner)
        {
        }

        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Window;
        }
    }
}
