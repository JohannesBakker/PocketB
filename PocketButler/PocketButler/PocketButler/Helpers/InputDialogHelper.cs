using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

#if __ANDROID__
using Android.App;
using Android.Widget;
using Android.Content;
#elif __IOS__
using MonoTouch.UIKit;
#endif

namespace PocketButler
{
    public class InputDialogHelper
    {
		public delegate void InputDialogResultDelegate (String msg);
		public static InputDialogResultDelegate InputDialogResultEvent = null;

#if __ANDROID__
        public static EditText input;
		public static void ShowInputDialog(Context context, String msgtitle)
        {
            input = new EditText(context);
			input.InputType = Android.Text.InputTypes.TextVariationPassword | Android.Text.InputTypes.ClassText;
            var builder = new AlertDialog.Builder(context)
				.SetTitle(msgtitle)
                .SetView(input)
                //.SetMessage("aa")
                .SetCancelable(false)
				.SetNegativeButton("Cancel", (s, args) => {

				})
				.SetPositiveButton("OK", (s, args) => {
					if (InputDialogResultEvent != null)
						InputDialogResultEvent(input.Text);
				});

            var dialog = builder.Create();
            dialog.Show();
        }
#elif __IOS__
		public static void ShowInputDialog(String msgtitle)
        {
			UIAlertView inputView = new UIAlertView ();
			inputView.Title = msgtitle;
			inputView.AddButton ("OK");
			inputView.AlertViewStyle = UIAlertViewStyle.PlainTextInput;
            inputView.Clicked += (sender, e) => AlertViewItemClickedEvent((UIAlertView)sender, e.ButtonIndex);
			inputView.Show ();
        }

        private static void AlertViewItemClickedEvent(UIAlertView view, int index)
		{
			if (InputDialogResultEvent != null)
				InputDialogResultEvent(view.GetTextField (0).Text);
		}
#endif
    }
}
