namespace Orc.WorkspaceManagement
{
    using System.Windows;

    public static partial class FrameworkElementExtensions
    {
        public static void SaveSizeToWorkspace(this FrameworkElement frameworkElement, IWorkspace? workspace = null, string? prefix = null)
        {
            frameworkElement.SaveValueToWorkspace("Width", frameworkElement.ActualWidth, workspace, prefix);
            frameworkElement.SaveValueToWorkspace("Height", frameworkElement.ActualHeight, workspace, prefix);
        }

        public static void LoadSizeFromWorkspace(this FrameworkElement frameworkElement, IWorkspace? workspace = null, string? prefix = null)
        {
            var width = frameworkElement.LoadValueFromWorkspace("Width", frameworkElement.Width, workspace, prefix);
            var height = frameworkElement.LoadValueFromWorkspace("Height", frameworkElement.Height, workspace, prefix);

            frameworkElement.SetCurrentValue(FrameworkElement.WidthProperty, width);
            frameworkElement.SetCurrentValue(FrameworkElement.HeightProperty, height);
        }
    }
}
