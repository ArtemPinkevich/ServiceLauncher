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
        private bool _isStateChangeInProgress;

        #endregion

        #region Properties

        public DelegateCommand StartCommand { get; }

        public DelegateCommand StopCommand { get; }

        public DelegateCommand RestartCommand { get; }

        #endregion

        #region Constructors

        public ServerLauncherViewModel(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
            _serviceManager.StatusChanged += HandleServiceManagerStatusChanged;

            StartCommand = new DelegateCommand(ExecuteStartCommand, CanExecuteStart);
            StopCommand = new DelegateCommand(ExecuteStopCommand, CanExecuteStop);
            RestartCommand = new DelegateCommand(ExecuteRestartCommand, CanRestartCommand);
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

            _serviceManager.StartAsync();
        }

        private void ExecuteStopCommand()
        {
            _isStateChangeInProgress = true;

            _serviceManager.StopAsync();
        }

        private void ExecuteRestartCommand()
        {
            _serviceManager.RestartAsync();
        }

        private void HandleServiceManagerStatusChanged(object sender, EventArgs eventArgs)
        {
            _isStateChangeInProgress = false;
            StartCommand.RaiseCanExecuteChanged();
            StopCommand.RaiseCanExecuteChanged();
            RestartCommand.RaiseCanExecuteChanged();
        }

        #endregion
    }
}
