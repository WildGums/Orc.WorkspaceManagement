// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWorkspace.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement
{
    using Catel.Data;

    public interface IWorkspace : IModel
    {
        string Title { get; set; }
    }
}