using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HideStartButton
{
    public partial class Main : Form
    {
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        [DllImport("USER32.DLL", CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string lclassName, string windowTitle);
        [DllImport("user32.dll")]
        static extern bool SetWindowText(IntPtr hWnd, string text);
        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        Dictionary<IntPtr, IntPtr> ShellTrays = new Dictionary<IntPtr, IntPtr>();

        public Main()
        {
            InitializeComponent();
            ExitButton.Click += ExitButton_Click;
            RestartButton.Click += RestartButton_Click;
        }

        private void Main_Load(object sender, EventArgs e)
        {
            Size = new(0, 0);
            Location = new(0, 0);
            NotifyIcon notifyIcon = new();
            notifyIcon.Visible = true;
            notifyIcon.Icon = Icon;
            notifyIcon.Text = "You can close me by right clicking my icon!";
            notifyIcon.ContextMenuStrip = MainStrip;
            notifyIcon.BalloonTipTitle = "Hide Menu v1";
            notifyIcon.BalloonTipText = "You can close me by right clicking my icon in the tray!";
            notifyIcon.ShowBalloonTip(1);
            SetupTrays();

            HideStartButton();
        }

        private void SetupTrays()
        {
            int MonitorCount = Screen.AllScreens.Count();

            Console.WriteLine(MonitorCount);

            if (MonitorCount > 1)
            {
                for (int i = 1; i <= MonitorCount; i++)
                {
                    if (i == 1)
                    {
                        IntPtr ShellTray = FindWindow("Shell_TrayWnd", string.Empty);
                        IntPtr Start = FindWindowEx(ShellTray, IntPtr.Zero, "Start", "Start");
                        ShellTrays.Add(ShellTray, Start);
                    }
                    else
                    {
                        IntPtr ShellTray = FindWindow("Shell_SecondaryTrayWnd", string.Empty);
                        IntPtr Start = FindWindowEx(ShellTray, IntPtr.Zero, "Start", "Start");

                        if (Start != IntPtr.Zero)
                        {
                            ShellTrays.Add(ShellTray, Start);
                            SetWindowText(ShellTray, i.ToString());
                        }
                    }
                }
            }
            else
            { 
                IntPtr ShellTray = FindWindow("Shell_TrayWnd", string.Empty);
                IntPtr Start = FindWindowEx(ShellTray, IntPtr.Zero, "Start", "Start");
                ShellTrays.Add(ShellTray, Start);
            }
        }

        private void ShowStartButton()
        {
            foreach (KeyValuePair<IntPtr, IntPtr> Pair in ShellTrays)
            {
                SetWindowText(Pair.Key, string.Empty);
                ShowWindow(Pair.Value, 9);
            }
        }

        private void HideStartButton()
        {
            foreach (KeyValuePair<IntPtr, IntPtr> Pair in ShellTrays)
            {
                ShowWindow(Pair.Value, 0);
            }
        }

        private void RestartButton_Click(object sender, System.EventArgs e)
        {
            ShowStartButton();
            Process.Start(Application.ExecutablePath);
            Process.GetCurrentProcess().Kill();
        }

        private void ExitButton_Click(object sender, System.EventArgs e)
        {
            ShowStartButton();
            Process.GetCurrentProcess().Kill();
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            ShowStartButton();
        }

        private void Main_FormClosed(object sender, FormClosedEventArgs e)
        {
            ShowStartButton();
        }
    }
}
