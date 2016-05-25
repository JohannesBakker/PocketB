using System;
using Xamarin.Forms;
using System.Collections.Generic;

namespace PocketButler
{
	public class CustomSlidePicker : Label
	{
		public List<String> PickerList {
			get {
				return (List<String>)GetValue (PickerListProperty);
			}

			set {
				this.SetValue (PickerListProperty, value);
			}
		}

		public int SelectedIndex {
			get {
				return (int)GetValue (SelectedIndexProperty);
			}

			set {
				this.SetValue (SelectedIndexProperty, value);
			}
		}

		public int Idx { get; set; }

		public CustomSlidePicker ()
		{
			PickerList = new List<String> ();
			XAlign = TextAlignment.Center;
			YAlign = TextAlignment.Center;
			BackgroundColor = Color.Transparent;
		}

		public Action SelectionChanged { get; set; }

		public readonly BindableProperty SelectedIndexProperty =
			BindableProperty.Create<CustomSlidePicker, int> (
				p => p.SelectedIndex, -1);

		public readonly BindableProperty PickerListProperty =
			BindableProperty.Create<CustomSlidePicker, List<String>> (
				p => p.PickerList, null);
	}
}

