// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkspaceViewModel.cs" company="WildGums">
//   Copyright (c) 2008 - 2014 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement.ViewModels
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Catel;
    using Catel.Data;
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

            Workspace = workspace;

            Title = !string.IsNullOrEmpty(workspace.Title) ? string.Format(languageService.GetString("WorkspaceManagement_EditWorkspace"), workspace.Title) : languageService.GetString("WorkspaceManagement_CreateNewWorkspace");
        }

        [Model]
        public IWorkspace Workspace { get; private set; }

        [ViewModelToModel("Workspace", "Title")]
        public string WorkspaceTitle { get; set; }

        protected override void ValidateFields(List<IFieldValidationResult> validationResults)
        {
            base.ValidateFields(validationResults);

            if (string.IsNullOrWhiteSpace(WorkspaceTitle))
            {
                validationResults.Add(FieldValidationResult.CreateError(nameof(WorkspaceTitle), "Title is required"));
            }
        }
    }
}