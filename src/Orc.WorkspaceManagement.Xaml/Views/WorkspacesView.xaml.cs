// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkspacesView.xaml.cs" company="WildGums">
//   Copyright (c) 2008 - 2014 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orc.WorkspaceManagement.Views
{
    using System;
    using System.ComponentModel;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using Catel.MVVM.Views;
    using Catel.Windows;
    using ViewModels;

    public partial class WorkspacesView
    {
        #region Constructors
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

        public static readonly DependencyProperty ScopeProperty = DependencyProperty.Register(nameof(Scope), typeof(object),
            typeof(WorkspacesView), new FrameworkPropertyMetadata((sender, e) => ((WorkspacesView)sender).OnScopeChanged(e)));

        public bool HasRefreshButton
        {
            get { return (bool) GetValue(HasRefreshButtonProperty); }
            set { SetValue(HasRefreshButtonProperty, value); }
        }

        public static readonly DependencyProperty HasRefreshButtonProperty = DependencyProperty.Register(nameof(HasRefreshButton), 
            typeof(bool), typeof(WorkspacesView), new PropertyMetadata(false));
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
        #endregion

        private void OnWorkspacePreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // Don't handle if source is a button
            if (e.Source is Button)
            {
                return;
            }

            var workspace = ((FrameworkElement)sender).DataContext as IWorkspace;
            if (workspace == null)
            {
                return;
            }

            if (ViewModel is WorkspacesViewModel vm)
            {
                vm.SelectedWorkspace = workspace;
            }
        }

        private void UIElement_OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;
          
            var mouseWheelEventArgs = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
            {
                RoutedEvent = UIElement.MouseWheelEvent
            };

            MainScroll.RaiseEvent(mouseWheelEventArgs);
        }
    }
}
