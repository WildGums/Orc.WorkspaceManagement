[assembly: System.Resources.NeutralResourcesLanguageAttribute("en-US")]
[assembly: System.Runtime.Versioning.TargetFrameworkAttribute(".NETFramework,Version=v4.6", FrameworkDisplayName=".NET Framework 4.6")]
public class static ModuleInitializer
{
    public static void Initialize() { }
}
namespace Orc.WorkspaceManagement
{
    public class EmptyWorkspaceInitializer : Orc.WorkspaceManagement.IWorkspaceInitializer
    {
        public EmptyWorkspaceInitializer() { }
        public System.Threading.Tasks.Task InitializeAsync(Orc.WorkspaceManagement.IWorkspace workspace) { }
    }
    public class InvalidWorkspaceException : Orc.WorkspaceManagement.WorkspaceException
    {
        public InvalidWorkspaceException(Orc.WorkspaceManagement.IWorkspace workspace) { }
        public InvalidWorkspaceException(Orc.WorkspaceManagement.IWorkspace workspace, string message) { }
        public InvalidWorkspaceException(Orc.WorkspaceManagement.IWorkspace workspace, string message, System.Exception innerException) { }
    }
    public interface IWorkspace
    {
        bool CanDelete { get; set; }
        bool CanEdit { get; set; }
        string DisplayName { get; }
        bool IsDirty { get; }
        bool IsVisible { get; set; }
        bool Persist { get; set; }
        [Catel.Runtime.Serialization.ExcludeFromSerializationAttribute()]
        object Scope { get; set; }
        string Title { get; set; }
        void ClearWorkspaceValues();
        System.Collections.Generic.List<string> GetAllWorkspaceValueNames();
        T GetWorkspaceValue<T>(string name, T defaultValue);
        void SetWorkspaceValue(string name, object value);
        void UpdateIsDirtyFlag(bool isDirty);
    }
    public interface IWorkspaceInitializer
    {
        System.Threading.Tasks.Task InitializeAsync(Orc.WorkspaceManagement.IWorkspace workspace);
    }
    public interface IWorkspaceManager
    {
        bool AutoRefreshEnabled { get; set; }
        string BaseDirectory { get; set; }
        string DefaultWorkspaceTitle { get; set; }
        System.Collections.Generic.IEnumerable<Orc.WorkspaceManagement.IWorkspaceProvider> Providers { get; }
        Orc.WorkspaceManagement.IWorkspace RefreshingWorkspace { get; }
        object Scope { get; set; }
        Orc.WorkspaceManagement.IWorkspace Workspace { get; }
        System.Collections.Generic.IEnumerable<Orc.WorkspaceManagement.IWorkspace> Workspaces { get; }
        public event System.EventHandler<System.EventArgs> Initialized;
        public event System.EventHandler<System.ComponentModel.CancelEventArgs> Initializing;
        public event System.EventHandler<System.EventArgs> Saved;
        public event Catel.AsyncEventHandler<System.ComponentModel.CancelEventArgs> SavingAsync;
        public event System.EventHandler<Orc.WorkspaceManagement.WorkspaceEventArgs> WorkspaceAdded;
        public event System.EventHandler<Orc.WorkspaceManagement.WorkspaceEventArgs> WorkspaceInfoRequested;
        public event System.EventHandler<Orc.WorkspaceManagement.WorkspaceProviderEventArgs> WorkspaceProviderAdded;
        public event System.EventHandler<Orc.WorkspaceManagement.WorkspaceProviderEventArgs> WorkspaceProviderRemoved;
        public event System.EventHandler<Orc.WorkspaceManagement.WorkspaceEventArgs> WorkspaceRemoved;
        public event System.EventHandler<System.EventArgs> WorkspacesChanged;
        public event System.EventHandler<Orc.WorkspaceManagement.WorkspaceUpdatedEventArgs> WorkspaceUpdated;
        public event Catel.AsyncEventHandler<Orc.WorkspaceManagement.WorkspaceUpdatingEventArgs> WorkspaceUpdatingAsync;
        System.Threading.Tasks.Task AddAsync(Orc.WorkspaceManagement.IWorkspace workspace);
        void AddProvider(Orc.WorkspaceManagement.IWorkspaceProvider workspaceProvider);
        System.Threading.Tasks.Task ApplyWorkspaceUsingProvidersAsync(Orc.WorkspaceManagement.IWorkspace workspace);
        System.Threading.Tasks.Task GetInformationFromProvidersAsync(Orc.WorkspaceManagement.IWorkspace workspace);
        System.Collections.Generic.List<Orc.WorkspaceManagement.IWorkspaceProvider> GetWorkspaceProviders();
        System.Threading.Tasks.Task InitializeAsync();
        System.Threading.Tasks.Task InitializeAsync(bool autoSelect);
        System.Threading.Tasks.Task RefreshWorkspaceAsync(Orc.WorkspaceManagement.IWorkspace workspace);
        System.Threading.Tasks.Task<bool> RemoveAsync(Orc.WorkspaceManagement.IWorkspace workspace);
        bool RemoveProvider(Orc.WorkspaceManagement.IWorkspaceProvider workspaceProvider);
        System.Threading.Tasks.Task<bool> SaveAsync();
        System.Threading.Tasks.Task SetWorkspaceAsync(Orc.WorkspaceManagement.IWorkspace value);
        System.Threading.Tasks.Task StoreWorkspaceAsync();
        System.Threading.Tasks.Task StoreWorkspaceAsync(Orc.WorkspaceManagement.IWorkspace workspace);
        System.Threading.Tasks.Task<bool> TryInitializeAsync();
        System.Threading.Tasks.Task<bool> TryInitializeAsync(bool autoSelect);
        System.Threading.Tasks.Task<bool> TrySetWorkspaceAsync(Orc.WorkspaceManagement.IWorkspace value);
        System.Threading.Tasks.Task UpdateIsDirtyFlagAsync(Orc.WorkspaceManagement.IWorkspace workspace);
    }
    public class static IWorkspaceManagerExtensions
    {
        public static System.Threading.Tasks.Task AddAsync(this Orc.WorkspaceManagement.IWorkspaceManager workspaceManager, Orc.WorkspaceManagement.IWorkspace workspace, bool autoSelect) { }
        public static System.Threading.Tasks.Task AddProviderAsync(this Orc.WorkspaceManagement.IWorkspaceManager workspaceManager, Orc.WorkspaceManagement.IWorkspaceProvider workspaceProvider, bool callApplyWorkspaceForCurrentWorkspace) { }
        public static System.Threading.Tasks.Task AddProviderAsync<TWorkspaceProvider>(this Orc.WorkspaceManagement.IWorkspaceManager workspaceManager, bool callApplyWorkspaceForCurrentWorkspace)
            where TWorkspaceProvider : Orc.WorkspaceManagement.IWorkspaceProvider { }
        public static System.Threading.Tasks.Task<bool> CheckIsDirtyAsync(this Orc.WorkspaceManagement.IWorkspaceManager workspaceManager) { }
        public static System.Threading.Tasks.Task EnsureDefaultWorkspaceAsync(this Orc.WorkspaceManagement.IWorkspaceManager workspaceManager, string defaultWorkspaceName = "Default", bool autoSelect = True) { }
        public static Orc.WorkspaceManagement.IWorkspace FindWorkspace(this Orc.WorkspaceManagement.IWorkspaceManager workspaceManager, string workspaceName) { }
        public static TWorkspace FindWorkspace<TWorkspace>(this Orc.WorkspaceManagement.IWorkspaceManager workspaceManager, string workspaceName)
            where TWorkspace : Orc.WorkspaceManagement.IWorkspace { }
        public static TWorkspace GetWorkspace<TWorkspace>(this Orc.WorkspaceManagement.IWorkspaceManager workspaceManager)
            where TWorkspace : Orc.WorkspaceManagement.IWorkspace { }
        public static System.Threading.Tasks.Task InitializeAsync(this Orc.WorkspaceManagement.IWorkspaceManager workspaceManager, bool addDefaultWorkspaceIfNoWorkspacesAreFound = True, bool alwaysEnsureDefaultWorkspace = True, string defaultWorkspaceName = "Default", bool autoSelect = True) { }
        public static System.Threading.Tasks.Task<bool> IsWorkspaceDirtyAsync(this Orc.WorkspaceManagement.IWorkspaceManager workspaceManager, Orc.WorkspaceManagement.IWorkspace workspace) { }
        public static System.Threading.Tasks.Task RefreshCurrentWorkspaceAsync(this Orc.WorkspaceManagement.IWorkspaceManager workspaceManager) { }
        public static System.Threading.Tasks.Task SetWorkspaceSchemesDirectoryAsync(this Orc.WorkspaceManagement.IWorkspaceManager workspaceManager, string directoryName, bool addDefaultWorkspaceIfNoWorkspacesAreFound = True, bool alwaysEnsureDefaultWorkspace = True, string defaultWorkspaceName = "Default", bool autoselectDefault = True) { }
        public static System.Threading.Tasks.Task StoreAndSaveAsync(this Orc.WorkspaceManagement.IWorkspaceManager workspaceManager) { }
    }
    public interface IWorkspaceProvider
    {
        object Scope { get; set; }
        object Tag { get; set; }
        System.Threading.Tasks.Task ApplyWorkspaceAsync(Orc.WorkspaceManagement.IWorkspace workspace);
        System.Threading.Tasks.Task<bool> CheckIsDirtyAsync(Orc.WorkspaceManagement.IWorkspace workspace);
        System.Threading.Tasks.Task ProvideInformationAsync(Orc.WorkspaceManagement.IWorkspace workspace);
    }
    public interface IWorkspacesStorageService
    {
        string GetWorkspaceFileName(string directory, Orc.WorkspaceManagement.IWorkspace workspace);
        Orc.WorkspaceManagement.IWorkspace LoadWorkspace(string fileName);
        System.Collections.Generic.IEnumerable<Orc.WorkspaceManagement.IWorkspace> LoadWorkspaces(string path);
        void SaveWorkspace(string fileName, Orc.WorkspaceManagement.IWorkspace workspace);
        void SaveWorkspaces(string path, System.Collections.Generic.IEnumerable<Orc.WorkspaceManagement.IWorkspace> workspaces);
    }
    public class Workspace : Catel.Configuration.DynamicConfiguration, Orc.WorkspaceManagement.IWorkspace
    {
        public static readonly Catel.Data.PropertyData CanDeleteProperty;
        public static readonly Catel.Data.PropertyData CanEditProperty;
        public static readonly Catel.Data.PropertyData DisplayNameProperty;
        public static readonly Catel.Data.PropertyData IsDirtyProperty;
        public static readonly Catel.Data.PropertyData IsVisibleProperty;
        public static readonly Catel.Data.PropertyData PersistProperty;
        public static readonly Catel.Data.PropertyData ScopeProperty;
        public static readonly Catel.Data.PropertyData TagProperty;
        public static readonly Catel.Data.PropertyData TitleProperty;
        public Workspace() { }
        public bool CanDelete { get; set; }
        public bool CanEdit { get; set; }
        public string DisplayName { get; }
        public bool IsDirty { get; }
        public bool IsVisible { get; set; }
        public bool Persist { get; set; }
        [Catel.Runtime.Serialization.ExcludeFromSerializationAttribute()]
        public object Scope { get; set; }
        [Catel.Runtime.Serialization.ExcludeFromSerializationAttribute()]
        public object Tag { get; set; }
        public string Title { get; set; }
        public void ClearWorkspaceValues() { }
        public override bool Equals(object obj) { }
        public System.Collections.Generic.List<string> GetAllWorkspaceValueNames() { }
        public override int GetHashCode() { }
        public T GetWorkspaceValue<T>(string name, T defaultValue) { }
        protected override void OnPropertyChanged(Catel.Data.AdvancedPropertyChangedEventArgs e) { }
        public void SetWorkspaceValue(string name, object value) { }
        public override string ToString() { }
        public void UpdateIsDirtyFlag(bool isDirty) { }
    }
    public class WorkspaceEventArgs : System.EventArgs
    {
        public WorkspaceEventArgs(Orc.WorkspaceManagement.IWorkspace workspace) { }
        public Orc.WorkspaceManagement.IWorkspace Workspace { get; }
    }
    public class WorkspaceException : System.Exception
    {
        public WorkspaceException(Orc.WorkspaceManagement.IWorkspace workspace) { }
        public WorkspaceException(Orc.WorkspaceManagement.IWorkspace workspace, string message) { }
        public WorkspaceException(Orc.WorkspaceManagement.IWorkspace workspace, string message, System.Exception innerException) { }
        public Orc.WorkspaceManagement.IWorkspace Workspace { get; }
    }
    public class static WorkspaceExtensions
    {
        public static void SynchronizeWithWorkspace(this Orc.WorkspaceManagement.IWorkspace workspace, Orc.WorkspaceManagement.IWorkspace newWorkspaceData) { }
    }
    public class WorkspaceManagementInitializationException : System.Exception
    {
        public WorkspaceManagementInitializationException(Orc.WorkspaceManagement.IWorkspaceManager workspaceManager) { }
        public WorkspaceManagementInitializationException(Orc.WorkspaceManagement.IWorkspaceManager workspaceManager, string message) { }
        public WorkspaceManagementInitializationException(Orc.WorkspaceManagement.IWorkspaceManager workspaceManager, string message, System.Exception innerException) { }
        public Orc.WorkspaceManagement.IWorkspaceManager WorkspaceManager { get; }
    }
    public class WorkspaceManager : Orc.WorkspaceManagement.IWorkspaceManager
    {
        public WorkspaceManager(Orc.WorkspaceManagement.IWorkspaceInitializer workspaceInitializer, Orc.WorkspaceManagement.IWorkspacesStorageService workspacesStorageService, Catel.IoC.IServiceLocator serviceLocator) { }
        public bool AutoRefreshEnabled { get; set; }
        public string BaseDirectory { get; set; }
        public string DefaultWorkspaceTitle { get; set; }
        public System.Collections.Generic.IEnumerable<Orc.WorkspaceManagement.IWorkspaceProvider> Providers { get; }
        public Orc.WorkspaceManagement.IWorkspace RefreshingWorkspace { get; }
        public object Scope { get; set; }
        public int UniqueIdentifier { get; }
        public Orc.WorkspaceManagement.IWorkspace Workspace { get; }
        public System.Collections.Generic.IEnumerable<Orc.WorkspaceManagement.IWorkspace> Workspaces { get; }
        public event System.EventHandler<System.EventArgs> Initialized;
        public event System.EventHandler<System.ComponentModel.CancelEventArgs> Initializing;
        public event System.EventHandler<System.EventArgs> Saved;
        public event Catel.AsyncEventHandler<System.ComponentModel.CancelEventArgs> SavingAsync;
        public event System.EventHandler<Orc.WorkspaceManagement.WorkspaceEventArgs> WorkspaceAdded;
        public event System.EventHandler<Orc.WorkspaceManagement.WorkspaceEventArgs> WorkspaceInfoRequested;
        public event System.EventHandler<Orc.WorkspaceManagement.WorkspaceProviderEventArgs> WorkspaceProviderAdded;
        public event System.EventHandler<Orc.WorkspaceManagement.WorkspaceProviderEventArgs> WorkspaceProviderRemoved;
        public event System.EventHandler<Orc.WorkspaceManagement.WorkspaceEventArgs> WorkspaceRemoved;
        public event System.EventHandler<System.EventArgs> WorkspacesChanged;
        public event System.EventHandler<Orc.WorkspaceManagement.WorkspaceUpdatedEventArgs> WorkspaceUpdated;
        public event Catel.AsyncEventHandler<Orc.WorkspaceManagement.WorkspaceUpdatingEventArgs> WorkspaceUpdatingAsync;
        public System.Threading.Tasks.Task AddAsync(Orc.WorkspaceManagement.IWorkspace workspace) { }
        public void AddProvider(Orc.WorkspaceManagement.IWorkspaceProvider workspaceProvider) { }
        public System.Threading.Tasks.Task ApplyWorkspaceUsingProvidersAsync(Orc.WorkspaceManagement.IWorkspace workspace) { }
        public System.Threading.Tasks.Task GetInformationFromProvidersAsync(Orc.WorkspaceManagement.IWorkspace workspace) { }
        public System.Collections.Generic.List<Orc.WorkspaceManagement.IWorkspaceProvider> GetWorkspaceProviders() { }
        public System.Threading.Tasks.Task InitializeAsync() { }
        public System.Threading.Tasks.Task InitializeAsync(bool autoSelect) { }
        public System.Threading.Tasks.Task RefreshWorkspaceAsync(Orc.WorkspaceManagement.IWorkspace workspace) { }
        public System.Threading.Tasks.Task ReloadWorkspaceAsync() { }
        public System.Threading.Tasks.Task<bool> RemoveAsync(Orc.WorkspaceManagement.IWorkspace workspace) { }
        public bool RemoveProvider(Orc.WorkspaceManagement.IWorkspaceProvider workspaceProvider) { }
        public System.Threading.Tasks.Task<bool> SaveAsync() { }
        public System.Threading.Tasks.Task SetWorkspaceAsync(Orc.WorkspaceManagement.IWorkspace value) { }
        public System.Threading.Tasks.Task StoreWorkspaceAsync() { }
        public System.Threading.Tasks.Task StoreWorkspaceAsync(Orc.WorkspaceManagement.IWorkspace workspace) { }
        public System.Threading.Tasks.Task<bool> TryInitializeAsync() { }
        public System.Threading.Tasks.Task<bool> TryInitializeAsync(bool autoSelect) { }
        public System.Threading.Tasks.Task<bool> TrySetWorkspaceAsync(Orc.WorkspaceManagement.IWorkspace value) { }
        public System.Threading.Tasks.Task UpdateIsDirtyFlagAsync(Orc.WorkspaceManagement.IWorkspace workspace) { }
    }
    public abstract class WorkspaceProviderBase : Orc.WorkspaceManagement.IWorkspaceProvider
    {
        protected readonly Catel.IoC.IServiceLocator ServiceLocator;
        protected WorkspaceProviderBase(Orc.WorkspaceManagement.IWorkspaceManager workspaceManager, Catel.IoC.IServiceLocator serviceLocator) { }
        public virtual object Scope { get; set; }
        public object Tag { get; set; }
        protected Orc.WorkspaceManagement.IWorkspaceManager WorkspaceManager { get; set; }
        public abstract System.Threading.Tasks.Task ApplyWorkspaceAsync(Orc.WorkspaceManagement.IWorkspace workspace);
        public virtual System.Threading.Tasks.Task<bool> CheckIsDirtyAsync(Orc.WorkspaceManagement.IWorkspace workspace) { }
        public abstract System.Threading.Tasks.Task ProvideInformationAsync(Orc.WorkspaceManagement.IWorkspace workspace);
    }
    public class WorkspaceProviderEventArgs : System.EventArgs
    {
        public WorkspaceProviderEventArgs(Orc.WorkspaceManagement.IWorkspaceProvider workspaceProvider) { }
        public Orc.WorkspaceManagement.IWorkspaceProvider WorkspaceProvider { get; }
    }
    public class WorkspacesStorageService : Orc.WorkspaceManagement.IWorkspacesStorageService
    {
        public WorkspacesStorageService(Catel.Runtime.Serialization.ISerializationManager serializationManager, Catel.Runtime.Serialization.Xml.IXmlSerializer xmlSerializer) { }
        public string GetWorkspaceFileName(string directory, Orc.WorkspaceManagement.IWorkspace workspace) { }
        public Orc.WorkspaceManagement.IWorkspace LoadWorkspace(string fileName) { }
        public System.Collections.Generic.IEnumerable<Orc.WorkspaceManagement.IWorkspace> LoadWorkspaces(string path) { }
        public void SaveWorkspace(string fileName, Orc.WorkspaceManagement.IWorkspace workspace) { }
        public void SaveWorkspaces(string path, System.Collections.Generic.IEnumerable<Orc.WorkspaceManagement.IWorkspace> workspaces) { }
    }
    public class WorkspaceUpdatedEventArgs : System.EventArgs
    {
        public WorkspaceUpdatedEventArgs(Orc.WorkspaceManagement.IWorkspace oldWorkspace, Orc.WorkspaceManagement.IWorkspace newWorkspace) { }
        public bool IsRefresh { get; }
        public Orc.WorkspaceManagement.IWorkspace NewWorkspace { get; }
        public Orc.WorkspaceManagement.IWorkspace OldWorkspace { get; }
    }
    public class WorkspaceUpdatingEventArgs : System.ComponentModel.CancelEventArgs
    {
        public WorkspaceUpdatingEventArgs(Orc.WorkspaceManagement.IWorkspace oldWorkspace, Orc.WorkspaceManagement.IWorkspace newWorkspace, bool cancel = False) { }
        public bool IsRefresh { get; }
        public Orc.WorkspaceManagement.IWorkspace NewWorkspace { get; set; }
        public Orc.WorkspaceManagement.IWorkspace OldWorkspace { get; set; }
    }
    public abstract class WorkspaceWatcherBase : System.IDisposable
    {
        protected readonly Orc.WorkspaceManagement.IWorkspaceManager WorkspaceManager;
        protected WorkspaceWatcherBase(Orc.WorkspaceManagement.IWorkspaceManager workspaceManager) { }
        protected bool IgnoreSwitchToNewlyCreatedWorkspace { get; set; }
        public void Dispose() { }
        protected virtual void OnSaved() { }
        protected virtual System.Threading.Tasks.Task<bool> OnSavingAsync() { }
        protected virtual void OnWorkspaceAdded(Orc.WorkspaceManagement.IWorkspace workspace) { }
        protected virtual void OnWorkspaceProviderAdded(Orc.WorkspaceManagement.IWorkspaceProvider workspaceProvider) { }
        protected virtual void OnWorkspaceProviderRemoved(Orc.WorkspaceManagement.IWorkspaceProvider workspaceProvider) { }
        protected virtual void OnWorkspaceRemoved(Orc.WorkspaceManagement.IWorkspace workspace) { }
        protected virtual void OnWorkspaceUpdated(Orc.WorkspaceManagement.IWorkspace oldWorkspace, Orc.WorkspaceManagement.IWorkspace newWorkspace, bool isRefresh) { }
        protected virtual System.Threading.Tasks.Task<bool> OnWorkspaceUpdatingAsync(Orc.WorkspaceManagement.IWorkspace oldWorkspace, Orc.WorkspaceManagement.IWorkspace newWorkspace, bool isRefresh) { }
        protected virtual bool ShouldIgnoreWorkspaceChange() { }
    }
}