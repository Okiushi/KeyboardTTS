using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;


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
            get => _height;
            set
            {
                if (value != _height)
                {
                    _height = value;
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("WindowHeight"));
                }
            }
        }

        public List<TextBlock> TextBlocks;
        
        public int WindowWidth
        {
            get => _width;
            set
            {
                if (value != _width)
                {
                    _width = value;
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("WindowWidth"));
                }
            }
        }
        
        public MainWindow()
        {
            InitializeComponent();
            TextBlocks = new List<TextBlock>
            {
                LogoTxt,
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
            Left = (screenWidth / 2) - (windowWidth / 2);
            Top = (screenHeight / 2) - (windowHeight / 2);
        }
        
        private void Window_OnInitialized(object sender, EventArgs e)
        {
            Console.WriteLine(@"Window Initialized");
            // object timer that can tick after a given time
            DispatcherTimer timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(50) };
            // the tick is the event that happens at the end of the timer
            timer.Tick += (s, args) =>
            {
                CenterWindowOnScreen();
                Console.WriteLine(@"Window Centered");
                // after tick is raised, stop the timer (to prevent loop)
                timer.Stop();
            };
            
            
            
            timer.Start();
        }

        private void SizeSlider_OnDragCompleted(object sender, DragCompletedEventArgs e)
        {
            double screenWidth = SystemParameters.PrimaryScreenWidth;
            double screenHeight = SystemParameters.PrimaryScreenHeight;

            double newValue = SizeSlider.Value;
            
            Console.WriteLine($@"Slider Value: {newValue}");
            
            Height = (int) (screenHeight * newValue);
            Width = (int) (screenWidth * newValue);
            CenterWindowOnScreen();
        }

        // private void FontSlider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        // {
        //     Console.WriteLine($@"Font Slider Value: {e.NewValue}");
        // }
        
        private void FontSlider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int selectedFontIndex = (int)FontSlider.Value;

            // Define an array of font families

            List<FontFamily> fontFamilies = new List<FontFamily>
            {
                new FontFamily(new Uri("pack://application:,,,/Fonts/"), "./#Tiresias Infofont"),
                new FontFamily(new Uri("pack://application:,,,/Fonts/"), "./#Luciole"),
                new FontFamily(new Uri("pack://application:,,,/Fonts/"), "./#HelveticaNeue"),
            };
            if (selectedFontIndex >= 0 && selectedFontIndex < fontFamilies.Count)
            {
                // Set the selected font family to the TextBlock
                LogoTxt.FontFamily = fontFamilies[selectedFontIndex];
            }
            
            
        }
    }
}