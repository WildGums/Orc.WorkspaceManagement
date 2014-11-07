namespace Orc.WorkspaceManagement.Example.Views
{
    using Catel.Logging;
    using Catel.Windows;
    using Logging;

    /// <summary>
    /// Interaction logic for MainWindow.xaml.
    /// </summary>
    public partial class MainWindow : DataWindow
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
            : base(DataWindowMode.Custom)
        {
            InitializeComponent();

            //LogManager.AddListener(new TextBoxLogListener(outputTextBox));

            Log.Info("Welcome to the example of Orc.WorkspaceManagement. Use any of the buttons above to control the workspace. Log messages will appear here");
        }
    }
}
