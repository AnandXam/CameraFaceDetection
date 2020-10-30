using System;
using System.Drawing;
using System.Threading.Tasks;
using Android.Content;
using Android.Hardware;
using Android.Media;
using Android.Util;
using CameraFaceDetection.Droid.CustomRender;
using CameraFaceDetection;
using CameraFaceDetection.CustomCamera;
using Java.IO;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using CameraFaceDetection.Model;


[assembly: ExportRenderer(typeof(CameraFaceDetection.CustomCamera.CameraPreview), typeof(CameraPreviewRenderer))]
namespace CameraFaceDetection.Droid.CustomRender
{
    public class CameraPreviewRenderer : ViewRenderer<CameraFaceDetection.CustomCamera.CameraPreview, CameraFaceDetection.Droid.CustomRender.CameraPreview>, Camera.IFaceDetectionListener, Camera.IPictureCallback, Camera.IShutterCallback
    {
        CameraPreview cameraPreview;
        String Picture_Name = "";
        private CameraFacing camerainfo = CameraFacing.Front;
        int DetectedFaceCount = 0;

        [get: Android.Runtime.Register("getMaxNumDetectedFaces", "()I", "GetGetMaxNumDetectedFacesHandler", ApiSince = 14)]
        public virtual int MaxNumDetectedFaces { get; }

        public CameraPreviewRenderer(Context context) : base(context)
        {
            MessagingCenter.Subscribe<Object>(this, "CaptureClick", async (sender) => {
                try
                {
                    Picture_Name = "facecapture_image" + ".jpg";
                    var CameraParaMeters = cameraPreview.camera.GetParameters();
                    if (CameraParaMeters.MaxNumDetectedFaces > 0)
                    {

                        if (DetectedFaceCount == 1)
                        {
                            Control.Preview.StopFaceDetection();
                            await Task.Run(() => takepicture());
                        }
                        else if (DetectedFaceCount == 0)
                        {
                            //MessagingCenter.Send<CameraPopup, string>(new CameraPopup(null,null,null), "PictureTakenFailed", "No Face Detected");
                            MessagingCenter.Send(new MyMessage() { Myvalue = "No Face Detected !" }, "PictureTakenFailed");
                        }
                        else if (DetectedFaceCount > 1)
                        {
                            // MessagingCenter.Send<CameraPopup, string>(new CameraPopup(null,null,null), "PictureTakenFailed", "Multiple face Detected");
                            MessagingCenter.Send(new MyMessage() { Myvalue = "Multiple faces Detected !" }, "PictureTakenFailed");
                        }
                    }
                    else
                    {
                        await Task.Run(() => takepicture());
                    }
                }
                catch (Exception ex)
                {
                   
                }
            });
        }

        [Obsolete]
        protected override void OnElementChanged(ElementChangedEventArgs<CameraFaceDetection.CustomCamera.CameraPreview> e)
        {
            try
            {
                base.OnElementChanged(e);
                if (Control == null)
                {
                    try
                    {
                        cameraPreview = new CameraPreview(Context);
                        SetNativeControl(cameraPreview);
                    }
                    catch (Exception ex)
                    {
                                             
                    }
                }
                if (e.OldElement != null)
                {
                }
                if (e.NewElement != null)
                {
                    try
                    {
                        if (Control == null)
                        {
                            cameraPreview = new CameraPreview(Context);
                            SetNativeControl(cameraPreview);
                        }
                        Control.Preview = Camera.Open((int)e.NewElement.Camera);
                        Control.CameraID = 1;

                        var CameraParaMeters = cameraPreview.camera.GetParameters();
                        if (CameraParaMeters != null)
                        {
                            if (CameraParaMeters.MaxNumDetectedFaces > 0)
                            {
                                Device.BeginInvokeOnMainThread(() =>
                                {
                                    Control.Preview.SetFaceDetectionListener(this);
                                    Control.Preview.StartFaceDetection();
                                });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                      
                    }

                    MessagingCenter.Subscribe<object>(this, "FlipClick", (sender) =>
                    {
                        try
                        {

                            if (camerainfo == Camera.CameraInfo.CameraFacingFront)
                            {
                                camerainfo = Camera.CameraInfo.CameraFacingBack;
                                e.NewElement.Camera = CameraOptions.Rear;
                                Control.CameraID = 0;
                            }
                            else
                            {
                                camerainfo = Camera.CameraInfo.CameraFacingFront;
                                e.NewElement.Camera = CameraOptions.Front;
                                Control.CameraID = 1;
                            }
                            cameraPreview.camera.Lock();
                            cameraPreview.camera.StopPreview();
                            cameraPreview.camera.Release();
                            cameraPreview.camera = null;
                            Control.Preview = Camera.Open((int)e.NewElement.Camera);
                            SetNativeControl(cameraPreview);

                            var CameraParaMeters = cameraPreview.camera.GetParameters();
                            if (CameraParaMeters.MaxNumDetectedFaces > 0)
                            {
                                Device.BeginInvokeOnMainThread(() =>
                                {
                                    Control.Preview.SetFaceDetectionListener(this);
                                    Control.Preview.StartFaceDetection();
                                });
                            }
                        }
                        catch (Exception ex)
                        {
                         
                        }
                    });
                }
            }
            catch (Exception ex)
            {
               
            }
            //  MessagingCenter.Unsubscribe<Object>(this, "CaptureClick");
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    Control.Preview.SetFaceDetectionListener(null);
                    Control.Preview.Release();
                    MessagingCenter.Unsubscribe<Object>(this, "CaptureClick");
                    MessagingCenter.Unsubscribe<Object>(this, "FlipClick");
                }
                //Device.BeginInvokeOnMainThread(base.Dispose);

            }
            catch (Exception ex)
            {
               
            }

        }

        [Obsolete]
        public void OnFaceDetection(Camera.Face[] faces, Camera camera)
        {
            try
            {
                DetectedFaceCount = faces.Length;
            }
            catch (Exception ex)
            {
                
            }

        }

        private void takepicture()
        {
            try
            {
                Control.Preview.TakePicture(this, this, this);
            }
            catch (Exception ex)
            {
              
            }
        }


        public void OnPictureTaken(byte[] data, Camera camera)
        {
            try
            {

                var path = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures);
                File pictureFile = new File(path, "/" + Picture_Name);
                if (pictureFile.Exists())
                {
                    pictureFile.Delete();
                }
                File NewPicture = new File(path, "/" + "face.JPG");
                if (NewPicture.Exists())
                {
                    NewPicture.Delete();
                }
                FileOutputStream file = null;
                FileOutputStream newfile = null;

                if (data != null)
                {
                    try
                    {
                        file = new FileOutputStream(pictureFile);
                        file.Write(data);
                        file.Flush();
                        System.IO.MemoryStream imageStream = new System.IO.MemoryStream(data);                       
                        var CurrentStatus = System.Convert.ToString(pictureFile.Path);
                        MessagingCenter.Send(new MyMessage() { Myvalue = CurrentStatus }, "PictureTaken");
                    }
                    catch (FileNotFoundException e)
                    {
                       
                    }
                    catch (IOException ie)
                    {
                       
                    }
                    finally
                    {
                        file?.Close();
                    }
                }
                File deleteFile = new File(pictureFile.Path);
                bool deleted = deleteFile.Delete();
            }
            catch (Exception ex)
            {
               
            }
        }
        public void OnShutter() { }

    }
}