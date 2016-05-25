using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Widget;
using Android.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PocketButler.Droid.Renderer;
using PocketButler.Controls;
using Android.Text;
using Android.Text.Style;

[assembly: ExportCell(typeof(CustomImageCell), typeof(CustomImageCellRenderer))]
namespace PocketButler.Droid.Renderer
{
    public class CustomImageCellRenderer : ImageCellRenderer
    {
        protected override Android.Views.View GetCellCore(Cell item, Android.Views.View convertView, Android.Views.ViewGroup parent, Context context)
        {
			float imgPadding = 15;
			float fDensity = context.Resources.DisplayMetrics.Density;
			int Padding = (int)(imgPadding * fDensity);

            var cell = (LinearLayout)base.GetCellCore(item, convertView, parent, context);
            cell.Layout(0, 0, cell.Width, 55);
            var image = (ImageView)cell.GetChildAt(0);
            image.SetScaleType(ImageView.ScaleType.FitCenter);
			image.SetPadding(Padding, Padding, Padding, Padding);
            var textLayout = (LinearLayout)cell.GetChildAt(1);
            var text = (TextView)textLayout.GetChildAt(0);
            text.SetTextColor(Android.Graphics.Color.White);
            text.SetTextSize(Android.Util.ComplexUnitType.Dip, 14);
			text.SetPadding (0, 0, 0, 0);

			var detail = (TextView)textLayout.GetChildAt(1);
			detail.SetTextColor(Android.Graphics.Color.Rgb(171, 146, 91));
			detail.SetTextSize(Android.Util.ComplexUnitType.Dip, 11);
			detail.SetLines (1);
			detail.SetPadding (0, 0, 0, 0);

            return cell;
        }
    }
}