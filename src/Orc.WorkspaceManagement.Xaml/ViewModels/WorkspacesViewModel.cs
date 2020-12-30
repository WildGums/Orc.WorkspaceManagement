﻿namespace Orc.WorkspaceManagement.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Catel;
    using Catel.Collections;
    using Catel.Data;
    using Catel.IoC;
    using Catel.Logging;
    using Catel.MVVM;
    using Catel.Services;
    using Catel.Threading;

    public class WorkspacesViewModel : ViewModelBase
    {
        #region Fields
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly IUIVisualizerService _uiVisualizerService;
        private readonly IServiceLocator _serviceLocator;
        private readonly IDispatcherService _dispatcherService;
        private readonly IMessageService _messageService;
        private readonly ILanguageService _languageService;

        private IWorkspaceManager _workspaceManager;
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

            WorkspaceGroups = new List<WorkspaceGroup>();

            EditWorkspace = new TaskCommand<IWorkspace>(OnEditWorkspaceExecuteAsync, OnEditWorkspaceCanExecute);
            RemoveWorkspace = new TaskCommand<IWorkspace>(OnRemoveWorkspaceExecuteAsync, OnRemoveWorkspaceCanExecute);
            Refresh = new TaskCommand<IWorkspace>(OnRefreshAsync, OnRefreshCanExecute);
        }
        #endregion

        #region Properties
        public List<WorkspaceGroup> WorkspaceGroups { get; private set; }

        public IWorkspace SelectedWorkspace
        {
            get => _workspaceManager?.Workspace;
            set
            {
                if (value != null)
                {
                    _dispatcherService.InvokeTaskAsync(async () => await _workspaceManager.TrySetWorkspaceAsync(value))
                        .ContinueWith(_ => RaiseSelectedWorkspaceChanged());
                }
            }
        }

        public object Scope { get; set; }
        #endregion

        #region Commands
        public TaskCommand<IWorkspace> Refresh { get; private set; }

        private bool OnRefreshCanExecute(IWorkspace workspace)
        {
            if (workspace is null)
            {
                return false;
            }

            return workspace.IsDirty;
        }

        private async Task OnRefreshAsync(IWorkspace workspace)
        {
            if (!workspace.IsDirty)
            {
                return;
            }

            await _workspaceManager.RefreshWorkspaceAsync(workspace);
        }

        public TaskCommand<IWorkspace> EditWorkspace { get; private set; }

        private bool OnEditWorkspaceCanExecute(IWorkspace workspace)
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

        private Task OnEditWorkspaceExecuteAsync(IWorkspace workspace)
        {
            var modelValidation = workspace as IValidatable;

            EventHandler<ValidationEventArgs> handler = null;
            handler = (sender, e) =>
            {
                if (_workspaceManager.Workspaces.Any(x => x.Title.EqualsIgnoreCase(workspace.Title) && x != workspace))
                {
                    e.ValidationContext.Add(FieldValidationResult.CreateError(nameof(Title),
                        _languageService.GetString("WorkspaceManagement_WorkspaceWithCurrentTitleAlreadyExists")));
                }
            };

            if (modelValidation != null)
            {
                modelValidation.Validating += handler;
            }

            // Dispatch to make sure this plays nice with Fluent.Ribbon dropdowns
#pragma warning disable AvoidAsyncVoid // Avoid async void
            _dispatcherService.BeginInvoke(async () =>
#pragma warning restore AvoidAsyncVoid // Avoid async void
            {
                await Task.Delay(50);

                if (await _uiVisualizerService.ShowDialogAsync<WorkspaceViewModel>(workspace) ?? false)
                {
                    if (modelValidation != null)
                    {
                        modelValidation.Validating -= handler;
                    }

                    await _workspaceManager.SaveAsync();
                }
            }, false);

            return TaskHelper.Completed;
        }

        public TaskCommand<IWorkspace> RemoveWorkspace { get; private set; }

        private bool OnRemoveWorkspaceCanExecute(IWorkspace workspace)
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

        private Task OnRemoveWorkspaceExecuteAsync(IWorkspace workspace)
        {
            // Dispatch to make sure this plays nice with Fluent.Ribbon dropdowns
#pragma warning disable AvoidAsyncVoid // Avoid async void
            _dispatcherService.BeginInvoke(async () =>
#pragma warning restore AvoidAsyncVoid // Avoid async void
            {
                await Task.Delay(50);

                if (await _messageService.ShowAsync(string.Format(_languageService.GetString("WorkspaceManagement_AreYouSureYouWantToRemoveTheWorkspace"), workspace.Title),
                        _languageService.GetString("WorkspaceManagement_AreYouSure"), MessageButton.YesNo, MessageImage.Question) == MessageResult.No)
                {
                    return;
                }

                await _workspaceManager.RemoveAsync(workspace);
                await _workspaceManager.SaveAsync();
            }, false);

            return TaskHelper.Completed;
        }
        #endregion

        #region Methods
        private void RaiseSelectedWorkspaceChanged()
        {
            _dispatcherService.Invoke(() => RaisePropertyChanged(nameof(SelectedWorkspace)));
        }

        private void OnScopeChanged()
        {
            var scope = Scope;

            Log.Debug($"Scope has changed to '{scope}'");

            DeactivateWorkspaceManager();
            ActivateWorkspaceManager();

            UpdateWorkspaces();
        }

        protected override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            ActivateWorkspaceManager();
        }

        protected override Task CloseAsync()
        {
            DeactivateWorkspaceManager(false);

            return base.CloseAsync();
        }

        private void OnWorkspacesChanged(object sender, EventArgs e)
        {
            var workspaceManager = _workspaceManager;

            Log.Debug($"Workspaces have changed, updating workspaces, current workspace manager scope is '{workspaceManager.Scope}'");

            UpdateWorkspaces();
        }

        private void SetWorkspaceManager(IWorkspaceManager workspaceManager)
        {
            var previousWorkspaceManager = _workspaceManager;
            if (ReferenceEquals(workspaceManager, previousWorkspaceManager))
            {
                return;
            }

            if (previousWorkspaceManager != null)
            {
                previousWorkspaceManager.WorkspaceUpdated -= OnWorkspacesChanged;
            }

            Log.Debug($"Updating current workspace manager with scope '{workspaceManager?.Scope}' to new instance with '{workspaceManager?.Workspaces.Count() ?? 0}' workspaces");

            _workspaceManager = workspaceManager;

            if (workspaceManager != null)
            {
                _workspaceManager.WorkspaceUpdated += OnWorkspacesChanged;
            }
        }

        private void ActivateWorkspaceManager()
        {
            var scope = Scope;

            Log.Debug($"Activating workspace manager using scope '{scope}'");

            var workspaceManager = _serviceLocator.ResolveType<IWorkspaceManager>(scope);
            SetWorkspaceManager(workspaceManager);

            UpdateWorkspaces();
        }

        private void DeactivateWorkspaceManager(bool setToNull = true)
        {
            Log.Debug($"Deactivating workspace manager");

            SelectedWorkspace = null;

            var workspaceManager = _workspaceManager;
            if (workspaceManager != null)
            {
                workspaceManager.WorkspaceUpdated -= OnWorkspacesChanged;

                if (setToNull)
                {
                    _workspaceManager = null;
                }
            }

            WorkspaceGroups.Clear();
        }

        private bool _updatingWorkspace;

        private void UpdateWorkspaces()
        {
            if (_updatingWorkspace)
            {
                return;
            }

            _updatingWorkspace = true;

            try
            {
                var workspaceGroups = (from workspace in _workspaceManager.Workspaces
                                       where workspace.IsVisible
                                       orderby workspace.WorkspaceGroup, workspace.Title, workspace.CanDelete
                                       group workspace by workspace.WorkspaceGroup into g
                                       select new WorkspaceGroup(string.IsNullOrWhiteSpace(g.Key) ? null : g.Key, g)).ToList();

                Log.Debug($"Updating available workspaces using workspace manager with scope '{_workspaceManager?.Scope}', '{workspaceGroups.Count}' workspace groups available");

                WorkspaceGroups = workspaceGroups;

                RaiseSelectedWorkspaceChanged();
            }
            finally
            {
                _updatingWorkspace = false;
            }
        }
        #endregion
    }
}
