using System;
using Xamarin.Forms.Platform.iOS;
using MonoTouch.UIKit;
using Xamarin.Forms;
using PocketButler.Controls;
using PocketButler.iOS;

[assembly: ExportRenderer (typeof(DILabel), typeof(DILabelRenderer))]

namespace PocketButler.iOS
{
	public class DILabelRenderer : LabelRenderer
	{
		bool _disposed;

		public DILabelRenderer ()
		{
		}

		protected override void OnElementPropertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged (sender, e);

			switch (e.PropertyName) {

			case "ShouldDispose":
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

				var nativeControl = (UILabel)Control; 

				if (disposing) {
					Console.WriteLine ("Disposing Label: {0}", disposing);
					// free other managed objects that implement
					// IDisposable only

					// Implement if adding tapped to this control
//					nativeControl.RemoveGestureRecognizer(TappedRecognizer);

					Control.Dispose();
				}

				// release any unmanaged objects
				// set the object references to null

				// Brute force, remove everything
				foreach (var view in Subviews)
					view.RemoveFromSuperview ();

				Control.RemoveFromSuperview();


			});

			_disposed = true;
		}

		#endregion
	}
}

