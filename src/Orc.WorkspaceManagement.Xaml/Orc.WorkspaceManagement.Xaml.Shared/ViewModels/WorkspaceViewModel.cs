// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkspaceViewModel.cs" company="WildGums">
//   Copyright (c) 2008 - 2014 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement.ViewModels
{
    using System.Threading.Tasks;
    using Catel;
    using Catel.Fody;
    using Catel.MVVM;
    using Catel.Services;

    public class WorkspaceViewModel : ViewModelBase
    {
        public WorkspaceViewModel(IWorkspace workspace, ILanguageService languageService)
        {
            Argument.IsNotNull(() => workspace);
            Argument.IsNotNull(() => languageService);

            DeferValidationUntilFirstSaveCall = true;
            SuspendValidation = false;

            Workspace = workspace;

            Title = !string.IsNullOrEmpty(workspace.Title) ? string.Format(languageService.GetString("WorkspaceManagement_EditWorkspace"), workspace.Title) : languageService.GetString("WorkspaceManagement_CreateNewWorkspace");
        }

        [Model]
        [Expose("WorkspaceTitle", "Title")]
        public IWorkspace Workspace { get; private set; }
    }
}