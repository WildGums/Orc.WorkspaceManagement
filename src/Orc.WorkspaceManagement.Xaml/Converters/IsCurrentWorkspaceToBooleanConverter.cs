namespace Orc.WorkspaceManagement.Converters;

using System;
using Catel;
using Catel.MVVM.Converters;
using WorkspaceManagement;

public partial class IsCurrentWorkspaceToBooleanConverter : ValueConverterBase
{
    private readonly IWorkspaceManager _workspaceManager;

    public IsCurrentWorkspaceToBooleanConverter(IWorkspaceManager workspaceManager)
    {
        _workspaceManager = workspaceManager;
    }

    protected override object? Convert(object? value, Type targetType, object? parameter)
    {
        var workspace = value as IWorkspace;
        if (workspace is null)
        {
            return false;
        }

        return ObjectHelper.AreEqual(_workspaceManager.Workspace, workspace);
    }
}
