using Android.Graphics;
using Android.Widget;
using PocketButler;
using PocketButler.Controls;
using PocketButler.Droid.Renderer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Text;
using Android.Text.Util;

[assembly: ExportRenderer(typeof(DILabel), typeof(DILabelRenderer))]
namespace PocketButler.Droid.Renderer
{
    public class DILabelRenderer : LabelRenderer
    {
        public DILabelRenderer()
        {
            
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);

            var view = (DILabel)Element;
            var control = Control;

			control.Clickable = !view.IsDefaultLabel;

			if (view.IsHtmlLabel) {
				control.TextFormatted = Html.FromHtml (view.Text);
				Linkify.AddLinks(control, MatchOptions.All);
			}
			else {
				if (view.IsDefaultLabel == false) {
					control.Click += delegate
					{
						if (view.TappedWithId != null)
							view.TappedWithId.Invoke(view.ItemID);
						else if (view.Tapped != null)
							view.Tapped.Invoke();
					};
				}
			}

			if (view.HasBackground)
				control.SetBackgroundResource (Resource.Drawable.quantity);

			if (view.Lines != 0)
				control.SetLines (view.Lines);
        }
    }
}
