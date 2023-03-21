namespace Orc.WorkspaceManagement.Automation;

using Orc.Automation;

public class WorkspaceViewPeer : AutomationControlPeerBase<Views.WorkspacesView>
{
    public WorkspaceViewPeer(Views.WorkspacesView owner) 
        : base(owner)
    {
    }
}