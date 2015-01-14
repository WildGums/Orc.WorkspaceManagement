// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindowViewModel.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement.Example.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Catel;
    using Catel.Collections;
    using Catel.Data;
    using Catel.Fody;
    using Catel.Logging;
    using Catel.MVVM;
    using Catel.Services;
    using Orc.WorkspaceManagement.ViewModels;

    /// <summary>
    /// MainWindow view model.
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly IWorkspaceManager _workspaceManager;
        private readonly IUIVisualizerService _uiVisualizerService;
        private readonly IViewModelFactory _viewModelFactory;
        private readonly ISelectDirectoryService _selectDirectoryService;

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel"/> class.
        /// </summary>
        public MainViewModel(IWorkspaceManager workspaceManager, IUIVisualizerService uiVisualizerService, IViewModelFactory viewModelFactory, ISelectDirectoryService selectDirectoryService)
        {
            Argument.IsNotNull(() => workspaceManager);
            Argument.IsNotNull(() => uiVisualizerService);
            Argument.IsNotNull(() => viewModelFactory);
            Argument.IsNotNull(() => selectDirectoryService);

            _workspaceManager = workspaceManager;
            _uiVisualizerService = uiVisualizerService;
            _viewModelFactory = viewModelFactory;
            _selectDirectoryService = selectDirectoryService;

            UpdateWorkspace = new Command(OnUpdateWorkspaceExecute, OnUpdateWorkspaceCanExecute);
            AddWorkspace = new Command(OnAddWorkspaceExecute);
            EditWorkspace = new Command(OnEditWorkspaceExecute, OnEditWorkspaceCanExecute);
            RemoveWorkspace = new Command(OnRemoveWorkspaceExecute, OnRemoveWorkspaceCanExecute);
            ChooseBaseDirectory = new Command(OnChooseBaseDirectory);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the title of the view model.
        /// </summary>
        /// <value>The title.</value>
        public override string Title
        {
            get { return "Orc.WorkspaceManagement.Example"; }
        }

        public IWorkspace SelectedWorkspace { get; set; }
        #endregion

        #region Commands
        public Command UpdateWorkspace { get; private set; }

        private bool OnUpdateWorkspaceCanExecute()
        {
            return (SelectedWorkspace != null);
        }

        private void OnUpdateWorkspaceExecute()
        {
            _workspaceManager.StoreWorkspace();
        }

        public Command AddWorkspace { get; private set; }

        private async void OnAddWorkspaceExecute()
        {
            var workspace = new Workspace();

            var vm = _viewModelFactory.CreateViewModel<WorkspaceViewModel>(workspace);
            if (await _uiVisualizerService.ShowDialog(vm) ?? false)
            {
                _workspaceManager.Add(workspace, true);
            }
        }

        public Command EditWorkspace { get; private set; }

        private bool OnEditWorkspaceCanExecute()
        {
            return (SelectedWorkspace != null);
        }

        private async void OnEditWorkspaceExecute()
        {
            var vm = _viewModelFactory.CreateViewModel<WorkspaceViewModel>(SelectedWorkspace);
            await _uiVisualizerService.ShowDialog(vm);
        }

        public Command RemoveWorkspace { get; private set; }

        private bool OnRemoveWorkspaceCanExecute()
        {
            if (SelectedWorkspace == null)
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
            _workspaceManager.Remove(SelectedWorkspace);
        }

        public Command ChooseBaseDirectory { get; private set; }

        public void OnChooseBaseDirectory()
        {
            _selectDirectoryService.ShowNewFolderButton = true;

            if (_selectDirectoryService.DetermineDirectory())
            {
                _workspaceManager.SetWorkspaceSchemesDirectory(_selectDirectoryService.DirectoryName);
            }
            
        }
        #endregion

        #region Methods
        private void OnSelectedWorkspaceChanged()
        {
            _workspaceManager.Workspace = SelectedWorkspace;
        }

        protected override async Task Initialize()
        {
            await base.Initialize();

            _workspaceManager.WorkspaceUpdated += OnWorkspaceUpdated;

            await _workspaceManager.Initialize(true);

#if DEBUG
            var defaultWorkspace = (from workspace in _workspaceManager.Workspaces
                                    where string.Equals(workspace.Title, "Default workspace")
                                    select workspace).FirstOrDefault();
            if (defaultWorkspace != null)
            {
                defaultWorkspace.CanEdit = false;
                defaultWorkspace.CanDelete = false;
            }
#endif

            SelectedWorkspace = _workspaceManager.Workspace;
        }

        protected override async Task Close()
        {
            _workspaceManager.WorkspaceUpdated -= OnWorkspaceUpdated;

            await _workspaceManager.Save();

            await base.Close();
        }

        private void OnWorkspaceUpdated(object sender, WorkspaceUpdatedEventArgs e)
        {
            SelectedWorkspace = e.NewWorkspace;
        }
        #endregion
    }
}