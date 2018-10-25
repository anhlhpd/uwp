using MusicBox.Entity;
using MusicBox.Service;
using MusicBox.Views;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Data.Json;
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
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MusicBox
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            Check_Member_Info();
            this.InitializeComponent();
        }
        public async void Load_Page()
        {
            Login login = new Login();
            await login.ShowAsync();
        }

        public async void Check_Member_Info()
        {
            var file = await ApplicationData.Current.LocalFolder.TryGetItemAsync("token.txt");
            if (file != null)
            {
                var response = await APIHandle.Get_Member_Infor();
                var responseContent = await response.Content.ReadAsStringAsync();
                
                if (response.StatusCode == HttpStatusCode.Created)
                {
                    await Task.Delay(5000); 
                    this.Frame.Navigate(typeof(Views.NavigationView));
                }
                else
                {
                    Load_Page();
                }                
            }
            else
            {
                Load_Page();
            }
        }
    }
}
