using Android.Views;
using PocketButler;
using PocketButler.Droid.Renderer;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CustomButton), typeof(CustomButtonRenderer))]

namespace PocketButler.Droid.Renderer
{
	public class CustomButtonRenderer : ButtonRenderer
	{
		protected override void OnElementChanged (ElementChangedEventArgs<Button> e) {
			base.OnElementChanged (e);
			Control.Focusable = false;
		}
	}
}

