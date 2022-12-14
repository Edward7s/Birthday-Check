using Microsoft.Win32;

namespace BirthdayCheck
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
            RegistryKey startup = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            if (startup.GetValue("Brithdaycheckerx") == null)
                startup.SetValue("Brithdaycheckerx", Directory.GetCurrentDirectory() + "\\BirthdayCheck.exe");
        }
    }
}