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
        public static Process PythonProcess { get; set; }

        static MainWindow()
        {
            PythonProcess = null;
        }

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
                    StartPython(close:true);
                }
                catch (Win32Exception)
                {
                    ShowError("Blind Helper a besoin d'être lancé en tant qu'administrateur pour fonctionner.");
                }
            }
        }

        private void StartPython(bool close = false)
        {
            try
            {
                
                string closePython = (close) ? "--mode stop" : "";
                
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    RedirectStandardInput = true,
                    UseShellExecute = false,
                    CreateNoWindow = false,
                    WorkingDirectory = $@"{BaseDir}\\python\\", // Set your Python script's directory
                    WindowStyle = ProcessWindowStyle.Minimized
                };

                Process pythonProcess = new Process { StartInfo = startInfo };
                pythonProcess.Start();

                // Run the Python script by sending the command to CMD
                pythonProcess.StandardInput.WriteLine($@"python main.py {closePython}");
                
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }
    
    public static class StartupManager
    {
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
                
                // MainWindow.ShowError("Blind Helper a été ajouté au démarrage de Windows.");
            }
            catch (Exception)
            {
                // MainWindow.ShowError("Error enabling startup");
            }
        }
    }
}