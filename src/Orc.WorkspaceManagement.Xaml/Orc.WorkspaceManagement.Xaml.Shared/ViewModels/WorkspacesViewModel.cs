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
    using Catel.Data;

    public class WorkspacesViewModel : ViewModelBase
    {
        #region Fields
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private IWorkspaceManager _workspaceManager;
        private readonly IUIVisualizerService _uiVisualizerService;
        private readonly IServiceLocator _serviceLocator;
        private readonly IDispatcherService _dispatcherService;
        private readonly IMessageService _messageService;
        private readonly ILanguageService _languageService;

        private bool _updatingSelectedWorkspace;
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

        public object Scope { get; set; }
        #endregion

        #region Commands
        public TaskCommand<IWorkspace> EditWorkspace { get; private set; }

        private bool OnEditWorkspaceCanExecute(IWorkspace workspace)
        {
            if (workspace == null)
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

        private async Task OnEditWorkspaceExecuteAsync(IWorkspace workspace)
        {
            var modelValidation = workspace as IModelValidation;

            EventHandler<ValidationEventArgs> handler = null;
            handler = (sender, e) =>
            {
                if (_workspaceManager.Workspaces.Any(x => x.Title.EqualsIgnoreCase(workspace.Title) && x != workspace))
                {
                    e.ValidationContext.AddFieldValidationResult(FieldValidationResult.CreateError("Title",
                        _languageService.GetString("WorkspaceManagement_WorkspaceWithCurrentTitleAlreadyExists")));
                }
            };

            if (modelValidation != null)
            {
                modelValidation.Validating += handler;
            }

            if (_uiVisualizerService.ShowDialog<WorkspaceViewModel>(workspace) ?? false)
            {
                if (modelValidation != null)
                {
                    modelValidation.Validating -= handler;
                }

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
#pragma warning disable AsyncFixer03 // Avoid fire & forget async void methods
        private async void OnScopeChanged()
        {
            var scope = Scope;

            Log.Debug($"Scope has changed to '{scope}'");

            await DeactivateWorkspaceManagerAsync();
            ActivateWorkspaceManager();
            await UpdateCurrentWorkspaceAsync();
        }

        private async void OnSelectedWorkspaceChanged()
        {
            if (_updatingSelectedWorkspace)
            {
                return;
            }

            var workspace = SelectedWorkspace;

            Log.Info($"Selected workspace changed to '{workspace?.Title}'");

            if (workspace != null)
            {
                await _workspaceManager.TrySetWorkspaceAsync(workspace);
            }
        }
#pragma warning restore AsyncFixer03 // Avoid fire & forget async void methods

        protected override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            ActivateWorkspaceManager();
        }

        protected override async Task CloseAsync()
        {
            await DeactivateWorkspaceManagerAsync(false);

            await base.CloseAsync();
        }

        private void OnWorkspacesChanged(object sender, EventArgs e)
        {
            var workspaceManager = _workspaceManager;

            Log.Debug($"Workspaces have changed, updating workspaces, current workspace manager scope is '{workspaceManager.Scope}'");

            UpdateWorkspaces();
        }

        private async Task SetSelectedWorkspaceAsync(IWorkspace workspace)
        {
            await _dispatcherService.InvokeAsync(() =>
            {
                try
                {
                    _updatingSelectedWorkspace = true;

                    Log.Debug($"Setting selected workspace to '{workspace?.Title}'");

                    SelectedWorkspace = workspace;
                }
                finally
                {
                    _updatingSelectedWorkspace = false;
                }
            });
        }

        private IWorkspaceManager GetWorkspaceManager()
        {
            if (_workspaceManager == null)
            {
                var workspaceManager = _serviceLocator.ResolveType<IWorkspaceManager>(Scope);
                SetWorkspaceManager(workspaceManager);
            }

            return _workspaceManager;
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

        private async Task DeactivateWorkspaceManagerAsync(bool setToNull = true)
        {
            Log.Debug($"Deactivating workspace manager");

            await SetSelectedWorkspaceAsync(null);

            var workspaceManager = _workspaceManager;
            if (workspaceManager != null)
            {
                workspaceManager.WorkspaceUpdated -= OnWorkspacesChanged;

                if (setToNull)
                {
                    _workspaceManager = null;
                }
            }

            AvailableWorkspaces.Clear();
        }

        private void UpdateWorkspaces()
        {
            var workspaces = _workspaceManager.Workspaces;

            var finalItems = new List<IWorkspace>();

            var visibleWorkspaces = (from workspace in workspaces
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

            Log.Debug($"Updating available workspaces using workspace manager with scope '{_workspaceManager?.Scope}', '{finalItems.Count}' workspaces available");

            using (AvailableWorkspaces.SuspendChangeNotifications())
            {
                ((ICollection<IWorkspace>)AvailableWorkspaces).ReplaceRange(finalItems);
            }
        }

        private Task UpdateCurrentWorkspaceAsync()
        {
            var workspaceManager = GetWorkspaceManager();

            return SetSelectedWorkspaceAsync(workspaceManager?.Workspace);
        }
        #endregion
    }
}