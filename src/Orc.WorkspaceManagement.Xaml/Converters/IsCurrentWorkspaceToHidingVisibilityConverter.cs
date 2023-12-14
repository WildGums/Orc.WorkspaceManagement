
namespace Orc.WorkspaceManagement.Converters;

using System;
using System.Windows;
using Catel;
using Catel.IoC;
using Catel.MVVM.Converters;

public class IsCurrentWorkspaceToHidingVisibilityConverter : VisibilityConverterBase
{
#pragma warning disable IDISP006 // Implement IDisposable.
    private readonly IServiceLocator _serviceLocator;
#pragma warning restore IDISP006 // Implement IDisposable.

    public IsCurrentWorkspaceToHidingVisibilityConverter()
        : base(Visibility.Hidden)
    {
        _serviceLocator = this.GetServiceLocator();
    }

    protected override bool IsVisible(object? value, Type targetType, object? parameter)
    {
        if (value is not IWorkspace workspace)
        {
            return false;
        }

        var workspaceManager = _serviceLocator.ResolveType<IWorkspaceManager>(workspace.Scope);

        return workspaceManager is not null && ObjectHelper.AreEqual(workspaceManager.Workspace, workspace);
    }
}
