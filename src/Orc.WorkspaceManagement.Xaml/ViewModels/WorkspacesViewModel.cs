namespace Orc.WorkspaceManagement.ViewModels;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Catel;
using Catel.Data;
using Catel.IoC;
using Catel.Logging;
using Catel.MVVM;
using Catel.Services;
using Microsoft.Extensions.Logging;

public class WorkspacesViewModel : ViewModelBase
{
    private static readonly ILogger Logger = LogManager.GetLogger(typeof(WorkspaceViewModel));

    private readonly IUIVisualizerService _uiVisualizerService;
    private readonly IDispatcherService _dispatcherService;
    private readonly IMessageService _messageService;
    private readonly ILanguageService _languageService;

    private readonly IWorkspaceManager _workspaceManager;

    public WorkspacesViewModel(IServiceProvider serviceProvider, IWorkspaceManager workspaceManager,
        IUIVisualizerService uiVisualizerService, IDispatcherService dispatcherService, IMessageService messageService,
        ILanguageService languageService)
        : base(serviceProvider)
    {
        _workspaceManager = workspaceManager;
        _uiVisualizerService = uiVisualizerService;
        _dispatcherService = dispatcherService;
        _messageService = messageService;
        _languageService = languageService;

        WorkspaceGroups = new List<WorkspaceGroup>();

        EditWorkspace = new TaskCommand<IWorkspace>(serviceProvider, OnEditWorkspaceExecuteAsync, OnEditWorkspaceCanExecute);
        RemoveWorkspace = new TaskCommand<IWorkspace>(serviceProvider, OnRemoveWorkspaceExecuteAsync, OnRemoveWorkspaceCanExecute);
        Refresh = new TaskCommand<IWorkspace>(serviceProvider, OnRefreshAsync, OnRefreshCanExecute);
    }

    public List<WorkspaceGroup> WorkspaceGroups { get; private set; }

    public IWorkspace? SelectedWorkspace
    {
        get => _workspaceManager?.Workspace;
        set
        {
            if (value is not null && _workspaceManager is not null)
            {
                _dispatcherService.InvokeTaskAsync(async () => await _workspaceManager.TrySetWorkspaceAsync(value))
                    .ContinueWith(_ => RaiseSelectedWorkspaceChanged());
            }
        }
    }

    public TaskCommand<IWorkspace> Refresh { get; private set; }

    private bool OnRefreshCanExecute(IWorkspace? workspace)
    {
        if (workspace is null)
        {
            return false;
        }

        return workspace.IsDirty;
    }

    private async Task OnRefreshAsync(IWorkspace? workspace)
    {
        if (workspace is null)
        {
            return;
        }

        if (!workspace.IsDirty)
        {
            return;
        }

        if (_workspaceManager is null)
        {
            return;
        }

        await _workspaceManager.RefreshWorkspaceAsync(workspace);
    }

    public TaskCommand<IWorkspace> EditWorkspace { get; private set; }

    private bool OnEditWorkspaceCanExecute(IWorkspace? workspace)
    {
        if (workspace is null)
        {
            return false;
        }

        if (!workspace.Persist)
        {
            return false;
        }

        if (!workspace.CanEdit)
        {
            return false;
        }

        return true;
    }

    private Task OnEditWorkspaceExecuteAsync(IWorkspace? workspace)
    {
        if (workspace is null)
        {
            return Task.CompletedTask;
        }

        if (_workspaceManager is null)
        {
            return Task.CompletedTask;
        }

        var modelValidation = workspace as IValidatable;

        EventHandler<ValidationEventArgs>? handler = null;
        handler = (sender, e) =>
        {
            if (_workspaceManager.Workspaces.Any(x => x.Title.EqualsIgnoreCase(workspace.Title) && x != workspace))
            {
                e.ValidationContext.Add(FieldValidationResult.CreateError(nameof(Title),
                    _languageService.GetRequiredString("WorkspaceManagement_WorkspaceWithCurrentTitleAlreadyExists")));
            }
        };

        if (modelValidation is not null)
        {
            modelValidation.Validating += handler;
        }

        // Dispatch to make sure this plays nice with Fluent.Ribbon dropdowns
#pragma warning disable AvoidAsyncVoid // Avoid async void
        _dispatcherService.BeginInvoke(async () =>
#pragma warning restore AvoidAsyncVoid // Avoid async void
        {
            await Task.Delay(50);

            var result = await _uiVisualizerService.ShowDialogAsync<WorkspaceViewModel>(workspace);
            if (result.DialogResult ?? false)
            {
                if (modelValidation is not null)
                {
                    modelValidation.Validating -= handler;
                }

                await _workspaceManager.SaveAsync();
            }
        }, false);

        return Task.CompletedTask;
    }

    public TaskCommand<IWorkspace> RemoveWorkspace { get; private set; }

    private bool OnRemoveWorkspaceCanExecute(IWorkspace? workspace)
    {
        if (workspace is null)
        {
            return false;
        }

        if (!workspace.CanDelete)
        {
            return false;
        }

        return true;
    }

    private Task OnRemoveWorkspaceExecuteAsync(IWorkspace? workspace)
    {
        if (workspace is null)
        {
            return Task.CompletedTask;
        }

        if (_workspaceManager is null)
        {
            return Task.CompletedTask;
        }

        // Dispatch to make sure this plays nice with Fluent.Ribbon dropdowns
#pragma warning disable AvoidAsyncVoid // Avoid async void
        _dispatcherService.BeginInvoke(async () =>
#pragma warning restore AvoidAsyncVoid // Avoid async void
        {
            await Task.Delay(50);

            if (await _messageService.ShowAsync(string.Format(_languageService.GetRequiredString("WorkspaceManagement_AreYouSureYouWantToRemoveTheWorkspace"), workspace.Title),
                    _languageService.GetRequiredString("WorkspaceManagement_AreYouSure"), MessageButton.YesNo, MessageImage.Question) == MessageResult.No)
            {
                return;
            }

            await _workspaceManager.RemoveAsync(workspace);
            await _workspaceManager.SaveAsync();
        }, false);

        return Task.CompletedTask;
    }

    private void RaiseSelectedWorkspaceChanged()
    {
        _dispatcherService.Invoke(() => RaisePropertyChanged(nameof(SelectedWorkspace)));
    }

    protected override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        _workspaceManager.WorkspaceUpdated += OnWorkspacesChanged;
    }

    protected override Task CloseAsync()
    {
        _workspaceManager.WorkspaceUpdated -= OnWorkspacesChanged;

        return base.CloseAsync();
    }

    private void OnWorkspacesChanged(object? sender, EventArgs e)
    {
        Logger.LogDebug($"Workspaces have changed, updating workspaces");

        UpdateWorkspaces();
    }

    private bool _updatingWorkspace;

    private void UpdateWorkspaces()
    {
        if (_updatingWorkspace)
        {
            return;
        }

        var workspaceManager = _workspaceManager;
        if (workspaceManager is null)
        {
            return;
        }

        _updatingWorkspace = true;

        try
        {
            var workspaceGroups = (from workspace in workspaceManager.Workspaces
                                   where workspace.IsVisible
                                   orderby workspace.WorkspaceGroup, workspace.Title, workspace.CanDelete
                                   group workspace by workspace.WorkspaceGroup into g
                                   select new WorkspaceGroup(string.IsNullOrWhiteSpace(g.Key) ? null : g.Key, g)).ToList();

            WorkspaceGroups = workspaceGroups;

            RaiseSelectedWorkspaceChanged();
        }
        finally
        {
            _updatingWorkspace = false;
        }
    }
}
