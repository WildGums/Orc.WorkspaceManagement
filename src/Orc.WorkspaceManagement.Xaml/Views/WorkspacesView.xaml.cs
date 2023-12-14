namespace Orc.WorkspaceManagement.Views;

using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Input;
using Automation;
using Catel.MVVM.Views;
using ViewModels;

public partial class WorkspacesView
{
    public WorkspacesView()
    {
        InitializeComponent();
    }

    [ViewToViewModel(MappingType = ViewToViewModelMappingType.ViewToViewModel)]
    public object? Scope
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

    private void OnScopeChanged(DependencyPropertyChangedEventArgs e)
    {
        if (ViewModel is WorkspacesViewModel vm)
        {
            vm.Scope = Scope;
        }
    }

    private void OnWorkspacePreviewMouseLeftButtonDown(object? sender, MouseButtonEventArgs e)
    {
        // Don't handle if source is a button
        if (e.Source is Button)
        {
            return;
        }

        var fxElement = sender as FrameworkElement;
        if (fxElement?.DataContext is not IWorkspace workspace)
        {
            return;
        }

        if (ViewModel is WorkspacesViewModel vm)
        {
            vm.SelectedWorkspace = workspace;
        }
    }

    private void UIElement_OnPreviewMouseWheel(object? sender, MouseWheelEventArgs e)
    {
        e.Handled = true;
          
        var mouseWheelEventArgs = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
        {
            RoutedEvent = UIElement.MouseWheelEvent
        };

        MainScroll.RaiseEvent(mouseWheelEventArgs);
    }

    protected override AutomationPeer OnCreateAutomationPeer()
    {
        return new WorkspaceViewPeer(this);
    }
}