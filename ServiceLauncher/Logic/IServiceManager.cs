namespace ServiceLauncher.Logic
{
    using System;

    public interface IServiceManager
    {
        event EventHandler<EventArgs> StatusChanged;
        event EventHandler<EventArgs> Connected;
        event EventHandler<EventArgs> ConnectionFailed;
        event EventHandler<EventArgs> ServiceActionFailed;

        bool IsServiceExists();
        bool IsServiceRunning();
        bool IsServiceStopped();

        void ConnectToService(string serviceName);
        void RestartAsync();
        void Start();
        void Stop();
    }
}
