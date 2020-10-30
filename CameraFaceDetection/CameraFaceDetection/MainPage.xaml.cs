using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace CameraFaceDetection
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {   
            InitializeComponent();
        }

        private async void Open_Camera(object sender, EventArgs e)
        {
            var PhotoRequeststatus = await Permissions.RequestAsync<Permissions.Camera>();
            var StorageRequStatus = await Permissions.RequestAsync<Permissions.StorageWrite>();
            if(PhotoRequeststatus != Xamarin.Essentials.PermissionStatus.Granted || StorageRequStatus != Xamarin.Essentials.PermissionStatus.Granted)
            {
                await DisplayAlert("Enable Permission", "Please allow camera permission", "Close");
            }
            else
            {
   
                await Navigation.PushModalAsync(new CameraPage());

            }


        }
    }
}
