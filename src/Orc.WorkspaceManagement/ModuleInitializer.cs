using Catel.IoC;
using Orc.WorkspaceManagement;
using Orc.WorkspaceManagement.Services;

/// <summary>
/// Used by the ModuleInit. All code inside the Initialize method is ran as soon as the assembly is loaded.
/// </summary>
public static class ModuleInitializer
{
    /// <summary>
    /// Initializes the module.
    /// </summary>
    public static void Initialize()
    {
        var serviceLocator = ServiceLocator.Default;

        serviceLocator.RegisterType<ICommandLineService, CommandLineService>();
        serviceLocator.RegisterType<IWorkspaceManager, WorkspaceManager>();
        serviceLocator.RegisterType<IWorkspaceInitializer, EmptyWorkspaceInitializer>();
        //serviceLocator.RegisterType<IWorkspaceReaderService, WorkspaceReaderService>();
        //serviceLocator.RegisterType<IWorkspaceWriterService, WorkspaceWriterService>();
    }
}