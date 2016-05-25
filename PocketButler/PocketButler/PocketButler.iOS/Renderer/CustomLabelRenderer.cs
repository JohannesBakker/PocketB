using System;
using Xamarin.Forms;
using PocketButler;
using Xamarin.Forms.Platform.iOS;
using System.ComponentModel;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using PocketButler.iOS;

[assembly: ExportRenderer (typeof (CustomLabel), typeof (CustomLabelRenderer))]
namespace PocketButler.iOS
{
	public class CustomLabelRenderer : LabelRenderer
	{
		/// <summary>
		/// The on element changed callback.
		/// </summary>
		/// <param name="e">
		/// The event arguments.
		/// </param>
		protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
		{
			base.OnElementChanged(e);
			var view = (CustomLabel)Element;

			UpdateUi(view, this.Control);
		}

		/// <summary>
		/// Updates the UI.
		/// </summary>
		/// <param name="view">
		/// The view.
		/// </param>
		/// <param name="control">
		/// The control.
		/// </param>
		private static void UpdateUi(CustomLabel view, UILabel control)
		{
			// Prefer font set through Font property.
			if(view.Font == Font.Default){
				if (view.FontSize > 0)
				{
					control.Font = UIFont.FromName(control.Font.Name,(float)view.FontSize);
				}

				if (!string.IsNullOrEmpty(view.FontName))
				{
					string fontName = view.FontName;
					//if extension given then remove it for iOS
					if (fontName.LastIndexOf(".", System.StringComparison.Ordinal) == fontName.Length - 4)
					{
						fontName = fontName.Substring(0, fontName.Length - 4);
					}

					var font = UIFont.FromName(
						fontName, control.Font.PointSize);

					if (font != null)
					{
						control.Font = font;
					}
				}

				//======= This is for backward compatability with obsolete attrbute 'FontNameIOS' ========
				if (!string.IsNullOrEmpty(view.FontNameIOS))
				{
					var font = UIFont.FromName(
						view.FontNameIOS,
						(view.FontSize > 0) ? (float)view.FontSize : 12.0f);

					if (font != null)
					{
						control.Font = font;
					}
				}
				//====== End of obsolete section ==========================================================

			}else{
				//Font si set by the base class
			}

			//Do not create attributed string if it is not necesarry
			if(view.IsUnderline || view.IsStrikeThrough || view.IsDropShadow)
			{

				var attrString = new NSMutableAttributedString(control.Text);

				if(view.IsUnderline)
				{
					//control.AttributedText = new NSAttributedString(
					//    control.Text,
					//    underlineStyle: NSUnderlineStyle.Single);

					attrString.AddAttribute(UIStringAttributeKey.UnderlineStyle, NSNumber.FromInt32((int)NSUnderlineStyle.Single), new NSRange(0, attrString.Length));
				}

				if(view.IsStrikeThrough)
				{
					attrString.AddAttribute(UIStringAttributeKey.StrikethroughStyle, NSNumber.FromInt32((int)NSUnderlineStyle.Single), new NSRange(0, attrString.Length));
				}
				attrString.AddAttribute(UIStringAttributeKey.Font,control.Font,new NSRange(0, attrString.Length));

				if (view.IsDropShadow)
				{
					var shadow = new NSShadow();
					shadow.ShadowColor = UIColor.DarkGray;
					shadow.ShadowBlurRadius = 1.4f;
					shadow.ShadowOffset = new System.Drawing.SizeF(new System.Drawing.PointF(0.3f, 0.8f));

					attrString.AddAttribute(UIStringAttributeKey.Shadow, shadow, new NSRange(0, attrString.Length));
				}

				control.TextColor = view.TextColor.ToUIColor();
				control.AttributedText = attrString;
			}
		}
	}
}

