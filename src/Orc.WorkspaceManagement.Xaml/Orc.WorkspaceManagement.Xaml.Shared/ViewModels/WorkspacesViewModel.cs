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
            handler = (object sender, ValidationEventArgs e) =>
            {
                if (_workspaceManager.Workspaces.Where(x => string.Equals(x.Title, workspace.Title) && x != workspace).Any())
                {
                    e.ValidationContext.AddFieldValidationResult(FieldValidationResult.CreateError("Title",
                        _languageService.GetString("WorkspaceManagement_WorkspaceWithCurrentTitleAlreadyExists")));
                }
            };

            modelValidation.Validating += handler;

            if (_uiVisualizerService.ShowDialog<WorkspaceViewModel>(workspace) ?? false)
            {
                modelValidation.Validating -= handler;

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
            UpdateWorkspaces();
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
                var workspaceManager = _serviceLocator.ResolveType<IWorkspaceManager>(Scope);
                SetWorkspaceManager(workspaceManager);
            }

            return _workspaceManager;
        }

        private void SetWorkspaceManager(IWorkspaceManager workspaceManager)
        {
            if (_workspaceManager != null)
            {
                _workspaceManager.WorkspaceUpdated -= OnWorkspacesChanged;
            }

            _workspaceManager = workspaceManager;
            _workspaceManager.WorkspaceUpdated += OnWorkspacesChanged;
        }

        private void ActivateWorkspaceManager()
        {
            var workspaceManager = _serviceLocator.ResolveType<IWorkspaceManager>(Scope);
            SetWorkspaceManager(workspaceManager);

            UpdateWorkspaces();
        }

        private async Task DeactivateWorkspaceManagerAsync(bool setToNull = true)
        {
            await SetSelectedWorkspaceAsync(null);

            if (_workspaceManager != null)
            {
                _workspaceManager.WorkspaceUpdated -= OnWorkspacesChanged;

                if (setToNull)
                {
                    _workspaceManager = null;
                }
            }

            AvailableWorkspaces.Clear();
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