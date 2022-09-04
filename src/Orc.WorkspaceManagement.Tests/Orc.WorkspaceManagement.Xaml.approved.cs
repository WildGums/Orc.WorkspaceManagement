[assembly: System.Resources.NeutralResourcesLanguage("en-US")]
[assembly: System.Runtime.Versioning.TargetFramework(".NETCoreApp,Version=v6.0", FrameworkDisplayName="")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.wildgums.com/orc/workspacemanagement", "Orc.WorkspaceManagement")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.wildgums.com/orc/workspacemanagement", "Orc.WorkspaceManagement.Behaviors")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.wildgums.com/orc/workspacemanagement", "Orc.WorkspaceManagement.Converters")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.wildgums.com/orc/workspacemanagement", "Orc.WorkspaceManagement.Views")]
[assembly: System.Windows.Markup.XmlnsPrefix("http://schemas.wildgums.com/orc/workspacemanagement", "orcworkspacemanagement")]
[assembly: System.Windows.ThemeInfo(System.Windows.ResourceDictionaryLocation.None, System.Windows.ResourceDictionaryLocation.SourceAssembly)]
public static class ModuleInitializer
{
    public static void Initialize() { }
}
namespace Orc.WorkspaceManagement.Automation
{
    public class WorkspaceViewGroupItem
    {
        public WorkspaceViewGroupItem() { }
        public string GroupName { get; set; }
        public System.Collections.Generic.IReadOnlyList<Orc.WorkspaceManagement.Automation.WorkspaceViewItem> Items { get; set; }
    }
    [Orc.Automation.AutomatedControl(ControlTypeName="Pane")]
    public class WorkspaceViewGroupList : Orc.Automation.Controls.FrameworkElement
    {
        public WorkspaceViewGroupList(System.Windows.Automation.AutomationElement element) { }
        public System.Collections.Generic.IReadOnlyList<Orc.WorkspaceManagement.Automation.WorkspaceViewGroupItem> GetGroupItems() { }
    }
    public class WorkspaceViewItem : Orc.Automation.Controls.ListItem
    {
        public WorkspaceViewItem(System.Windows.Automation.AutomationElement element) { }
        public bool CanDelete() { }
        public bool CanEdit() { }
        public bool CanRefresh() { }
        public void Delete() { }
        public Orc.WorkspaceManagement.Automation.WorkspaceWindow Edit() { }
        public void Refresh() { }
        public override void Select() { }
    }
    public class WorkspaceViewItemMap : Orc.Automation.AutomationBase
    {
        public WorkspaceViewItemMap(System.Windows.Automation.AutomationElement element) { }
        public Orc.Automation.Controls.Button EditWorkspaceButton { get; }
        public Orc.Automation.Controls.Button RefreshWorkspaceButton { get; }
        public Orc.Automation.Controls.Button RemoveWorkspaceButton { get; }
        public Orc.Automation.Controls.Text Title { get; }
    }
    public class WorkspaceViewMap : Orc.Automation.AutomationBase
    {
        public WorkspaceViewMap(System.Windows.Automation.AutomationElement element) { }
        public Orc.WorkspaceManagement.Automation.WorkspaceViewGroupList GroupList { get; }
    }
    [Orc.Automation.ActiveAutomationModel]
    public class WorkspaceViewModel : Orc.Automation.ControlModel
    {
        public static readonly Catel.Data.PropertyData HasRefreshButtonProperty;
        public static readonly Catel.Data.PropertyData ScopeProperty;
        public WorkspaceViewModel(Orc.Automation.AutomationElementAccessor accessor) { }
        public bool HasRefreshButton { get; set; }
        public object Scope { get; set; }
    }
    public class WorkspaceViewPeer : Orc.Automation.AutomationControlPeerBase<Orc.WorkspaceManagement.Views.WorkspacesView>
    {
        public WorkspaceViewPeer(Orc.WorkspaceManagement.Views.WorkspacesView owner) { }
    }
    [Orc.Automation.AutomatedControl(Class=typeof(Orc.WorkspaceManagement.Views.WorkspaceWindow), ControlTypeName="Window")]
    public class WorkspaceWindow : Orc.Automation.Controls.Window
    {
        public WorkspaceWindow(System.Windows.Automation.AutomationElement element) { }
        public string Title { get; set; }
        public void Accept() { }
        public void Decline() { }
    }
    public class WorkspaceWindowMap : Orc.Automation.AutomationBase
    {
        public WorkspaceWindowMap(System.Windows.Automation.AutomationElement element) { }
        public Orc.Automation.Controls.Button CancelButton { get; }
        public Orc.Automation.Controls.Button OkButton { get; }
        public Orc.Automation.Controls.Edit TitleEdit { get; }
    }
    public class WorkspaceWindowPeer : Orc.Automation.AutomationControlPeerBase<Orc.WorkspaceManagement.Views.WorkspaceWindow>
    {
        public WorkspaceWindowPeer(Orc.WorkspaceManagement.Views.WorkspaceWindow owner) { }
        protected override System.Windows.Automation.Peers.AutomationControlType GetAutomationControlTypeCore() { }
    }
    [Orc.Automation.AutomatedControl(Class=typeof(Orc.WorkspaceManagement.Views.WorkspacesView))]
    public class WorkspacesView : Orc.Automation.Controls.FrameworkElement<Orc.WorkspaceManagement.Automation.WorkspaceViewModel, Orc.WorkspaceManagement.Automation.WorkspaceViewMap>
    {
        public WorkspacesView(System.Windows.Automation.AutomationElement element) { }
        public System.Collections.Generic.IReadOnlyList<Orc.WorkspaceManagement.Automation.WorkspaceViewGroupItem> GroupItems { get; }
    }
    public static class WorkspacesViewExtensions
    {
        public static void DeleteItem(this Orc.WorkspaceManagement.Automation.WorkspacesView workspacesView, string name, string groupName = null) { }
        public static Orc.WorkspaceManagement.Automation.WorkspaceWindow EditItem(this Orc.WorkspaceManagement.Automation.WorkspacesView workspacesView, string name, string groupName = null) { }
        public static Orc.WorkspaceManagement.Automation.WorkspaceViewItem GetItem(this Orc.WorkspaceManagement.Automation.WorkspacesView workspacesView, string name, string groupName = null) { }
        public static void SelectWorkspace(this Orc.WorkspaceManagement.Automation.WorkspacesView workspacesView, string name, string groupName = null) { }
    }
}
namespace Orc.WorkspaceManagement
{
    public class BehaviorWorkspaceProvider : Orc.WorkspaceManagement.WorkspaceProviderBase
    {
        public BehaviorWorkspaceProvider(Orc.WorkspaceManagement.IWorkspaceManager workspaceManager, Orc.WorkspaceManagement.Behaviors.IWorkspaceBehavior workspaceBehavior, Catel.Services.IDispatcherService dispatcherService, Catel.IoC.IServiceLocator serviceLocator) { }
        public override System.Threading.Tasks.Task ApplyWorkspaceAsync(Orc.WorkspaceManagement.IWorkspace workspace) { }
        public override System.Threading.Tasks.Task ProvideInformationAsync(Orc.WorkspaceManagement.IWorkspace workspace) { }
    }
    public static class FrameworkElementExtensions
    {
        public static void LoadGridValuesFromWorkspace(this System.Windows.FrameworkElement frameworkElement, Orc.WorkspaceManagement.IWorkspace workspace = null, string prefix = null) { }
        public static void LoadSizeFromWorkspace(this System.Windows.FrameworkElement frameworkElement, Orc.WorkspaceManagement.IWorkspace workspace = null, string prefix = null) { }
        public static T LoadValueFromWorkspace<T>(this System.Windows.FrameworkElement frameworkElement, string name, T defaultValue, Orc.WorkspaceManagement.IWorkspace workspace = null, string prefix = null) { }
        public static void SaveGridValuesToWorkspace(this System.Windows.FrameworkElement frameworkElement, Orc.WorkspaceManagement.IWorkspace workspace = null, string prefix = null) { }
        public static void SaveSizeToWorkspace(this System.Windows.FrameworkElement frameworkElement, Orc.WorkspaceManagement.IWorkspace workspace = null, string prefix = null) { }
        public static void SaveValueToWorkspace(this System.Windows.FrameworkElement frameworkElement, string name, object value, Orc.WorkspaceManagement.IWorkspace workspace = null, string prefix = null) { }
    }
    [System.Diagnostics.DebuggerDisplay("{Title}")]
    public class WorkspaceGroup
    {
        public WorkspaceGroup(string title, System.Collections.Generic.IEnumerable<Orc.WorkspaceManagement.IWorkspace> workspaces) { }
        public string Title { get; }
        public System.Collections.Generic.List<Orc.WorkspaceManagement.IWorkspace> Workspaces { get; }
    }
}
namespace Orc.WorkspaceManagement.Behaviors
{
    public class AutoWorkspace : Orc.WorkspaceManagement.Behaviors.WorkspaceBehaviorBase<System.Windows.FrameworkElement>
    {
        public static readonly System.Windows.DependencyProperty PersistGridSettingsProperty;
        public static readonly System.Windows.DependencyProperty PersistSizeProperty;
        public AutoWorkspace() { }
        public bool PersistGridSettings { get; set; }
        public bool PersistSize { get; set; }
        protected override void LoadSettings(Orc.WorkspaceManagement.IWorkspace workspace, string prefix) { }
        protected override void SaveSettings(Orc.WorkspaceManagement.IWorkspace workspace, string prefix) { }
    }
    public class AutoWorkspaceGrid : Orc.WorkspaceManagement.Behaviors.WorkspaceBehaviorBase<System.Windows.Controls.Grid>
    {
        public static readonly System.Windows.DependencyProperty ColumnsToPersistProperty;
        public static readonly System.Windows.DependencyProperty RowsToPersistProperty;
        public AutoWorkspaceGrid() { }
        public string ColumnsToPersist { get; set; }
        public string RowsToPersist { get; set; }
        protected override void LoadSettings(Orc.WorkspaceManagement.IWorkspace workspace, string prefix) { }
        protected override void OnAssociatedObjectLoaded() { }
        protected override void SaveSettings(Orc.WorkspaceManagement.IWorkspace workspace, string prefix) { }
    }
    public interface IWorkspaceBehavior
    {
        void Load(Orc.WorkspaceManagement.IWorkspace workspace);
        void Save(Orc.WorkspaceManagement.IWorkspace workspace);
    }
    public abstract class WorkspaceBehaviorBase<T> : Catel.Windows.Interactivity.BehaviorBase<T>, Orc.WorkspaceManagement.Behaviors.IWorkspaceBehavior
        where T : System.Windows.FrameworkElement
    {
        public static readonly System.Windows.DependencyProperty KeyPrefixProperty;
        protected WorkspaceBehaviorBase() { }
        public string KeyPrefix { get; set; }
        protected Orc.WorkspaceManagement.IWorkspaceManager WorkspaceManager { get; }
        public void Load(Orc.WorkspaceManagement.IWorkspace workspace) { }
        protected abstract void LoadSettings(Orc.WorkspaceManagement.IWorkspace workspace, string prefix);
        protected override void OnAssociatedObjectLoaded() { }
        protected override void OnAssociatedObjectUnloaded() { }
        public void Save(Orc.WorkspaceManagement.IWorkspace workspace) { }
        protected abstract void SaveSettings(Orc.WorkspaceManagement.IWorkspace workspace, string prefix);
    }
}
namespace Orc.WorkspaceManagement.Converters
{
    public class IsCurrentWorkspaceToBooleanConverter : Catel.MVVM.Converters.ValueConverterBase
    {
        public IsCurrentWorkspaceToBooleanConverter() { }
        protected override object Convert(object value, System.Type targetType, object parameter) { }
    }
    public class IsCurrentWorkspaceToCollapsingVisibilityConverter : Catel.MVVM.Converters.VisibilityConverterBase
    {
        public IsCurrentWorkspaceToCollapsingVisibilityConverter() { }
        protected override bool IsVisible(object value, System.Type targetType, object parameter) { }
    }
    public class IsCurrentWorkspaceToHidingVisibilityConverter : Catel.MVVM.Converters.VisibilityConverterBase
    {
        public IsCurrentWorkspaceToHidingVisibilityConverter() { }
        protected override bool IsVisible(object value, System.Type targetType, object parameter) { }
    }
    public class TriggerConverter : System.Windows.Data.IMultiValueConverter
    {
        public TriggerConverter() { }
        public object Convert(object[] values, System.Type targetType, object parameter, System.Globalization.CultureInfo culture) { }
        public object[] ConvertBack(object value, System.Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture) { }
    }
    public class UnderscoreToDoubleUnderscoresStringConverter : Catel.MVVM.Converters.ValueConverterBase<string>
    {
        public UnderscoreToDoubleUnderscoresStringConverter() { }
        protected override object Convert(string value, System.Type targetType, object parameter) { }
    }
}
namespace Orc.WorkspaceManagement.ViewModels
{
    public class WorkspaceViewModel : Catel.MVVM.ViewModelBase
    {
        public static readonly Catel.Data.PropertyData WorkspaceProperty;
        public static readonly Catel.Data.PropertyData WorkspaceTitleProperty;
        public WorkspaceViewModel(Orc.WorkspaceManagement.IWorkspace workspace, Catel.Services.ILanguageService languageService) { }
        [Catel.MVVM.Model]
        public Orc.WorkspaceManagement.IWorkspace Workspace { get; }
        [Catel.MVVM.ViewModelToModel("Workspace", "Title")]
        public string WorkspaceTitle { get; set; }
        protected override void ValidateFields(System.Collections.Generic.List<Catel.Data.IFieldValidationResult> validationResults) { }
    }
    public class WorkspacesViewModel : Catel.MVVM.ViewModelBase
    {
        public static readonly Catel.Data.PropertyData ScopeProperty;
        public static readonly Catel.Data.PropertyData WorkspaceGroupsProperty;
        public WorkspacesViewModel(Orc.WorkspaceManagement.IWorkspaceManager workspaceManager, Catel.Services.IUIVisualizerService uiVisualizerService, Catel.IoC.IServiceLocator serviceLocator, Catel.Services.IDispatcherService dispatcherService, Catel.Services.IMessageService messageService, Catel.Services.ILanguageService languageService) { }
        public Catel.MVVM.TaskCommand<Orc.WorkspaceManagement.IWorkspace> EditWorkspace { get; }
        public Catel.MVVM.TaskCommand<Orc.WorkspaceManagement.IWorkspace> Refresh { get; }
        public Catel.MVVM.TaskCommand<Orc.WorkspaceManagement.IWorkspace> RemoveWorkspace { get; }
        public object Scope { get; set; }
        public Orc.WorkspaceManagement.IWorkspace SelectedWorkspace { get; set; }
        public System.Collections.Generic.List<Orc.WorkspaceManagement.WorkspaceGroup> WorkspaceGroups { get; }
        protected override System.Threading.Tasks.Task CloseAsync() { }
        protected override System.Threading.Tasks.Task InitializeAsync() { }
    }
}
namespace Orc.WorkspaceManagement.Views
{
    public class WorkspaceWindow : Catel.Windows.DataWindow, System.Windows.Markup.IComponentConnector
    {
        public WorkspaceWindow() { }
        public WorkspaceWindow(Orc.WorkspaceManagement.ViewModels.WorkspaceViewModel viewModel) { }
        public void InitializeComponent() { }
        protected override System.Windows.Automation.Peers.AutomationPeer OnCreateAutomationPeer() { }
    }
    public class WorkspacesView : Catel.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector, System.Windows.Markup.IStyleConnector
    {
        public static readonly System.Windows.DependencyProperty HasRefreshButtonProperty;
        public static readonly System.Windows.DependencyProperty ScopeProperty;
        public WorkspacesView() { }
        public bool HasRefreshButton { get; set; }
        [Catel.MVVM.Views.ViewToViewModel("", MappingType=Catel.MVVM.Views.ViewToViewModelMappingType.ViewToViewModel)]
        public object Scope { get; set; }
        public void InitializeComponent() { }
        protected override System.Windows.Automation.Peers.AutomationPeer OnCreateAutomationPeer() { }
    }
}