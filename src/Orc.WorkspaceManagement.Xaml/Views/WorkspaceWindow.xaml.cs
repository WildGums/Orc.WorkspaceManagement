namespace Orc.WorkspaceManagement.Views
{
    using ViewModels;

    /// <summary>
    /// Interaction logic for WorkspaceWindow.xaml.
    /// </summary>
    public partial class WorkspaceWindow
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkspaceWindow"/> class.
        /// </summary>
        public WorkspaceWindow()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkspaceWindow"/> class.
        /// </summary>
        /// <param name="viewModel">The view model to inject.</param>
        /// <remarks>
        /// This constructor can be used to use view-model injection.
        /// </remarks>
        public WorkspaceWindow(WorkspaceViewModel viewModel)
            : base(viewModel)
        {
            InitializeComponent();
        }
        #endregion
    }
}
