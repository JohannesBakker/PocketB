using PocketButler.Controls;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace PocketButler
{
    public class AboutUsPage : BasePage
    {
        protected AbsoluteLayout UILayout { get; set; }

		public AboutUsPage(Action RefreshEvent)
        {
			BackAppearingEvent = RefreshEvent;

            Title = "About Us";

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
			var MenuImage = new DarkIceImage {
				Source = ImageSource.FromFile ("navbar_sidebar.png"),
				Aspect = Aspect.AspectFit,
				IsEnablePadding = true,
			};
            MenuImage.Tapped += MenuButton_Clicked;

			var webhtml = new WebView {
				BackgroundColor = Color.Transparent,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
			};

			var html  = new UrlWebViewSource (){
				Url = System.IO.Path.Combine("file:///android_asset/", "about_us.htm")
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

			var ScrollView = new ScrollView { Content = MainLayout };
			AbsoluteLayout.SetLayoutFlags(ScrollView, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(ScrollView, new Rectangle(0, 0.7, 1, 0.9));
			UILayout.Children.Add(ScrollView);

            MakeCustomNavigationBar(UILayout, MenuImage, null, true);

            Content = UILayout;
        }
        #endregion

        #region EVENTS
        void MenuButton_Clicked()
        {
            MasterPage.IsPresented = true;
        }
        #endregion
    }
}
