// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Factories.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2015 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Orc.WorkspaceManagement.Test
{
    using System.Collections.Generic;
    using Moq;

    public static class Factories
    {
         public static class WorkspaceManager
         {
             public static IWorkspaceManager WithEmptyInitializer(IWorkspacesStorageService workspacesStorageService = null)
             {
                 if (workspacesStorageService == null)
                 {
                     workspacesStorageService = Mock.Of<IWorkspacesStorageService>();
                 }

                 return new WorkspaceManagement.WorkspaceManager(new EmptyWorkspaceInitializer(), workspacesStorageService);
             }
         }        
    }
}