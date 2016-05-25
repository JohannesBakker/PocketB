using Android.Views;
using PocketButler;
using PocketButler.Droid.Renderer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using System.Threading;
using Android.App;
using Android.Content;
using Java.Util;
using Xamarin;
using Android.Widget;

[assembly: ExportRenderer(typeof(CustomDateTimePicker), typeof(CustomDateTimePickerRenderer))]
namespace PocketButler.Droid.Renderer
{
	public class CustomDateTimePickerRenderer : LabelRenderer
    {
		public CustomDateTimePicker _picker;
		public TimePickerDialog _timepickerDialog;

		DateTime _curDateTime;

        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
		{
			base.OnElementChanged(e);

			var view = (CustomDateTimePicker)Element;
			_picker = view;
			_curDateTime = _picker.Date;

			Control.SetPadding (10, 0, 10, 0);
			Control.Hint = view.Hint;
		}

		protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			switch (e.PropertyName)
			{

			case "ShowTimePicker":
				{
					var view = (CustomDateTimePicker)Element;
					_timepickerDialog = new TimePickerDialog (base.Context, OnTimeSet, _curDateTime.Hour, _curDateTime.Minute, true);
					_timepickerDialog.Show ();
				}
				break;			
			default:
				//System.Diagnostics.Debug.WriteLine ("Property change for {0} has not been implemented.", e.PropertyName);
				break;
			}

		}

		void OnTimeSet (object sender, TimePickerDialog.TimeSetEventArgs e)
		{
			if (e.HourOfDay < DateTime.Now.Hour) {
				Toast.MakeText (base.Context, "Time is not valid. Please select again", ToastLength.Long).Show ();
				return;
			}

			if (e.Minute < DateTime.Now.Minute && e.HourOfDay == DateTime.Now.Hour){
				Toast.MakeText (base.Context, "Time is not valid. Please select again", ToastLength.Long).Show ();
				return;
			}

			_picker.Date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, e.HourOfDay, e.Minute, 0);
			_curDateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, e.HourOfDay, e.Minute, 0);
			_picker.IsDateSelected = true;

			Control.SetText (_curDateTime.ToString (_picker.DateFormat), Android.Widget.TextView.BufferType.Normal);
		}
    }
}
