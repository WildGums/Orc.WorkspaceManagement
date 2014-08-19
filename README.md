# Orc.WorkspaceManagement

Manage workspaces the easy way using this library.

# Quick introduction

The workspace management library makes it easy to manage workspaces. The main component is the *IWorkspaceManager* that contains the current workspace and allows to load or save a workspace. Note that this library does not force you to use a specific workspace location of any sort, so it can even be a database or server call to read / write the workspace. 

Below is an overview of the most important components:

- **IWorkspace** => the actual workspace object
- **IWorkspaceManager** => the workspace manager with events and management methods
- **IWorkspaceInitializer** => allows customization of initial settings of a workspace
- **IWorkspaceReader** => reads a workspace from a location
- **IWorkspaceWriter** => writes a workspace to a location

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
* **DirectoryWorkspaceInitializer** => First checks if there is an app config setting called *DataLocation*. If so, it will use that. If not, it will fall back to *%AppData%\\[assembly company]\\[assembly product]\\data*. Then it will also check if a command line directory is passed (first argument). If so, all previous will be overriden by the command line directory.
* **FileWorkspaceInitializer** => This will check if a command line file is passed (first argument). If so, it will be used as initial workspace. Otherwise no workspace will be loaded.

To create a custom workspace initializer, see the example below:

Next it can be registered in the ServiceLocator (so it will automatically be injected into the *WorkspaceManager*):

	ServiceLocator.Default.RegisterType<IWorkspaceReaderService, MyWorkspaceReaderService>();


**Make sure to register the service before instantiating the *IWorkspaceManager* because it will be injected**

# Creating a workspace reader service

Workspaces must be read via the *IWorkspaceReaderService*. The workspace manager automatically knows when to read a workspace. First, one must create a workspace reader as shown in the example below:

	public class WorkspaceWriter : WorkspaceWriterBase<MyWorkspace>
	{
	    protected override async Task WriteToLocation(MyWorkspace workspace, string location)
	    {
	        // TODO: Write to a file / directory / database / anything
	    }
	}

Next it can be registered in the ServiceLocator (so it will automatically be injected into the *WorkspaceManager*):

	ServiceLocator.Default.RegisterType<IWorkspaceReaderService, MyWorkspaceReaderService>();

# Creating a workspace writer service

	public class WorkspaceReader : WorkspaceReaderBase
	{
	    protected override async Task<IWorkspace> ReadFromLocation(string location)
	    {
	        var workspace = new MyWorkspace(location);
	
	        // TODO: Read from a file / directory / database / anything

			return workspace;
	    }
	}

Next it can be registered in the ServiceLocator (so it will automatically be injected into the *WorkspaceManager*):

	ServiceLocator.Default.RegisterType<IWorkspaceWriterService, MyWorkspaceWriterService>();

# Initializing the workspace manager

Because the workspace manager is using async, the initialization is a separate method. This gives the developer the option to load the workspace whenever it is required. To (optionally) initialize the workspace manager, use the code below:

	await workspaceManager.Initialize(); 

# Retrieving a typed instance of the workspace

The library contains extension methods for the *IWorkspaceManager* to retrieve a typed instance:

	var myWorkspace = workspaceManager.GetWorkspace<MyWorkspace>();