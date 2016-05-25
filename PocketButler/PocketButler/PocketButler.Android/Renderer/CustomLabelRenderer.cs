using Android.Graphics;
using Android.Widget;
using PocketButler;
using PocketButler.Droid.Renderer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Xamarin;

[assembly: ExportRenderer(typeof(CustomLabel), typeof(CustomLabelRenderer))]
namespace PocketButler.Droid.Renderer
{
    public class CustomLabelRenderer : LabelRenderer
    {
        public CustomLabelRenderer()
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);

            var view = (CustomLabel)Element;
            var control = Control;
			//control.SetMaxLines (2);
            control.Click += delegate
            {
                if (view.Tapped != null)
                    view.Tapped.Invoke();
            };

            UpdateUi(view, control);

        }

        void UpdateUi(CustomLabel view, TextView control)
        {
            if(!string.IsNullOrEmpty(view.FontName))
            {
                string filename = view.FontName;
                //if no extension given then assume and add .ttf
                if(filename.LastIndexOf(".", System.StringComparison.Ordinal) != filename.Length - 4)
                {
                    filename = string.Format("{0}.ttf", filename);
                }
                control.Typeface = TrySetFont(filename);
            }

            //======= This is for backward compatability with obsolete attrbute 'FontNameAndroid' ========
            else if(!string.IsNullOrEmpty(view.FontNameAndroid))
            {
                control.Typeface = TrySetFont(view.FontNameAndroid);

            }
            //====== End of obsolete section ==========================================================

            else if(view.Font != Font.Default)
            {
                //control.Typeface = view.Font.ToExtendedTypeface(Context);
            }

            if(view.FontSize > 0)
            {
                control.TextSize = (float)view.FontSize;
            }

            if(view.IsUnderline)
            {
                control.PaintFlags = control.PaintFlags | PaintFlags.UnderlineText;
            }

            if(view.IsStrikeThrough)
            {
                control.PaintFlags = control.PaintFlags | PaintFlags.StrikeThruText;
            }

        }

        private Typeface TrySetFont(string fontName)
        {
            try
            {                
                return Typeface.CreateFromAsset(Context.Assets, "fonts/" + fontName);
            } catch(Exception ex)
            {
				Insights.Report (ex);
                Console.WriteLine("not found in assets. Exception: {0}", ex);
                try
                {
                    return Typeface.CreateFromFile("fonts/" + fontName);
                } catch(Exception ex1)
                {
					Insights.Report (ex);
                    Console.WriteLine("not found by file. Exception: {0}", ex1);

                    return Typeface.Default;
                }
            }
        }
    }
}
