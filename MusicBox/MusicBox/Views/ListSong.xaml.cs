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
using Windows.Media.Playback;
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
    public sealed partial class ListSong : Page
    {
        private bool isPlaying = false;

        int onPlay = 0;
        
        private ObservableCollection<Song> listAllSong;

        internal ObservableCollection<Song> ListAllSong { get => listAllSong; set => listAllSong = value; }

        private Song currentSong;

        private SystemMediaTransportControls MediaControls { get; }

        public ListSong()
        { 
            Get_All_Song();
            this.InitializeComponent();
            this.currentSong = new Song();

            MediaControls = SystemMediaTransportControls.GetForCurrentView();
           
            MediaControls.ButtonPressed += MediaControls_ButtonPressed;

            //MediaPlayer.MediaPlayer.Position = MediaPlayer.MediaPlayer.Position.Add(new TimeSpan(0, 0, 5));
        }

        private void MediaControls_ButtonPressed(SystemMediaTransportControls sender,  SystemMediaTransportControlsButtonPressedEventArgs args)
        {
            switch (args.Button)
            {
                case SystemMediaTransportControlsButton.Next:
                    Debug.WriteLine("Next");
                    MediaControls.PlaybackStatus = MediaPlaybackStatus.Changing;
                    //playbackList.MoveNext();
                    if (onPlay < listAllSong.Count - 1)
                    {
                        onPlay = onPlay + 1;
                    }
                    else
                    {
                        onPlay = 0;
                    }
                    Load_Song(listAllSong[onPlay]);
                    PlaySong();
                    MenuListAllSong.SelectedIndex = onPlay;
                    break;
                case SystemMediaTransportControlsButton.Previous:
                    Debug.WriteLine("previouse");
                    if (onPlay > 0)
                    {
                        onPlay = onPlay - 1;
                    }
                    else
                    {
                        onPlay = listAllSong.Count - 1;
                    }
                    Load_Song(listAllSong[onPlay]);
                    PlaySong();
                    MenuListAllSong.SelectedIndex = onPlay;
                    break;
            }
        }

        private async void Get_All_Song() {
            this.listAllSong = new ObservableCollection<Song>();
            var response = await APIHandle.Get_List_Songs();
            var responseContent = await response.Content.ReadAsStringAsync();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var array = JArray.Parse(responseContent);
                foreach (var obj in array)
                {
                    Song song = obj.ToObject<Song>();
                    this.listAllSong.Add(song);
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
            this.MediaPlayer.Source = MediaSource.CreateFromUri(new Uri(currentSong.link));
        }

        private void PlaySong()
        {
            this.MediaPlayer.MediaPlayer.Play();
            isPlaying = true;
        }

        public bool Validate_Song()
        {
            var val = true;
            var nameText = this.Name.Text;
            var descriptionText = this.Description.Text;
            var singerText = this.Singer.Text;
            var authorText = this.Author.Text;
            var linkText = this.Link.Text;
            var thumbnailText = this.Thumbnail.Text;

            if (nameText == "")
            {
                this.name.Text = "You have to fill in this blank.";
                val = false;
            }
            else
            {
                this.name.Text = "";
            }

            if (descriptionText == "")
            {
                this.description.Text = "You have to fill in this blank.";
                val = false;
            }
            else
            {
                this.description.Text = "";
            }

            if (singerText == "")
            {
                this.singer.Text = "You have to fill in this blank.";
                val = false;
            }
            else
            {
                this.singer.Text = "";
            }

            if (authorText == "")
            {
                this.author.Text = "You have to fill in this blank.";
                val = false;
            }
            else
            {
                this.author.Text = "";
            }

            if (linkText == "")
            {
                this.link.Text = "You have to fill in this blank.";
                val = false;
            }
            else
            {
                this.link.Text = "";
            }

            if (thumbnailText == "")
            {
                this.thumbnail.Text = "You have to fill in this blank.";
                val = false;
            }
            else
            {
                this.thumbnail.Text = "";
            }

            return val;
        }

        private async void BtnCreateSong_Click(object sender, RoutedEventArgs e)
        {
            if (Validate_Song() == true)
            {
                this.currentSong.name = this.Name.Text;
                this.currentSong.description = this.Description.Text;
                this.currentSong.singer = this.Singer.Text;
                this.currentSong.author = this.Author.Text;
                this.currentSong.thumbnail = this.Thumbnail.Text;
                this.currentSong.link = this.Link.Text;

                var response = await APIHandle.Create_Song(this.currentSong);
                var responseContent = await response.Content.ReadAsStringAsync();
                if (response.StatusCode == HttpStatusCode.Created)
                {
                    var dialog = new ContentDialog()
                    {
                        Title = "Success!",
                        Width = 250,
                        MaxWidth = this.ActualWidth,
                        Content = "You have created a song",
                        CloseButtonText = "I know!"
                    };
                    var result = await dialog.ShowAsync();

                    var rootFrame = Window.Current.Content as Frame;
                }
                else
                {
                    ErrorResponse errorObject = JsonConvert.DeserializeObject<ErrorResponse>(responseContent);
                    if (errorObject != null && errorObject.error.Count > 0)
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
        }

        private void StackPanel_Tap(object sender, TappedRoutedEventArgs e)
        {
            StackPanel panel = sender as StackPanel;
            Song selectedSong = panel.Tag as Song;
            onPlay = MenuListAllSong.SelectedIndex;
            Load_Song(selectedSong);
            PlaySong();
        }

        private static T FindParent<T>(DependencyObject dependencyObject) where T : DependencyObject
        {
            var parent = VisualTreeHelper.GetParent(dependencyObject);

            if (parent == null) return null;

            var parentT = parent as T;
            return parentT ?? FindParent<T>(parent);
        }

        
    }
}
