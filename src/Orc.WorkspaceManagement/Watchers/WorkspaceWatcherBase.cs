namespace Orc.WorkspaceManagement;

using System;
using System.Threading.Tasks;
using Catel.Logging;

#if DEBUG
using System.Diagnostics;
#endif

public abstract class WorkspaceWatcherBase : IDisposable
{
    private static readonly ILog Log = LogManager.GetCurrentClassLogger();

    protected readonly IWorkspaceManager WorkspaceManager;

    private bool _justAddedWorkspace;

#if DEBUG
    private Stopwatch? _switchStopwatch;
    private Stopwatch? _totalStopwatch;
#endif

    protected WorkspaceWatcherBase(IWorkspaceManager workspaceManager)
    {
        ArgumentNullException.ThrowIfNull(workspaceManager);

        WorkspaceManager = workspaceManager;

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

    protected bool IgnoreSwitchToNewlyCreatedWorkspace { get; set; }

    protected virtual void Dispose(bool disposing)
    {
#pragma warning disable IDISP023 // Don't use reference types in finalizer context.
        WorkspaceManager.WorkspaceUpdatingAsync -= OnWorkspaceUpdatingAsync;
        WorkspaceManager.WorkspaceUpdated -= OnWorkspaceUpdated;

        WorkspaceManager.WorkspaceAdded -= OnWorkspaceAdded;
        WorkspaceManager.WorkspaceRemoved -= OnWorkspaceRemoved;

        WorkspaceManager.WorkspaceProviderAdded -= OnWorkspaceProviderAdded;
        WorkspaceManager.WorkspaceProviderRemoved -= OnWorkspaceProviderRemoved;

        WorkspaceManager.WorkspaceSavingAsync -= OnSavingAsync;
        WorkspaceManager.WorkspaceSaved -= OnSaved;
#pragma warning restore IDISP023 // Don't use reference types in finalizer context.
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

    protected virtual Task<bool> OnWorkspaceUpdatingAsync(IWorkspace? oldWorkspace, IWorkspace? newWorkspace, bool isRefresh)
    {
        return Task.FromResult(true);
    }

    protected virtual void OnWorkspaceUpdated(IWorkspace? oldWorkspace, IWorkspace? newWorkspace, bool isRefresh)
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
        return Task.FromResult(true);
    }

    protected virtual void OnSaved()
    {
    }

    private async Task OnWorkspaceUpdatingAsync(object? sender, WorkspaceUpdatingEventArgs e)
    {
#if DEBUG
        if (_switchStopwatch is not null)
        {
            _switchStopwatch.Stop();
            _switchStopwatch = null;
        }

        if (_totalStopwatch is not null)
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

    private void OnWorkspaceUpdated(object? sender, WorkspaceUpdatedEventArgs e)
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

        _switchStopwatch?.Stop();
        MethodTimeLogger.Log(type, "Switch", _switchStopwatch?.ElapsedMilliseconds ?? 0, string.Empty);

        _totalStopwatch?.Stop();
        MethodTimeLogger.Log(type, "Total", _totalStopwatch?.ElapsedMilliseconds ?? 0, string.Empty);
#endif
    }


    private void OnWorkspaceAdded(object? sender, WorkspaceEventArgs e)
    {
        OnWorkspaceAdded(e.Workspace);

        if (IgnoreSwitchToNewlyCreatedWorkspace)
        {
            _justAddedWorkspace = true;
        }
    }

    private void OnWorkspaceRemoved(object? sender, WorkspaceEventArgs e)
    {
        OnWorkspaceRemoved(e.Workspace);
    }

    private void OnWorkspaceProviderAdded(object? sender, WorkspaceProviderEventArgs e)
    {
        OnWorkspaceProviderAdded(e.WorkspaceProvider);
    }

    private void OnWorkspaceProviderRemoved(object? sender, WorkspaceProviderEventArgs e)
    {
        OnWorkspaceProviderRemoved(e.WorkspaceProvider);
    }

    private async Task OnSavingAsync(object? sender, CancelWorkspaceEventArgs e)
    {
        e.Cancel = !await OnSavingAsync();
    }

    private void OnSaved(object? sender, WorkspaceEventArgs e)
    {
        OnSaved();
    }
}
