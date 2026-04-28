namespace Orc.WorkspaceManagement;

using System.Collections.Generic;

public interface IWorkspace
{
    string Title { get; }

    bool Persist { get; set; }
    bool CanEdit { get; set; }
    bool CanDelete { get; set; }
    bool IsVisible { get; set; }
    bool IsDirty { get; }

    string? WorkspaceGroup { get; set; }
    string? DisplayName { get; }

    void SetWorkspaceValue(string name, object? value);
    T GetWorkspaceValue<T>(string name, T defaultValue);
    IReadOnlyList<string> GetAllWorkspaceValueNames();
    void ClearWorkspaceValues();
    void UpdateIsDirtyFlag(bool isDirty);
}
