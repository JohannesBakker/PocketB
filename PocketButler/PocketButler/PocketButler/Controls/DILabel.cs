using System;
using Xamarin.Forms;

namespace PocketButler.Controls
{
	public class DILabel : Label
	{
		public DILabel ()
		{
			//LineBreakMode = LineBreakMode.TailTruncation;
		}

		public void SetBoldFont(NamedSize namedSize)
		{
#if __IOS__
			Font = Font.SystemFontOfSize(namedSize, FontAttributes.Bold);
#elif __ANDROID__
			Font = Font.SystemFontOfSize(namedSize, FontAttributes.Bold);
#endif
		}

		public void SetBoldFont(double pointSize)
		{
#if __IOS__
			Font = Font.SystemFontOfSize(pointSize, FontAttributes.Bold);
#elif __ANDROID__
			Font = Font.SystemFontOfSize(pointSize, FontAttributes.Bold);
#endif
		}
        public Action Tapped { get; set; }
		public Action<String> TappedWithId { get; set; }
		public bool IsHtmlLabel { get; set; }
		public bool IsDefaultLabel { get; set; }
		public bool HasBackground { get; set; }
		public int Lines { get; set; }

		public static readonly BindableProperty DataItemIDProperty =
			BindableProperty.Create<CustomButton, String> (
				p => p.ItemID, "");

		public String ItemID {
			get {
				return (String)GetValue (DataItemIDProperty);
			}

			set {
				this.SetValue (DataItemIDProperty, value);
			}
		}

		public static readonly BindableProperty ShouldDisposeProperty =
			BindableProperty.Create<DILabel, bool> (
				p => p.ShouldDispose, false);


		public bool ShouldDispose {
			get {
				return (bool)GetValue (ShouldDisposeProperty);
			}

			set {
				this.SetValue (ShouldDisposeProperty, value);
			}
		}

	}

	public static class DILabelExtensions
	{
		public static void DisposeLabelIfNotNull (this DILabel item)
		{
			if (item != null)
				item.ShouldDispose = true;
		}
	}
}

