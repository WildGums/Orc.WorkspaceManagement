// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkspacesView.xaml.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement.Views
{
    using System.Windows;
    using Catel.MVVM.Views;

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
           DependencyProperty.Register("ManagerTag", typeof(object), typeof(WorkspacesView), new FrameworkPropertyMetadata(null));

        [ViewToViewModel("Tag", MappingType = ViewToViewModelMappingType.ViewToViewModel)]
        public object ManagerTag
        {
            get { return GetValue(ManagerTagProperty); }
            set { SetValue(ManagerTagProperty, value); }
        }
        #endregion
    }
}