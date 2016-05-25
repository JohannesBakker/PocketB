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

[assembly: ExportRenderer(typeof(CustomEntry), typeof(CustomEntryRenderer))]
namespace PocketButler.Droid.Renderer
{
    public class CustomEntryRenderer : EntryRenderer
    {
		private bool _inititialized = false;
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
		{
			base.OnElementChanged(e);

            var view = (CustomEntry)Element;

            SetFont(view);
            SetTextAlignment(view);
            //SetBorder(view);
            SetPlaceholderTextColor(view);

			Control.SetPadding (10, 0, 10, 0);

			if (this.Control != null)
			{
				if(!_inititialized)
				{
					this.Control.FocusChange+=((sender, evt) => {
						if(evt.HasFocus)
						{
							ThreadPool.QueueUserWorkItem(s =>
								{
									Thread.Sleep(100); // For some reason, a short delay is required here.
									((Android.Views.InputMethods.InputMethodManager)Context.GetSystemService(Android.Content.Context.InputMethodService)).ShowSoftInput(this.Control, Android.Views.InputMethods.ShowFlags.Implicit);
								});
						}
					});
					_inititialized = true;
				}
			}
		}

	    protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

            var view = (CustomEntry)Element;

            if (e.PropertyName == CustomEntry.FontProperty.PropertyName)
                SetFont(view);
            if (e.PropertyName == CustomEntry.XAlignProperty.PropertyName || e.PropertyName == CustomEntry.YAlignProperty.PropertyName)
                SetTextAlignment(view);
            //if (e.PropertyName == CustomEntry.HasBorderProperty.PropertyName)
            //    SetBorder(view);
            if (e.PropertyName == CustomEntry.PlaceholderTextColorProperty.PropertyName)
                SetPlaceholderTextColor(view);

			if (e.PropertyName == CustomEntry.HasBackgroundProperty.PropertyName)
				Control.SetBackgroundResource (Resource.Drawable.quantity);

			Control.SetBackgroundColor(Android.Graphics.Color.Transparent);
		}

        private void SetBorder(CustomEntry view)
	    {
            //NotCurrentlySupported: HasBorder peroperty not suported on Android
	    }

        private void SetTextAlignment(CustomEntry view)
	    {
            switch (view.XAlign)
            {
                case Xamarin.Forms.TextAlignment.Center:
                    Control.Gravity = GravityFlags.CenterHorizontal;
                    break;
                case Xamarin.Forms.TextAlignment.End:
                    Control.Gravity = GravityFlags.End;
                    break;
                case Xamarin.Forms.TextAlignment.Start:
                    Control.Gravity = GravityFlags.Start;
                    break;
            }
            switch (view.YAlign)
            {
                case Xamarin.Forms.TextAlignment.Center:
                    Control.Gravity = GravityFlags.CenterVertical;
                    break;
                case Xamarin.Forms.TextAlignment.End:
                    Control.Gravity = GravityFlags.End;
                    break;
                case Xamarin.Forms.TextAlignment.Start:
                    Control.Gravity = GravityFlags.Start;
                    break;
            }
        }

        private void SetFont(CustomEntry view)
	    {
			if(view.Font != Font.Default) {
				Control.TextSize = view.Font.ToScaledPixel();
				//Control.Typeface = view.Font.ToExtendedTypeface(Context);
			}
	    }

        private void SetPlaceholderTextColor(CustomEntry view)
        {
			if(view.PlaceholderTextColor != Color.Default) 
				Control.SetHintTextColor(view.PlaceholderTextColor.ToAndroid());			
		}
    }
}
