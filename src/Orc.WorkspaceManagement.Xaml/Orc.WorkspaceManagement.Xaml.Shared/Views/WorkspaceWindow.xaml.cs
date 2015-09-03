// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkspaceWindow.xaml.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement.Views
{
    using Catel.Windows;
    using ViewModels;

    /// <summary>
    /// Interaction logic for WorkspaceWindow.xaml.
    /// </summary>
    public partial class WorkspaceWindow : DataWindow
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