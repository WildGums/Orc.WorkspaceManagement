namespace Orc.WorkspaceManagement.Behaviors
{
    using System.Windows;
    using System.Windows.Controls;

    class WorkspaceContainerLocatorDeault : IWorkspaceContainerLocator
    {
        public Panel GetContainerByWorkspaceParent(UIElement parent)
        {
            return null;
        }
    }
}