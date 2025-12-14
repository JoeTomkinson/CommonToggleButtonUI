# Package Assets

This folder contains the visual assets for the MSIX package.

## Required Images

You need to create the following PNG images with the specified sizes:

| File Name | Size (pixels) | Purpose |
|-----------|---------------|---------|
| `StoreLogo.scale-200.png` | 100x100 | Store listing logo |
| `Square44x44Logo.scale-200.png` | 88x88 | Taskbar, Start menu small |
| `Square44x44Logo.targetsize-24_altform-unplated.png` | 24x24 | Unplated taskbar icon |
| `Square150x150Logo.scale-200.png` | 300x300 | Start menu medium tile |
| `Wide310x150Logo.scale-200.png` | 620x300 | Start menu wide tile |
| `SplashScreen.scale-200.png` | 1240x600 | App splash screen |
| `LockScreenLogo.scale-200.png` | 48x48 | Lock screen badge |

## Quick Setup

### Option 1: Use Visual Studio's Asset Generator
1. Open the solution in Visual Studio
2. Double-click `Package.appxmanifest`
3. Go to the **Visual Assets** tab
4. Click **Generate** to create all required assets from a single source image

### Option 2: Manual Creation
1. Create a 1024x1024 PNG source image with your app icon
2. Use an online tool like:
   - https://www.pwabuilder.com/imageGenerator
   - https://appicon.co/
3. Generate all required sizes and place them in this folder

## Recommended Design Guidelines
- Use a simple, recognizable icon that works at small sizes
- Include some padding (about 16% of the image size)
- Use a transparent or solid color background
- The accent color for Toggle Notifier is `#0078D4` (blue)
