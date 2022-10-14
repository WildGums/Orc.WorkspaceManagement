using System.Reflection;
using System.Resources;
using System.Windows;
using System.Windows.Markup;

// All other assembly info is defined in SharedAssembly.cs

[assembly: AssemblyTitle("Orc.WorkspaceManagement.Xaml")]
[assembly: AssemblyProduct("Orc.WorkspaceManagement.Xaml")]
[assembly: AssemblyDescription("Orc.WorkspaceManagement.Xaml library")]

[assembly: NeutralResourcesLanguage("en-US")]

[assembly: XmlnsPrefix("http://schemas.wildgums.com/orc/workspacemanagement", "orcworkspacemanagement")]
[assembly: XmlnsDefinition("http://schemas.wildgums.com/orc/workspacemanagement", "Orc.WorkspaceManagement")]
[assembly: XmlnsDefinition("http://schemas.wildgums.com/orc/workspacemanagement", "Orc.WorkspaceManagement.Behaviors")]
//[assembly: XmlnsDefinition("http://schemas.wildgums.com/orc/workspacemanagement", "Orc.WorkspaceManagement.Controls")]
[assembly: XmlnsDefinition("http://schemas.wildgums.com/orc/workspacemanagement", "Orc.WorkspaceManagement.Converters")]
//[assembly: XmlnsDefinition("http://schemas.wildgums.com/orc/workspacemanagement", "Orc.WorkspaceManagement.Fonts")]
//[assembly: XmlnsDefinition("http://schemas.wildgums.com/orc/workspacemanagement", "Orc.WorkspaceManagement.Markup")]
[assembly: XmlnsDefinition("http://schemas.wildgums.com/orc/workspacemanagement", "Orc.WorkspaceManagement.Views")]
//[assembly: XmlnsDefinition("http://schemas.wildgums.com/orc/workspacemanagement", "Orc.WorkspaceManagement.Windows")]

[assembly: ThemeInfo(
    ResourceDictionaryLocation.None, //where theme specific resource dictionaries are located
    //(used if a resource is not found in the page, 
    // or application resource dictionaries)
    ResourceDictionaryLocation.SourceAssembly //where the generic resource dictionary is located
    //(used if a resource is not found in the page, 
    // app, or any theme specific resource dictionaries)
)]
