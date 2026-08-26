namespace Orc.WorkspaceManagement.ViewModels;

using System;
using System.Collections.Generic;
using Catel.Data;
using Catel.MVVM;
using Catel.Services;

public class WorkspaceViewModel : FeaturedViewModelBase
{
    private readonly ILanguageService _languageService;

    public WorkspaceViewModel(IWorkspace workspace, IServiceProvider serviceProvider, ILanguageService languageService)
        : base(serviceProvider)
    {
        _languageService = languageService;

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
            validationResults.Add(FieldValidationResult.CreateError(nameof(WorkspaceTitle), _languageService.GetRequiredString("WorkspaceManagement_TitleIsRequired")));
        }
    }
}
