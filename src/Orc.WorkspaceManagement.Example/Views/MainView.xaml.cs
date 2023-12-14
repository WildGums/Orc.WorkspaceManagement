﻿namespace Orc.WorkspaceManagement.Example.Views;

using Catel.Logging;

public partial class MainView
{
    private static readonly ILog Log = LogManager.GetCurrentClassLogger();

    public MainView()
    {
        InitializeComponent();

        Log.Info("Welcome to the example of Orc.WorkspaceManagement. Use any of the buttons above to control the workspace. Log messages will appear here");
    }
}
