namespace Orc.WorkspaceManagement.Converters;

using System;
using Catel;
using Catel.MVVM.Converters;

public partial class IsCurrentWorkspaceToCollapsingVisibilityConverter : VisibilityConverterBase
{
    private readonly IWorkspaceManager _workspaceManager;

    public IsCurrentWorkspaceToCollapsingVisibilityConverter(IWorkspaceManager workspaceManager)
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
