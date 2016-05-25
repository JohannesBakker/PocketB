using System;
using Xamarin.Forms;

namespace PocketButler
{
	public class CustomSeekBar : View
	{
		public int MinValue { get; set; }
		public int MaxValue { get; set; }
		public int SelectedValue { get; set; }

		public Action ValueChangedEvent;
	}
}

