// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkspaceViewModel.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement.Example.ViewModels
{
    using Catel;
    using Catel.Fody;
    using Catel.MVVM;

    public class WorkspaceViewModel : ViewModelBase
    {
        public WorkspaceViewModel(IWorkspace workspace)
        {
            Argument.IsNotNull(() => workspace);

            Workspace = workspace;
        }

        [Model]
        [Expose("WorkspaceTitle", "Title")]
        public IWorkspace Workspace { get; private set; }
    }
}