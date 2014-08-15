// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindowViewModel.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement.Example.ViewModels
{
    using System.ComponentModel;
    using System.Threading.Tasks;
    using Catel;
    using Catel.Data;
    using Catel.Fody;
    using Catel.Logging;
    using Catel.MVVM;
    using Catel.Services;
    using Models;

    /// <summary>
    /// MainWindow view model.
    /// </summary>
    public class MainWindowViewModel : ViewModelBase
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly IWorkspaceManager _workspaceManager;
        private readonly IOpenFileService _openFileService;
        private readonly ISaveFileService _saveFileService;
        private readonly IProcessService _processService;

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        public MainWindowViewModel(IWorkspaceManager workspaceManager, IOpenFileService openFileService,
            ISaveFileService saveFileService, IProcessService processService)
        {
            Argument.IsNotNull(() => workspaceManager);
            Argument.IsNotNull(() => openFileService);
            Argument.IsNotNull(() => saveFileService);
            Argument.IsNotNull(() => processService);

            _workspaceManager = workspaceManager;
            _openFileService = openFileService;
            _saveFileService = saveFileService;
            _processService = processService;

            LoadWorkspace = new Command(OnLoadWorkspaceExecute);
            RefreshWorkspace = new Command(OnRefreshWorkspaceExecute, OnRefreshWorkspaceCanExecute);
            SaveWorkspace = new Command(OnSaveWorkspaceExecute, OnSaveWorkspaceCanExecute);
            SaveWorkspaceAs = new Command(OnSaveWorkspaceAsExecute, OnSaveWorkspaceAsCanExecute);
            CloseWorkspace = new Command(OnCloseWorkspaceExecute, OnCloseWorkspaceCanExecute);
            OpenFile = new Command(OnOpenFileExecute, OnOpenFileCanExecute);
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

        [Model]
        [Expose("FirstName")]
        [Expose("MiddleName")]
        [Expose("LastName")]
        public PersonWorkspace Workspace { get; private set; }
        #endregion

        #region Commands
        public Command LoadWorkspace { get; private set; }

        private void OnLoadWorkspaceExecute()
        {
            _openFileService.Filter = "*.txt|Text files (*.txt)|*.*|All files (*.*)";
            if (_openFileService.DetermineFile())
            {
                _workspaceManager.Load(_openFileService.FileName);
            }
        }

        public Command RefreshWorkspace { get; private set; }

        private bool OnRefreshWorkspaceCanExecute()
        {
            return _workspaceManager.Workspace != null;
        }

        private void OnRefreshWorkspaceExecute()
        {
            _workspaceManager.Refresh();
        }

        public Command SaveWorkspace { get; private set; }

        private bool OnSaveWorkspaceCanExecute()
        {
            return _workspaceManager.Workspace != null;
        }

        private void OnSaveWorkspaceExecute()
        {
            _workspaceManager.Save();
        }

        public Command SaveWorkspaceAs { get; private set; }

        private bool OnSaveWorkspaceAsCanExecute()
        {
            return _workspaceManager.Workspace != null;
        }

        private void OnSaveWorkspaceAsExecute()
        {
            _saveFileService.Filter = "*.txt|Text files (*.txt)|*.*|All files (*.*)";
            if (_saveFileService.DetermineFile())
            {
                _workspaceManager.Save(_openFileService.FileName);
            }
        }

        public Command CloseWorkspace { get; private set; }

        private bool OnCloseWorkspaceCanExecute()
        {
            return _workspaceManager.Workspace != null;
        }

        private void OnCloseWorkspaceExecute()
        {
            _workspaceManager.Close();
        }

        public Command OpenFile { get; private set; }

        private bool OnOpenFileCanExecute()
        {
            return _workspaceManager.Workspace != null;
        }

        private void OnOpenFileExecute()
        {
            _processService.StartProcess(_workspaceManager.Location);
        }
        #endregion

        #region Methods
        protected override void Initialize()
        {
            base.Initialize();

            _workspaceManager.WorkspaceUpdated += OnWorkspaceUpdated;

            ReloadWorkspace();
        }

        protected override Task Close()
        {
            _workspaceManager.WorkspaceUpdated -= OnWorkspaceUpdated;

            return base.Close();
        }

        private void OnWorkspaceUpdated(object sender, WorkspaceUpdatedEventArgs e)
        {
            ReloadWorkspace();
        }

        private void ReloadWorkspace()
        {
            Workspace = _workspaceManager.GetWorkspace<PersonWorkspace>();
        }
        #endregion
    }
}