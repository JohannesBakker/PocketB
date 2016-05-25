using System;
using Xamarin.Forms;

namespace PocketButler
{
	public class CustomPicker : Picker
	{
		public CustomPicker ()
		{
		}

		public static readonly BindableProperty DataItemIDProperty =
			BindableProperty.Create<CustomPicker, int> (
				p => p.ItemID, 0);

		public int ItemID {
			get {
				return (int)GetValue (DataItemIDProperty);
			}

			set {
				this.SetValue (DataItemIDProperty, value);
			}
		}
	}
}

