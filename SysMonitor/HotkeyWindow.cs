using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MonitorTray
{
    public class HotkeyWindow : NativeWindow, IDisposable
    {
        private const int WM_HOTKEY = 0x0312;

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        public event Action HotkeyPressed;

        public HotkeyWindow()
        {
            CreateHandle(new CreateParams());
        }

        public bool RegisterHotKey(int id, uint modifiers, Keys key)
        {
            return RegisterHotKey(this.Handle, id, modifiers, (uint)key);
        }

        public void UnregisterHotKey(int id)
        {
            UnregisterHotKey(this.Handle, id);
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_HOTKEY)
            {
                HotkeyPressed?.Invoke();
            }
            base.WndProc(ref m);
        }

        public void Dispose()
        {
            DestroyHandle();
        }
    }
}
