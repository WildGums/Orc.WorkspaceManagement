namespace Orc.WorkspaceManagement.Automation
{
    using Orc.Automation;

    public class WorkspaceWindowPeer : AutomationControlPeerBase<Views.WorkspaceWindow>
    {
        public WorkspaceWindowPeer(Views.WorkspaceWindow owner) 
            : base(owner)
        {
        }
    }
}
