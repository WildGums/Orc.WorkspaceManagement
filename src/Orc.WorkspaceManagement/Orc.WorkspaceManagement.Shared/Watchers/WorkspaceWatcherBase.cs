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
        protected readonly IWorkspaceManager WorkspaceManager;

#region Constructors
        protected WorkspaceWatcherBase(IWorkspaceManager workspaceManager)
        {
            WorkspaceManager = workspaceManager;
            Argument.IsNotNull(() => workspaceManager);

            IgnoreSwitchToNewlyCreatedWorkspace = true;

            workspaceManager.WorkspaceUpdating += OnWorkspaceUpdating;
            workspaceManager.WorkspaceUpdatingAsync += OnWorkspaceUpdatingAsync;
            workspaceManager.WorkspaceUpdated += OnWorkspaceUpdated;
            workspaceManager.WorkspaceUpdatedAsync += OnWorkspaceUpdatedAsync;

            workspaceManager.WorkspaceAdded += OnWorkspaceAdded;
            workspaceManager.WorkspaceAddedAsync += OnWorkspaceAddedAsync;
            workspaceManager.WorkspaceRemoved += OnWorkspaceRemoved;
            workspaceManager.WorkspaceRemovedAsync += OnWorkspaceRemovedAsync;

            workspaceManager.WorkspaceProviderAdded += OnWorkspaceProviderAdded;
            workspaceManager.WorkspaceProviderAddedAsync += OnWorkspaceProviderAddedAsync;
            workspaceManager.WorkspaceProviderRemoved += OnWorkspaceProviderRemoved;
            workspaceManager.WorkspaceProviderRemovedAsync += OnWorkspaceProviderRemovedAsync;

            workspaceManager.Saving += OnSaving;
            workspaceManager.SavingAsync += OnSavingAsync;
            workspaceManager.Saved += OnSaved;
            workspaceManager.SavedAsync += OnSavedAsync;
        } 
        #endregion

#region Properties
        protected bool IgnoreSwitchToNewlyCreatedWorkspace { get; set; }
#endregion

#region Fields
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private bool _justAddedWorkspace;
#if DEBUG
        private Stopwatch _switchStopwatch;
        private Stopwatch _totalStopwatch;
