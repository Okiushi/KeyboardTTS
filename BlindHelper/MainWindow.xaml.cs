using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Threading;
using FontFamily = System.Windows.Media.FontFamily;


namespace BlindHelper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private int _height, _width;

        public int WindowHeight
        {
            set
            {
                if (value == _height) return;
                _height = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("WindowHeight"));
            }
        }
        public int WindowWidth
        {
            set
            {
                if (value == _width) return;
                _width = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("WindowWidth"));
            }
        }

        private readonly List<FontFamily> _fontFamilies = new List<FontFamily>
        {
            new FontFamily(new Uri("pack://application:,,,/Fonts/"), "./#Tiresias Infofont"),
            new FontFamily(new Uri("pack://application:,,,/Fonts/"), "./#Luciole"),
            new FontFamily(new Uri("pack://application:,,,/Fonts/"), "./#HelveticaNeue")
        };

        public MainWindow()
        {
            InitializeComponent();
            
            NotifyIcon ni = new NotifyIcon();
            ni.Icon = new Icon("Main.ico");
            ni.Visible = true;
            ni.Click += 
                delegate
                {
                    Show();
                    WindowState = WindowState.Normal;
                };

            DataContext = this;
            WindowHeight = 400;
            WindowWidth = 800;
            CenterWindowOnScreen();
        }
        
        public event PropertyChangedEventHandler PropertyChanged;
        
        private void CenterWindowOnScreen()
        {
            double screenWidth = SystemParameters.PrimaryScreenWidth;
            double screenHeight = SystemParameters.PrimaryScreenHeight;
            double windowWidth = Width;
            double windowHeight = Height;
            Left = screenWidth / 2 - windowWidth / 2;
            Top = screenHeight / 2 - windowHeight / 2;
        }
        
        private void Window_OnInitialized(object sender, EventArgs e)
        {
            Console.WriteLine(@"Window Initialized");
            // object timer that can tick after a given time
            DispatcherTimer timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(50) };
            // the tick is the event that happens at the end of the timer
            timer.Tick += (s, args) =>
            {
                Resize_Window(factor: 0.7d);
                Console.WriteLine(@"Window Resized");
                
                CenterWindowOnScreen();
                Console.WriteLine(@"Window Centered");
                
                // after tick is raised, stop the timer (to prevent loop)
                timer.Stop();
            };
            
            
            
            timer.Start();
        }

        private void SizeSlider_OnDragCompleted(object sender, DragCompletedEventArgs e)
        {

            double newValue = SizeSlider.Value;
            
            Console.WriteLine($@"Slider Value: {newValue}");
            
            Resize_Window(factor: newValue);
            
            CenterWindowOnScreen();
        }

        private void Resize_Window(double factor)
        {
            if (Math.Abs(factor - 1) < 0.000000001)  // fix for double precision, equivalent to factor == 1
            {
                WindowState = WindowState.Maximized;
            }
            else
            {
                WindowState = WindowState.Normal;
                double screenWidth = SystemParameters.PrimaryScreenWidth;
                double screenHeight = SystemParameters.PrimaryScreenHeight;
            
                Height = (int) (screenHeight * factor);
                Width = (int) (screenWidth * factor);
            }
        }
        
        private void FontSlider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int selectedFontIndex = (int)FontSlider.Value;

            // Define an array of font families
            
            if (selectedFontIndex >= 0 && selectedFontIndex < _fontFamilies.Count)
            {
                // Set the selected font family to the TextBlock
                FontFamily = _fontFamilies[selectedFontIndex];
            }
        }

        private void ReduceButton_OnClick(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(@"Reduce Button Clicked");
        }

        private void CloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(@"Close Button Clicked");
        }
    }
}