// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkspaceWatcherBase.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement
{
    using System;
    using System.Diagnostics;
    using Catel;
    using Catel.Logging;

    public abstract class WorkspaceWatcherBase
    {
        protected readonly IWorkspaceManager WorkspaceManager;

        #region Constructors
        protected WorkspaceWatcherBase(IWorkspaceManager workspaceManager)
        {
            WorkspaceManager = workspaceManager;
            Argument.IsNotNull(() => workspaceManager);

            IgnoreSwitchToNewlyCreatedWorkspace = true;

            workspaceManager.WorkspaceUpdating += OnWorkspaceUpdating;
            workspaceManager.WorkspaceUpdated += OnWorkspaceUpdated;

            workspaceManager.WorkspaceAdded += OnWorkspaceAdded;
            workspaceManager.WorkspaceRemoved += OnWorkspaceRemoved;

            workspaceManager.WorkspaceProviderAdded += OnWorkspaceProviderAdded;
            workspaceManager.WorkspaceProviderRemoved += OnWorkspaceProviderRemoved;

            workspaceManager.Saving += OnSaving;
            workspaceManager.Saved += OnSaved;
        }
        #endregion

        #region Properties
        protected bool IgnoreSwitchToNewlyCreatedWorkspace { get; set; }
        #endregion

        

        #region Fields
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private bool _justAddedWorkspace;

        private Stopwatch _switchStopwatch;
        private Stopwatch _totalStopwatch;
        #endregion

        #region Methods
        protected virtual bool ShouldIgnoreWorkspaceChange()
        {
            if (IgnoreSwitchToNewlyCreatedWorkspace)
            {
                if (_justAddedWorkspace)
                {
                    return true;
                }
            }

            return false;
        }

        protected virtual void OnWorkspaceUpdating(IWorkspace oldWorkspace, IWorkspace newWorkspace, bool isRefresh)
        {
        }

        protected virtual void OnWorkspaceUpdated(IWorkspace oldWorkspace, IWorkspace newWorkspace, bool isRefresh)
        {
        }

        protected virtual void OnWorkspaceAdded(IWorkspace workspace)
        {
        }

        protected virtual void OnWorkspaceRemoved(IWorkspace workspace)
        {
        }

        protected virtual void OnWorkspaceProviderAdded(IWorkspaceProvider workspaceProvider)
        {
        }

        protected virtual void OnWorkspaceProviderRemoved(IWorkspaceProvider workspaceProvider)
        {
        }

        protected virtual void OnSaving()
        {
        }

        protected virtual void OnSaved()
        {
        }

        private void OnWorkspaceUpdating(object sender, WorkspaceUpdatedEventArgs e)
        {
            if (_switchStopwatch != null)
            {
                _switchStopwatch.Stop();
                _switchStopwatch = null;
            }

            if (_totalStopwatch != null)
            {
                _totalStopwatch.Stop();
                _totalStopwatch = null;
            }

            _switchStopwatch = Stopwatch.StartNew();
            _totalStopwatch = Stopwatch.StartNew();

            if (!ShouldIgnoreWorkspaceChange())
            {
                OnWorkspaceUpdating(e.OldWorkspace, e.NewWorkspace, e.IsRefresh);
            }
            else
            {
                Log.Debug("Ignoring WorkspaceUpdating event because this is a newly added workspace");
            }
        }

        private void OnWorkspaceUpdated(object sender, WorkspaceUpdatedEventArgs e)
        {
            if (!ShouldIgnoreWorkspaceChange())
            {
                OnWorkspaceUpdated(e.OldWorkspace, e.NewWorkspace, e.IsRefresh);
            }
            else
            {
                Log.Debug("Ignoring WorkspaceUpdated event because this is a newly added workspace");
            }

            _justAddedWorkspace = false;

            var type = GetType();

            _switchStopwatch.Stop();
            MethodTimeLogger.Log(type, "Switch", _switchStopwatch.ElapsedMilliseconds);

            _totalStopwatch.Stop();
            MethodTimeLogger.Log(type, "Total", _totalStopwatch.ElapsedMilliseconds);
        }

        private void OnWorkspaceAdded(object sender, WorkspaceEventArgs e)
        {
            OnWorkspaceAdded(e.Workspace);

            if (IgnoreSwitchToNewlyCreatedWorkspace)
            {
                _justAddedWorkspace = true;
            }
        }

        private void OnWorkspaceRemoved(object sender, WorkspaceEventArgs e)
        {
            OnWorkspaceRemoved(e.Workspace);
        }

        private void OnWorkspaceProviderAdded(object sender, WorkspaceProviderEventArgs e)
        {
            OnWorkspaceProviderAdded(e.WorkspaceProvider);
        }

        private void OnWorkspaceProviderRemoved(object sender, WorkspaceProviderEventArgs e)
        {
            OnWorkspaceProviderRemoved(e.WorkspaceProvider);
        }

        private void OnSaving(object sender, EventArgs e)
        {
            OnSaving();
        }

        private void OnSaved(object sender, EventArgs e)
        {
            OnSaved();
        }
        #endregion
    }
}