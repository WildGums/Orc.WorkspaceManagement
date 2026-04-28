namespace Orc.WorkspaceManagement.Example.Views;

using Catel.Logging;
using Microsoft.Extensions.Logging;

public partial class MainView
{
    private static readonly ILogger Logger = LogManager.GetLogger(typeof(MainView));

    partial void OnInitializedComponent()
    {
        Logger.LogInformation("Welcome to the example of Orc.WorkspaceManagement. Use any of the buttons above to control the workspace. Log messages will appear here");
    }
}
