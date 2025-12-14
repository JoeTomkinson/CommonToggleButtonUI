using System;

namespace ToggleNotifier.Services;

public class KeyStateChangedEventArgs : EventArgs
{
    public KeyStateChangedEventArgs(string keyName, bool isOn)
    {
        KeyName = keyName;
        IsOn = isOn;
    }

    public string KeyName { get; }

    public bool IsOn { get; }
}
