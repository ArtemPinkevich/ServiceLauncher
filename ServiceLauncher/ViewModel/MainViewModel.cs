namespace ServiceLauncher.ViewModel
{
    using System;

    using Logic;

    using Prism.Mvvm;

    class MainViewModel : BindableBase
    {
        #region Fields

        private bool _isInEditMode;
        private double _opacity;
        private string _serviceName;
        private readonly ISettingsManager _settingsManager;
        private bool _showTitleEnabled;

        #endregion

        #region Properties

        public ServerLauncherViewModel ServerLauncherVm { get; }

        public SettingsViewModel SettingsVm { get; }

        public bool ShowTitleEnabled
        {
            get => _showTitleEnabled;
            set => SetProperty(ref _showTitleEnabled, value);
        }

        public string ServiceName
        {
            get => _serviceName;
            set => SetProperty(ref _serviceName, value);
        }

        public bool IsInEditMode
        {
            get => _isInEditMode;
            set => SetProperty(ref _isInEditMode, value);
        }

        public double Opacity
        {
            get => _opacity;
            set => SetProperty(ref _opacity, value);
        }

        #endregion

        #region Constructors

        public MainViewModel(
            ServerLauncherViewModel serverLauncherVm,
            SettingsViewModel settingsVm,
            ISettingsManager settingsManager)
        {
            ServerLauncherVm = serverLauncherVm;
            SettingsVm = settingsVm;

            _settingsManager = settingsManager;
            _settingsManager.ServiceNameChanged += HandleSettingsManagerOnServiceNameChanged;
            _settingsManager.OpacityChanged += HandleSettingsManagerOnOpacityChanged;
            _settingsManager.ShowTitleChanged += HandleSettingsManagerOnShowTitleChanged;

            ServiceName = _settingsManager.GetSettings().ServiceName;
            Opacity = _settingsManager.GetSettings().Opacity;
            ShowTitleEnabled = _settingsManager.GetSettings().ShowTitleEnabled;
        }

        #endregion

        #region Methods

        private void HandleSettingsManagerOnOpacityChanged(object sender, EventArgs eventArgs)
        {
            Opacity = _settingsManager.GetSettings().Opacity;
        }

        private void HandleSettingsManagerOnServiceNameChanged(object sender, EventArgs eventArgs)
        {
            ServiceName = _settingsManager.GetSettings().ServiceName;
        }

        private void HandleSettingsManagerOnShowTitleChanged(object sender, EventArgs eventArgs)
        {
            ShowTitleEnabled = _settingsManager.GetSettings().ShowTitleEnabled;
        }

        #endregion
    }
}
