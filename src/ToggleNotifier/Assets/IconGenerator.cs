using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace ToggleNotifier.Assets;

/// <summary>
/// Generates application icons programmatically for the tray and window icon.
/// </summary>
public static class IconGenerator
{
    /// <summary>
    /// Creates the main application tray icon.
    /// </summary>
    public static Icon CreateTrayIcon(bool isDarkMode = false)
    {
        const int size = 32;
        using var bitmap = new Bitmap(size, size);
        using var g = Graphics.FromImage(bitmap);
        
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
        g.Clear(Color.Transparent);

        // Draw rounded rectangle background
        var bgColor = isDarkMode ? Color.FromArgb(255, 96, 165, 250) : Color.FromArgb(255, 0, 120, 212);
        using var bgBrush = new SolidBrush(bgColor);
        using var path = CreateRoundedRectangle(2, 2, size - 4, size - 4, 6);
        g.FillPath(bgBrush, path);

        // Draw keyboard key symbol
        using var keyBrush = new SolidBrush(Color.White);
        using var keyPath = CreateRoundedRectangle(7, 10, 18, 12, 2);
        g.FillPath(keyBrush, keyPath);

        // Draw toggle indicator
        g.FillEllipse(Brushes.LimeGreen, size - 12, 4, 8, 8);

        return Icon.FromHandle(bitmap.GetHicon());
    }

    /// <summary>
    /// Creates a state-specific icon showing which key was toggled.
    /// </summary>
    public static Icon CreateStateIcon(string keyName, bool isOn)
    {
        const int size = 32;
        using var bitmap = new Bitmap(size, size);
        using var g = Graphics.FromImage(bitmap);

        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
        g.Clear(Color.Transparent);

        // Background
        var bgColor = isOn ? Color.FromArgb(255, 16, 185, 129) : Color.FromArgb(255, 107, 114, 128);
        using var bgBrush = new SolidBrush(bgColor);
        using var path = CreateRoundedRectangle(2, 2, size - 4, size - 4, 6);
        g.FillPath(bgBrush, path);

        // Key letter
        var letter = keyName switch
        {
            "Caps Lock" => "A",
            "Num Lock" => "#",
            "Scroll Lock" => "S",
            _ => "?"
        };

        using var font = new Font("Segoe UI", 14, FontStyle.Bold);
        using var textBrush = new SolidBrush(Color.White);
        var textSize = g.MeasureString(letter, font);
        var x = (size - textSize.Width) / 2;
        var y = (size - textSize.Height) / 2;
        g.DrawString(letter, font, textBrush, x, y);

        return Icon.FromHandle(bitmap.GetHicon());
    }

    private static GraphicsPath CreateRoundedRectangle(float x, float y, float width, float height, float radius)
    {
        var path = new GraphicsPath();
        var diameter = radius * 2;

        path.AddArc(x, y, diameter, diameter, 180, 90);
        path.AddArc(x + width - diameter, y, diameter, diameter, 270, 90);
        path.AddArc(x + width - diameter, y + height - diameter, diameter, diameter, 0, 90);
        path.AddArc(x, y + height - diameter, diameter, diameter, 90, 90);
        path.CloseFigure();

        return path;
    }
}
