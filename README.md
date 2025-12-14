# CommonToggleButtonUI

Simple Windows 11 compatible UI for keyboard buttons that typically toggle a state.

## Desktop app

The `ToggleIndicatorUI` project contains a WPF (WinUI 3 styling) sample that displays runtime indicator badges and exposes a settings dialog. You can adjust:

- Enable/disable each indicator (Caps/Num/Scroll Lock)
- Color themes/presets
- Indicator size and opacity
- Screen corner anchor and X/Y offsets

Settings are saved to `%LOCALAPPDATA%/CommonToggleButtonUI/settings.json` and loaded on startup so the runtime indicator display stays in sync with user preferences.
