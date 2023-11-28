using System;
using System.Windows;
using System.Windows.Threading;

namespace GameOfLife
{
    public partial class MainWindow : Window
    {
        private readonly Grid mainGrid;
        private readonly DispatcherTimer timer;   //  Generation timer
        private readonly AdWindow[] adWindows;
        private int genCounter;

        public MainWindow()
        {
            InitializeComponent();
            mainGrid = new Grid(MainCanvas);
            adWindows = new AdWindow[Settings.AdWindowsCount];

            timer = new DispatcherTimer();
            timer.Tick += OnTimer;
            timer.Interval = TimeSpan.FromMilliseconds(Settings.TimeBetweenGenerations);
        }


        private void StartAd()
        {
            for (int i = 0; i < Settings.AdWindowsCount; i++)
            {
                if (adWindows[i] == null)
                {
                    adWindows[i] = new AdWindow(this);
                    adWindows[i].Closed += AdWindowOnClosed;
                    adWindows[i].Top = this.Top + (330 * i) + 70;
                    adWindows[i].Left = this.Left + 240;
                    adWindows[i].Show();
                }
            }
        }

        private void AdWindowOnClosed(object sender, EventArgs eventArgs)
        {
            var adWindow = sender as AdWindow;
            adWindow.Closed -= AdWindowOnClosed;
            var index = Array.IndexOf(adWindows, adWindow);
            adWindows[index] = null;
        }


        private void Button_OnClick(object sender, EventArgs e)
        {
            if (!timer.IsEnabled)
            {
                timer.Start();
                ButtonStart.Content = "Stop";
                StartAd();
            }
            else
            {
                timer.Stop();
                ButtonStart.Content = "Start";
            }
        }

        private void OnTimer(object sender, EventArgs e)
        {
            mainGrid.Update();
            genCounter++;
            lblGenCount.Content = $"Generations: {genCounter}";
        }

        private void ButtonClear_Click(object sender, RoutedEventArgs e)
        {
            mainGrid.Clear();
        }


    }
}
