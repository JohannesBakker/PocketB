using PocketButler.Controls;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Android.Text;
using Android.Text.Style;

namespace PocketButler
{
	public interface IBaseUrl { string Get(); }

    public class TermsConditionsPage : BasePage
    {
        protected AbsoluteLayout UILayout { get; set; }

		public TermsConditionsPage(Action RefreshEvent)
        {
			BackAppearingEvent = RefreshEvent;

            Title = "Terms and Conditions";

            UILayout = new AbsoluteLayout
            {
                VerticalOptions = LayoutOptions.Fill,
                BackgroundColor = Color.Transparent
            };

            BuildUI();            
        }

        #region PRIVATE METHODS
        private void BuildUI()
        {
			var webhtml = new WebView {
				BackgroundColor = Color.Transparent,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
			};

			var html  = new UrlWebViewSource (){
				Url = System.IO.Path.Combine("file:///android_asset/", "termsconditions.htm")
			};
			webhtml.Source = html;
			webhtml.BackgroundColor = Color.Transparent;
			webhtml.Navigating += (object sender, WebNavigatingEventArgs e) => {
				if (String.IsNullOrEmpty(e.Url) == false)
				{
					e.Cancel = true;
					App.PageLoaderManager.StartIntent(e.Url);
				}
			};

			var MainLayout = new StackLayout
            {
                Padding = 0,
                Spacing = 0,
                Children = {
					webhtml
                }
            };

            var scrollView = new ScrollView { Content = MainLayout };
            AbsoluteLayout.SetLayoutFlags(scrollView, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(scrollView, new Rectangle(0, 1, 1, 0.92));
            UILayout.Children.Add(scrollView);

            MakeCustomNavigationBar(UILayout, null, null);

            Content = UILayout;
        }
        #endregion

        #region EVENTS
        async void MenuButton_Clicked()
        {
			if (BackAppearingEvent != null)
				BackAppearingEvent.Invoke ();
            await Navigation.PopAsync();
        }
        #endregion
    }
}
