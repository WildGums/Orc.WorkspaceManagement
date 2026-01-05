
namespace Orc.WorkspaceManagement.Converters;

using System;
using System.Windows;
using Catel;
using Catel.IoC;
using Catel.MVVM.Converters;

public partial class IsCurrentWorkspaceToHidingVisibilityConverter : VisibilityConverterBase
{
    private readonly IWorkspaceManager _workspaceManager;

    public IsCurrentWorkspaceToHidingVisibilityConverter(IWorkspaceManager workspaceManager)
        : base(Visibility.Hidden)
    {
        _workspaceManager = workspaceManager;
    }

    protected override bool IsVisible(object? value, Type targetType, object? parameter)
    {
        if (value is not IWorkspace workspace)
        {
            return false;
        }

        return ObjectHelper.AreEqual(_workspaceManager.Workspace, workspace);
    }
}
