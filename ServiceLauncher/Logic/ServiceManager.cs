// ---------------------------------------------------------------------------------------------------------------------------------------------------
// Copyright ElcomPlus LLC. All rights reserved.
// Author: Artem Pinkevich
// ---------------------------------------------------------------------------------------------------------------------------------------------------

namespace ServiceLauncher.Logic
{
    using System;
    using System.Linq;
    using System.ServiceProcess;
    using System.Threading;
    using System.Threading.Tasks;

    public class ServiceManager : IServiceManager
    {
        #region Constants

        private const int UPDATE_SERVICE_STATUS_PERIOD = 1000;

        #endregion

        #region Fields

        private string _serviceName;
        private ServiceController _serviceController;
        private readonly Timer _updateServiceStatusTimer;

        #endregion

        #region Events

        public event EventHandler<EventArgs> StatusChanged;
        public event EventHandler<EventArgs> Connected;
        public event EventHandler<EventArgs> ConnectionFailed;
        public event EventHandler<EventArgs> ServiceActionFailed;

        #endregion

        #region Constructors

        public ServiceManager()
        {
            _updateServiceStatusTimer = new Timer(UpdateServiceStatus, null, 0, UPDATE_SERVICE_STATUS_PERIOD);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a value indicating whether the service is exists.
        /// </summary>
        public bool IsServiceExists()
        {
            return _serviceController != null;
        }

        /// <summary>
        /// Gets a value indicating whether the service status is "Running".
        /// </summary>
        public bool IsServiceRunning()
        {
            return _serviceController?.Status == ServiceControllerStatus.Running;
        }

        /// <summary>
        /// Gets a value indicating whether the service status is "Stopped".
        /// </summary>
        public bool IsServiceStopped()
        {
            return _serviceController?.Status == ServiceControllerStatus.Stopped;
        }

        public void ConnectToService(string serviceName)
        {
            _serviceName = serviceName;
            _serviceController = string.IsNullOrEmpty(_serviceName)
                                     ? null
                                     : ServiceController.GetServices().FirstOrDefault(o => o.ServiceName.Equals(_serviceName));

            if (_serviceController == null)
            {
                ConnectionFailed?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                Connected?.Invoke(this, EventArgs.Empty);
            }
        }

        public async void RestartAsync()
        {
            try
            {
                Stop();
                OnStatusChanged();

                await Task.Run(() => _serviceController.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(10)));
                _serviceController.Start();
            }
            catch (Exception)
            {
                ServiceActionFailed?.Invoke(this, EventArgs.Empty);
            }

        }

        public void Start()
        {
            try
            {
                _serviceController.Start();
            }
            catch (Exception)
            {
                ServiceActionFailed?.Invoke(this, EventArgs.Empty);
            }
        }

        public void Stop()
        {
            try
            {
                _serviceController.Stop();
            }
            catch (Exception)
            {
                ServiceActionFailed?.Invoke(this, EventArgs.Empty);
            }
        }

        private void OnStatusChanged()
        {
            StatusChanged?.Invoke(this, EventArgs.Empty);
        }

        private void UpdateServiceStatus(object state)
        {
            if (_serviceController != null)
            {
                try
                {
                    ServiceControllerStatus oldState = _serviceController.Status;
                    _serviceController.Refresh();
                    if (_serviceController.Status != oldState)
                    {
                        OnStatusChanged();
                    }
                }
                catch
                {
                    // Perhaps service was deleted without Configurator
                    _serviceController.Dispose();
                    _serviceController = null;
                    OnStatusChanged();
                }
            }
        }

        #endregion
    }
}
