using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using ToggleNotifier.Interop;

namespace ToggleNotifier.Services;

public class KeyToggleListener : IDisposable
{
    private NativeMethods.LowLevelKeyboardProc? _proc;
    private IntPtr _hookId = IntPtr.Zero;
    private bool _capsOn;
    private bool _numOn;
    private bool _scrollOn;

    public event EventHandler<KeyStateChangedEventArgs>? KeyStateChanged;

    public void Start()
    {
        _proc = HookCallback;
        using var curProcess = Process.GetCurrentProcess();
        using var curModule = curProcess.MainModule;
        var moduleHandle = NativeMethods.GetModuleHandle(curModule?.ModuleName);
        _hookId = NativeMethods.SetWindowsHookEx(NativeMethods.WH_KEYBOARD_LL, _proc, moduleHandle, 0);
        UpdateStates();
    }

    public void Dispose()
    {
        if (_hookId != IntPtr.Zero)
        {
            NativeMethods.UnhookWindowsHookEx(_hookId);
            _hookId = IntPtr.Zero;
        }
    }

    private void UpdateStates()
    {
        _capsOn = ReadToggle(NativeMethods.VK_CAPITAL);
        _numOn = ReadToggle(NativeMethods.VK_NUMLOCK);
        _scrollOn = ReadToggle(NativeMethods.VK_SCROLL);
    }

    private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0)
        {
            var vkCode = Marshal.ReadInt32(lParam);

            // Toggle keys: detect state change on key up, when the toggle state is reliably updated
            if (wParam == (IntPtr)NativeMethods.WM_KEYUP || wParam == (IntPtr)NativeMethods.WM_SYSKEYUP)
            {
                switch (vkCode)
                {
                    case NativeMethods.VK_CAPITAL:
                        HandleToggleChanged("Caps Lock", ref _capsOn, NativeMethods.VK_CAPITAL);
                        break;
                    case NativeMethods.VK_NUMLOCK:
                        HandleToggleChanged("Num Lock", ref _numOn, NativeMethods.VK_NUMLOCK);
                        break;
                    case NativeMethods.VK_SCROLL:
                        HandleToggleChanged("Scroll Lock", ref _scrollOn, NativeMethods.VK_SCROLL);
                        break;
                }
            }
            // For other keys, occasionally re-evaluate toggle states in case they changed via remote input
            else if (wParam == (IntPtr)NativeMethods.WM_KEYDOWN || wParam == (IntPtr)NativeMethods.WM_SYSKEYDOWN)
            {
                if (vkCode != NativeMethods.VK_CAPITAL && vkCode != NativeMethods.VK_NUMLOCK && vkCode != NativeMethods.VK_SCROLL)
                {
                    HandleToggleChanged("Caps Lock", ref _capsOn, NativeMethods.VK_CAPITAL, silentWhenUnchanged: true);
                    HandleToggleChanged("Num Lock", ref _numOn, NativeMethods.VK_NUMLOCK, silentWhenUnchanged: true);
                    HandleToggleChanged("Scroll Lock", ref _scrollOn, NativeMethods.VK_SCROLL, silentWhenUnchanged: true);
                }
            }
        }

        return NativeMethods.CallNextHookEx(_hookId, nCode, wParam, lParam);
    }

    private void HandleToggleChanged(string keyName, ref bool cache, int keyCode, bool silentWhenUnchanged = false)
    {
        var isOn = ReadToggle(keyCode);
        if (isOn == cache && silentWhenUnchanged)
        {
            return;
        }

        if (isOn != cache)
        {
            cache = isOn;
            KeyStateChanged?.Invoke(this, new KeyStateChangedEventArgs(keyName, isOn));
        }
    }

    private static bool ReadToggle(int keyCode)
    {
        return (NativeMethods.GetKeyState(keyCode) & 0x0001) != 0;
    }
}
