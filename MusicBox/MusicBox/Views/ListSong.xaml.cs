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
        private ObservableCollection<Song> list;

        internal ObservableCollection<Song> List { get => list; set => list = value; }
        
        public ListSong()
        { 
            Get_Songs();
            this.InitializeComponent();
        }

        private async void Get_Songs() {
            this.list = new ObservableCollection<Song>();
            var response = await APIHandle.Get_List_Songs();
            var responseContent = await response.Content.ReadAsStringAsync();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                Debug.WriteLine("Success");
                var array = JArray.Parse(responseContent);
                Debug.WriteLine(array);
                foreach (var obj in array)
                {
                    Song song = obj.ToObject<Song>();
                    this.list.Add(song);
                    Debug.WriteLine(song.name);
                }
            }
            else
            {
                var dialog = new ContentDialog()
                {
                    Title = "Error!",
                    MaxWidth = this.ActualWidth,
                    Content = "There's an error! Please try later!",
                    CloseButtonText = "I know!"
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
    }
}
