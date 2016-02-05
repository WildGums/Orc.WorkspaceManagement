// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkspacesViewModel.cs" company="WildGums">
//   Copyright (c) 2008 - 2014 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Catel;
    using Catel.Collections;
    using Catel.IoC;
    using Catel.Logging;
    using Catel.MVVM;
    using Catel.Services;
    using Catel.Threading;

    public class WorkspacesViewModel : ViewModelBase
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        #region Fields
        private IWorkspaceManager _workspaceManager;
        private readonly IUIVisualizerService _uiVisualizerService;
        private readonly IServiceLocator _serviceLocator;
        private readonly IDispatcherService _dispatcherService;
        private readonly IMessageService _messageService;
        private readonly ILanguageService _languageService;
        #endregion

        #region Constructors
        public WorkspacesViewModel(IWorkspaceManager workspaceManager, IUIVisualizerService uiVisualizerService, 
            IServiceLocator serviceLocator, IDispatcherService dispatcherService, IMessageService messageService,
            ILanguageService languageService)
        {
            Argument.IsNotNull(() => workspaceManager);
            Argument.IsNotNull(() => uiVisualizerService);
            Argument.IsNotNull(() => serviceLocator);
            Argument.IsNotNull(() => dispatcherService);
            Argument.IsNotNull(() => messageService);
            Argument.IsNotNull(() => languageService);

            _workspaceManager = workspaceManager;
            _uiVisualizerService = uiVisualizerService;
            _serviceLocator = serviceLocator;
            _dispatcherService = dispatcherService;
            _messageService = messageService;
            _languageService = languageService;

            AvailableWorkspaces = new FastObservableCollection<IWorkspace>();

            EditWorkspace = new TaskCommand<IWorkspace>(OnEditWorkspaceExecuteAsync, OnEditWorkspaceCanExecute);
            RemoveWorkspace = new TaskCommand<IWorkspace>(OnRemoveWorkspaceExecuteAsync, OnRemoveWorkspaceCanExecute);
        }
        #endregion

        #region Properties
        public FastObservableCollection<IWorkspace> AvailableWorkspaces { get; private set; }

        public IWorkspace SelectedWorkspace { get; set; }

        public object ManagerTag { get; set; }
        #endregion

        #region Commands
        public TaskCommand<IWorkspace> EditWorkspace { get; private set; }

        private bool OnEditWorkspaceCanExecute(IWorkspace workspace)
        {
            if (workspace == null)
            {
                return false;
            }

            if (!workspace.CanEdit)
            {
                return false;
            }

            return true;
        }

        private async Task OnEditWorkspaceExecuteAsync(object workspace)
        {
            if (_uiVisualizerService.ShowDialog<WorkspaceViewModel>(workspace) ?? false)
            {
                await _workspaceManager.SaveAsync();
            }
        }

        public TaskCommand<IWorkspace> RemoveWorkspace { get; private set; }

        private bool OnRemoveWorkspaceCanExecute(IWorkspace workspace)
        {
            if (workspace == null)
            {
                return false;
            }

            if (!workspace.CanDelete)
            {
                return false;
            }

            return true;
        }

        private async Task OnRemoveWorkspaceExecuteAsync(IWorkspace workspace)
        {
            if (await _messageService.ShowAsync(string.Format(_languageService.GetString("WorkspaceManagement_AreYouSureYouWantToRemoveTheWorkspace"), workspace.Title),
                _languageService.GetString("WorkspaceManagement_AreYouSure"), MessageButton.YesNo, MessageImage.Question) == MessageResult.No)
            {
                return;
            }

            await _workspaceManager.RemoveAsync(workspace);

            await _workspaceManager.SaveAsync();
        }
        #endregion

        #region Methods
        private async void OnManagerTagChanged()
        {
            await DeactivateWorkspaceManagerAsync();
            ActivateWorkspaceManager();
            await UpdateCurrentWorkspaceAsync();
        }

        private async void OnSelectedWorkspaceChanged()
        {
            if (_settingSelectedWokspace)
            {
                return;
            }

            var workspace = SelectedWorkspace;
            if (workspace != null)
            {
                await _workspaceManager.TrySetWorkspaceAsync(workspace);
            }
        }

        protected override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            _workspaceManager.WorkspacesChangedAsync += OnWorkspacesChangedAsync;

            UpdateWorkspaces();
        }

        protected override async Task CloseAsync()
        {
            await DeactivateWorkspaceManagerAsync();
            ActivateWorkspaceManager();
            await UpdateCurrentWorkspaceAsync();
        }

        private Task OnWorkspacesChangedAsync(object sender, EventArgs e)
        {
            UpdateWorkspaces();

            return TaskHelper.Completed;
        }

        private bool _settingSelectedWokspace;

        private async Task SetSelectedWorkspaceAsync(IWorkspace workspace)
        {
            try
            {
                _settingSelectedWokspace = true;
                await _dispatcherService.InvokeAsync(() => SelectedWorkspace = workspace);
            }
            finally
            {
                _settingSelectedWokspace = false;
            }
        }

        private IWorkspaceManager GetWorkspaceManager()
        {
            if (_workspaceManager == null)
            {
                SetWorkspaceManager(_serviceLocator.ResolveType<IWorkspaceManager>(ManagerTag));
            }

            return _workspaceManager;
        }

        private void SetWorkspaceManager(IWorkspaceManager workspaceManager)
        {
            if (_workspaceManager != null)
            {
                _workspaceManager.WorkspaceUpdatedAsync -= OnWorkspacesChangedAsync;
            }

            _workspaceManager = workspaceManager;
            _workspaceManager.WorkspaceUpdatedAsync += OnWorkspacesChangedAsync;
        }

        private async Task DeactivateWorkspaceManagerAsync()
        {
            await SetSelectedWorkspaceAsync(null);
            var workspaceManager = GetWorkspaceManager();
            workspaceManager.WorkspaceUpdatedAsync -= OnWorkspacesChangedAsync;

            AvailableWorkspaces.Clear();
            _workspaceManager = null;
        }

        private void ActivateWorkspaceManager()
        {
            SetWorkspaceManager(_serviceLocator.ResolveType<IWorkspaceManager>(ManagerTag));
            UpdateWorkspaces();
        }

        private void UpdateWorkspaces()
        {
            var finalItems = new List<IWorkspace>();

            var visibleWorkspaces = (from workspace in _workspaceManager.Workspaces
                                     where workspace.IsVisible
                                     select workspace).ToList();

            // 1) Items that cannot be deleted
            finalItems.AddRange(from workspace in visibleWorkspaces
                                where !workspace.CanDelete
                                orderby workspace.Title
                                select workspace);

            // 2) Items that can be deleted
            finalItems.AddRange(from workspace in visibleWorkspaces
                                where workspace.CanDelete
                                orderby workspace.Title
                                select workspace);

            using (AvailableWorkspaces.SuspendChangeNotifications())
            {
                AvailableWorkspaces.ReplaceRange(finalItems);
            }
        }

        private async Task UpdateCurrentWorkspaceAsync()
        {
            var workspaceManager = GetWorkspaceManager();

            await SetSelectedWorkspaceAsync(workspaceManager != null ? workspaceManager.Workspace : null);
        }
        #endregion
    }
}