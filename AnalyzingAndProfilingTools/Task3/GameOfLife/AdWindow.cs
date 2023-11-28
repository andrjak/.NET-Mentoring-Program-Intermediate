using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace GameOfLife
{
    class AdWindow : Window
    {
        private static readonly List<string> adLinks = new List<string>
            {
                "http://example.com",
                "http://example.com",
                "http://example.com",
            };
        private static readonly List<BitmapImage> images = new List<BitmapImage>
            {
                new BitmapImage(new Uri("ad1.jpg", UriKind.Relative)),
                new BitmapImage(new Uri("ad2.jpg", UriKind.Relative)),
                new BitmapImage(new Uri("ad3.jpg", UriKind.Relative)),
            };

        private readonly DispatcherTimer adTimer = new DispatcherTimer()
        {
            Interval = TimeSpan.FromMilliseconds(Settings.TimeBetweenAds)
        };
        private readonly ImageBrush imageBrush = new ImageBrush();

        private int imageNumber;
        private string currentLink;


        public AdWindow(Window owner)
        {
            Random rnd = new Random();
            Owner = owner;
            Width = 350;
            Height = 100;
            Background = imageBrush;
            ResizeMode = ResizeMode.NoResize;
            WindowStyle = WindowStyle.ToolWindow;
            Title = "Support us by clicking the ads";
            Cursor = Cursors.Hand;
            ShowActivated = false;
            MouseDown += OnClick;

            imageNumber = rnd.Next(0, images.Count);
            ChangeAds(this, new EventArgs());

            adTimer.Tick += ChangeAds;
            adTimer.Start();
        }

        private void OnClick(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start(currentLink);
            Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            Unsubscribe();
            base.OnClosed(e);
        }

        public void Unsubscribe()
        {
            adTimer.Tick -= ChangeAds;
        }

        private void ChangeAds(object sender, EventArgs eventArgs)
        {
            imageBrush.ImageSource = images[imageNumber];
            currentLink = adLinks[imageNumber];
            imageNumber = imageNumber == images.Count - 1 ? 0 : ++imageNumber;
        }
    }
}