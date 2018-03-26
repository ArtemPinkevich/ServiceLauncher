namespace ServiceLauncher.Logic
{
    using System;

    public class ServicesHistoryEventArgs : EventArgs
    {
        public ServicesHistoryActions Action { get; }

        public string ServiceName { get; }
        
        public ServicesHistoryEventArgs(ServicesHistoryActions action, string serviceName)
        {
            Action = action;
            ServiceName = serviceName;
        }
    }
}
