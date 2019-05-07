// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkspaceWatcherBase.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orc.WorkspaceManagement
{
    using System;
    using System.ComponentModel;
    using System.Threading.Tasks;
    using Catel;
    using Catel.Logging;
    using Catel.Threading;
#if DEBUG
    using System.Diagnostics;
#endif

    public abstract class WorkspaceWatcherBase : IDisposable
    {
        #region Fields
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        protected readonly IWorkspaceManager WorkspaceManager;

        private bool _justAddedWorkspace;

#if DEBUG
        private Stopwatch _switchStopwatch;
        private Stopwatch _totalStopwatch;
#endif
        #endregion

        #region Constructors
        protected WorkspaceWatcherBase(IWorkspaceManager workspaceManager)
        {
            WorkspaceManager = workspaceManager;
            Argument.IsNotNull(() => workspaceManager);

            IgnoreSwitchToNewlyCreatedWorkspace = true;

            workspaceManager.WorkspaceUpdatingAsync += OnWorkspaceUpdatingAsync;
            workspaceManager.WorkspaceUpdated += OnWorkspaceUpdated;

            workspaceManager.WorkspaceAdded += OnWorkspaceAdded;
            workspaceManager.WorkspaceRemoved += OnWorkspaceRemoved;

            workspaceManager.WorkspaceProviderAdded += OnWorkspaceProviderAdded;
            workspaceManager.WorkspaceProviderRemoved += OnWorkspaceProviderRemoved;

            workspaceManager.WorkspaceSavingAsync += OnSavingAsync;
            workspaceManager.WorkspaceSaved += OnSaved;
        }
        #endregion

        #region Properties
        protected bool IgnoreSwitchToNewlyCreatedWorkspace { get; set; }
        #endregion

        #region Methods
        protected virtual void Dispose(bool disposing)
        {
            WorkspaceManager.WorkspaceUpdatingAsync -= OnWorkspaceUpdatingAsync;
            WorkspaceManager.WorkspaceUpdated -= OnWorkspaceUpdated;

            WorkspaceManager.WorkspaceAdded -= OnWorkspaceAdded;
            WorkspaceManager.WorkspaceRemoved -= OnWorkspaceRemoved;

            WorkspaceManager.WorkspaceProviderAdded -= OnWorkspaceProviderAdded;
            WorkspaceManager.WorkspaceProviderRemoved -= OnWorkspaceProviderRemoved;

            WorkspaceManager.WorkspaceSavingAsync -= OnSavingAsync;
            WorkspaceManager.WorkspaceSaved -= OnSaved;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual bool ShouldIgnoreWorkspaceChange()
        {
            return IgnoreSwitchToNewlyCreatedWorkspace && _justAddedWorkspace;
        }

        protected virtual Task<bool> OnWorkspaceUpdatingAsync(IWorkspace oldWorkspace, IWorkspace newWorkspace, bool isRefresh)
        {
            return TaskHelper<bool>.FromResult(true);
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

        protected virtual Task<bool> OnSavingAsync()
        {
            return TaskHelper<bool>.FromResult(true);
        }

        protected virtual void OnSaved()
        {
        }

        private async Task OnWorkspaceUpdatingAsync(object sender, WorkspaceUpdatingEventArgs e)
        {
#if DEBUG
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
#endif

            if (!ShouldIgnoreWorkspaceChange())
            {
                e.Cancel = !await OnWorkspaceUpdatingAsync(e.OldWorkspace, e.NewWorkspace, e.IsRefresh);
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

#if DEBUG
            var type = GetType();

            _switchStopwatch.Stop();
            MethodTimeLogger.Log(type, "Switch", _switchStopwatch.ElapsedMilliseconds, "");

            _totalStopwatch.Stop();
            MethodTimeLogger.Log(type, "Total", _totalStopwatch.ElapsedMilliseconds, "");
#endif
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

        private async Task OnSavingAsync(object sender, CancelWorkspaceEventArgs e)
        {
            e.Cancel = !await OnSavingAsync();
        }

        private void OnSaved(object sender, WorkspaceEventArgs e)
        {
            OnSaved();
        }
        #endregion
    }
}
