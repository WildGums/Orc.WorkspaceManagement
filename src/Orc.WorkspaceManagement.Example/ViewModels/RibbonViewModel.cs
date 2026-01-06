namespace Orc.WorkspaceManagement.Example.ViewModels;

using System;
using System.Linq;
using System.Threading.Tasks;
using Catel.MVVM;
using Catel.Services;
using Catel.Threading;
using Orc.WorkspaceManagement.ViewModels;

public class RibbonViewModel : ViewModelBase
{
    private readonly ISelectDirectoryService _selectDirectoryService;
    private readonly IUIVisualizerService _uiVisualizerService;
    private readonly IViewModelFactory _viewModelFactory;
    private readonly IWorkspaceManager _workspaceManager;
    private readonly IMessageService _messageService;
    private readonly IDispatcherService _dispatcherService;

    public RibbonViewModel(IServiceProvider serviceProvider, IWorkspaceManager workspaceManager, 
        IViewModelFactory viewModelFactory, IUIVisualizerService uiVisualizerService, 
        ISelectDirectoryService selectDirectoryService, IMessageService messageService,
        IDispatcherService dispatcherService)
        : base(serviceProvider)
    {
        _workspaceManager = workspaceManager;
        _viewModelFactory = viewModelFactory;
        _uiVisualizerService = uiVisualizerService;
        _selectDirectoryService = selectDirectoryService;
        _messageService = messageService;
        _dispatcherService = dispatcherService;

        AddWorkspace = new TaskCommand(serviceProvider, OnAddWorkspaceExecuteAsync);
        SaveWorkspace = new TaskCommand(serviceProvider, OnSaveWorkspaceExecuteAsync, OnSaveWorkspaceCanExecute);

        EditWorkspace = new TaskCommand(serviceProvider, OnEditWorkspaceExecuteAsync, OnEditWorkspaceCanExecute);
        RemoveWorkspace = new TaskCommand(serviceProvider, OnRemoveWorkspaceExecuteAsync, OnRemoveWorkspaceCanExecute);
        ChooseBaseDirectory = new TaskCommand(serviceProvider, OnChooseBaseDirectoryAsync);
    }

    public IWorkspace? CurrentWorkspace { get; private set; }

    public TaskCommand AddWorkspace { get; private set; }

    private async Task OnAddWorkspaceExecuteAsync()
    {
        var workspace = new Workspace();

        var result = await _uiVisualizerService.ShowDialogAsync<WorkspaceViewModel>(workspace);
        if (result.DialogResult ?? false)
        {
            var existingWorkspace = _workspaceManager.FindWorkspace(workspace.Title);
            if (existingWorkspace is not null)
            {
                if (await _messageService.ShowAsync(
                        $"Workspace '{workspace}' already exists. Are you sure you want to overwrite the existing workspace?",
                        "Are you sure?",
                        MessageButton.YesNo) != MessageResult.Yes)
                {
                    return;
                }

                await _workspaceManager.RemoveAsync(existingWorkspace);
            }

            await _workspaceManager.AddAsync(workspace, true);
            await _workspaceManager.SaveAsync();
        }
    }

    public TaskCommand SaveWorkspace { get; private set; }

    private bool OnSaveWorkspaceCanExecute()
    {
        return (CurrentWorkspace is not null);
    }

    private async Task OnSaveWorkspaceExecuteAsync()
    {
        await _workspaceManager.StoreAndSaveAsync();
        UpdateCurrentWorkspace();
    }

    public TaskCommand EditWorkspace { get; private set; }

    private bool OnEditWorkspaceCanExecute()
    {
        return (CurrentWorkspace is not null);
    }

    private async Task OnEditWorkspaceExecuteAsync()
    {
        await _uiVisualizerService.ShowDialogAsync<WorkspaceViewModel>(CurrentWorkspace);
    }

    public TaskCommand RemoveWorkspace { get; private set; }

    private bool OnRemoveWorkspaceCanExecute()
    {
        if (CurrentWorkspace is null)
        {
            return false;
        }

        if (_workspaceManager.Workspaces.Count() <= 1)
        {
            return false;
        }

        return true;
    }

    private async Task OnRemoveWorkspaceExecuteAsync()
    {
        await _workspaceManager.RemoveAsync(CurrentWorkspace);
        UpdateCurrentWorkspace();
    }

    public TaskCommand ChooseBaseDirectory { get; private set; }

    public async Task OnChooseBaseDirectoryAsync()
    {
        var result = await _selectDirectoryService.DetermineDirectoryAsync(new DetermineDirectoryContext
        {
            ShowNewFolderButton = true,
        });

        if (result.Result)
        {
            await _workspaceManager.SetWorkspaceSchemesDirectoryAsync(result.DirectoryName);
        }
    }
        
    protected override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        _workspaceManager.WorkspaceUpdated += OnCurrentWorkspaceChanged;

        // Dispatch to load other views first
        _dispatcherService.BeginInvoke(async () =>
        {
            await Task.Delay(100);

            await _workspaceManager.InitializeAsync(true);

            UpdateCurrentWorkspace();
        });
    }

    protected override Task CloseAsync()
    {
        _workspaceManager.WorkspaceUpdated -= OnCurrentWorkspaceChanged;

        return base.CloseAsync();
    }

    private void OnCurrentWorkspaceChanged(object? sender, WorkspaceUpdatedEventArgs e)
    {
        UpdateCurrentWorkspace();
    }

    private void UpdateCurrentWorkspace()
    {
        CurrentWorkspace = _workspaceManager.Workspace;
    }
}
