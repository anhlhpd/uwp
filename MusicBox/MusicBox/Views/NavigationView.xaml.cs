using MusicBox.Entity;
using MusicBox.Service;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace MusicBox.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NavigationView : Page
    {
        public NavigationView()
        {
            this.InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.MyNavigationView.IsPaneOpen = !this.MyNavigationView.IsPaneOpen;
            if (!this.MyNavigationView.IsPaneOpen)
            {
                this.StackIcon.Margin = new Thickness(10, 50, 0, 0);
            }
            else
            {
                this.StackIcon.Margin = new Thickness(50, 50, 0, 0);
            }
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            RadioButton radio = sender as RadioButton;
            switch (radio.Tag.ToString())
            {
                case "CreateSong":
                    this.MainFrame.Navigate(typeof(SongForm), null, new EntranceNavigationTransitionInfo());
                    break;
                case "ListSong":
                    this.MainFrame.Navigate(typeof(ListSong), null, new EntranceNavigationTransitionInfo());
                    break;
                default:
                    break;
            }
        }
    }
}
