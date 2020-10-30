using CameraFaceDetection.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CameraFaceDetection
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CameraPage : ContentPage
    {
        public CameraPage()
        {
            InitializeComponent();
        }

        protected async override void OnAppearing()
        {
            CameraPreview.IsVisible = true;
            CameraPreview.IsEnabled = true;

            base.OnAppearing();
            await MessegingCenterDetection();
            base.OnAppearing();
        }
        private async void Capture_Tapped(object sender, EventArgs e)
        {
            try
            {
                MessagingCenter.Send<Object>(new Object(), "CaptureClick");
                await CaptureButton.ScaleTo(0.8, 100, Easing.Linear);
                await CaptureButton.ScaleTo(1, 100, Easing.Linear);
            }
            catch (Exception)
            {

            }
        }


        private async Task MessegingCenterDetection()
        {
            //MessagingCenter.Subscribe<CameraPopup, string>(this, "PictureTaken", async (sender1, arg) =>
            MessagingCenter.Subscribe<MyMessage>(this, "PictureTaken", (value) =>
            {

                try
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        WarningFrame.IsVisible = false;
                        switchCamera.IsEnabled = false;
                        CaptureButtonOuterFrame.IsEnabled = false;
                        CaptureButtonOuterFrame.ScaleTo(0, 400, Easing.Linear);
                        switchCamera.TranslateTo(120, 0, 400, Easing.Linear);                      
                        switchCamera.FadeTo(0, 10, Easing.Linear);                    
                        SuccessFrame.IsVisible = true;
                        SuccessFrame.Opacity = 0;
                        SuccessFrame.FadeTo(1, 400, Easing.Linear);
                        SuccessAnimationView.Play();
                    });

                }
                catch (Exception ex)
                {
                    return;
                }

            });




            try
            {

                MessagingCenter.Subscribe<MyMessage>(this, "PictureTakenFailed", (value) =>
                {

                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        //await DisplayAlert("", arg, "ok");
                        WarningFrame.Opacity = 0;
                        errorlabel.Text = value.Myvalue;
                        WarningFrame.IsVisible = true;
                        WarningFrame.FadeTo(1, 400, Easing.CubicIn);
                        await Task.Delay(4000);
                        WarningFrame.FadeTo(0, 100, Easing.CubicIn);
                    });
                });
            }

            catch (Exception)
            {

            }
        }



        private async void CameraFlip_Tapped(object sender, EventArgs e)
        {
            await switchCamera.ScaleTo(0.8, 100);
            await switchCamera.ScaleTo(1, 100);

            try
            {
                MessagingCenter.Send<object>(this, "FlipClick");
            }
            catch (Exception ex)
            {

            }
        }

        protected async override void OnDisappearing()
        {
            base.OnDisappearing();
            
        }


    }
}