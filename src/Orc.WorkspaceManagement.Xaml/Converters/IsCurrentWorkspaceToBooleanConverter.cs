namespace Orc.WorkspaceManagement.Converters
{
    using System;
    using Catel;
    using Catel.IoC;
    using Catel.MVVM.Converters;
    using Orc.WorkspaceManagement;

    public class IsCurrentWorkspaceToBooleanConverter : ValueConverterBase
    {
        private readonly IWorkspaceManager _workspaceManager;

        public IsCurrentWorkspaceToBooleanConverter()
        {
            var dependencyResolver = this.GetDependencyResolver();
            _workspaceManager = dependencyResolver.ResolveRequired<IWorkspaceManager>();
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
}
