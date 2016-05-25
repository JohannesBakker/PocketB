using System;
using Xamarin.Forms;

namespace PocketButler
{
	public class CustomDatePicker : Label
	{
		public DateTime Date { get; set; }
		public bool IsFromCreatePage { get; set; }
		public bool IsDateSelected { get; set; }

		public CustomDatePicker ()
		{
			DateFormat = "dd/MM/yyyy";
			HideCalendarIndex = "";
			Hint = "";
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
		public bool IsHtmlLabel { get; set; }
		public bool IsDefaultLabel { get; set; }
		public bool HasBackground { get; set; }
		public int Lines { get; set; }

		public String DateFormat { get; set; }
		public String HideCalendarIndex { get; set; }

		public String Hint { get; set; }

		public static readonly BindableProperty ShouldDisposeProperty =
			BindableProperty.Create<CustomDatePicker, bool> (
				p => p.ShouldDispose, false);


		public bool ShouldDispose {
			get {
				return (bool)GetValue (ShouldDisposeProperty);
			}

			set {
				this.SetValue (ShouldDisposeProperty, value);
			}
		}

		public static readonly BindableProperty ShowTimePickerProperty =
			BindableProperty.Create<CustomDatePicker, bool> (
				p => p.ShowTimePicker, false);


		public bool ShowTimePicker {
			get {
				return (bool)GetValue (ShowTimePickerProperty);
			}

			set {
				this.SetValue (ShowTimePickerProperty, value);
			}
		}
	}
}

