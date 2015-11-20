// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkspaceProviderLocator.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class WorkspaceProviderLocator : IWorkspaceProviderLocator
    {
        public virtual IEnumerable<Task<IWorkspaceProvider>> ResolveAllProviders(object tag = null)
        {
            return Enumerable.Empty<Task<IWorkspaceProvider>>();
        }
    }
}