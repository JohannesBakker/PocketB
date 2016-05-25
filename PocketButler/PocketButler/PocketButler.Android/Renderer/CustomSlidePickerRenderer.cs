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
using Android.Widget;

[assembly: ExportRenderer(typeof(CustomSlidePicker), typeof(CustomSlidePickerRenderer))]
namespace PocketButler.Droid.Renderer
{
    public class CustomSlidePickerRenderer : LabelRenderer
    {
		public CustomSlidePicker _control;
		public String[] itemList;
		public bool hasList = false;

        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
		{
			base.OnElementChanged(e);

			var view = (CustomSlidePicker)Element;
			_control = view;

			Control.SetPadding (10, 0, 10, 0);
			if (_control.PickerList == null || _control.PickerList.Count == 0)
				hasList = false;
			else {
				hasList = true;
				//itemList = new String[_control.PickerList.Count];
				itemList = _control.PickerList.ToArray ();

				if (_control.SelectedIndex != -1) {
					Control.Text = _control.PickerList [_control.SelectedIndex];
					if (_control.SelectionChanged != null)
						_control.SelectionChanged.Invoke ();
				} else {
					Control.Text = "";
				}
			}

			this.Control.Clickable = true;
			this.Control.Click += (object sender, EventArgs e1) => {
				if (hasList == true)
				{
					var builder = new AlertDialog.Builder(base.Context)
						.SetTitle("")
						.SetItems(itemList, ItemClickedEvent)
						.SetCancelable(false)
						.SetNegativeButton("Cancel", (s, args) => {

						})
						.SetPositiveButton("OK", (s, args) => {
							if (args.Which < 0)
								return;

							Control.Text  = itemList[args.Which];
						});

					var dialog = builder.Create();
					dialog.Show();
				}
			};

			//Control.SetBackgroundResource (Resource.Drawable.edit_background);
		}

		protected override void OnElementPropertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged (sender, e);

			if (e.PropertyName.Equals ("SelectedIndex")) {
				if (_control.SelectedIndex != -1) {
					Control.Text = _control.PickerList [_control.SelectedIndex];
					if (_control.SelectionChanged != null)
						_control.SelectionChanged.Invoke ();
				} else {
					Control.Text = "";
				}
			}
			else if (e.PropertyName.Equals ("PickerList")) {
				if (_control.PickerList == null || _control.PickerList.Count == 0)
					hasList = false;
				else {
					hasList = true;
					//itemList = new String[_control.PickerList.Count];
					itemList = _control.PickerList.ToArray ();

					if (_control.SelectedIndex != -1) {
						Control.Text = _control.PickerList [_control.SelectedIndex];
						if (_control.SelectionChanged != null)
							_control.SelectionChanged.Invoke ();
					} else {
						Control.Text = "";
					}
				}
			}
		}

		private void ItemClickedEvent(object sender, DialogClickEventArgs args)
		{
			if (args.Which < 0)
				return;

			_control.SelectedIndex = args.Which;
			if (_control.SelectionChanged != null)
				_control.SelectionChanged.Invoke ();
		}
    }
}
