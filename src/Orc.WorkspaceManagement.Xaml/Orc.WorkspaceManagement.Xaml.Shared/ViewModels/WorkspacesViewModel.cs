﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkspacesViewModel.cs" company="Simulation Modelling Services">
//   Copyright (c) 2008 - 2014 Simulation Modelling Services. All rights reserved.
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
    using Catel.Logging;
    using Catel.MVVM;
    using Catel.Services;

    public class WorkspacesViewModel : ViewModelBase
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        #region Fields
        private readonly IWorkspaceManager _workspaceManager;
        private readonly IUIVisualizerService _uiVisualizerService;
        #endregion

        #region Constructors
        public WorkspacesViewModel(IWorkspaceManager workspaceManager, IUIVisualizerService uiVisualizerService)
        {
            Argument.IsNotNull(() => workspaceManager);
            Argument.IsNotNull(() => uiVisualizerService);

            _workspaceManager = workspaceManager;
            _uiVisualizerService = uiVisualizerService;

            AvailableWorkspaces = new FastObservableCollection<IWorkspace>();

            EditWorkspace = new Command<IWorkspace>(OnEditWorkspaceExecute, OnEditWorkspaceCanExecute);
            RemoveWorkspace = new TaskCommand<IWorkspace>(OnRemoveWorkspaceExecuteAsync, OnRemoveWorkspaceCanExecute);
        }
        #endregion

        #region Properties
        public FastObservableCollection<IWorkspace> AvailableWorkspaces { get; private set; }

        public IWorkspace SelectedWorkspace { get; set; }
        #endregion

        #region Commands
        public Command<IWorkspace> EditWorkspace { get; private set; }

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

        private void OnEditWorkspaceExecute(object workspace)
        {
            if (_uiVisualizerService.ShowDialog<WorkspaceViewModel>(workspace) ?? false)
            {
                _workspaceManager.Save();
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
            await _workspaceManager.RemoveAsync(workspace);

            _workspaceManager.Save();
        }
        #endregion

        #region Methods
        private async void OnSelectedWorkspaceChanged()
        {
            var workspace = SelectedWorkspace;
            if (workspace != null)
            {
                await _workspaceManager.SetWorkspaceAsync(workspace);
            }
        }

        protected override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            _workspaceManager.WorkspacesChanged += OnWorkspacesChanged;

            UpdateWorkspaces();
        }

        protected override async Task CloseAsync()
        {
            _workspaceManager.WorkspacesChanged -= OnWorkspacesChanged;

            await base.CloseAsync();
        }

        private void OnWorkspacesChanged(object sender, EventArgs e)
        {
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
        #endregion
    }
}