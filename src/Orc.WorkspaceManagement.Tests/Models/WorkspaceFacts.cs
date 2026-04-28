namespace Orc.WorkspaceManagement.Test.Models;

using NUnit.Framework;

public class WorkspaceFacts
{
    [TestFixture]
    public class TheGetConfigurationValueMethod
    {
        [TestCase("bool", false, null, true, true)]
        [TestCase("bool", true, false, true, false)]
        public void Correctly_Handles_Values_With_Defaults(string configurationKey, bool setValueBeforeRetrieving, object? valueToSet, object defaultValue, object expectedValue)
        {
            var workspace = new Workspace(WorkspaceNameHelper.GetRandomWorkspaceName());

            if (setValueBeforeRetrieving)
            {
                workspace.SetWorkspaceValue(configurationKey, valueToSet);
            }

            var currentValue = workspace.GetWorkspaceValue(configurationKey, defaultValue);

            Assert.That(currentValue, Is.EqualTo(expectedValue));
        }
    }
}
