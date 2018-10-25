using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace WindowStoreApplication.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddPage : Page
    {
        public AddPage()
        {
            this.InitializeComponent();
        }

        private async void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFile file =
                await folder.CreateFileAsync(this.Name.Text,
                    CreationCollisionOption.OpenIfExists);
            if (await ApplicationData.Current.LocalFolder.TryGetItemAsync(this.Name.Text) == null)
            {
                await FileIO.WriteTextAsync(file, this.Content.Text);
            }
            else
            {
                var list = new List<string>();
                list.Add(this.Content.Text);
                await FileIO.AppendLinesAsync(file, list);
            }
        }
    }
}
