// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RibbonViewModel.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement.Example.ViewModels
{
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

        public RibbonViewModel(IWorkspaceManager workspaceManager, IViewModelFactory viewModelFactory,
            IUIVisualizerService uiVisualizerService, ISelectDirectoryService selectDirectoryService, IMessageService messageService)
        {
            _workspaceManager = workspaceManager;
            _viewModelFactory = viewModelFactory;
            _uiVisualizerService = uiVisualizerService;
            _selectDirectoryService = selectDirectoryService;
            _messageService = messageService;

            AddWorkspace = new TaskCommand(OnAddWorkspaceExecuteAsync);
            SaveWorkspace = new TaskCommand(OnSaveWorkspaceExecuteAsync, OnSaveWorkspaceCanExecute);

            EditWorkspace = new TaskCommand(OnEditWorkspaceExecuteAsync, OnEditWorkspaceCanExecute);
            RemoveWorkspace = new TaskCommand(OnRemoveWorkspaceExecuteAsync, OnRemoveWorkspaceCanExecute);
            ChooseBaseDirectory = new TaskCommand(OnChooseBaseDirectoryAsync);
        }

        #region Properties
        public IWorkspace CurrentWorkspace { get; private set; }
        #endregion

        #region Commands
        public TaskCommand AddWorkspace { get; private set; }

        private async Task OnAddWorkspaceExecuteAsync()
        {
            var workspace = new Workspace();

            if (await _uiVisualizerService.ShowDialogAsync<WorkspaceViewModel>(workspace) ?? false)
            {
                var existingWorkspace = _workspaceManager.FindWorkspace(workspace.Title);
                if (existingWorkspace != null)
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
            return (CurrentWorkspace != null);
        }

        private async Task OnSaveWorkspaceExecuteAsync()
        {
            await _workspaceManager.StoreAndSaveAsync();
            UpdateCurrentWorkspace();
        }

        public TaskCommand EditWorkspace { get; private set; }

        private bool OnEditWorkspaceCanExecute()
        {
            return (CurrentWorkspace != null);
        }

        private async Task OnEditWorkspaceExecuteAsync()
        {
            await _uiVisualizerService.ShowDialogAsync<WorkspaceViewModel>(CurrentWorkspace);
        }

        public TaskCommand RemoveWorkspace { get; private set; }

        private bool OnRemoveWorkspaceCanExecute()
        {
            if (CurrentWorkspace == null)
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
            _selectDirectoryService.ShowNewFolderButton = true;

            if (await _selectDirectoryService.DetermineDirectoryAsync())
            {
                await _workspaceManager.SetWorkspaceSchemesDirectoryAsync(_selectDirectoryService.DirectoryName);
            }
        }
        #endregion

        #region Methods
        protected override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            _workspaceManager.WorkspaceUpdated += OnCurrentWorkspaceChanged;

            await _workspaceManager.InitializeAsync(true);

            UpdateCurrentWorkspace();
        }

        protected override Task CloseAsync()
        {
            _workspaceManager.WorkspaceUpdated -= OnCurrentWorkspaceChanged;

            return base.CloseAsync();
        }

        private void OnCurrentWorkspaceChanged(object sender, WorkspaceUpdatedEventArgs e)
        {
            UpdateCurrentWorkspace();
        }

        private void UpdateCurrentWorkspace()
        {
            CurrentWorkspace = _workspaceManager.Workspace;
        }
        #endregion
    }
}