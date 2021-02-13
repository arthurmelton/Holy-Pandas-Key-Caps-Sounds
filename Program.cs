using System;
using System.Diagnostics;
using System.Drawing;
using System.Media;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Holy_Pandas_Key_Caps_Sounds.Properties;
using Microsoft.Win32;

namespace Holy_Pandas_Key_Caps_Sounds
{
    static class Program
    {
        private const int WhKeyboardLl = 13;
        private const int WmKeydown = 0x0100;
        private const int WmKeyup = 0x0101;
        private static LowLevelKeyboardProc _proc = HookCallback;
        private static IntPtr _hookId = IntPtr.Zero;
        private static SoundPlayer _soundDown;
        private static SoundPlayer _soundUp;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);


        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        public static void Main()
        {
            _soundDown = new SoundPlayer(Resources.Holy_Pandas_down);
            _soundUp = new SoundPlayer(Resources.Holy_Pandas_up);
            _hookId = SetHook(_proc);
            Application.Run(new MyCustomApplicationContext());
            UnhookWindowsHookEx(_hookId);
        }

        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using var curProcess = Process.GetCurrentProcess();
            using var curModule = curProcess.MainModule;

            return curModule != null ? SetWindowsHookEx(WhKeyboardLl, proc, GetModuleHandle(curModule.ModuleName), 0) : default;

        }

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WmKeydown)
            {
                //int vkCode = Marshal.ReadInt32(lParam);
                //Console.WriteLine((Keys)vkCode);
                new Thread(() =>
                {
                    Thread.CurrentThread.IsBackground = true;
                    _soundDown.Play();
                }).Start();
            }
            if (nCode >= 0 && wParam == (IntPtr)WmKeyup)
            {
                //int vkCode = Marshal.ReadInt32(lParam);
                //Console.WriteLine((Keys)vkCode);
                new Thread(() =>
                {
                    Thread.CurrentThread.IsBackground = true;
                    _soundUp.Play();
                }).Start();
            }
            return CallNextHookEx(_hookId, nCode, wParam, lParam);
        }
    }

    public class MyCustomApplicationContext : ApplicationContext
    {
        private readonly NotifyIcon _trayIcon;

        public MyCustomApplicationContext()
        {

            // Initialize Tray Icon
            _trayIcon = new NotifyIcon
            {
                Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location), Text = @"Holy Pandas Key Caps Sounds", ContextMenuStrip = new ContextMenuStrip(), Visible = true
            };

            _trayIcon.ContextMenuStrip.Items.Add("Exit", null, Exit);
            _trayIcon.ContextMenuStrip.Items.Add("Start on startup", null, AddReg());

        }

        private void Exit(object sender, EventArgs e)
        {
            // Hide tray icon, otherwise it will remain shown until user mouse's over it
            _trayIcon.Visible = false;

            Application.Exit();
        }

        private static EventHandler AddReg()
        {
            var reg = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            reg?.SetValue("Holy Pandas Key Caps Sounds", Application.ExecutablePath);

            return null;
        }
    }
}