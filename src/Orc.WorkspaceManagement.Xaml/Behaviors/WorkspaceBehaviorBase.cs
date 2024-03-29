﻿namespace Orc.WorkspaceManagement.Behaviors;

using System.Windows;
using Catel.IoC;
using Catel.Services;
using Catel.Windows.Interactivity;

public abstract class WorkspaceBehaviorBase<T> : BehaviorBase<T>, IWorkspaceBehavior
    where T : FrameworkElement
{
    private readonly BehaviorWorkspaceProvider _workspaceProvider;

    protected WorkspaceBehaviorBase()
    {
        var dependencyResolver = this.GetDependencyResolver();
        WorkspaceManager = dependencyResolver.ResolveRequired<IWorkspaceManager>();
        var dispatcherService = dependencyResolver.ResolveRequired<IDispatcherService>();

        _workspaceProvider = new BehaviorWorkspaceProvider(WorkspaceManager, this, dispatcherService, this.GetServiceLocator());
    }

    protected IWorkspaceManager WorkspaceManager { get; private set; }

    public string? KeyPrefix
    {
        get { return (string?)GetValue(KeyPrefixProperty); }
        set { SetValue(KeyPrefixProperty, value); }
    }

    // Using a DependencyProperty as the backing store for KeyPrefix.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty KeyPrefixProperty =
        DependencyProperty.Register(nameof(KeyPrefix), typeof(string), typeof(WorkspaceBehaviorBase<T>), new PropertyMetadata(string.Empty));

    protected override void OnAssociatedObjectLoaded()
    {
        base.OnAssociatedObjectLoaded();

        WorkspaceManager.AddProvider(_workspaceProvider);

        var workspace = WorkspaceManager.Workspace;
        if (workspace is not null)
        {
            LoadSettings(workspace, KeyPrefix);
        }
    }

    protected override void OnAssociatedObjectUnloaded()
    {
        WorkspaceManager.RemoveProvider(_workspaceProvider);

        base.OnAssociatedObjectUnloaded();
    }

    protected abstract void SaveSettings(IWorkspace workspace, string? prefix);

    protected abstract void LoadSettings(IWorkspace workspace, string? prefix);

    public void Load(IWorkspace workspace)
    {
        LoadSettings(workspace, KeyPrefix);
    }

    public void Save(IWorkspace workspace)
    {
        SaveSettings(workspace, KeyPrefix);
    }
}