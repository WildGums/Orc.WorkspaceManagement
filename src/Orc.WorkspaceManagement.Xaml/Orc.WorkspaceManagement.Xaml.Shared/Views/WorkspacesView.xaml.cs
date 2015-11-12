// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkspacesView.xaml.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement.Views
{
    using System.Runtime.CompilerServices;
    using System.Windows;
    using Catel.MVVM.Views;
    using ViewModels;

    /// <summary>
    /// Interaction logic for WorkspacesView.xaml.
    /// </summary>
    public partial class WorkspacesView
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkspacesView"/> class.
        /// </summary>
        public WorkspacesView()
        {
            InitializeComponent();
        }
        #endregion


        #region Properties
        public static readonly DependencyProperty ManagerTagProperty =
           DependencyProperty.Register("ManagerTag", typeof(object), typeof(WorkspacesView), new FrameworkPropertyMetadata(OnTagChanged));

        private static void OnTagChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var workspacesView = d as WorkspacesView;
            if (workspacesView != null)
            {
                var viewModel = (WorkspacesViewModel)workspacesView.ViewModel;
                if (viewModel != null)
                {
                    viewModel.ManagerTag = workspacesView.ManagerTag;
                }
            }
        }

        [ViewToViewModel(MappingType = ViewToViewModelMappingType.ViewToViewModel)]
        public object ManagerTag
        {
            get { return GetValue(ManagerTagProperty); }
            set { SetValue(ManagerTagProperty, value); }
        }
        #endregion
    }
}