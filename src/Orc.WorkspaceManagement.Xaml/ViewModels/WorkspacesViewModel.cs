﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkspacesViewModel.cs" company="Simulation Modelling Services">
//   Copyright (c) 2008 - 2014 Simulation Modelling Services. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using Catel;
    using Catel.Collections;
    using Catel.MVVM;
    using Catel.Services;
    using WorkspaceManagement;

    public class WorkspacesViewModel : ViewModelBase
    {
        #region Fields
        private readonly IWorkspaceManager _workspaceManager;
        private readonly IUIVisualizerService _uiVisualizerService;
        private readonly IDispatcherService _dispatcherService;
        #endregion

        #region Constructors
        public WorkspacesViewModel(IWorkspaceManager workspaceManager, IUIVisualizerService uiVisualizerService, IDispatcherService dispatcherService)
        {
            Argument.IsNotNull(() => workspaceManager);
            Argument.IsNotNull(() => uiVisualizerService);
            Argument.IsNotNull(() => dispatcherService);

            _workspaceManager = workspaceManager;
            _uiVisualizerService = uiVisualizerService;
            _dispatcherService = dispatcherService;

            AvailableWorkspaces = new ObservableCollection<IWorkspace>();

            EditWorkspace = new Command<IWorkspace>(OnEditWorkspaceExecute, OnEditWorkspaceCanExecute);
            RemoveWorkspace = new Command<IWorkspace>(OnRemoveWorkspaceExecute, OnRemoveWorkspaceCanExecute);
        }
        #endregion

        #region Properties
        public ObservableCollection<IWorkspace> AvailableWorkspaces { get; private set; }

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

        private async void OnEditWorkspaceExecute(object workspace)
        {
            if (await _uiVisualizerService.ShowDialog<WorkspaceViewModel>(workspace) ?? false)
            {
                await _workspaceManager.Save();
            }
        }

        public Command<IWorkspace> RemoveWorkspace { get; private set; }

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

        private async void OnRemoveWorkspaceExecute(IWorkspace workspace)
        {
            _workspaceManager.Remove(workspace);

            await _workspaceManager.Save();
        }
        #endregion

        #region Methods
        private void OnSelectedWorkspaceChanged()
        {
            var workspace = SelectedWorkspace;
            if (workspace != null && !ObjectHelper.AreEqual(workspace, _workspaceManager.Workspace))
            {
                _workspaceManager.Workspace = workspace;
            }
        }

        protected override async Task Initialize()
        {
            await base.Initialize();

            _workspaceManager.WorkspacesChanged += OnWorkspacesChanged;

            UpdateWorkspaces();

            // Required because command parameter bindings are not working without this
            _dispatcherService.BeginInvoke(() => ViewModelCommandManager.InvalidateCommands());
        }

        protected override async Task Close()
        {
            _workspaceManager.WorkspacesChanged -= OnWorkspacesChanged;

            await base.Close();
        }

        private void OnWorkspacesChanged(object sender, EventArgs e)
        {
            UpdateWorkspaces();
        }

        private void UpdateWorkspaces()
        {
            var finalItems = new List<IWorkspace>();

            // 1) Items that cannot be deleted
            finalItems.AddRange(from workspace in _workspaceManager.Workspaces
                                where !workspace.CanDelete
                                orderby workspace.Title
                                select workspace);

            // 2) Items that can be deleted
            finalItems.AddRange(from workspace in _workspaceManager.Workspaces
                                where workspace.CanDelete
                                orderby workspace.Title
                                select workspace);

            AvailableWorkspaces.ReplaceRange(finalItems);

            SelectedWorkspace = _workspaceManager.Workspace;
        }
        #endregion
    }
}