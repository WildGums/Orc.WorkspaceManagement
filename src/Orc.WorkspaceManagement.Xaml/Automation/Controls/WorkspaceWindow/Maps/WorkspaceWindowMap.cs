namespace Orc.WorkspaceManagement.Automation;

using System.Windows.Automation;
using Orc.Automation;
using Orc.Automation.Controls;

public class WorkspaceWindowMap : AutomationBase
{
    public WorkspaceWindowMap(AutomationElement element) 
        : base(element)
    {
    }

    //   public Text TitleText => By.One<Text>();
    public Edit? TitleEdit => By.One<Edit>();
    public Button? OkButton => By.Name("OK").One<Button>();
    public Button? CancelButton => By.Name("Cancel").One<Button>();
}