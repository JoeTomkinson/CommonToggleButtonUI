using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ToggleIndicatorUI.Models
{
    public enum ScreenCorner
    {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight
    }

    public class IndicatorSettings : INotifyPropertyChanged
    {
        private bool _capsLockEnabled = true;
        private bool _numLockEnabled = true;
        private bool _scrollLockEnabled = true;
        private string _selectedTheme = "Fluent";
        private double _indicatorSize = 28;
        private double _indicatorOpacity = 0.9;
        private ScreenCorner _corner = ScreenCorner.TopRight;
        private double _offsetX = 16;
        private double _offsetY = 16;

        public event PropertyChangedEventHandler? PropertyChanged;

        public bool CapsLockEnabled
        {
            get => _capsLockEnabled;
            set => SetProperty(ref _capsLockEnabled, value);
        }

        public bool NumLockEnabled
        {
            get => _numLockEnabled;
            set => SetProperty(ref _numLockEnabled, value);
        }

        public bool ScrollLockEnabled
        {
            get => _scrollLockEnabled;
            set => SetProperty(ref _scrollLockEnabled, value);
        }

        public string SelectedTheme
        {
            get => _selectedTheme;
            set => SetProperty(ref _selectedTheme, value);
        }

        public double IndicatorSize
        {
            get => _indicatorSize;
            set => SetProperty(ref _indicatorSize, value);
        }

        public double IndicatorOpacity
        {
            get => _indicatorOpacity;
            set => SetProperty(ref _indicatorOpacity, value);
        }

        public ScreenCorner Corner
        {
            get => _corner;
            set => SetProperty(ref _corner, value);
        }

        public double OffsetX
        {
            get => _offsetX;
            set => SetProperty(ref _offsetX, value);
        }

        public double OffsetY
        {
            get => _offsetY;
            set => SetProperty(ref _offsetY, value);
        }

        protected void SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (Equals(field, value))
            {
                return;
            }

            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
