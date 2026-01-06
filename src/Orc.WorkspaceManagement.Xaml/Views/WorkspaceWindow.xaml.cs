namespace Orc.WorkspaceManagement.Views;

using System.Windows.Automation.Peers;
using Automation;

public partial class WorkspaceWindow
{
    partial void OnInitializingComponent()
    {
        Mode = Catel.Windows.DataWindowMode.OkCancel;
    }

    protected override AutomationPeer OnCreateAutomationPeer()
    {
        return new WorkspaceWindowPeer(this);
    }
}
