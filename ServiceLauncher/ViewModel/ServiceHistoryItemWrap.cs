namespace ServiceLauncher.ViewModel
{
    using Prism.Mvvm;

    class ServiceHistoryItemWrap : BindableBase
    {
        private bool _isCurrentService;

        public string ServiceName { get; set; }

        public bool IsCurrentService
        {
            get => _isCurrentService;
            set => SetProperty(ref _isCurrentService, value);
        }
    }
}
