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

        private const string SERVICE_NAME = "";
        private const int UPDATE_SERVICE_STATUS_PERIOD = 1000;

        #endregion

        #region Fields

        private ServiceController _serviceController;
        private readonly Timer _updateServiceStatusTimer;

        #endregion

        #region Events

        /// <summary>
        /// Occurs when Status of the windows service is changed.
        /// </summary>
        public event EventHandler<EventArgs> StatusChanged;

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

        public async void RestartAsync()
        {
            if (_serviceController == null)
            {
                return;
            }

            await Task.Run(() => Stop());
            OnStatusChanged();

            await Task.Run(
                () =>
                {
                    try
                    {
                        _serviceController.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(10));
                        _serviceController.Start();
                    }
                    catch (Exception)
                    {
                    }
                });
        }

        /// <summary>
        /// Starts the service.
        /// </summary>
        public async void StartAsync()
        {
            if (_serviceController == null)
            {
                return;
            }

            await Task.Run(() => Start());
        }

        /// <summary>
        /// Stops this service and any services that are dependent on this service.
        /// </summary>
        public async void StopAsync()
        {
            if (_serviceController == null)
            {
                return;
            }

            await Task.Run(() => Stop());
        }

        private void OnStatusChanged()
        {
            StatusChanged?.Invoke(this, EventArgs.Empty);
        }

        private void Start()
        {
            try
            {
                _serviceController.Start();
            }
            catch (Exception)
            {
                // TODO Need write to log
            }
        }

        private void Stop()
        {
            try
            {
                _serviceController.Stop();
            }
            catch (Exception)
            {
                // TODO Need write to log
            }
        }

        private void UpdateServiceStatus(object state)
        {
            if (_serviceController == null)
            {
                _serviceController = ServiceController.GetServices().FirstOrDefault(o => o.ServiceName.Contains(SERVICE_NAME));

                if (_serviceController != null)
                {
                    OnStatusChanged();
                }
            }
            else
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
