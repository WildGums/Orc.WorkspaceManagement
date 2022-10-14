namespace Orc.WorkspaceManagement.Example.ViewModels
{
    using Catel.Logging;
    using Catel.MVVM;

    /// <summary>
    /// MainWindow view model.
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        public MainViewModel()
        {
            Title = "Orc.WorkspaceManagement example";
        }
    }
}
