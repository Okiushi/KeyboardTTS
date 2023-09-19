using System;
using System.ComponentModel;
using System.Windows;

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
    }
}