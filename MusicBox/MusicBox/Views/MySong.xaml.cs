using MusicBox.Entity;
using MusicBox.Service;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media;
using Windows.Media.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace MusicBox.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MySong : Page
    {
        private bool isPlaying = false;

        int onPlay = 0;

        private ObservableCollection<Song> listMySong;

        internal ObservableCollection<Song> ListMySong { get => listMySong; set => listMySong = value; }

        public MySong()
        {
            Get_My_Song();
            this.InitializeComponent();            
        }

        private async void Get_My_Song()
        {
            this.listMySong = new ObservableCollection<Song>();
            var response = await APIHandle.Get_My_Songs();
            var responseContent = await response.Content.ReadAsStringAsync();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var array = JArray.Parse(responseContent);
                foreach (var obj in array)
                {
                    Song song = obj.ToObject<Song>();
                    this.listMySong.Add(song);
                    Debug.WriteLine(song.link);
                }
            }
            else
            {
                var dialog = new ContentDialog()
                {
                    Title = "Error!",
                    MaxWidth = this.ActualWidth,
                    Content = "There's an error! Please try later!",
                    CloseButtonText = "OK!"
                };
                ErrorResponse errorObject = JsonConvert.DeserializeObject<ErrorResponse>(responseContent);
                if (errorObject != null)
                {
                    foreach (var key in errorObject.error.Keys)
                    {
                        var textMessage = this.FindName(key);
                        if (textMessage == null)
                        {
                            continue;
                        }
                        TextBlock textBlock = textMessage as TextBlock;
                        textBlock.Text = errorObject.error[key];
                        textBlock.Visibility = Visibility.Visible;
                    }
                }
            }
        }

        private void Load_Song(Song currentSong)
        {
            this.Player.Source = MediaSource.CreateFromUri(new Uri(currentSong.link));
        }

        private void PlaySong()
        {
            this.Player.MediaPlayer.Play();
            isPlaying = true;
        }

        private void StackPanel_Tap(object sender, TappedRoutedEventArgs e)
        {
            StackPanel panel = sender as StackPanel;
            Song selectedSong = panel.Tag as Song;
            onPlay = MenuListMySong.SelectedIndex;
            Load_Song(selectedSong);
            PlaySong();
        }
    }
}
