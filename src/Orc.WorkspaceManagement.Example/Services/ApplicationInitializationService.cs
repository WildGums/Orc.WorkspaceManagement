namespace Orc.WorkspaceManagement.Example.Services
{
    using System;
    using Orchestra.Services;
    using System.Threading.Tasks;
    using System.Windows.Media;
    using Behaviors;
    using Catel;
    using Catel.IoC;
    using Catel.Threading;
    using Orchestra.Markup;
    using WorkspaceManagement;

    public class ApplicationInitializationService : ApplicationInitializationServiceBase
    {
        private readonly IServiceLocator _serviceLocator;

        public ApplicationInitializationService(IServiceLocator serviceLocator)
        {
            Argument.IsNotNull(() => serviceLocator);

            _serviceLocator = serviceLocator;
        }

        public override Task InitializeBeforeCreatingShellAsync()
        {
            InitializeFonts();
            RegisterTypes();

            return TaskHelper.Completed;
        }

        private void InitializeFonts()
        {
            Orc.Controls.FontImage.RegisterFont("FontAwesome", new FontFamily(new Uri("pack://application:,,,/Orc.WorkspaceManagement.Example;component/Resources/Fonts/", UriKind.RelativeOrAbsolute), "./#FontAwesome"));

            Orc.Controls.FontImage.DefaultBrush = new SolidColorBrush(Color.FromArgb(255, 87, 87, 87));
            Orc.Controls.FontImage.DefaultFontFamily = "FontAwesome";
        }

        private void RegisterTypes()
        {
            _serviceLocator.RegisterType<IWorkspaceInitializer, WorkspaceInitializer>();
        }
    }
}
