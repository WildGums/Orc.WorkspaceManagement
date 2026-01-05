namespace Orc.WorkspaceManagement.Views;

using System.Windows.Automation.Peers;
using Automation;
using WorkspaceViewModel = ViewModels.WorkspaceViewModel;

/// <summary>
/// Interaction logic for WorkspaceWindow.xaml.
/// </summary>
public partial class WorkspaceWindow
{
    protected override AutomationPeer OnCreateAutomationPeer()
    {
        return new WorkspaceWindowPeer(this);
    }
}
