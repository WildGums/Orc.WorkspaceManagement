// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PublicApiFacts.cs" company="WildGums">
//   Copyright (c) 2008 - 2017 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement.Tests
{
    using ApiApprover;
    using NUnit.Framework;
    using ViewModels;

    [TestFixture]
    public class PublicApiFacts
    {
        [Test]
        public void Orc_WorkspaceManagement_HasNoBreakingChanges()
        {
            var assembly = typeof(WorkspaceManager).Assembly;

            PublicApiApprover.ApprovePublicApi(assembly);
        }

        [Test]
        public void Orc_WorkspaceManagement_Xaml_HasNoBreakingChanges()
        {
            var assembly = typeof(WorkspacesViewModel).Assembly;

            PublicApiApprover.ApprovePublicApi(assembly);
        }
    }
}