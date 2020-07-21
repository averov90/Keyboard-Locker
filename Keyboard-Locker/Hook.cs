using System;
using System.Runtime.InteropServices;

namespace HookKeyboard
{
    public static class HookAPI
    {
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProcDelegate lpfn, IntPtr hMod, int dwThreadId);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetModuleHandle(IntPtr lpModuleName);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);


        [StructLayout(LayoutKind.Sequential)]
        public struct KeyboardHookStruct
        {
            public readonly int VirtualKeyCode;
            public readonly int ScanCode;
            public readonly int Flags;
            public readonly int Time;
            public readonly IntPtr ExtraInfo;
        }
        public delegate IntPtr LowLevelKeyboardProcDelegate(int nCode, IntPtr wParam, IntPtr lParam);
    }

    public static class HookK
    {
        public static bool current_pressed = false; static bool active = false; public static byte mode = 0;
        
        private const int WH_KEYBOARD_LL = 13;

        private static HookAPI.LowLevelKeyboardProcDelegate m_callback;
        private static IntPtr m_hHook = IntPtr.Zero;

        private static GCHandle gc_h;

        static IntPtr LowLevelKeyboardHookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            var khs = (HookAPI.KeyboardHookStruct) Marshal.PtrToStructure(lParam, typeof(HookAPI.KeyboardHookStruct));
            if (nCode < 0)
            {
                return HookAPI.CallNextHookEx(m_hHook, nCode, wParam, lParam);
            }
            else
            {
                if (mode == 1)
                {
                    if ((khs.Flags & 128) != 0)
                    {
                        current_pressed = true;
                    }

                    GCHandle gch = GCHandle.Alloc(m_hHook);

                    return HookAPI.CallNextHookEx(m_hHook, nCode, wParam, lParam);
                }
                else
                {
                    if (mode == 2)
                    {
                        current_pressed = true;
                    }
                    IntPtr val = new IntPtr(1);
                    return val;
                }
            }
        }

        static void SetHook()
        {
            m_callback = new HookAPI.LowLevelKeyboardProcDelegate(LowLevelKeyboardHookProc);
            gc_h = GCHandle.Alloc(m_callback);
            m_hHook = HookAPI.SetWindowsHookEx(WH_KEYBOARD_LL, m_callback, HookAPI.GetModuleHandle(IntPtr.Zero), 0);
        }

        static void Unhook()
        {
            HookAPI.UnhookWindowsHookEx(m_hHook);
            gc_h.Free();
        }

        public static bool Hook_active
        {
            get { return active; }
            set
            {
                if (value && !active) { SetHook(); active = true; }
                else if (!value && active) { Unhook(); active = false; }
            }
        }
        public static byte Hook_workmode
        {
            get { return mode; }
            set
            {
                mode = value;
            }
        }
    }
}