// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkspaceProvider.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement.Test.Mocks
{
    using Catel;

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
    }
}