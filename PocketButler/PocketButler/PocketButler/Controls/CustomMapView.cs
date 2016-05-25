using System;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using System.Collections.Generic;

namespace PocketButler
{
	public class CustomMapView : Map
	{
		public CustomMapView (MapSpan mapSpan) : base (mapSpan)
		{
		}

		public static readonly BindableProperty SelectedPinProperty = BindableProperty.Create<CustomMapView, CustomPin> (x => x.SelectedPin, new CustomPin{ Label = "" });

		public CustomPin SelectedPin {
			get{ return (CustomPin)base.GetValue (SelectedPinProperty); }
			set{ base.SetValue (SelectedPinProperty, value); }
		}

		public static readonly BindableProperty CustomPinsProperty = BindableProperty.Create<CustomMapView, List<CustomPin>> (x => x.CustomPins, new List<CustomPin> (){ new CustomPin (){ Label = "" } });

		public List<CustomPin> CustomPins {
			get{ return (List<CustomPin>)base.GetValue (CustomPinsProperty); }
			set{ base.SetValue (CustomPinsProperty, value); }
		}

		public static readonly BindableProperty ForceRedrawProperty = BindableProperty.Create<CustomMapView, bool> (x => x.ForceRedraw, false);

		public bool ForceRedraw {
			get{ return (bool)base.GetValue (ForceRedrawProperty); }
			set{ base.SetValue (ForceRedrawProperty, value); }
		}

		public Action SelectedPinChanged { get; set; }
	}
}

