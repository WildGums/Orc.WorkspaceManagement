// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PublicApiFacts.cs" company="WildGums">
//   Copyright (c) 2008 - 2017 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.WorkspaceManagement.Tests
{
    using ApiApprover;
    using NUnit.Framework;

    [TestFixture]
    public class PublicApiFacts
    {
        [Test]
        public void Orc_WorkspaceManagement_HasNoBreakingChanges()
        {
            var assembly = typeof(WorkspaceManager).Assembly;

            PublicApiApprover.ApprovePublicApi(assembly);
        }
    }
}