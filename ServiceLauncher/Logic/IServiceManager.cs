namespace ServiceLauncher.Logic
{
    using System;

    public interface IServiceManager
    {
        event EventHandler<EventArgs> StatusChanged;

        bool IsServiceExists();
        bool IsServiceRunning();
        bool IsServiceStopped();

        void RestartAsync();
        void StartAsync();
        void StopAsync();
    }
}
