using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace KorzhLocker.Input
{
    internal sealed class HotkeyManager
    {
        public static HotkeyManager Instance => field ??= new();

        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);
    }
}
