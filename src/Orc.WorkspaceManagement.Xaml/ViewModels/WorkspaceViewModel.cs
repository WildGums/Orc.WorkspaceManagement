namespace Orc.WorkspaceManagement.ViewModels;

using System;
using System.Collections.Generic;
using Catel.Data;
using Catel.MVVM;
using Catel.Services;

public class WorkspaceViewModel : ViewModelBase
{
    public WorkspaceViewModel(IWorkspace workspace, ILanguageService languageService)
    {
        ArgumentNullException.ThrowIfNull(workspace);
        ArgumentNullException.ThrowIfNull(languageService);

        DeferValidationUntilFirstSaveCall = true;

        Workspace = workspace;

        Title = !string.IsNullOrEmpty(workspace.Title) ? string.Format(languageService.GetRequiredString("WorkspaceManagement_EditWorkspace"), workspace.Title) : languageService.GetRequiredString("WorkspaceManagement_CreateNewWorkspace");
    }

    [Model]
    public IWorkspace? Workspace { get; private set; }

    [ViewModelToModel(nameof(Workspace), nameof(IWorkspace.Title))]
    public string? WorkspaceTitle { get; set; }

    protected override void ValidateFields(List<IFieldValidationResult> validationResults)
    {
        base.ValidateFields(validationResults);

        if (string.IsNullOrWhiteSpace(WorkspaceTitle))
        {
            validationResults.Add(FieldValidationResult.CreateError(nameof(WorkspaceTitle), "Title is required"));
        }
    }
}
