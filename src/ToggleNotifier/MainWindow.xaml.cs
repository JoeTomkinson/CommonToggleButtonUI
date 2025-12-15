using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ToggleNotifier.Assets;
using ToggleNotifier.Configuration;
using ToggleNotifier.Notifications;
using ToggleNotifier.Services;
using ToggleNotifier.Theming;
using Color = System.Windows.Media.Color;
using WinFormsColorDialog = System.Windows.Forms.ColorDialog;
using WinFormsDialogResult = System.Windows.Forms.DialogResult;

namespace ToggleNotifier;

public partial class MainWindow : Window
{
    private readonly ConfigService _configService;
    private readonly StartupManager _startupManager;
    private readonly OverlayService _overlayService;
    private readonly ThemeService _themeService;
    private AppSettings _settings;
    private Color _currentBorderColor;
    private bool _useAccentColor = true;

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

        // Header and footer background (slightly different from main background for visual separation)
        var headerFooterBackground = _themeService.IsDarkMode
            ? new SolidColorBrush(Color.FromRgb(45, 45, 48))
            : new SolidColorBrush(Color.FromRgb(255, 255, 255));
        Resources["HeaderFooterBackgroundBrush"] = headerFooterBackground;

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

        // Toast style
        var toastStyleIndex = _settings.ToastStyle switch
        {
            ToastStyle.Standard => 0,
            ToastStyle.Compact => 1,
            _ => 0
        };
        ToastStyleComboBox.SelectedIndex = toastStyleIndex;

        // Position
        var positionIndex = (int)_settings.ToastPosition;
        PositionComboBox.SelectedIndex = positionIndex;

        // Border settings
        EnableCustomBorderBox.IsChecked = _settings.BorderSettings.EnableCustomBorder;
        BorderColorCard.Visibility = _settings.BorderSettings.EnableCustomBorder ? Visibility.Visible : Visibility.Collapsed;
        BorderThicknessBox.Text = _settings.BorderSettings.BorderThickness.ToString(CultureInfo.InvariantCulture);
        _useAccentColor = _settings.BorderSettings.UseAccentColor;
        
        // Set the border color preview
        if (_settings.BorderSettings.UseAccentColor || string.IsNullOrEmpty(_settings.BorderSettings.CustomBorderColor))
        {
            _currentBorderColor = ThemeService.GetWindowsAccentColor();
        }
        else
        {
            _currentBorderColor = ThemeService.ParseHexColor(_settings.BorderSettings.CustomBorderColor) 
                                  ?? ThemeService.GetWindowsAccentColor();
        }
        UpdateBorderColorPreview();

        // Key settings
        CapsLockBox.IsChecked = _settings.KeySettings.CapsLock;
        NumLockBox.IsChecked = _settings.KeySettings.NumLock;
        ScrollLockBox.IsChecked = _settings.KeySettings.ScrollLock;

        // Sound
        PlaySoundBox.IsChecked = _settings.PlaySound;

        // Behavior
        SuppressInFullscreenBox.IsChecked = _settings.SuppressInFullscreen;

        // Startup
        LaunchOnSignInBox.IsChecked = _settings.LaunchOnSignIn;
    }

    private void UpdateBorderColorPreview()
    {
        BorderColorPreview.Background = new SolidColorBrush(_currentBorderColor);
    }

    private void OnCustomBorderToggled(object sender, RoutedEventArgs e)
    {
        if (!IsLoaded) return;
        
        BorderColorCard.Visibility = EnableCustomBorderBox.IsChecked == true 
            ? Visibility.Visible 
            : Visibility.Collapsed;
    }

    private void OnBorderColorClick(object sender, MouseButtonEventArgs e)
    {
        using var dialog = new WinFormsColorDialog
        {
            Color = System.Drawing.Color.FromArgb(_currentBorderColor.R, _currentBorderColor.G, _currentBorderColor.B),
            FullOpen = true,
            AnyColor = true
        };

        if (dialog.ShowDialog() == WinFormsDialogResult.OK)
        {
            _currentBorderColor = Color.FromRgb(dialog.Color.R, dialog.Color.G, dialog.Color.B);
            _useAccentColor = false;
            UpdateBorderColorPreview();
        }
    }

    private void OnResetBorderColor(object sender, RoutedEventArgs e)
    {
        _currentBorderColor = ThemeService.GetWindowsAccentColor();
        _useAccentColor = true;
        UpdateBorderColorPreview();
    }

    private void OnPreview(object sender, RoutedEventArgs e)
    {
        // Apply current form values temporarily for preview
        var tempSettings = CreateSettingsFromForm();
        
        Window toast = tempSettings.ToastStyle switch
        {
            ToastStyle.Compact => new CompactToastWindow("Caps Lock", true, tempSettings, _themeService)
            {
                Topmost = true
            },
            _ => new ToastWindow("Caps Lock", true, tempSettings, _themeService)
            {
                Topmost = true
            }
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

        if (!double.TryParse(BorderThicknessBox.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out var borderThickness))
        {
            borderThickness = _settings.BorderSettings.BorderThickness;
        }

        var theme = ThemeComboBox.SelectedIndex switch
        {
            1 => AppTheme.Light,
            2 => AppTheme.Dark,
            _ => AppTheme.System
        };

        var toastStyle = ToastStyleComboBox.SelectedIndex switch
        {
            1 => ToastStyle.Compact,
            _ => ToastStyle.Standard
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
            ToastStyle = toastStyle,
            ToastPosition = position,
            PlaySound = PlaySoundBox.IsChecked == true,
            SuppressInFullscreen = SuppressInFullscreenBox.IsChecked == true,
            KeySettings = new KeyNotificationSettings
            {
                CapsLock = CapsLockBox.IsChecked == true,
                NumLock = NumLockBox.IsChecked == true,
                ScrollLock = ScrollLockBox.IsChecked == true
            },
            BorderSettings = new ToastBorderSettings
            {
                EnableCustomBorder = EnableCustomBorderBox.IsChecked == true,
                UseAccentColor = _useAccentColor,
                CustomBorderColor = _useAccentColor ? null : ThemeService.ColorToHex(_currentBorderColor),
                BorderThickness = Math.Max(0.5, Math.Min(10, borderThickness))
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
