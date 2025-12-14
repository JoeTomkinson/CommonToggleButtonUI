# CommonToggleButtonUI

Simple Windows 11 compatible UI for keyboard buttons that typically toggle a state.

## Desktop app

The `ToggleNotifier` project contains the WPF desktop app. It runs in the background, listens for lock key changes, and shows a small toast overlay near the active display's bottom-right corner. A tray icon lets users open the settings window or exit the app. You can adjust:

- Launch-on-sign-in behavior
- Overlay horizontal/vertical offsets
- Auto-dismiss timing

Settings are saved to `%APPDATA%/CommonToggleButtonUI/appsettings.json` and loaded on startup so the runtime indicator display stays in sync with user preferences.
## What it does
- Listens for Caps Lock, Num Lock, and Scroll Lock changes in the background.
- Pops a toast-like overlay near the bottom-right corner of the active display with configurable offsets and an auto-dismiss timer.
- Runs from the system tray with a quick exit option and a settings window for tweaking offsets, dismiss timing, and startup behavior.
- Uses system brushes for high-contrast mode and keeps overlays from stealing focus.

## Project layout
- `CommonToggleButtonUI.sln` – Visual Studio solution.
- `src/ToggleNotifier/ToggleNotifier.csproj` – WPF desktop application entry point (`App.xaml`).
- `src/ToggleNotifier/appsettings.json` – default overlay offsets, dismiss timing, and launch-on-sign-in preference.
- `src/ToggleNotifier/Notifications/*` – overlay window and service for showing toasts.
- `src/ToggleNotifier/Services/*` – keyboard hook listener, startup registration, and tray icon wiring.

## Running locally
1. Open the solution in Visual Studio 2022 or newer on Windows with the .NET 10 SDK (Preview) and the Windows desktop workload installed.
2. Restore NuGet packages and build the `ToggleNotifier` project.
3. Run the application. A tray icon appears; toggle Caps Lock/Num Lock/Scroll Lock to see the overlay. Use the tray menu to open settings or exit.

Settings are stored under `%APPDATA%/CommonToggleButtonUI/appsettings.json` after first run.
