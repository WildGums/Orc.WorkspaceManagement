// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PublicApiFacts.cs" company="WildGums">
//   Copyright (c) 2008 - 2017 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement.Tests
{
    using System.Runtime.CompilerServices;
    using ApiApprover;
    using NUnit.Framework;
    using ViewModels;

    [TestFixture]
    public class PublicApiFacts
    {
        [Test, MethodImpl(MethodImplOptions.NoInlining)]
        public void Orc_WorkspaceManagement_HasNoBreakingChanges()
        {
            var assembly = typeof(WorkspaceManager).Assembly;

            PublicApiApprover.ApprovePublicApi(assembly);
        }

        [Test, MethodImpl(MethodImplOptions.NoInlining)]
        public void Orc_WorkspaceManagement_Xaml_HasNoBreakingChanges()
        {
            var assembly = typeof(WorkspacesViewModel).Assembly;

            PublicApiApprover.ApprovePublicApi(assembly);
        }
    }
}