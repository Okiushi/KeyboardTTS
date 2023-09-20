using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Security.Principal;
using System.Windows;
using Microsoft.Win32;

namespace BlindHelper
{
    public partial class MainWindow
    {
        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
            
            base.OnClosing(e);
        }

        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
                Hide();
            
            base.OnStateChanged(e);
        }
        
        private void OnLoaded_WindowsStartup()
        {
            CheckAdmin();
            StartupManager.EnableStartup("BlindHelper", Assembly.GetExecutingAssembly().Location);
        }
        
        private void CheckAdmin()
        {
            WindowsPrincipal pricipal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
            bool hasAdministrativeRight = pricipal.IsInRole(WindowsBuiltInRole.Administrator);
            if (!hasAdministrativeRight)
            {
                // relaunch the application with admin rights
                string fileName = Assembly.GetExecutingAssembly().Location;
                ProcessStartInfo processInfo = new ProcessStartInfo
                {
                    Verb = "runas",
                    FileName = fileName
                };

                try
                {
                    Process.Start(processInfo);
                    Application.Current.Shutdown();
                }
                catch (Win32Exception)
                {
                    ShowError("Blind Helper a besoin d'être lancé en tant qu'administrateur pour fonctionner.");
                }
 
                return;
            }
        }
    }
    
    public static class StartupManager
    {
        public static bool IsStartupEnabled(string appName)
        {
            try
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", false);
                if (key != null)
                {
                    return key.GetValue(appName) != null;
                }
            }
            catch (Exception)
            {
                MainWindow.ShowError("Error checking startup status");
            }
            return false;
        }

        public static void EnableStartup(string appName, string appPath)
        {
            try
            {
                RegistryKey key =
                    Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                if (key == null)
                {
                    key = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");
                }

                key?.SetValue(appName, appPath);

                key?.Close();
                
                MainWindow.ShowError("Blind Helper a été ajouté au démarrage de Windows.");
            }
            catch (Exception)
            {
                MainWindow.ShowError("Error enabling startup");
            }
        }
    }
}