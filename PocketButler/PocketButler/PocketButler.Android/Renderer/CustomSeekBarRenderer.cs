using Android.Views;
using PocketButler;
using PocketButler.Droid.Renderer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using System.Threading;
using Android.App;
using Android.Content;
using Java.Util;
using Android.Widget;
using Android.Graphics.Drawables;
using Android.Content.Res;

[assembly: ExportRenderer(typeof(CustomSeekBar), typeof(CustomSeekBarRenderer))]
namespace PocketButler.Droid.Renderer
{
	public class CustomSeekBarRenderer : ViewRenderer
    {
		CustomSeekBar MainView;
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.View> e)
		{
			base.OnElementChanged(e);

			var view = (CustomSeekBar)Element;
			MainView = view;

			SeekBar seekBar = new SeekBar (base.Context);
			seekBar.Max = MainView.MaxValue - MainView.MinValue;
			seekBar.Progress = MainView.SelectedValue > seekBar.Max ? seekBar.Max : MainView.SelectedValue;

			try{

				seekBar.ProgressDrawable = Resources.GetDrawable(Resource.Drawable.red_scrubber_progress);
				seekBar.SetThumb(Resources.GetDrawable (Resource.Drawable.red_scrubber_control));
			}
			catch (Exception ex) {
			}

			LayoutParams layout_params = new LayoutParams(MarginLayoutParams.MatchParent, MarginLayoutParams.MatchParent);
			seekBar.LayoutParameters = layout_params;

			seekBar.ProgressChanged += (object sender, SeekBar.ProgressChangedEventArgs e1) => {
				if (MainView.ValueChangedEvent != null)
				{
					MainView.SelectedValue = MainView.MinValue + e1.Progress;
					MainView.ValueChangedEvent.Invoke();
				}
			};

			this.SetNativeControl (seekBar);
		}
    }
}
