using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Hardware;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace CameraFaceDetection.Droid.CustomRender
{
	public class CameraPreview : ViewGroup, ISurfaceHolderCallback
	{
		public SurfaceView surfaceView;
		public ISurfaceHolder holder;
		Camera.Size previewSize;
		IList<Camera.Size> supportedPreviewSizes;
		public Camera camera;
		IWindowManager windowManager;
		Android.Graphics.Bitmap bitmap;

		private Context mContext;
		public int CameraID { get; set; }
		public bool IsPreviewing { get; set; }

		public Camera Preview
		{
			get { return camera; }
			set
			{
				camera = value;
				if (camera != null)
				{
					supportedPreviewSizes = Preview.GetParameters().SupportedPreviewSizes;
					RequestLayout();
				}
			}
		}

		public object MediaFilename { get; internal set; }

		public CameraPreview(Context context) : base(context)
		{
			try
			{
				mContext = context;
				surfaceView = new SurfaceView(context);
				AddView(surfaceView);
				windowManager = Context.GetSystemService(Context.WindowService).JavaCast<IWindowManager>();
				IsPreviewing = false;
				holder = surfaceView.Holder;
				holder.AddCallback(this);
			}
			catch (Exception ex)
			{
				
				return;
			}

		}
		protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
		{
			try
			{

				int width = ResolveSize(SuggestedMinimumWidth, widthMeasureSpec);
				int height = ResolveSize(SuggestedMinimumHeight, heightMeasureSpec);
				SetMeasuredDimension(width, height);

				if (supportedPreviewSizes != null)
				{
					previewSize = GetOptimalPreviewSize(supportedPreviewSizes, width, height);
				}

			}
			catch (Exception ex)
			{
				
			}
		}

		protected override void OnLayout(bool changed, int l, int t, int r, int b)
		{
			try
			{
				var msw = MeasureSpec.MakeMeasureSpec(r - l, MeasureSpecMode.Exactly);
				var msh = MeasureSpec.MakeMeasureSpec(b - t, MeasureSpecMode.Exactly);
				surfaceView.Measure(msw, msh);
				surfaceView.Layout(0, 0, r - l, b - t);
			}
			catch (Exception ex)
			{
				
			}


		}

		public void SurfaceCreated(ISurfaceHolder holder)
		{
			try
			{
				if (Preview != null)
				{
					Preview.SetPreviewDisplay(holder);
				}
			}
			catch (Exception ex)
			{
				
			}
		}

		public void SurfaceDestroyed(ISurfaceHolder holder)
		{
			try
			{
				if (Preview != null)
				{
					Preview.StopPreview();
					holder = null;
				}
			}
			catch (Exception ex)
			{
				
			}


		}


		public void SurfaceChanged(ISurfaceHolder holder, Android.Graphics.Format format, int width, int height)
		{
			try
			{

				var parameters = Preview.GetParameters();
				parameters.SetPreviewSize(previewSize.Width, previewSize.Height);
				RequestLayout();
				switch (windowManager.DefaultDisplay.Rotation)
				{
					case SurfaceOrientation.Rotation0:
						camera.SetDisplayOrientation(90);
						break;
					case SurfaceOrientation.Rotation90:
						camera.SetDisplayOrientation(0);
						break;
					case SurfaceOrientation.Rotation270:
						camera.SetDisplayOrientation(180);
						break;
				}

				Preview.SetParameters(parameters);
				Preview.StartPreview();
				IsPreviewing = true;
			}
			catch (Exception ex)
			{
				
			}
		}



		Camera.Size GetOptimalPreviewSize(IList<Camera.Size> sizes, int w, int h)
		{
			const double AspectTolerance = 0.1;
			double targetRatio = (double)h / w;

			if (sizes == null)
			{
				return null;
			}

			Camera.Size optimalSize = null;
			double minDiff = double.MaxValue;

			int targetHeight = h;
			foreach (Camera.Size size in sizes)
			{
				double ratio = (double)size.Height / size.Width;

				if (Math.Abs(ratio - targetRatio) > AspectTolerance)
					continue;
				if (Math.Abs(size.Height - targetHeight) < minDiff)
				{
					optimalSize = size;
					minDiff = Math.Abs(size.Height - targetHeight);
				}
			}

			if (optimalSize == null)
			{
				minDiff = double.MaxValue;
				foreach (Camera.Size size in sizes)
				{
					if (Math.Abs(size.Height - targetHeight) < minDiff)
					{
						optimalSize = size;
						minDiff = Math.Abs(size.Height - targetHeight);
					}
				}
			}

			return optimalSize;
		}

	}
}