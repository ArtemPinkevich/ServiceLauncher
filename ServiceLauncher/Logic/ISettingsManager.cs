namespace ServiceLauncher.Logic
{
    using System;

    using Data;

    public interface ISettingsManager
    {
        Settings GetSettings();
        void SetServiceName(string newServiceName);
        void SetOpacity(double value);
        void SetShowTitleEnabled(bool showTitleEnabled);
        void RemoveFromHistory(string serviceName);

        event EventHandler ServiceNameChanged;
        event EventHandler<ServicesHistoryEventArgs> ServiceIsAddedToHistory;
        event EventHandler<ServicesHistoryEventArgs> ServiceIsRemovedFromHistory;
        event EventHandler OpacityChanged;
        event EventHandler ShowTitleChanged;
    }
}
