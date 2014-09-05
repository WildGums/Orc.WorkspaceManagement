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

    /// <summary>
    /// MainWindow view model.
    /// </summary>
    public class MainWindowViewModel : ViewModelBase
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly IWorkspaceManager _workspaceManager;
        private readonly IUIVisualizerService _uiVisualizerService;
        private readonly IViewModelFactory _viewModelFactory;

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        public MainWindowViewModel(IWorkspaceManager workspaceManager, IUIVisualizerService uiVisualizerService, IViewModelFactory viewModelFactory)
        {
            Argument.IsNotNull(() => workspaceManager);
            Argument.IsNotNull(() => uiVisualizerService);
            Argument.IsNotNull(() => viewModelFactory);

            _workspaceManager = workspaceManager;
            _uiVisualizerService = uiVisualizerService;
            _viewModelFactory = viewModelFactory;

            AvailableWorkspaces = new ObservableCollection<IWorkspace>();

            SaveWorkspace = new Command(OnSaveWorkspaceExecute, OnSaveWorkspaceCanExecute);
            AddWorkspace = new Command(OnAddWorkspaceExecute);
            EditWorkspace = new Command(OnEditWorkspaceExecute, OnEditWorkspaceCanExecute);
            RemoveWorkspace = new Command(OnRemoveWorkspaceExecute, OnRemoveWorkspaceCanExecute);
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

        public ObservableCollection<IWorkspace> AvailableWorkspaces { get; private set; }

        public IWorkspace SelectedWorkspace { get; set; }
        #endregion

        #region Commands
        public Command SaveWorkspace { get; private set; }

        private bool OnSaveWorkspaceCanExecute()
        {
            return (SelectedWorkspace != null);
        }

        private void OnSaveWorkspaceExecute()
        {
            // TODO: Handle command logic here
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
        #endregion

        #region Methods
        private void OnSelectedWorkspaceChanged()
        {
            _workspaceManager.Workspace = SelectedWorkspace;
        }

        protected override async void Initialize()
        {
            base.Initialize();

            _workspaceManager.WorkspaceUpdated += OnWorkspaceUpdated;
            _workspaceManager.WorkspacesChanged += OnWorkspacesChanged;

            await _workspaceManager.Initialize();

            AvailableWorkspaces.AddRange(_workspaceManager.Workspaces);
            SelectedWorkspace = _workspaceManager.Workspace;
        }

        protected override async Task Close()
        {
            _workspaceManager.WorkspaceUpdated -= OnWorkspaceUpdated;
            _workspaceManager.WorkspacesChanged -= OnWorkspacesChanged;

            await base.Close();
        }

        private void OnWorkspaceUpdated(object sender, WorkspaceUpdatedEventArgs e)
        {
            SelectedWorkspace = e.NewWorkspace;
        }

        private void OnWorkspacesChanged(object sender, EventArgs e)
        {
            AvailableWorkspaces.ReplaceRange(_workspaceManager.Workspaces);
        }
        #endregion
    }
}