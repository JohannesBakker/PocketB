using System;
using Xamarin.Forms;
using PocketButler;
using PocketButler.iOS;
using Xamarin.Forms.Platform.iOS;
using System.ComponentModel;
using MonoTouch.UIKit;
using System.Drawing;
using MonoTouch.Foundation;

[assembly: ExportRenderer (typeof (CustomEntry), typeof (CustomEntryRenderer))]
namespace PocketButler.iOS
{
	public class CustomEntryRenderer : EntryRenderer
	{
		/// <summary>
		/// The on element changed callback.
		/// </summary>
		/// <param name="e">
		/// The event arguments.
		/// </param>
		protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
		{
			base.OnElementChanged(e);

			var view = (CustomEntry)Element;

			SetFont(view);
			SetTextAlignment(view);
			SetBorder(view);
			SetPlaceholderTextColor(view);

			ResizeHeight();
		}

		/// <summary>
		/// The on element property changed callback
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			var view = (CustomEntry)Element;

			if (e.PropertyName == CustomEntry.FontProperty.PropertyName)
				SetFont(view);
			if (e.PropertyName == CustomEntry.XAlignProperty.PropertyName)
				SetTextAlignment(view);
			if (e.PropertyName == CustomEntry.HasBorderProperty.PropertyName)
				SetBorder(view);
			if (e.PropertyName == CustomEntry.PlaceholderTextColorProperty.PropertyName)
				SetPlaceholderTextColor(view);

			ResizeHeight();
		}

		private void SetTextAlignment(CustomEntry view)
		{
			switch (view.XAlign)
			{
			case TextAlignment.Center:
				Control.TextAlignment = UITextAlignment.Center;
				break;
			case TextAlignment.End:
				Control.TextAlignment = UITextAlignment.Right;
				break;
			case TextAlignment.Start:
				Control.TextAlignment = UITextAlignment.Left;
				break;
			}
		}

		private void SetFont(CustomEntry view)
		{
			UIFont uiFont;
			if (view.Font != Font.Default && (uiFont = view.Font.ToUIFont()) != null)
				Control.Font = uiFont;
			else if (view.Font == Font.Default)
				Control.Font = UIFont.SystemFontOfSize(17f);
		}

		private void SetBorder(CustomEntry view)
		{
			Control.BorderStyle = view.HasBorder ? UITextBorderStyle.RoundedRect : UITextBorderStyle.None;
		}

		private void ResizeHeight()
		{
			if (Element.HeightRequest >= 0) return;

			var height = Math.Max(Bounds.Height,
				new UITextField {Font = Control.Font}.IntrinsicContentSize.Height);

			Control.Frame = new RectangleF(0.0f, 0.0f, (float) Element.Width, height);

			Element.HeightRequest = height;
		}

		void SetPlaceholderTextColor(CustomEntry view)
		{
			/*
UIColor *color = [UIColor lightTextColor];
YOURTEXTFIELD.attributedPlaceholder = [[NSAttributedString alloc] initWithString:@"PlaceHolder Text" attributes:@{NSForegroundColorAttributeName: color}];
			*/
			if(string.IsNullOrEmpty(view.Placeholder) == false && view.PlaceholderTextColor != Color.Default) {
				NSAttributedString placeholderString = new NSAttributedString(view.Placeholder, new UIStringAttributes(){ ForegroundColor = view.PlaceholderTextColor.ToUIColor() });
				Control.AttributedPlaceholder = placeholderString;
			}
		}
	}
}

