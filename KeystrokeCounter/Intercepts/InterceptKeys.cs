using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace KeystrokeCounter.Intercepts
{
    public class InterceptKeys
    {

        public InterceptKeys(KeyCounter callback)
        {
            _callback = callback;
            _proc = HookCallback;
        }

        private KeyCounter _callback;
        private LowLevelKeyboardProc _proc;
        private IntPtr _hookID = IntPtr.Zero;
        private HashSet<Key> _pressedKeys = new HashSet<Key>();

        public void Enable()
        {
            _hookID = SetHook(_proc);
        }

        public void Disable()
        {
            UnhookWindowsHookEx(_hookID);
        }

        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                    GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private IntPtr HookCallback(
            int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                var key = KeyInterop.KeyFromVirtualKey(Marshal.ReadInt32(lParam));
                switch ((KeysMessages)wParam)
                {
                    case KeysMessages.WM_KEYDOWN:
                        if (_pressedKeys.Add(key))
                            _callback.Record(key);
                        break;
                    case KeysMessages.WM_KEYUP:
                        _pressedKeys.Remove(key);
                        break;
                }
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        #region DLL Imports

        private const int WH_KEYBOARD_LL = 13;

        private enum KeysMessages
        {
            WM_KEYDOWN = 0x0100,
            WM_KEYUP = 0x0101
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook,
            LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
            IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        private delegate IntPtr LowLevelKeyboardProc(
            int nCode, IntPtr wParam, IntPtr lParam);

        #endregion

    }
}
