// ---------------------------------------------------------------------------------------------------------------------------------------------------
// Copyright ElcomPlus LLC. All rights reserved.
// Author: Artem Pinkevich
// ---------------------------------------------------------------------------------------------------------------------------------------------------

namespace ServiceLauncher.ViewModel
{
    using System;

    using Logic;

    using Prism.Commands;
    using Prism.Mvvm;

    public class ServerLauncherViewModel : BindableBase
    {
        #region Fields
        
        private readonly IServiceManager _serviceManager;
        private readonly ISettingsManager _settingsManager;
        private bool _isStateChangeInProgress;
        private string _errorMessage;

        #endregion

        #region Properties

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public DelegateCommand StartCommand { get; }

        public DelegateCommand StopCommand { get; }

        public DelegateCommand RestartCommand { get; }

        #endregion

        #region Constructors

        public ServerLauncherViewModel(IServiceManager serviceManager, ISettingsManager settingsManager)
        {
            _serviceManager = serviceManager;
            _settingsManager = settingsManager;

            StartCommand = new DelegateCommand(ExecuteStartCommand, CanExecuteStart);
            StopCommand = new DelegateCommand(ExecuteStopCommand, CanExecuteStop);
            RestartCommand = new DelegateCommand(ExecuteRestartCommand, CanRestartCommand);

            _serviceManager.StatusChanged += HandleServiceManagerStatusChanged;
            _serviceManager.Connected += HandleServiceManagerOnConnected;
            _serviceManager.ConnectionFailed += HandleServiceManagerOnConnectionFailed;
            _serviceManager.ServiceActionFailed += HandleServiceManagerOnServiceActionFailed;

            _serviceManager.ConnectToService(_settingsManager.GetSettings().ServiceName);
        }

        #endregion

        #region Methods

        private bool CanExecuteStart()
        {
            if (!CommonCanExecuteStartStop())
            {
                return false;
            }

            return _serviceManager.IsServiceStopped();
        }

        private bool CanExecuteStop()
        {
            if (!CommonCanExecuteStartStop())
            {
                return false;
            }

            return _serviceManager.IsServiceRunning();
        }

        private bool CanRestartCommand()
        {
            if (!CommonCanExecuteStartStop())
            {
                return false;
            }

            return _serviceManager.IsServiceRunning();
        }

        private bool CommonCanExecuteStartStop()
        {
            if (_isStateChangeInProgress)
            {
                return false;
            }

            if (!_serviceManager.IsServiceExists())
            {
                return false;
            }

            return true;
        }

        private void ExecuteStartCommand()
        {
            _isStateChangeInProgress = true;
            
            _serviceManager.Start();
        }

        private void ExecuteStopCommand()
        {
            _isStateChangeInProgress = true;

            _serviceManager.Stop();
        }

        private void ExecuteRestartCommand()
        {
            _serviceManager.RestartAsync();
        }

        private void RefreshView()
        {
            _isStateChangeInProgress = false;
            StartCommand.RaiseCanExecuteChanged();
            StopCommand.RaiseCanExecuteChanged();
            RestartCommand.RaiseCanExecuteChanged();
        }

        private void HandleServiceManagerStatusChanged(object sender, EventArgs eventArgs)
        {
            RefreshView();
        }

        private void HandleServiceManagerOnServiceActionFailed(object sender, EventArgs eventArgs)
        {
            RefreshView();
        }

        private void HandleServiceManagerOnConnected(object sender, EventArgs eventArgs)
        {
            ErrorMessage = string.Empty;
            RefreshView();
        }

        private void HandleServiceManagerOnConnectionFailed(object sender, EventArgs eventArgs)
        {
            ErrorMessage = $"Service \"{_settingsManager.GetSettings().ServiceName}\" doesn't exist";
            RefreshView();
        }

        #endregion
    }
}
