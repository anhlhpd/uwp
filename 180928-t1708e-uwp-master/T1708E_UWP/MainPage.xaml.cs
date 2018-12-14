using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Newtonsoft.Json;
using T1708E_UWP.Entity;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace T1708E_UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private Member currentMember;

        public MainPage()
        {
            this.currentMember = new Member();
            this.InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            //            Member member = new Member
            //            {
            //                firtName = "Hung",
            //                lastName = "Dao",
            //                avatar = "http://img.com",
            //                phone = "09123121212",
            //                address = "Hanoi",
            //                introduction = "Yolo",
            //                gender = 1,
            //                email = "xuanhung2401@gmail.com",
            //                password = "12"
            //            };
            //            string jsonObject = JsonConvert.SerializeObject(member);
            //            Member m1 = JsonConvert.DeserializeObject<Member>(jsonObject);
            //            HttpClient httpClient = new HttpClient();
            ////            
            ////            Debug.WriteLine(JsonConvert.SerializeObject(member));
            //            var content = new StringContent(JsonConvert.SerializeObject(member), System.Text.Encoding.UTF8, "application/json");
            //            var result  = httpClient.PostAsync("http://backup-server-002.appspot.com/member/register", content).Result;
            ////            Debug.WriteLine(result.Content.ToString());
        }

        private void Handle_Signup(object sender, RoutedEventArgs e)
        {
            string _address = string.Empty;
            string _introduction = string.Empty;
            this.Address.Document.GetText(Windows.UI.Text.TextGetOptions.AdjustCrlf, out _address);
            this.Introduction.Document.GetText(Windows.UI.Text.TextGetOptions.AdjustCrlf, out _introduction);
            Member member = new Member
            {
                firtName = this.FirstName.Text,
                lastName = this.LastName.Text,
                email = this.Email.Text,
                password = this.Password.Password.ToString(),
                avatar = this.ImageUrl.Text,
                phone = this.Phone.Text,
                address = _address,
                introduction = _introduction,
                gender = (int) Gender.SelectedValue,
                birthday = "1999-12-19"
            };
            HttpClient httpClient = new HttpClient();
            var content = new StringContent(JsonConvert.SerializeObject(member), System.Text.Encoding.UTF8, "application/json");
            var result = httpClient.PostAsync("http://1-dot-backup-server-002.appspot.com/member/register", content).Result;
            Debug.WriteLine(JsonConvert.SerializeObject(member));
        }

        private async void Capture_Photo(object sender, RoutedEventArgs e)
        {
            CameraCaptureUI captureUI = new CameraCaptureUI();
            captureUI.PhotoSettings.Format = CameraCaptureUIPhotoFormat.Jpeg;
            captureUI.PhotoSettings.CroppedSizeInPixels = new Size(200, 200);
            StorageFile photo = await captureUI.CaptureFileAsync(CameraCaptureUIMode.Photo);

            if (photo == null)
            {
                // User cancelled photo capture
                return;
            }
        }

        private void Change_Birthday(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            this.currentMember.birthDay = sender.Date.Value.ToString("yyyy-MM-dd");
        }
    }
}
