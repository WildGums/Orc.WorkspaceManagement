// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWorkspaceProviderLocator.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IWorkspaceProviderLocator
    {
        IEnumerable<Task<IWorkspaceProvider>> ResolveAllProviders(object tag = null);
    }
}