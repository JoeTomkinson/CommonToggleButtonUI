using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ToggleNotifier.Assets;
using ToggleNotifier.Configuration;
using ToggleNotifier.Notifications;
using ToggleNotifier.Services;
using ToggleNotifier.Theming;
using Color = System.Windows.Media.Color;

namespace ToggleNotifier;

public partial class MainWindow : Window
{
    private readonly ConfigService _configService;
    private readonly StartupManager _startupManager;
    private readonly OverlayService _overlayService;
    private readonly ThemeService _themeService;
    private AppSettings _settings;

    public MainWindow(
        AppSettings settings,
        ConfigService configService,
        StartupManager startupManager,
        OverlayService overlayService,
        ThemeService themeService)
    {
        InitializeComponent();
        _settings = settings;
        _configService = configService;
        _startupManager = startupManager;
        _overlayService = overlayService;
        _themeService = themeService;

        _themeService.ThemeChanged += OnThemeChanged;
        _themeService.ApplyThemeToWindow(this);
        ApplyTheme();
        SetWindowIcon();
        PopulateFields();
    }

    private void SetWindowIcon()
    {
        var icon = IconGenerator.CreateTrayIcon(_themeService.IsDarkMode);
        Icon = Imaging.CreateBitmapSourceFromHIcon(
            icon.Handle,
            Int32Rect.Empty,
            BitmapSizeOptions.FromEmptyOptions());
    }

    private void OnThemeChanged(object? sender, bool isDarkMode)
    {
        Dispatcher.Invoke(() =>
        {
            ApplyTheme();
            SetWindowIcon();
        });
    }

    private void ApplyTheme()
    {
        var colors = _themeService.GetColors();

        Resources["WindowBackgroundBrush"] = colors.BackgroundBrush;
        Resources["CardBackgroundBrush"] = colors.SurfaceBrush;
        Resources["CardBorderBrush"] = colors.BorderBrush;
        Resources["TextPrimaryBrush"] = colors.TextBrush;
        Resources["TextSecondaryBrush"] = colors.TextSecondaryBrush;
        Resources["AccentBrush"] = colors.AccentBrush;

        // Toggle switch off state colors
        var toggleOffBackground = _themeService.IsDarkMode
            ? new SolidColorBrush(Color.FromRgb(70, 70, 70))
            : new SolidColorBrush(Color.FromRgb(229, 229, 229));
        var toggleOffBorder = _themeService.IsDarkMode
            ? new SolidColorBrush(Color.FromRgb(90, 90, 90))
            : new SolidColorBrush(Color.FromRgb(212, 212, 212));
        Resources["ToggleOffBackgroundBrush"] = toggleOffBackground;
        Resources["ToggleOffBorderBrush"] = toggleOffBorder;

        // Control colors (ComboBox, TextBox, Button)
        var controlBackground = _themeService.IsDarkMode
            ? new SolidColorBrush(Color.FromRgb(50, 50, 50))
            : new SolidColorBrush(Color.FromRgb(255, 255, 255));
        var controlBorder = _themeService.IsDarkMode
            ? new SolidColorBrush(Color.FromRgb(80, 80, 80))
            : new SolidColorBrush(Color.FromRgb(212, 212, 212));
        var controlHover = _themeService.IsDarkMode
            ? new SolidColorBrush(Color.FromRgb(65, 65, 65))
            : new SolidColorBrush(Color.FromRgb(240, 240, 240));
        Resources["ControlBackgroundBrush"] = controlBackground;
        Resources["ControlBorderBrush"] = controlBorder;
        Resources["ControlHoverBrush"] = controlHover;
    }

    private void OnThemeSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!IsLoaded) return;

        var theme = ThemeComboBox.SelectedIndex switch
        {
            1 => AppTheme.Light,
            2 => AppTheme.Dark,
            _ => AppTheme.System
        };

        _themeService.CurrentTheme = theme;
        _themeService.ApplyThemeToWindow(this);
        ApplyTheme();
        SetWindowIcon();
    }

    private void PopulateFields()
    {
        // Offsets
        HorizontalOffsetBox.Text = _settings.ToastOffsets.Horizontal.ToString(CultureInfo.InvariantCulture);
        VerticalOffsetBox.Text = _settings.ToastOffsets.Vertical.ToString(CultureInfo.InvariantCulture);

        // Timing
        DismissBox.Text = _settings.ToastDismissMilliseconds.ToString(CultureInfo.InvariantCulture);

        // Theme
        var themeIndex = _settings.Theme switch
        {
            AppTheme.System => 0,
            AppTheme.Light => 1,
            AppTheme.Dark => 2,
            _ => 0
        };
        ThemeComboBox.SelectedIndex = themeIndex;

        // Position
        var positionIndex = (int)_settings.ToastPosition;
        PositionComboBox.SelectedIndex = positionIndex;

        // Key settings
        CapsLockBox.IsChecked = _settings.KeySettings.CapsLock;
        NumLockBox.IsChecked = _settings.KeySettings.NumLock;
        ScrollLockBox.IsChecked = _settings.KeySettings.ScrollLock;

        // Sound
        PlaySoundBox.IsChecked = _settings.PlaySound;

        // Startup
        LaunchOnSignInBox.IsChecked = _settings.LaunchOnSignIn;
    }

    private void OnPreview(object sender, RoutedEventArgs e)
    {
        // Apply current form values temporarily for preview
        var tempSettings = CreateSettingsFromForm();
        var toast = new ToastWindow("Caps Lock", true, tempSettings, _themeService)
        {
            Topmost = true
        };
        toast.Show();
    }

    private void OnSave(object sender, RoutedEventArgs e)
    {
        _settings = CreateSettingsFromForm();

        _configService.Save(_settings);
        _overlayService.UpdateSettings(_settings);
        _themeService.CurrentTheme = _settings.Theme;

        if (_settings.LaunchOnSignIn)
        {
            _startupManager.EnableStartup();
        }
        else
        {
            _startupManager.DisableStartup();
        }

        Hide();
    }

    private AppSettings CreateSettingsFromForm()
    {
        if (!double.TryParse(HorizontalOffsetBox.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out var horizontal))
        {
            horizontal = _settings.ToastOffsets.Horizontal;
        }

        if (!double.TryParse(VerticalOffsetBox.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out var vertical))
        {
            vertical = _settings.ToastOffsets.Vertical;
        }

        if (!int.TryParse(DismissBox.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var dismissMs))
        {
            dismissMs = _settings.ToastDismissMilliseconds;
        }

        var theme = ThemeComboBox.SelectedIndex switch
        {
            1 => AppTheme.Light,
            2 => AppTheme.Dark,
            _ => AppTheme.System
        };

        var position = (ToastPosition)PositionComboBox.SelectedIndex;

        return new AppSettings
        {
            ToastOffsets = new ToastOffsets
            {
                Horizontal = Math.Max(0, horizontal),
                Vertical = Math.Max(0, vertical)
            },
            ToastDismissMilliseconds = Math.Max(500, dismissMs),
            LaunchOnSignIn = LaunchOnSignInBox.IsChecked == true,
            Theme = theme,
            ToastPosition = position,
            PlaySound = PlaySoundBox.IsChecked == true,
            KeySettings = new KeyNotificationSettings
            {
                CapsLock = CapsLockBox.IsChecked == true,
                NumLock = NumLockBox.IsChecked == true,
                ScrollLock = ScrollLockBox.IsChecked == true
            }
        };
    }

    private void OnClose(object sender, RoutedEventArgs e)
    {
        // Reset fields to saved values
        PopulateFields();
        Hide();
    }
}
