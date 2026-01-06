namespace Orc.WorkspaceManagement.Example.Services;

using System;
using Orchestra;
using System.Threading.Tasks;

public class ApplicationInitializationService : ApplicationInitializationServiceBase
{
    public ApplicationInitializationService(IServiceProvider serviceProvider)
        : base(serviceProvider)
    {

    }

    public override Task InitializeBeforeCreatingShellAsync()
    {
        return Task.CompletedTask;
    }
}
