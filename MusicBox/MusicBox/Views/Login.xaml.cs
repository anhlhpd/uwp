using MusicBox.Entity;
using MusicBox.Service;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Data.Json;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
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
    public sealed partial class Login : ContentDialog
    {
        private static string API_REGISTER = "https://2-dot-backup-server-002.appspot.com/_api/v2/members";

        public Login()
        {
            this.InitializeComponent();
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            var rootFrame = Window.Current.Content as Frame;
            this.Hide();
            rootFrame.Navigate(typeof(Register), null, new EntranceNavigationTransitionInfo());
        }
        
        private async void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            var val = true;
            var emailText = this.Email.Text;
            var passwordText = this.Password.Password;
            if (emailText == "")
            {
                this.email.Foreground = new SolidColorBrush(Windows.UI.Colors.Red);
                this.email.Text = "Email can't be empty";
                val = false;
            }
            else
            {
                this.email.Text = "";
            }
            if (passwordText == "")
            {
                this.password.Text = "Password can't be empty";
                this.password.Foreground = new SolidColorBrush(Windows.UI.Colors.Red);
                val = false;
            }
            else
            {
                this.password.Text = "";
            }
            
            if (val == true)
            {
                var response = await APIHandle.Sign_In(emailText, passwordText);
                var responseContent = await response.Content.ReadAsStringAsync();
                if (response.StatusCode == HttpStatusCode.Created)
                {
                    TokenResponse token = JsonConvert.DeserializeObject<TokenResponse>(responseContent);

                    StorageFolder folder = ApplicationData.Current.LocalFolder;
                    StorageFile file = await folder.CreateFileAsync("token.txt", CreationCollisionOption.ReplaceExisting);
                    await FileIO.WriteTextAsync(file, responseContent);

                    this.Hide();
                    var rootFrame = Window.Current.Content as Frame;
                    rootFrame.Navigate(typeof(ListSong), null, new EntranceNavigationTransitionInfo());
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
}
