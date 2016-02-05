// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkspacesView.xaml.cs" company="WildGums">
//   Copyright (c) 2008 - 2014 WildGums. All rights reserved.
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
           DependencyProperty.Register("ManagerTag", typeof(object), typeof(WorkspacesView), new FrameworkPropertyMetadata(OnManagerTagChanged));

        private static void OnManagerTagChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
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