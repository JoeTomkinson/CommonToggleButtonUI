using System;
using System.Globalization;
using System.Windows;
using ToggleNotifier.Configuration;
using ToggleNotifier.Notifications;
using ToggleNotifier.Services;

namespace ToggleNotifier;

public partial class MainWindow : Window
{
    private readonly ConfigService _configService;
    private readonly StartupManager _startupManager;
    private readonly OverlayService _overlayService;
    private AppSettings _settings;

    public MainWindow(AppSettings settings, ConfigService configService, StartupManager startupManager, OverlayService overlayService)
    {
        InitializeComponent();
        _settings = settings;
        _configService = configService;
        _startupManager = startupManager;
        _overlayService = overlayService;
        PopulateFields();
    }

    private void PopulateFields()
    {
        HorizontalOffsetBox.Text = _settings.ToastOffsets.Horizontal.ToString(CultureInfo.InvariantCulture);
        VerticalOffsetBox.Text = _settings.ToastOffsets.Vertical.ToString(CultureInfo.InvariantCulture);
        DismissBox.Text = _settings.ToastDismissMilliseconds.ToString(CultureInfo.InvariantCulture);
        LaunchOnSignInBox.IsChecked = _settings.LaunchOnSignIn;
    }

    private void OnSave(object sender, RoutedEventArgs e)
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

        _settings.ToastOffsets.Horizontal = Math.Max(0, horizontal);
        _settings.ToastOffsets.Vertical = Math.Max(0, vertical);
        _settings.ToastDismissMilliseconds = Math.Max(500, dismissMs);
        _settings.LaunchOnSignIn = LaunchOnSignInBox.IsChecked == true;

        _configService.Save(_settings);
        _overlayService.UpdateSettings(_settings);

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

    private void OnClose(object sender, RoutedEventArgs e)
    {
        Hide();
    }
}
