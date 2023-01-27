using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace KeystrokeCounter.Intercepts
{
    public class InterceptForeground
    {

        private const uint EVENT_SYSTEM_FOREGROUND = 3;
        private const uint WINEVENT_OUTOFCONTEXT = 0;

        public InterceptForeground(KeyCounter callback)
        {
            _callback = callback;
            _proc = HookCallback;
        }

        private KeyCounter _callback;
        private LowLevelForegroundProc _proc;
        private static IntPtr _hookID = IntPtr.Zero;
        
        public void Enable()
        {
            _hookID = SetHook(_proc);
        }

        public void Disable()
        {
            UnhookWinEvent(_hookID);
        }

        private static IntPtr SetHook(LowLevelForegroundProc proc)
        {
            return SetWinEventHook(EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND,
                IntPtr.Zero, proc, 0, 0, WINEVENT_OUTOFCONTEXT);
        }

        delegate void LowLevelForegroundProc(IntPtr hWinEventHook, uint eventType,
            IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

        private void HookCallback(
            IntPtr hWinEventHook, uint eventType,
            IntPtr hwnd, int idObject, int idChild,
            uint dwEventThread, uint dwmsEventTime)
        {
            int pid;
            GetWindowThreadProcessId(hwnd, out pid);
            var p = Process.GetProcessById(pid);
            if (p != null)
                _callback.SetForeground(p.ProcessName);
            else
                _callback.SetForeground("Unknown");
        }

        [DllImport("user32.dll")]
        static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr
           hmodWinEventProc, LowLevelForegroundProc lpfnWinEventProc, uint idProcess,
           uint idThread, uint dwFlags);

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern Int32 GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

        [DllImport("user32.dll")]
        static extern bool UnhookWinEvent(IntPtr hWinEventHook);

    }
}
