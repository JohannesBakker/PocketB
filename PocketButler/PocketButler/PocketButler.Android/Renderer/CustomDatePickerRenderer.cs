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

[assembly: ExportRenderer(typeof(CustomDatePicker), typeof(CustomDatePickerRenderer))]
namespace PocketButler.Droid.Renderer
{
    public class CustomDatePickerRenderer : LabelRenderer
    {
		public CustomDatePicker _picker;
		public CustomerDatePickerDialog _pickerDialog;

		DateTime _curDateTime;

        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
		{
			base.OnElementChanged(e);

			var view = (CustomDatePicker)Element;
			_picker = view;
			_curDateTime = _picker.Date;

			Control.SetPadding (10, 0, 10, 0);
			Control.Hint = view.Hint;
			if (view.IsFromCreatePage == false)
				Control.SetText(view.Date.ToString(view.DateFormat), Android.Widget.TextView.BufferType.Normal);
			
			this.Control.Clickable = true;
			this.Control.Click += (object sender, EventArgs e1) => {
				_pickerDialog = new CustomerDatePickerDialog(base.Context, OnDateSet, _curDateTime.Year, _curDateTime.Month - 1, _curDateTime.Day, _picker.DateFormat);
				_pickerDialog.Show();

				// Try to hide date control
				try{
					if (!view.HideCalendarIndex.Equals(""))
					{
						Android.Widget.DatePicker dp = findDatePicker((ViewGroup) _pickerDialog.Window.DecorView);
						try{
							Java.Lang.Reflect.Field[] f = dp.Class.GetDeclaredFields();
							foreach(Java.Lang.Reflect.Field field in f){
								if(field.Name.Equals(view.HideCalendarIndex)){
									field.Accessible = true;
									Object hidePicker = new Object();
									hidePicker = field.Get(dp);
									((Android.Views.View)hidePicker).Visibility = ViewStates.Gone;
								}
							}
						} catch(Exception ex){
							Insights.Report (ex);
							Console.WriteLine(ex.Message);
						}
					}
				}
				catch (Exception ex)
				{
					Insights.Report (ex);
				}
			};
		}

		// the event received when the user "sets" the date in the dialog
		void OnDateSet (object sender, DatePickerDialog.DateSetEventArgs e)
		{
			Control.SetText(e.Date.ToString(_picker.DateFormat), Android.Widget.TextView.BufferType.Normal);
			_picker.Date = e.Date;
			_curDateTime = e.Date;
			_picker.IsDateSelected = true;
		}

		private Android.Widget.DatePicker findDatePicker(ViewGroup group) {
			if (group != null) {
				for (int i = 0, j = group.ChildCount; i < j; i++) {
					Android.Views.View child = group.GetChildAt(i);
					if (child is Android.Widget.DatePicker) {
						return (Android.Widget.DatePicker) child;
					} else if (child is ViewGroup) {
						Android.Widget.DatePicker result = findDatePicker((ViewGroup) child);
						if (result != null)
							return result;
					}
				}
			}
			return null;
		}

		public class CustomerDatePickerDialog : DatePickerDialog {
			String DateFormat { get; set; }
			public CustomerDatePickerDialog(Context context, EventHandler<DateSetEventArgs> callBack, int year, int monthOfYear, int dayOfMonth, String dateFormat = "dd/MM/yyyy") : base(context, callBack, year, monthOfYear, dayOfMonth) {
				DateFormat = dateFormat;
				DateTime curDateTime = new DateTime (year, monthOfYear + 1, dayOfMonth);
				this.SetTitle (curDateTime.ToString(DateFormat));
			}

			public override void OnDateChanged (Android.Widget.DatePicker view, int year, int month, int day)
			{
				base.OnDateChanged (view, year, month, day);

				DateTime curDateTime = new DateTime (year, month + 1, day);
				this.SetTitle (curDateTime.ToString(DateFormat));
			}
		}
    }
}
