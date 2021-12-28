using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace ManyHelpers.Windows {
    public class WindowHelper {
        private delegate bool EnumWindowProc(IntPtr hwnd, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        [DllImport("user32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnumChildWindows(IntPtr window, EnumWindowProc callback, IntPtr lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowTextLength(IntPtr hWnd);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, string windowTitle);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetWindow(IntPtr hWnd, GetWindowType uCmd);
        [DllImport("User32.dll", CharSet = CharSet.Unicode, EntryPoint = "SendMessageW")]
        public static extern IntPtr SendMessageWStr(IntPtr hWnd, uint uMsg, IntPtr wParam, string lParam);
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr hWnd, uint msg, int wParam, IntPtr lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(int hWnd, int msg, int wParam, IntPtr lParam);
        [DllImport("user32.dll")]
        public static extern IntPtr PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        public const uint WM_SETTEXT = 0x000C;
        public const int VK_ENTER = 0x0D;
        public const uint WM_KEYDOWN = 0x0100;
        public const uint WM_KEYUP = 0x0101;
        public const uint WM_COMMAND = 0x0111;
        public const int BN_CLICKED = 245;
        public const int WM_LBUTTONDOWN = 0x201;
        public const int WM_LBUTTONUP = 0x202;
        public const int IDOK = 1;

        public enum GetWindowType : uint {
            GW_HWNDFIRST = 0,
            GW_HWNDLAST = 1,
            GW_HWNDNEXT = 2,
            GW_HWNDPREV = 3,
            GW_OWNER = 4,
            GW_CHILD = 5,
            GW_ENABLEDPOPUP = 6
        }

        public static IntPtr GetWindowHandle(string windowName) {
            IntPtr hWnd = IntPtr.Zero;

            foreach (Process pList in Process.GetProcesses()) {
                if (pList.MainWindowTitle.Contains(windowName)) {
                    hWnd = pList.MainWindowHandle;
                }
            }
            return hWnd;
        }

        public static List<IntPtr> GetAllChildHandles(IntPtr mainHandle) {
            List<IntPtr> childHandles = new List<IntPtr>();

            GCHandle gcChildhandlesList = GCHandle.Alloc(childHandles);
            IntPtr pointerChildHandlesList = GCHandle.ToIntPtr(gcChildhandlesList);

            try {
                EnumWindowProc childProc = new EnumWindowProc(EnumWindow);
                EnumChildWindows(mainHandle, childProc, pointerChildHandlesList);
            } finally {
                gcChildhandlesList.Free();
            }

            return childHandles;
        }

        private static bool EnumWindow(IntPtr hWnd, IntPtr lParam) {
            GCHandle gcChildhandlesList = GCHandle.FromIntPtr(lParam);

            if (gcChildhandlesList == null || gcChildhandlesList.Target == null) {
                return false;
            }

            List<IntPtr> childHandles = gcChildhandlesList.Target as List<IntPtr>;
            childHandles.Add(hWnd);

            return true;
        }

        public static String WndClassName(IntPtr handle) {
            int length = 1024;

            StringBuilder sb = new StringBuilder(length);

            GetClassName(handle, sb, length);

            return sb.ToString();
        }

        public static bool IsDialogClassName(IntPtr handle) {
            return "#32770".Equals(WndClassName(handle));
        }

        public static string GetWindowTitle(IntPtr handle) {
            int capacity = GetWindowTextLength(handle) * 2;
            StringBuilder stringBuilder = new StringBuilder(capacity);
            GetWindowText(handle, stringBuilder, stringBuilder.Capacity);

            return stringBuilder.ToString();
        }

        public static List<IntPtr> GetControls(IntPtr parentHandle, string className) {
            List<IntPtr> childHandles = new List<IntPtr>();
            var allcontrols = GetAllChildHandles(parentHandle);
            foreach (var control in allcontrols) {
                if (className.Equals(WndClassName(control))) {
                    childHandles.Add(control);
                }
            }
            return childHandles;
        }

        public static void Click(IntPtr handle) {
            SendMessage(handle, WM_LBUTTONDOWN, 0, IntPtr.Zero);
            SendMessage(handle, WM_LBUTTONUP, 0, IntPtr.Zero);
            SendMessage(handle, WM_LBUTTONDOWN, 0, IntPtr.Zero);
            SendMessage(handle, WM_LBUTTONUP, 0, IntPtr.Zero);
        }
    }
}
