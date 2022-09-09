using BirthdayCheck.Utils;
using Microsoft.Win32;
using System.DirectoryServices.ActiveDirectory;
using System.Drawing;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace BirthdayCheck
{
    public partial class Form1 : Form
    {
        private NotifyIcon _icon { get; set; }
        public Form1()
        {
            new InitializeConsole();
            this.InitializeComponent();
            _icon = new System.Windows.Forms.NotifyIcon();
            _icon.Text = "BirthDay Reminder";
            using (WebClient wc = new WebClient())
                _icon.Icon = new Icon(new MemoryStream(wc.DownloadData("https://nocturnal-client.xyz/cl/Download/Nocturnal%20Circle.ico")));
            _icon.Visible = true;
            _icon.ContextMenuStrip = new System.Windows.Forms.ContextMenuStrip();
            _icon.ContextMenuStrip.Items.Add("Close", null, (sender, e) => Application.Exit());
            _icon.ContextMenuStrip.Items.Add("Show / Hide Window", null, (sender, e) => InitializeConsole.Instance.ToggleConsole());
            this.Load += (sender, e) =>
            {
                this.Hide();
                this.ShowInTaskbar = false;
            };
            RegistryKey onStartup = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            if (onStartup.GetValue(Application.ProductName) != null) return;
            onStartup.SetValue(Application.ProductName, Directory.GetCurrentDirectory() + "\\BirthdayCheck.exe");
        }

    }
}