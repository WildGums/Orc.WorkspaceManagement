// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkspacesView.xaml.cs" company="WildGums">
//   Copyright (c) 2008 - 2014 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orc.WorkspaceManagement.Views
{
    using System;
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
        [ViewToViewModel(MappingType = ViewToViewModelMappingType.ViewToViewModel)]
        public object Scope
        {
            get { return GetValue(ScopeProperty); }
            set { SetValue(ScopeProperty, value); }
        }

        public static readonly DependencyProperty ScopeProperty = DependencyProperty.Register("Scope", typeof(object),
            typeof(WorkspacesView), new FrameworkPropertyMetadata((sender, e) => ((WorkspacesView)sender).OnScopeChanged(e)));
        #endregion

        #region Methods
        private void OnScopeChanged(DependencyPropertyChangedEventArgs e)
        {
            var vm = ViewModel as WorkspacesViewModel;
            if (vm != null)
            {
                vm.Scope = Scope;
            }
        }

        protected override void OnLoaded(EventArgs e)
        {
            base.OnLoaded(e);
        }

        protected override void OnViewModelChanged()
        {
            base.OnViewModelChanged();
        }
        #endregion
    }
}