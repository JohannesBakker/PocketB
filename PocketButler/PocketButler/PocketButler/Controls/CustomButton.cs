using System;
using Xamarin.Forms;

namespace PocketButler
{
	public class CustomButton : Button
	{
		public CustomButton ()
		{
		}

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

		public static readonly BindableProperty DataItemTypeProperty =
			BindableProperty.Create<CustomButton, String> (
				p => p.ItemType, "");

		public String ItemType {
			get {
				return (String)GetValue (DataItemTypeProperty);
			}

			set {
				this.SetValue (DataItemTypeProperty, value);
			}
		}
	}
}

