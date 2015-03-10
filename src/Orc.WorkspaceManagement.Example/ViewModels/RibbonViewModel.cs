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

            AddWorkspace = new Command(OnAddWorkspaceExecute);
            SaveWorkspace = new Command(() => OnSaveWorkspaceExecute(), OnSaveWorkspaceCanExecute);

            EditWorkspace = new Command(OnEditWorkspaceExecute, OnEditWorkspaceCanExecute);
            RemoveWorkspace = new Command(OnRemoveWorkspaceExecute, OnRemoveWorkspaceCanExecute);
            ChooseBaseDirectory = new Command(OnChooseBaseDirectory);
        }

        #region Properties
        public IWorkspace CurrentWorkspace { get; private set; }
        #endregion

        #region Commands
        public Command AddWorkspace { get; private set; }

        private async void OnAddWorkspaceExecute()
        {
            var workspace = new Workspace();

            if (_uiVisualizerService.ShowDialog<WorkspaceViewModel>(workspace) ?? false)
            {
                _workspaceManager.Add(workspace, true);
                await _workspaceManager.Save();
            }
        }

        public Command SaveWorkspace { get; private set; }

        private bool OnSaveWorkspaceCanExecute()
        {
            return (CurrentWorkspace != null);
        }

        private async Task OnSaveWorkspaceExecute()
        {
            await _workspaceManager.StoreAndSave();
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

        public Command RemoveWorkspace { get; private set; }

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

        private void OnRemoveWorkspaceExecute()
        {
            _workspaceManager.Remove(CurrentWorkspace);
            UpdateCurrentWorkspace();
        }

        public Command ChooseBaseDirectory { get; private set; }

        public async void OnChooseBaseDirectory()
        {
            _selectDirectoryService.ShowNewFolderButton = true;

            if (_selectDirectoryService.DetermineDirectory())
            {
                await _workspaceManager.SetWorkspaceSchemesDirectory(_selectDirectoryService.DirectoryName);
            }
        }
        #endregion

        #region Methods
        protected override async Task Initialize()
        {
            await base.Initialize();

            _workspaceManager.WorkspaceUpdated += OnCurrentWorkspaceChanged;

            await _workspaceManager.Initialize(true);

            UpdateCurrentWorkspace();
        }

        protected override async Task Close()
        {
            _workspaceManager.WorkspaceUpdated -= OnCurrentWorkspaceChanged;

            await base.Close();
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