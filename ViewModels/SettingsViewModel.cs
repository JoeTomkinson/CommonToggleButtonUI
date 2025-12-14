using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CommonToggleButtonUI.ViewModels;

public class SettingsViewModel : INotifyPropertyChanged
{
    private string _selectedTheme = "Light";
    private string _selectedAccent = "Blue";

    public ObservableCollection<string> ThemePresets { get; } = new(ThemeManager.ThemeNames);
    public ObservableCollection<string> AccentPresets { get; } = new(ThemeManager.AccentNames);

    public string SelectedTheme
    {
        get => _selectedTheme;
        set
        {
            if (_selectedTheme == value)
            {
                return;
            }

            _selectedTheme = value;
            ThemeManager.ApplyTheme(_selectedTheme);
            OnPropertyChanged();
        }
    }

    public string SelectedAccent
    {
        get => _selectedAccent;
        set
        {
            if (_selectedAccent == value)
            {
                return;
            }

            _selectedAccent = value;
            ThemeManager.ApplyAccent(_selectedAccent);
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
