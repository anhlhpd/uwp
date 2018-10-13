using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using MusicBox.Entity;
using MusicBox.Service;
using System.Diagnostics;
using System.Net;
using Newtonsoft.Json;
using Windows.Storage;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace MusicBox.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SongForm : Page
    {
        private Song currentSong;
        public SongForm()
        {
            this.InitializeComponent();
            this.currentSong = new Song();
            //this.Player.MediaPlayer.Play();
        }

        private async void BtnCreateSong_Click(object sender, RoutedEventArgs e)
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
                rootFrame.Navigate(typeof(ListSong));
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
}
