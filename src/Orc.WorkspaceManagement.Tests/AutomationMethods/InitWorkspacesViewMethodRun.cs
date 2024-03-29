﻿namespace Orc.WorkspaceManagement.Tests;

using System.Threading.Tasks;
using System.Windows;
using Catel.IoC;
using Catel.Services;
using Catel.Threading;
using FileSystem;
using FilterBuilder.Tests;
using Orc.Automation;
using ViewModels;

public class InitWorkspacesViewMethodRun : NamedAutomationMethodRun
{
    public override bool TryInvoke(FrameworkElement owner, AutomationMethod method, out AutomationValue result)
    {
        result = AutomationValue.FromValue(true);

        if (owner is not Views.WorkspacesView workspacesView)
        {
            return true;
        }

#pragma warning disable IDISP001 // Dispose created
        var serviceLocator = this.GetServiceLocator();
#pragma warning restore IDISP001 // Dispose created

        var fileServiceType = typeof(FileService);
        var languageService = serviceLocator.ResolveType<ILanguageService>();
        languageService.RegisterLanguageSource(new LanguageResourceSource("Orc.WorkspaceManagement.Xaml", "Orc.WorkspaceManagement.Properties", "Resources"));

        serviceLocator.RegisterType<IWorkspaceManager, WorkspaceManager>();
        serviceLocator.RegisterType<IWorkspaceInitializer, EmptyWorkspaceInitializer>();
        serviceLocator.RegisterType<IWorkspacesStorageService, WorkspacesStorageService>();

        foreach (var scope in WorkspacesViewTestData.AvailableScopes)
        {
            RegisterScope(scope);
        }

#pragma warning disable IDISP004 // Don't ignore created IDisposable
        var vm = this.GetTypeFactory().CreateInstanceWithParametersAndAutoCompletion<WorkspacesViewModel>();
#pragma warning restore IDISP004 // Don't ignore created IDisposable
        workspacesView.DataContext = vm;

        return true;
    }

    private void RegisterScope(object scope)
    {
#pragma warning disable IDISP001 // Dispose created
        var serviceLocator = this.GetServiceLocator();
        var typeFactory = this.GetTypeFactory();
#pragma warning restore IDISP001 // Dispose created

        var testStorageService = new TestWorkspaceStorageService();
        serviceLocator.RegisterInstance(typeof(IWorkspacesStorageService), testStorageService, scope);

        var workspaceManager = typeFactory.CreateInstanceWithParametersAndAutoCompletionWithTag<WorkspaceManager>(scope);
        workspaceManager.Scope = scope;
        workspaceManager.SetWorkspaceSchemesDirectoryAsync(scope?.ToString() ?? string.Empty);
            
        var task = Task.Run(async () => await workspaceManager.InitializeAsync());
        Task.WaitAll(task);
          
        serviceLocator.RegisterInstance(typeof(IWorkspaceManager), workspaceManager, scope);
    }
}
