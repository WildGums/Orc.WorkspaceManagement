namespace Orc.WorkspaceManagement.Behaviors;

public interface IWorkspaceBehavior
{
    void Load(IWorkspace workspace);
    void Save(IWorkspace workspace);
}