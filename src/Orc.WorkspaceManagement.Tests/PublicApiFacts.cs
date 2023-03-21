namespace Orc.WorkspaceManagement.Tests;

using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using NUnit.Framework;
using PublicApiGenerator;
using VerifyNUnit;
using ViewModels;

[TestFixture]
public class PublicApiFacts
{
    [Test, MethodImpl(MethodImplOptions.NoInlining)]
    public async Task Orc_WorkspaceManagement_HasNoBreakingChanges_Async()
    {
        var assembly = typeof(WorkspaceManager).Assembly;

        await PublicApiApprover.ApprovePublicApiAsync(assembly);
    }

    [Test, MethodImpl(MethodImplOptions.NoInlining)]
    public async Task Orc_WorkspaceManagement_Xaml_HasNoBreakingChanges_Async()
    {
        var assembly = typeof(WorkspacesViewModel).Assembly;

        await PublicApiApprover.ApprovePublicApiAsync(assembly);
    }

    internal static class PublicApiApprover
    {
        public static async Task ApprovePublicApiAsync(Assembly assembly)
        {
            var publicApi = ApiGenerator.GeneratePublicApi(assembly, new ApiGeneratorOptions());
            await Verifier.Verify(publicApi);
        }
    }
}
