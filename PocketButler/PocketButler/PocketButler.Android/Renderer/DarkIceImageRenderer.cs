using System;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using PocketButler;
using PocketButler.Droid;
using Android.Widget;
using PocketButler.Controls;
using Android.Graphics;
using Android.Graphics.Drawables;
using PocketButler.Droid.Renderer;

[assembly: ExportRenderer(typeof(DarkIceImage), typeof(DarkIceImageRenderer))]
namespace PocketButler.Droid.Renderer
{
    public class DarkIceImageRenderer : ImageRenderer
    {
        bool _disposed;
		DarkIceImage darkIceImage;
		static long lastEventTickTime = 0;
        public DarkIceImageRenderer()
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Image> e)
        {
            base.OnElementChanged(e);

            darkIceImage = Element as DarkIceImage;

            // Get a reference to the native control
            var nativeImage = (ImageView)Control;
			nativeImage.Clickable = !darkIceImage.IsDefaultImage;

			float imgPadding = 10;
			float fDensity = base.Context.Resources.DisplayMetrics.Density;
			int Padding = (int)(imgPadding * fDensity);

			if (darkIceImage.IsEnablePadding == true)
				nativeImage.SetPadding (Padding, Padding, Padding, Padding);

			if (darkIceImage.IsDefaultImage == false) {
				nativeImage.Click += (object sender, EventArgs e1) => {
					long curTickTime = DateTime.Now.Ticks / 1000 / 1000;
					if (Math.Abs (curTickTime - lastEventTickTime) < 3)
						return;
					lastEventTickTime = curTickTime;

					if (darkIceImage.DeleteDelegate != null) {
						darkIceImage.DeleteDelegate.Invoke (darkIceImage.RestaurantId, darkIceImage.MenuId, darkIceImage.IsShortCut, darkIceImage.IsShortCutList);
					} else {
						if (darkIceImage.Tapped != null)
							darkIceImage.Tapped.Invoke ();
					}
				};
			}

			if (darkIceImage.IsShowBorder == true)
				nativeImage.SetPadding (5, 5, 5, 5);

			if (darkIceImage.IsSwipeAction == true)
				nativeImage.Touch += TouchMeImageViewOnTouch;

            /*TappedRecognizer = new UITapGestureRecognizer();
            TappedRecognizer.AddTarget(() =>
            {
                Console.WriteLine("Dark Ice Image Tapped");

                // TODO: Implement parental gate

                // Invoke tapped event if it's been set
                if (imageElement.Tapped != null)
                    imageElement.Tapped.Invoke();

                // TODO: Add in event handler for the common code

            });


            nativeImage.AddGestureRecognizer(TappedRecognizer);*/


            //			nativeImage.DisposeIfNotNull ();
            Console.WriteLine("OnElementChanged");
        }

		private void TouchMeImageViewOnTouch(object sender, Android.Views.View.TouchEventArgs touchEventArgs)
		{
			string message;
			switch (touchEventArgs.Event.Action)// & MotionEventArgs.Mask)
			{
			case Android.Views.MotionEventActions.Down:
			case Android.Views.MotionEventActions.Move:
				message = "Touch Begins";
				break;

			case Android.Views.MotionEventActions.Up:
				message = "Touch Ends";
				if (darkIceImage.Tapped != null)
					darkIceImage.Tapped.Invoke();
				break;

			default:
				message = string.Empty;
				break;
			}

			Console.WriteLine (message);
		}

        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);


            switch (e.PropertyName)
            {

            case "ShouldDisposeImage":
                {
                    Dispose(true);
                }
                break;
			case "WidthRequestImage":
				{
					var darkIceImage = Element as DarkIceImage;
					Element.WidthRequest = darkIceImage.WidthRequestImage;
				}
				break;
            default:
                //System.Diagnostics.Debug.WriteLine ("Property change for {0} has not been implemented.", e.PropertyName);
                break;
           }

        }

        #region IDisposable implementation

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~DarkIceImageRenderer()
        {
            Dispose(false);
        }

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                Console.WriteLine("Disposing Image: {0}", disposing);
                // free other managed objects that implement
                // IDisposable only

                var nativeImage = (ImageView)Control;

                // TODO: Call recycle here on bitmap to clear memory properly
                nativeImage.Dispose();
                //nativeImage.RecycleBitmap();
                //nativeImage.DisposeIfNotNull();
                nativeImage = null;
            }

            // release any unmanaged objects
            // set the object references to null



            _disposed = true;
        }

        #endregion

        // TODO: Implement this for all custom image view controls

    }
}