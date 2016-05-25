using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using PocketButler.Controls;
using PocketButler.Droid.Renderer;
using System.ComponentModel;
using Android.Graphics.Drawables;

[assembly: ExportRenderer(typeof(CustomMasterDetailPage), typeof(CustomMasterDetailPageRenderer))]
namespace PocketButler.Droid.Renderer
{
    public class CustomMasterDetailPageRenderer : MasterDetailRenderer
    {
        private Activity activity;
        private const string COLOR = "#FFFFFF";
        private const string TEXTCOLOR = "#353535";

        protected override void OnElementChanged(VisualElement oldElement, VisualElement newElement)
        {
            base.OnElementChanged(oldElement, newElement);

            activity = this.Context as Activity;

            ActionBar actionBar = activity.ActionBar;

            ColorDrawable colorDrawable = new ColorDrawable(global::Android.Graphics.Color.ParseColor("#FF0000"));
            actionBar.SetStackedBackgroundDrawable(colorDrawable);
            actionBar.SetBackgroundDrawable(colorDrawable);

            if (actionBar.TabCount > 0)
            {
                ActionBarTabsSetup(actionBar);
            }
        }

        private void ActionBarTabsSetup(ActionBar actionBar)
        {

        }
    }
}