// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkspaceProvider.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement.Test.Mocks
{
    using System.Threading.Tasks;
    using Catel;
    using Catel.Threading;

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
            Argument.IsNotNull(() => workspace);

            workspace.SetWorkspaceValue(_key, _value);
        }

        public Task ProvideInformationAsync(IWorkspace workspace)
        {
            ProvideInformation(workspace);

            return TaskHelper.Completed;
        }

        public void ApplyWorkspace(IWorkspace workspace)
        {
            // location to respond to changes
        }

        public Task ApplyWorkspaceAsync(IWorkspace workspace)
        {
            ApplyWorkspace(workspace);

            return TaskHelper.Completed;

        }
    }
}