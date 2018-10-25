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
using System.Threading.Tasks;
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
    public sealed partial class MyAccount : Page
    {
        public MyAccount()
        {
            this.InitializeComponent();
            Get_Infor();
        }

        public async void Get_Infor()
        {
            var file = await ApplicationData.Current.LocalFolder.TryGetItemAsync("token.txt");
            if (file != null)
            {
                var response = await APIHandle.Get_Member_Infor();
                var responseContent = await response.Content.ReadAsStringAsync();
                Member member = JsonConvert.DeserializeObject<Member>(responseContent);
                this.Email.Text = member.email;
                this.FirstName.Text = member.firstName;
                this.LastName.Text = member.lastName;
                this.Phone.Text = member.phone;
                this.Address.Text = member.address;
                this.Gender.SelectedValue = member.gender.ToString();
                Debug.WriteLine(member.gender);
            }
        }

        private async void BtnLogOut_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog deleteFileDialog = new ContentDialog
            {
                Title = "Warning!",
                Content = "Do you want to log out?",
                PrimaryButtonText = "Log out",
                CloseButtonText = "Cancel"
            };

            ContentDialogResult result = await deleteFileDialog.ShowAsync();

            // Delete the file if the user clicked the primary button.
            /// Otherwise, do nothing.
            if (result == ContentDialogResult.Primary)
            {
                StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
                StorageFile file = await storageFolder.GetFileAsync("token.txt");
                await file.DeleteAsync(StorageDeleteOption.PermanentDelete);

                var rootFrame = Window.Current.Content as Frame;
                rootFrame.Navigate(typeof(MainPage), null, new EntranceNavigationTransitionInfo());
            }
            else
            {
                // The user clicked the CLoseButton, pressed ESC, Gamepad B, or the system back button.
                // Do nothing.
            }            
        }
    }
}
