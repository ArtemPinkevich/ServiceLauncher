namespace ServiceLauncher.ViewModel
{
    using Prism.Mvvm;

    class MainViewModel : BindableBase
    {
        public ServerLauncherViewModel ServerLauncherVm { get; }

        public MainViewModel(ServerLauncherViewModel serverLauncherVm)
        {
            ServerLauncherVm = serverLauncherVm;
        }
    }
}
