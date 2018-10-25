using System;
using System.Collections.Generic;
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
        public ViewPage()
        {
            this.InitializeComponent();
            Page_Load();
        }
        
        private async void Page_Load()
        {
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            var files = await folder.GetFilesAsync();
            IReadOnlyList<StorageFile> fileList = await folder.GetFilesAsync();
            foreach (StorageFile file in fileList)
            {
                Debug.WriteLine(file.Name + "\n");
            }
            Debug.WriteLine(files);
        }
    }
}
