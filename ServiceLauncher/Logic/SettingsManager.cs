namespace ServiceLauncher.Logic
{
    using System;
    using System.IO;
    using System.Linq;

    using Data;

    class SettingsManager : ISettingsManager
    {
        #region Constants

        private const int HISTORY_SIZE = 5;

        #endregion

        #region Fields

        private Settings _settings;

        #endregion

        #region Events

        public event EventHandler OpacityChanged;
        public event EventHandler<ServicesHistoryEventArgs> ServiceIsAddedToHistory;
        public event EventHandler<ServicesHistoryEventArgs> ServiceIsRemovedFromHistory;

        public event EventHandler ServiceNameChanged;
        public event EventHandler ShowTitleChanged;

        #endregion

        #region Methods

        public Settings GetSettings()
        {
            if (_settings == null)
            {
                string path = Path.Combine(Constants.SETTINGS_FOLDER, Constants.SETTINGS_FILE_NAME);
                _settings = JsonHelper.SafeReadFromFile<Settings>(path);
            }

            return _settings;
        }

        public void RemoveFromHistory(string serviceName)
        {
            _settings.ServicesHistory.Remove(serviceName);
            ServiceIsRemovedFromHistory?.Invoke(this, new ServicesHistoryEventArgs(ServicesHistoryActions.Remove, serviceName));
        }

        public void SetOpacity(double value)
        {
            _settings.Opacity = value;
            OpacityChanged?.Invoke(this, EventArgs.Empty);
            SaveSettings(_settings);
        }

        public void SetServiceName(string newServiceName)
        {
            _settings.ServiceName = newServiceName;
            AddToHistory(newServiceName);
            ServiceNameChanged?.Invoke(this, EventArgs.Empty);
        }

        public void SetShowTitleEnabled(bool value)
        {
            _settings.ShowTitleEnabled = value;
            ShowTitleChanged?.Invoke(this, EventArgs.Empty);
            SaveSettings(_settings);
        }

        private void AddToHistory(string serviceName)
        {
            if (!string.IsNullOrEmpty(serviceName) && !_settings.ServicesHistory.Contains(serviceName))
            {
                if (_settings.ServicesHistory.Count == HISTORY_SIZE)
                {
                    RemoveLastHistoryItem();
                }

                _settings.ServicesHistory.Insert(0, serviceName);

                ServiceIsAddedToHistory?.Invoke(this, new ServicesHistoryEventArgs(ServicesHistoryActions.Add, serviceName));
                SaveSettings(_settings);
            }
        }

        private void RemoveLastHistoryItem()
        {
            string serviceName = _settings.ServicesHistory.LastOrDefault();
            if (!string.IsNullOrEmpty(serviceName))
            {
                _settings.ServicesHistory.Remove(serviceName);
                ServiceIsRemovedFromHistory?.Invoke(this, new ServicesHistoryEventArgs(ServicesHistoryActions.Remove, serviceName));
            }
        }

        private void SaveSettings(Settings settings)
        {
            _settings = settings;
            JsonHelper.SafeSaveToFile(Constants.SETTINGS_FOLDER, Constants.SETTINGS_FILE_NAME, _settings);
        }

        #endregion
    }
}
