namespace ServiceLauncher
{
    using System.Windows;

    using Logic;

    using Microsoft.Practices.Unity;

    using Prism.Unity;

    using View;

    using ViewModel;

    public class Bootstrapper : UnityBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return Container.TryResolve<MainView>();
        }

        protected override void InitializeShell()
        {
            if (Application.Current.MainWindow != null)
            {
                Application.Current.MainWindow.DataContext = Container.Resolve<MainViewModel>();
                Application.Current.MainWindow.Show();
            }
        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

            Container.RegisterType<IServiceManager, ServiceManager>(new ContainerControlledLifetimeManager());
            Container.RegisterType<ServerLauncherViewModel>(new ContainerControlledLifetimeManager());
        }
    }
}
