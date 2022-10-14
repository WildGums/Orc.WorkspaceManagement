namespace Orc.WorkspaceManagement.Automation
{
    using System.Windows.Automation;
    using Orc.Automation;
    using Orc.Automation.Controls;

    [AutomatedControl(Class = typeof(Views.WorkspaceWindow), ControlTypeName = nameof(ControlType.Window))]
    public class WorkspaceWindow : Window
    {
        public WorkspaceWindow(AutomationElement element) 
            : base(element)
        {
        }

        private WorkspaceWindowMap Map => Map<WorkspaceWindowMap>();

        public string? Title
        {
            get => Map.TitleEdit?.Text;
            set
            {
                var titleEdit = Map.TitleEdit;
                if (titleEdit is null)
                {
                    return;
                }

                titleEdit.Text = value ?? string.Empty;
            }
        }
        public void Accept() => Map.OkButton?.Click();
        public void Decline() => Map.CancelButton?.Click();
    }
}
