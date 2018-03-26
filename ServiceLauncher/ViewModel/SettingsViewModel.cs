namespace ServiceLauncher.ViewModel
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows.Input;

    using Data;

    using Logic;

    using Prism.Commands;
    using Prism.Mvvm;

    internal class SettingsViewModel : BindableBase
    {
        #region Constants

        private const double FORBIDDEN_OPACITY = 0;

        #endregion

        #region Fields

        private readonly IServiceManager _serviceManager;
        private readonly ISettingsManager _settingsManager;

        private double _opacity;
        private string _serviceName;
        private bool _showTitleEnabled;

        #endregion

        #region Properties

        public string ServiseName
        {
            get => _serviceName;
            set
            {
                if (SetProperty(ref _serviceName, value))
                {
                    SetServiceName(value);
                }
            }
        }

        public double Opacity
        {
            get => _opacity;
            set
            {
                if (Math.Abs(value) > FORBIDDEN_OPACITY)
                    SetProperty(ref _opacity, value);
            }
        }

        public bool ShowTitleEnabled
        {
            get => _showTitleEnabled;
            set => SetProperty(ref _showTitleEnabled, value);
        }

        public ObservableCollection<ServiceHistoryItemWrap> Services { get; } = new ObservableCollection<ServiceHistoryItemWrap>();

        public ICommand ServiceNameChangedCommand { get; }
        public ICommand ChooseServiceCommand { get; }
        public ICommand OpacityChangedCommand { get; }
        public ICommand RemoveServiceCommand { get; }
        public ICommand ShowTitleChangedCommand { get; }

        #endregion

        #region Constructors

        public SettingsViewModel(ISettingsManager settingsManager, IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
            _settingsManager = settingsManager;
            Settings settings = _settingsManager.GetSettings();

            RemoveServiceCommand = new DelegateCommand<object>(ExecuteRemoveServiceCommand);
            ServiceNameChangedCommand = new DelegateCommand<object>(ExecuteServiceNameChangedCommand);
            ChooseServiceCommand = new DelegateCommand<object>(ExecuteChooseServiceCommand);
            OpacityChangedCommand = new DelegateCommand(ExecuteOpacityChangedCommand);
            ShowTitleChangedCommand = new DelegateCommand(ExecuteShowTitleChangedCommand);
            ServiseName = settings.ServiceName;

            foreach (string serviceNameFromHistory in settings.ServicesHistory)
            {
                Services.Add(CreateServiceWrap(serviceNameFromHistory));
            }

            _settingsManager.ServiceIsAddedToHistory += HandleSettingsManagerOnServiceIsAddedToHistory;
            _settingsManager.ServiceIsRemovedFromHistory += HandleSettingsManagerOnServiceIsRemovedFromHistory;
            _settingsManager.OpacityChanged += HandleSettingsManagerOnOpacityChanged;
            _settingsManager.ShowTitleChanged += HandleSettingsManagerOnShowTitleChanged;

            Opacity = _settingsManager.GetSettings().Opacity;
            ShowTitleEnabled = _settingsManager.GetSettings().ShowTitleEnabled;
        }

        #endregion

        #region Methods

        private ServiceHistoryItemWrap CreateServiceWrap(string serviceName)
        {
            return new ServiceHistoryItemWrap
            {
                ServiceName = serviceName,
                IsCurrentService = serviceName == ServiseName
            };
        }

        private void RefreshWrapsServices()
        {
            foreach (ServiceHistoryItemWrap serviceWrap in Services)
            {
                serviceWrap.IsCurrentService = serviceWrap.ServiceName == ServiseName;
            }
        }

        private void SetServiceName(string serviceName)
        {
            _settingsManager.SetServiceName(serviceName);

            _serviceManager.ConnectToService(serviceName);

            RefreshWrapsServices();
        }

        private void ExecuteServiceNameChangedCommand(object newServiceName)
        {
            if (newServiceName is string serviceName && serviceName != _serviceName)
            {
                _serviceName = serviceName;
                SetServiceName(serviceName);
            }
        }

        private void ExecuteChooseServiceCommand(object item)
        {
            var serviceWrap = item as ServiceHistoryItemWrap;
            if (serviceWrap != null)
            {
                ServiseName = serviceWrap.ServiceName;
            }
        }

        private void ExecuteOpacityChangedCommand()
        {
            _settingsManager.SetOpacity(_opacity);
        }

        private void ExecuteRemoveServiceCommand(object item)
        {
            var serviceWrap = item as ServiceHistoryItemWrap;
            if (serviceWrap != null)
            {
                _settingsManager.RemoveFromHistory(serviceWrap.ServiceName);
            }
        }

        private void ExecuteShowTitleChangedCommand()
        {
            _settingsManager.SetShowTitleEnabled(_showTitleEnabled);
        }

        private void HandleSettingsManagerOnOpacityChanged(object sender, EventArgs args)
        {
            Opacity = _settingsManager.GetSettings().Opacity;
        }

        private void HandleSettingsManagerOnServiceIsAddedToHistory(object sender, ServicesHistoryEventArgs args)
        {
            if (args.Action == ServicesHistoryActions.Add && Services.All(o => o.ServiceName != args.ServiceName))
            {
                Services.Insert(0, CreateServiceWrap(ServiseName));
            }
        }

        private void HandleSettingsManagerOnServiceIsRemovedFromHistory(object sender, ServicesHistoryEventArgs args)
        {
            ServiceHistoryItemWrap serviceWrap = Services.FirstOrDefault(o => o.ServiceName == args.ServiceName);
            if (serviceWrap != null)
            {
                Services.Remove(serviceWrap);
            }
        }

        private void HandleSettingsManagerOnShowTitleChanged(object sender, EventArgs eventArgs)
        {
            ShowTitleEnabled = _settingsManager.GetSettings().ShowTitleEnabled;
        }

        #endregion
    }
}
