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
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace MusicBox.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Register : Page
    {
        private Member currentMember;
        private static StorageFile file;
        private static string UploadUrl;

        public Register()
        {
            GetUploadUrl();
            this.currentMember = new Member();
            this.InitializeComponent();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            var rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(typeof(MainPage));
        }

        public bool Validate_Register()
        {
            var val = true;
            var emailText = this.Email.Text;
            var passwordText = this.Password.Password;
            var firstNameText = this.FirstName.Text;
            var addressText = this.Address.Text;
            var phoneText = this.Phone.Text;
            var avatarText = this.ImageUrl.Text;

            if (emailText == "")
            {
                this.email.Text = "You have to fill in this blank.";
                val = false;
            }
            else
            {
                this.email.Text = "";
            }

            if (passwordText == "")
            {
                this.password.Text = "You have to fill in this blank.";
                val = false;
            }
            else
            {
                this.password.Text = "";
            }

            if (firstNameText == "")
            {
                this.firstName.Text = "You have to fill in this blank.";
                val = false;
            }
            else
            {
                this.firstName.Text = "";
            }

            if (addressText == "")
            {
                this.address.Text = "You have to fill in this blank.";
                val = false;
            }
            else
            {
                this.address.Text = "";
            }

            if (phoneText == "")
            {
                this.phone.Text = "You have to fill in this blank.";
                val = false;
            }
            else
            {
                this.phone.Text = "";
            }

            if (avatarText == "")
            {
                this.avatar.Text = "You have to fill in this blank.";
                val = false;
            }
            else
            {
                this.avatar.Text = "";
            }

            return val;
        }

        private async void Handle_Signup(object sender, RoutedEventArgs e)
        {
            if (Validate_Register())
            {
                this.currentMember.firstName = this.FirstName.Text;
                this.currentMember.lastName = this.LastName.Text;
                this.currentMember.email = this.Email.Text;
                this.currentMember.password = this.Password.Password.ToString();
                this.currentMember.avatar = this.ImageUrl.Text;
                this.currentMember.phone = this.Phone.Text;
                this.currentMember.address = this.Address.Text;
                this.currentMember.introduction = this.Introduction.Text;

                var response = await APIHandle.Sign_Up(this.currentMember);
                var responseContent = await response.Content.ReadAsStringAsync();
                if (response.StatusCode == HttpStatusCode.Created)
                {
                    var rootFrame = Window.Current.Content as Frame;
                    rootFrame.Navigate(typeof(MainPage), null, new EntranceNavigationTransitionInfo());
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
                            Debug.WriteLine(errorObject.error[key]);
                            textBlock.Visibility = Visibility.Visible;
                        }
                    }
                }
            }
        }

        private async void Capture_Photo(object sender, RoutedEventArgs e)
        {
            CameraCaptureUI captureUI = new CameraCaptureUI();
            captureUI.PhotoSettings.Format = CameraCaptureUIPhotoFormat.Jpeg;
            captureUI.PhotoSettings.CroppedSizeInPixels = new Size(200, 200);
            file = await captureUI.CaptureFileAsync(CameraCaptureUIMode.Photo);
            if (file == null)
            {
                // User cancelled photo capture
                return;
            }
            HttpUploadFile(UploadUrl, "myFile", "image/png");
        }

        private static async void GetUploadUrl()
        {
            Windows.Web.Http.HttpClient httpClient = new Windows.Web.Http.HttpClient();
            Uri requestUri = new Uri("https://2-dot-backup-server-002.appspot.com/get-upload-token");
            Windows.Web.Http.HttpResponseMessage httpResponse = new Windows.Web.Http.HttpResponseMessage();
            string httpResponseBody = "";
            try
            {
                httpResponse = await httpClient.GetAsync(requestUri);
                httpResponse.EnsureSuccessStatusCode();
                httpResponseBody = await httpResponse.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                httpResponseBody = "Error: " + ex.HResult.ToString("X") + " Message: " + ex.Message;
            }
            Debug.WriteLine(httpResponseBody);
            UploadUrl = httpResponseBody;
        }

        public async void HttpUploadFile(string url, string paramName, string contentType)
        {
            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(url);
            wr.ContentType = "multipart/form-data; boundary=" + boundary;
            wr.Method = "POST";

            Stream rs = await wr.GetRequestStreamAsync();
            rs.Write(boundarybytes, 0, boundarybytes.Length);

            string header = string.Format("Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n", paramName, "path_file", contentType);
            byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
            rs.Write(headerbytes, 0, headerbytes.Length);

            // write file.
            Stream fileStream = await file.OpenStreamForReadAsync();
            byte[] buffer = new byte[4096];
            int bytesRead = 0;
            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                rs.Write(buffer, 0, bytesRead);
            }

            byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
            rs.Write(trailer, 0, trailer.Length);

            WebResponse wresp = null;
            try
            {
                wresp = await wr.GetResponseAsync();
                Stream stream2 = wresp.GetResponseStream();
                StreamReader reader2 = new StreamReader(stream2);
                //Debug.WriteLine(string.Format("File uploaded, server response is: @{0}@", reader2.ReadToEnd()));
                //string imgUrl = reader2.ReadToEnd();
                Uri u = new Uri(reader2.ReadToEnd(), UriKind.Absolute);
                Debug.WriteLine(u.AbsoluteUri);
                ImageUrl.Text = u.AbsoluteUri;
                MyAvatar.Source = new BitmapImage(u);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error uploading file", ex.StackTrace);
                Debug.WriteLine("Error uploading file", ex.InnerException);
                if (wresp != null)
                {
                    wresp = null;
                }
            }
            finally
            {
                wr = null;
            }
        }

        private void Select_Gender(object sender, RoutedEventArgs e)
        {
            RadioButton radioGender = sender as RadioButton;
            this.currentMember.gender = Int32.Parse(radioGender.Tag.ToString());
            Debug.WriteLine(this.currentMember.gender);
        }

        private void Change_Birthday(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            this.currentMember.birthday = sender.Date.Value.ToString("yyyy-MM-dd");
        }

        //private async void Choose_File(object sender, RoutedEventArgs e)
        //{
        //    var picker = new Windows.Storage.Pickers.FileOpenPicker();
        //    picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
        //    picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
        //    picker.FileTypeFilter.Add(".jpg");
        //    picker.FileTypeFilter.Add(".jpeg");
        //    picker.FileTypeFilter.Add(".png");
            
        //    StorageFile file = await picker.PickSingleFileAsync();
        //    if (file != null)
        //    {
        //        this.ChooseFile.Text = file.Name;
        //    }
        //    else
        //    {
        //        this.ChooseFile.Text = "Can't open the picture.";
        //    }
        //}
    }
}
