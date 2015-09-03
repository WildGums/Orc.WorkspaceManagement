// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RibbonViewModel.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2015 Orchestra development team. All rights reserved.
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

        public RibbonViewModel(IWorkspaceManager workspaceManager, IViewModelFactory viewModelFactory, IUIVisualizerService uiVisualizerService, ISelectDirectoryService selectDirectoryService)
        {
            _workspaceManager = workspaceManager;
            _viewModelFactory = viewModelFactory;
            _uiVisualizerService = uiVisualizerService;
            _selectDirectoryService = selectDirectoryService;

            AddWorkspace = new TaskCommand(OnAddWorkspaceExecuteAsync);
            SaveWorkspace = new TaskCommand(OnSaveWorkspaceExecuteAsync, OnSaveWorkspaceCanExecute);

            EditWorkspace = new Command(OnEditWorkspaceExecute, OnEditWorkspaceCanExecute);
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

            if (_uiVisualizerService.ShowDialog<WorkspaceViewModel>(workspace) ?? false)
            {
                await _workspaceManager.AddAsync(workspace, true);
                _workspaceManager.Save();
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

        public Command EditWorkspace { get; private set; }

        private bool OnEditWorkspaceCanExecute()
        {
            return (CurrentWorkspace != null);
        }

        private void OnEditWorkspaceExecute()
        {
            _uiVisualizerService.ShowDialog<WorkspaceViewModel>(CurrentWorkspace);
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

        public Task OnChooseBaseDirectoryAsync()
        {
            _selectDirectoryService.ShowNewFolderButton = true;

            if (_selectDirectoryService.DetermineDirectory())
            {
                return _workspaceManager.SetWorkspaceSchemesDirectoryAsync(_selectDirectoryService.DirectoryName);
            }

            return TaskHelper.Completed;
        }
        #endregion

        #region Methods
        protected override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            _workspaceManager.WorkspaceUpdated += OnCurrentWorkspaceChanged;

            _workspaceManager.InitializeAsync(true);

            UpdateCurrentWorkspace();
        }

        protected override async Task CloseAsync()
        {
            _workspaceManager.WorkspaceUpdated -= OnCurrentWorkspaceChanged;

            await base.CloseAsync();
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