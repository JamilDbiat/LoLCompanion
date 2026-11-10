using System.Drawing;
using System.Windows.Forms;
using Microsoft.Web.WebView2.WinForms;
using Microsoft.Win32;

namespace LeagueHelper
{
    public class MainForm : Form
    {
        private const string RunKey = @"Software\Microsoft\Windows\CurrentVersion\Run";
        private readonly NotifyIcon Tray;
        private bool Quitting;
        private bool StartHidden;
        private bool TipShown;

        public MainForm(string dataFolder)
        {
            Text = "LoL Companion";
            Width = 540;
            Height = 920;
            BackColor = Color.FromArgb(18, 10, 31);
            StartHidden = Environment.GetCommandLineArgs().Contains("--tray");

            var web = new WebView2
            {
                Dock = DockStyle.Fill,
                CreationProperties = new CoreWebView2CreationProperties { UserDataFolder = Path.Combine(dataFolder, "WebView2") },
                Source = new Uri("http://localhost:5000")
            };
            Controls.Add(web);

            var startup = new ToolStripMenuItem("Start with Windows") { CheckOnClick = true, Checked = IsStartup() };
            startup.CheckedChanged += (s, e) => SetStartup(startup.Checked);

            var menu = new ContextMenuStrip();
            menu.Items.Add("Open", null, (s, e) => ShowWindow());
            menu.Items.Add(startup);
            menu.Items.Add("Quit", null, (s, e) => Quit());

            Tray = new NotifyIcon
            {
                Icon = SystemIcons.Application,
                Text = "LoL Companion",
                Visible = true,
                ContextMenuStrip = menu
            };
            Tray.DoubleClick += (s, e) => ShowWindow();
        }

        protected override void SetVisibleCore(bool value)
        {
            if (StartHidden)
            {
                StartHidden = false;
                value = false;
                if (!IsHandleCreated) CreateHandle();
            }
            base.SetVisibleCore(value);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (!Quitting && e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
                if (!TipShown)
                {
                    TipShown = true;
                    Tray.ShowBalloonTip(3000, "LoL Companion", "Still running here. Right-click the icon to quit.", ToolTipIcon.Info);
                }
                return;
            }
            base.OnFormClosing(e);
        }

        private void ShowWindow()
        {
            Show();
            WindowState = FormWindowState.Normal;
            Activate();
        }

        private void Quit()
        {
            Quitting = true;
            Tray.Visible = false;
            Application.Exit();
        }

        private static bool IsStartup()
        {
            using var key = Registry.CurrentUser.OpenSubKey(RunKey);
            return key?.GetValue("LoLCompanion") != null;
        }

        private static void SetStartup(bool on)
        {
            using var key = Registry.CurrentUser.OpenSubKey(RunKey, true);
            if (key == null) return;
            if (on) key.SetValue("LoLCompanion", "\"" + Environment.ProcessPath + "\" --tray");
            else key.DeleteValue("LoLCompanion", false);
        }
    }
}