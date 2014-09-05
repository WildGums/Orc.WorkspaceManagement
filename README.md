# Orc.WorkspaceManagement

Manage workspaces the easy way using this library.

# Quick introduction

Workspaces are a combination of settings that a user can choose to configure an application. An example is the layout of all docking windows in Visual Studio. The advantage of the workspace management is that it takes away all the plumbing and you can concentrate on the actual usage of workspaces.

The workspace management library makes it easy to manage workspaces. The main component is the *IWorkspaceManager* that contains the current workspace and all available workspaces and allows to load or save workspaces.

Below is an overview of the most important components:

- **IWorkspace** => the actual workspace object
- **IWorkspaceManager** => the workspace manager with events and management methods
- **IWorkspaceInitializer** => allows customization of initial settings of a workspace

![Managing workspaces the easy way](doc/images/workspace_handling.gif)  

-- 

**Important note** 

The base directory will be used as repository. This means that it cannot contain other files and all other files will be deleted from the directory

-- 

# Creating a workspace

A workspace is a model that can be implemented by the developer and must implement the *IWorkspace* interface. The most convenient way to implement a workspace is by deriving from the *WorkspaceBase* class:

    public class MyWorkspace : WorkspaceBase
    {
    	public Workspace(string title)
    		: base(title)
	    {
	    }
	    
	    public string FirstName { get; private set; }
	    
	    public string LastName { get; private set; }
    }

# Creating a workspace initializer

When a workspace manager is created, it doesn't contain anything. The *IWorkspaceInitializer* interface allows the customization of that state. 

By default the following initializers are available:

* **EmptyWorkspaceInitializer** => initializes nothing, this is the default

To create a custom workspace initializer, see the example below:

    public class WorkspaceInitializer : IWorkspaceInitializer
    {
        public void Initialize(IWorkspace workspace)
        {
            workspace.SetValue("AView.Width", 200d);
            workspace.SetValue("BView.Width", 200d);
        }
    }

Next it can be registered in the ServiceLocator (so it will automatically be injected into the *WorkspaceManager*):

	ServiceLocator.Default.RegisterType<IWorkspaceInitializer, MyWorkspaceInitializer>();


**Make sure to register the service before instantiating the *IWorkspaceManager* because it will be injected**

# Initializing the workspace manager

Because the workspace manager is using async, the initialization is a separate method. This gives the developer the option to load the workspace whenever it is required. To (optionally) initialize the workspace manager, use the code below:

	await workspaceManager.Initialize(); 

# Retrieving a list of all workspaces

    var workspaces = workspaceManager.Workspaces;

# Retrieving the current workspace

The library contains extension methods for the *IWorkspaceManager* to retrieve a typed instance:

	var myWorkspace = workspaceManager.Workspace;

To customize the location where the workspaces are stored, use the *BaseDirectory* property.

# Storing information in a workspace

Storing information in a workspace is the responsibility of every single component in the application. The *IWorkspaceManager* will raise the *WorkspaceInfoRequested* event so every component can put in the required information into the workspace.

To store information in a workspace, set the workspace to be updated as current workspace. Then let the user (or software) customize all components. Call the following method to raise the *WorkspaceInfoRequested* event to update the workspace:

    workspaceManager.StoreWorkspace();

**Note that a workspace is only updated, not saved to disk by this method**.

# Saving all workspaces to disk

To save all workspaces to disk, use the code below:

    await workspaceManager.Save();

-- 

# Using the XAML behaviors

For the developers using XAML (WPF), behaviors and extension methods are available in the *Orc.WorkspaceManagement.Xaml* library.

## Using the extension methods

Using the extension methods still requires manual work by subscribing to events of both the view and workspace manager, but allow more control.

## Using the behaviors

The behavior is a wrapper around the extension methods and take away to need to manage anything. The behavior is aware of all the events and will handle everything accordingly. To use the behavior, use the code below:

    <views:BView Grid.Row="2" Grid.Column="4">
        <i:Interaction.Behaviors>
            <behaviors:AutoWorkspace />
        </i:Interaction.Behaviors>
    </views:BView>