#endif

        #endregion

        #region Methods
        public void Dispose()
        {
            WorkspaceManager.WorkspaceUpdating -= OnWorkspaceUpdating;
            WorkspaceManager.WorkspaceUpdatingAsync -= OnWorkspaceUpdatingAsync;
            WorkspaceManager.WorkspaceUpdated -= OnWorkspaceUpdated;
            WorkspaceManager.WorkspaceUpdatedAsync -= OnWorkspaceUpdatedAsync;

            WorkspaceManager.WorkspaceAdded -= OnWorkspaceAdded;
            WorkspaceManager.WorkspaceAddedAsync -= OnWorkspaceAddedAsync;
            WorkspaceManager.WorkspaceRemoved -= OnWorkspaceRemoved;
            WorkspaceManager.WorkspaceRemovedAsync -= OnWorkspaceRemovedAsync;

            WorkspaceManager.WorkspaceProviderAdded -= OnWorkspaceProviderAdded;
            WorkspaceManager.WorkspaceProviderAddedAsync -= OnWorkspaceProviderAddedAsync;
            WorkspaceManager.WorkspaceProviderRemoved -= OnWorkspaceProviderRemoved;
            WorkspaceManager.WorkspaceProviderRemovedAsync -= OnWorkspaceProviderRemovedAsync;

            WorkspaceManager.Saving -= OnSaving;
            WorkspaceManager.SavingAsync -= OnSavingAsync;
            WorkspaceManager.Saved -= OnSaved;
            WorkspaceManager.SavedAsync -= OnSavedAsync;
        }

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

        [ObsoleteEx(ReplacementTypeOrMember = "OnWorkspaceUpdatingAsync", TreatAsErrorFromVersion = "1.0", RemoveInVersion = "2.0")]
        protected virtual void OnWorkspaceUpdating(IWorkspace oldWorkspace, IWorkspace newWorkspace, bool isRefresh)
        {
        }

        protected virtual Task<bool> OnWorkspaceUpdatingAsync(IWorkspace oldWorkspace, IWorkspace newWorkspace, bool isRefresh)
        {
            return TaskHelper<bool>.FromResult(true);
        }

        [ObsoleteEx(ReplacementTypeOrMember = "OnWorkspaceUpdatedAsync", TreatAsErrorFromVersion = "1.0", RemoveInVersion = "2.0")]
        protected virtual void OnWorkspaceUpdated(IWorkspace oldWorkspace, IWorkspace newWorkspace, bool isRefresh)
        {
        }

        protected virtual Task OnWorkspaceUpdatedAsync(IWorkspace oldWorkspace, IWorkspace newWorkspace, bool isRefresh)
        {
            return TaskHelper.Completed;
        }

        [ObsoleteEx(ReplacementTypeOrMember = "OnWorkspaceAddedAsync", TreatAsErrorFromVersion = "1.0", RemoveInVersion = "2.0")]
        protected virtual void OnWorkspaceAdded(IWorkspace workspace)
        {
        }

        protected virtual Task OnWorkspaceAddedAsync(IWorkspace workspace)
        {
            return TaskHelper.Completed;
        }

        [ObsoleteEx(ReplacementTypeOrMember = "OnWorkspaceRemovedAsync", TreatAsErrorFromVersion = "1.0", RemoveInVersion = "2.0")]
        protected virtual void OnWorkspaceRemoved(IWorkspace workspace)
        {
        }

        protected virtual Task OnWorkspaceRemovedAsync(IWorkspace workspace)
        {
            return TaskHelper.Completed;
        }

        [ObsoleteEx(ReplacementTypeOrMember = "OnWorkspaceProviderAddedAsync", TreatAsErrorFromVersion = "1.0", RemoveInVersion = "2.0")]
        protected virtual void OnWorkspaceProviderAdded(IWorkspaceProvider workspaceProvider)
        {
        }

        protected virtual Task OnWorkspaceProviderAddedAsync(IWorkspaceProvider workspaceProvider)
        {
            return TaskHelper.Completed;
        }

        [ObsoleteEx(ReplacementTypeOrMember = "OnWorkspaceProviderRemovedAsync", TreatAsErrorFromVersion = "1.0", RemoveInVersion = "2.0")]
        protected virtual void OnWorkspaceProviderRemoved(IWorkspaceProvider workspaceProvider)
        {
        }

        protected virtual Task OnWorkspaceProviderRemovedAsync(IWorkspaceProvider workspaceProvider)
        {
            return TaskHelper.Completed;
        }

        [ObsoleteEx(ReplacementTypeOrMember = "OnSavingAsync", TreatAsErrorFromVersion = "1.0", RemoveInVersion = "2.0")]
        protected virtual void OnSaving()
        {
        }

        protected virtual Task<bool> OnSavingAsync()
        {
            return TaskHelper<bool>.FromResult(true);
        }

        [ObsoleteEx(ReplacementTypeOrMember = "OnSavedAsync", TreatAsErrorFromVersion = "1.0", RemoveInVersion = "2.0")]
        protected virtual void OnSaved()
        {
        }

        protected virtual Task OnSavedAsync()
        {
            return TaskHelper.Completed;
        }

        private void OnWorkspaceUpdating(object sender, WorkspaceUpdatedEventArgs e)
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
                OnWorkspaceUpdating(e.OldWorkspace, e.NewWorkspace, e.IsRefresh);
            }
            else
            {
                Log.Debug("Ignoring WorkspaceUpdating event because this is a newly added workspace");
            }
        }

        private async Task OnWorkspaceUpdatingAsync(object sender, WorkspaceUpdatingEventArgs e)
        {
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
            MethodTimeLogger.Log(type, "Switch", _switchStopwatch.ElapsedMilliseconds);

            _totalStopwatch.Stop();
            MethodTimeLogger.Log(type, "Total", _totalStopwatch.ElapsedMilliseconds);
#endif
        }

        private async Task OnWorkspaceUpdatedAsync(object sender, WorkspaceUpdatedEventArgs e)
        {
            if (!ShouldIgnoreWorkspaceChange())
            {
                await OnWorkspaceUpdatedAsync(e.OldWorkspace, e.NewWorkspace, e.IsRefresh);
            }
            else
            {
                Log.Debug("Ignoring WorkspaceUpdated event because this is a newly added workspace");
            }

            _justAddedWorkspace = false;
        }

        private void OnWorkspaceAdded(object sender, WorkspaceEventArgs e)
        {
            OnWorkspaceAdded(e.Workspace);

            if (IgnoreSwitchToNewlyCreatedWorkspace)
            {
                _justAddedWorkspace = true;
            }
        }

        private async Task OnWorkspaceAddedAsync(object sender, WorkspaceEventArgs e)
        {
            await OnWorkspaceAddedAsync(e.Workspace);

            if (IgnoreSwitchToNewlyCreatedWorkspace)
            {
                _justAddedWorkspace = true;
            }
        }

        private void OnWorkspaceRemoved(object sender, WorkspaceEventArgs e)
        {
            OnWorkspaceRemoved(e.Workspace);
        }

        private Task OnWorkspaceRemovedAsync(object sender, WorkspaceEventArgs e)
        {
            return OnWorkspaceRemovedAsync(e.Workspace);
        }

        private void OnWorkspaceProviderAdded(object sender, WorkspaceProviderEventArgs e)
        {
            OnWorkspaceProviderAdded(e.WorkspaceProvider);
        }

        private Task OnWorkspaceProviderAddedAsync(object sender, WorkspaceProviderEventArgs e)
        {
            return OnWorkspaceProviderAddedAsync(e.WorkspaceProvider);
        }

        private void OnWorkspaceProviderRemoved(object sender, WorkspaceProviderEventArgs e)
        {
            OnWorkspaceProviderRemoved(e.WorkspaceProvider);
        }

        private Task OnWorkspaceProviderRemovedAsync(object sender, WorkspaceProviderEventArgs e)
        {
            return OnWorkspaceProviderRemovedAsync(e.WorkspaceProvider);
        }

        private void OnSaving(object sender, EventArgs e)
        {
            OnSaving();
        }

        private async Task OnSavingAsync(object sender, CancelEventArgs e)
        {
            e.Cancel = !await OnSavingAsync();
        }

        private void OnSaved(object sender, EventArgs e)
        {
            OnSaved();
        }

        private Task OnSavedAsync(object sender, EventArgs e)
        {
            return OnSavedAsync();
        }
        #endregion
    }
}