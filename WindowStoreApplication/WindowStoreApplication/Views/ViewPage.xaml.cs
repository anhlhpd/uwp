using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
    public sealed partial class ViewPage : Page
    {
        public ObservableCollection<String> listFile = new ObservableCollection<String>();
        public ViewPage()
        {
            this.InitializeComponent();
            Page_Load();
        }
        
        private async void Page_Load()
        {
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            IReadOnlyList<StorageFile> fileList = await folder.GetFilesAsync();
            foreach (StorageFile file in fileList)
            {
                listFile.Add(file.Name);
            }
        }

        private async void Show_Content(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboxBox = (ComboBox)sender;
            string fileName = comboxBox.SelectedValue.ToString();
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            var file = await folder.GetFileAsync(fileName);
            this.Content.Text = await FileIO.ReadTextAsync(file);
        }
    }
}
