using System;
using MonoTouch.UIKit;
using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms;
using PocketButler.Controls;
using PocketButler.iOS;

[assembly: ExportRenderer (typeof(DarkIceImage), typeof(DarkIceImageRenderer))]

namespace PocketButler.iOS
{
	public class DarkIceImageRenderer : ImageRenderer
	{
		bool _disposed;

		UITapGestureRecognizer TappedRecognizer;

		public DarkIceImageRenderer ()
		{
		}

		protected override void OnElementChanged (ElementChangedEventArgs<Image> e)
		{
			base.OnElementChanged (e);

			var imageElement = Element as DarkIceImage;

			// Get a reference to the native control
			var nativeImage = (UIImageView)Control; 
			nativeImage.UserInteractionEnabled = true;

			// TODO: Implement animated glow
			//			var glow = new AnimatedGlow.Binding.AnimatedGlow(new RectangleF(0,0,200,200));
			//			this.Add (glow);
			//
			TappedRecognizer = new UITapGestureRecognizer ();
			TappedRecognizer.AddTarget (() => {
				Console.WriteLine ("Dark Ice Image Tapped");

				// TODO: Implement parental gate

				// Invoke tapped event if it's been set
				if (imageElement.Tapped != null)
					imageElement.Tapped.Invoke ();

				// TODO: Add in event handler for the common code

			});

			nativeImage.AddGestureRecognizer (TappedRecognizer);
		}

		protected override void OnElementPropertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged (sender, e);

			switch (e.PropertyName) {

			case "ShouldDisposeImage":
				{
					Dispose (true);
				}
				break;
			default:
				//System.Diagnostics.Debug.WriteLine ("Property change for {0} has not been implemented.", e.PropertyName);
				break;
			}

		}

		#region IDisposable implementation

		protected override void Dispose (bool disposing)
		{
			if (_disposed)
				return;

			// Ensure UI operations invoked on main thread
			InvokeOnMainThread (() => {

				var nativeImage = (UIImageView)Control; 

				if (disposing) {
					Console.WriteLine ("Disposing Image: {0}", disposing);
					// free other managed objects that implement
					// IDisposable only

					nativeImage.RemoveGestureRecognizer(TappedRecognizer);
					//if (nativeImage != null)
					//	nativeImage.Image.DisposeIfNotNull ();

					Control.Dispose();
					//nativeImage.DisposeIfNotNull ();
				}

				// release any unmanaged objects
				// set the object references to null

				// Brute force, remove everything
				foreach (var view in Subviews)
					view.RemoveFromSuperview ();

				if (nativeImage != null)
					nativeImage.Image = null;


			});

			_disposed = true;
		}

		#endregion
	}
}

