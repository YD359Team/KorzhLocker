using System;
using System.Runtime.InteropServices;
using System.Timers;

namespace KorzhLocker.Input;

public class ModifierKeyMonitor
{
    public static ModifierKeyMonitor Instance => _instance ??= new();
    private static ModifierKeyMonitor? _instance;

    [DllImport("user32.dll")]
    private static extern short GetAsyncKeyState(int vKey);

    [DllImport("user32.dll")]
    private static extern short GetKeyState(int nVirtKey);

    [DllImport("user32.dll")]
    private static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

    private const int VK_SHIFT = 0x10;
    private const int VK_CONTROL = 0x11;
    private const int VK_MENU = 0x12; // Alt
    private const int VK_CAPITAL = 0x14;

    private const int INPUT_KEYBOARD = 1;
    private const uint KEYEVENTF_KEYUP = 0x0002;
    private const uint KEYEVENTF_SCANCODE = 0x0008;

    private readonly System.Timers.Timer _timer;

    private readonly Dictionary<int, DateTime> _pressedKeys = new();

    private DateTime _lastCapsToggle = DateTime.MinValue;
    private const int CAPS_DEBOUNCE_MS = 300;

    private const int CHECK_INTERVAL_MS = 20;
    private const int STUCK_THRESHOLD_MS = 2000;

    private ModifierKeyMonitor()
    {
        _timer = new(CHECK_INTERVAL_MS)
        {
            AutoReset = true
        };

        _timer.Elapsed += OnTimerElapsed;
    }

    public void Start() => _timer.Start();
    public void Stop() => _timer.Stop();

    private void OnTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        CheckKey(VK_SHIFT);
        CheckKey(VK_CONTROL);
        CheckKey(VK_MENU);

        HandleCapsLock();
    }

    // Проверка "залипания"
    private void CheckKey(int vk)
    {
        bool isDown = (GetAsyncKeyState(vk) & 0x8000) != 0;

        if (isDown)
        {
            if (!_pressedKeys.ContainsKey(vk))
            {
                _pressedKeys[vk] = DateTime.Now;
                return;
            }

            var heldTime = DateTime.Now - _pressedKeys[vk];

            if (heldTime.TotalMilliseconds > STUCK_THRESHOLD_MS)
            {
                ReleaseKey(vk);
                _pressedKeys.Remove(vk);
            }
        }
        else
        {
            _pressedKeys.Remove(vk);
        }
    }

    private void HandleCapsLock()
    {
        bool isCapsOn = (GetKeyState(VK_CAPITAL) & 0x0001) != 0;

        if (!isCapsOn)
            return;

        if ((DateTime.Now - _lastCapsToggle).TotalMilliseconds < CAPS_DEBOUNCE_MS)
            return;

        ToggleCapsLock();
        _lastCapsToggle = DateTime.Now;
    }

    [DllImport("user32.dll")]
    private static extern void keybd_event(
        byte bVk,
        byte bScan,
        uint dwFlags,
        IntPtr dwExtraInfo);

    private const byte SCANCODE_CAPS = 0x3A;

    private void ToggleCapsLock()
    {
        keybd_event(VK_CAPITAL, SCANCODE_CAPS, 0, IntPtr.Zero);

        Thread.Sleep(50);

        keybd_event(VK_CAPITAL, SCANCODE_CAPS, KEYEVENTF_KEYUP, IntPtr.Zero);
    }

    private void ReleaseKey(int vk)
    {
        SendKey(vk, true);
    }

    private void SendKey(int vk, bool keyUp)
    {
        var input = new INPUT
        {
            type = INPUT_KEYBOARD,
            U = new InputUnion
            {
                ki = new KEYBDINPUT
                {
                    wVk = (ushort)vk,
                    dwFlags = keyUp ? KEYEVENTF_KEYUP : 0
                }
            }
        };

        SendInput(1, [input], Marshal.SizeOf(typeof(INPUT)));
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct INPUT
    {
        public int type;
        public InputUnion U;
    }

    [StructLayout(LayoutKind.Explicit)]
    private struct InputUnion
    {
        [FieldOffset(0)] public KEYBDINPUT ki;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct KEYBDINPUT
    {
        public ushort wVk;
        public ushort wScan;
        public uint dwFlags;
        public uint time;
        public IntPtr dwExtraInfo;
    }
}