namespace Orc.WorkspaceManagement.Test.Mocks
{
    using System;
    using System.Threading.Tasks;

    public class WorkspaceProvider : IWorkspaceProvider
    {
        private readonly string _key;
        private readonly string _value;

        public WorkspaceProvider(string key, string value)
        {
            _key = key;
            _value = value;
        }

        public void ProvideInformation(IWorkspace workspace)
        {
            ArgumentNullException.ThrowIfNull(workspace);

            workspace.SetWorkspaceValue(_key, _value);
        }

        public object Scope { get; set; }

        public object Tag { get; set; }

        public Task ProvideInformationAsync(IWorkspace workspace)
        {
            ProvideInformation(workspace);

            return Task.CompletedTask;
        }

        public void ApplyWorkspace(IWorkspace workspace)
        {
            // location to respond to changes
        }

        public Task ApplyWorkspaceAsync(IWorkspace workspace)
        {
            ApplyWorkspace(workspace);

            return Task.CompletedTask;

        }

        public Task<bool> CheckIsDirtyAsync(IWorkspace workspace)
        {
            return Task.FromResult(false);
        }
    }
}
