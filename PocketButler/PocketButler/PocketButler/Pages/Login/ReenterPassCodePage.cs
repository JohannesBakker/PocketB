using PocketButler.Controls;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace PocketButler
{
    public class ReenterPassCodePage : BasePage
    {
        protected AbsoluteLayout UILayout { get; set; }
        protected AbsoluteLayout MainLayout { get; set; }
        DILabel TopLabel;
        DILabel EnterCodeLabel;

		DarkIceImage PasscodeOneImage;
		DarkIceImage PasscodeTwoImage;
		DarkIceImage PasscodeThreeImage;
		DarkIceImage PasscodeFourImage;

		Button[] NumberButtons;
		Button DeleteButton;
		Button BlankButton;

		String[] PassCodeStrings;
		String UserInputPinCode;

		int curIndex = 0;
		List<DarkIceImage> PassCodeImageList = new List<DarkIceImage>();

        new DarkIceImage BackgroundImage;

		public ReenterPassCodePage(Action RefreshEvent)
		{
			BackAppearingEvent = RefreshEvent;
			PassCodeStrings = new String[4];
			for (int i = 0; i < 4; i++)
				PassCodeStrings [i] = "";
			UserInputPinCode = "";

            Title = "Passcode";

            UILayout = new AbsoluteLayout
            {
                VerticalOptions = LayoutOptions.Fill,
                BackgroundColor = Color.Transparent
            };

			MainLayout = new AbsoluteLayout {
				VerticalOptions = LayoutOptions.Fill,
                BackgroundColor = Color.Transparent
			};

			BuildUI ();
		}

        #region PRIVATE METHODS
        private void BuildUI()
        {
            // Set Background Image
            BackgroundImage = new DarkIceImage
            {
                VerticalOptions = LayoutOptions.Fill,
                HorizontalOptions = LayoutOptions.Fill,
                Source = ImageSource.FromFile("welcome_background.png"),
                Aspect = Aspect.Fill,
            };

            AbsoluteLayout.SetLayoutFlags(BackgroundImage, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(BackgroundImage, new Rectangle(0, 0, 1, 1));
            UILayout.Children.Add(BackgroundImage);

            // Register Custom Navigation Bar
            MakeCustomNavigationBar(UILayout, null, null);

			TopLabel = new DILabel {
				Text = "(This is required to secure payments from the app.)",
				TextColor = Color.White,
				Font = Font.SystemFontOfSize (NamedSize.Medium, FontAttributes.Italic)
			};

            AbsoluteLayout.SetLayoutFlags(TopLabel, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(TopLabel, new Rectangle(0.5, 0.05, 0.9, 0.09));
            MainLayout.Children.Add(TopLabel);

			EnterCodeLabel = new DILabel {
				Text = "Re-enter your passcode",
				TextColor = Color.White,
				Font = Font.SystemFontOfSize (25, FontAttributes.Bold),
				XAlign = TextAlignment.Center
			};

            AbsoluteLayout.SetLayoutFlags(EnterCodeLabel, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(EnterCodeLabel, new Rectangle(0.5, 0.18, 0.9, 0.1));
            MainLayout.Children.Add(EnterCodeLabel);

            CreatePassCodeList();

            AbsoluteLayout.SetLayoutFlags(MainLayout, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(MainLayout, new Rectangle(0, 0.7, 1, 0.9));
            UILayout.Children.Add(MainLayout);

            Content = UILayout;
        }

        private void CreatePassCodeList()
        {
			NumberButtons = new Button[10];
			for (int i = 0; i < 10; i++) {
				NumberButtons [i] = new Button {
					Text = i.ToString(),
					BackgroundColor = Color.FromRgb(50, 50, 50),
					BorderWidth = 0,
					BorderRadius = 0,
					Font = Font.SystemFontOfSize(NamedSize.Large),
				};

				NumberButtons[i].Clicked += async (object sender, EventArgs e) => {
					Button NumberButton = sender as Button;
					PassCodeStrings[curIndex] = NumberButton.Text;
					if (curIndex >= 0 && curIndex < 4)
					{
						PassCodeImageList[curIndex].Source = ImageSource.FromFile("papasscode_marker.png");
						curIndex++;
					}

					if (curIndex > 3)
					{
						UserInputPinCode = PassCodeStrings[0] + PassCodeStrings[1] + PassCodeStrings[2] + PassCodeStrings[3];

						App.PageLoaderManager.ShowMainPage ();
					}
				};
			}

			for (int i = 1; i < 10; i++) {
				int x = (i - 1) % 3;
				int y = (i - 1) / 3;
				AbsoluteLayout.SetLayoutFlags(NumberButtons[i], AbsoluteLayoutFlags.All);
				AbsoluteLayout.SetLayoutBounds(NumberButtons[i], new Rectangle(0.1 + 0.4 * x, 0.65 + 0.12 * y, 0.25, 0.1));
				MainLayout.Children.Add(NumberButtons[i]);
			}

			AbsoluteLayout.SetLayoutFlags(NumberButtons[0], AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(NumberButtons[0], new Rectangle(0.5, 0.65 + 0.12 * 3, 0.25, 0.1));
			MainLayout.Children.Add(NumberButtons[0]);

			BlankButton = new Button {
				Text = "",
				BackgroundColor = Color.FromRgb(100, 100, 100),
				BorderWidth = 0,
				BorderRadius = 0,
				Font = Font.SystemFontOfSize(NamedSize.Medium),
			};

			AbsoluteLayout.SetLayoutFlags(BlankButton, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(BlankButton, new Rectangle(0.1, 0.65 + 0.12 * 3, 0.25, 0.1));
			MainLayout.Children.Add(BlankButton);

			DeleteButton = new Button {
				Text = "Delete",
				BackgroundColor = Color.FromRgb(100, 100, 100),
				BorderWidth = 0,
				BorderRadius = 0,
				Font = Font.SystemFontOfSize(NamedSize.Medium),
			};

			DeleteButton.Clicked += (object sender, EventArgs e) => {
				if (curIndex >= 0 && curIndex < 4)
				{
					if (curIndex == 0)
					{
						PassCodeStrings[curIndex] = "";
						PassCodeImageList[curIndex].Source = ImageSource.FromFile("");
					}
					else{
						PassCodeStrings[curIndex - 1] = "";
						PassCodeImageList[curIndex - 1].Source = ImageSource.FromFile("");
					}
					curIndex--;
				}

				if (curIndex < 0)
					curIndex = 0;
			};

			AbsoluteLayout.SetLayoutFlags(DeleteButton, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(DeleteButton, new Rectangle(0.9, 0.65 + 0.12 * 3, 0.25, 0.1));
			MainLayout.Children.Add(DeleteButton);

			var PassCodeOneBackButton = new Button {
				Text = "",
				BorderColor = Color.FromRgb (171, 146, 91),
				BackgroundColor = Color.Transparent,
				BorderWidth = 1,
				BorderRadius = 1,
			};
			AbsoluteLayout.SetLayoutFlags(PassCodeOneBackButton, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(PassCodeOneBackButton, new Rectangle(0.1, 0.35, 0.2, 0.1));
			MainLayout.Children.Add(PassCodeOneBackButton);

			PasscodeOneImage = new DarkIceImage {
				Aspect = Aspect.AspectFit,
			};
			PassCodeImageList.Add(PasscodeOneImage);

			AbsoluteLayout.SetLayoutFlags(PasscodeOneImage, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(PasscodeOneImage, new Rectangle(0.17, 0.35, 0.05, 0.1));
			MainLayout.Children.Add(PasscodeOneImage);

			var PassCodeTwoBackButton = new Button {
				Text = "",
				BorderColor = Color.FromRgb (171, 146, 91),
				BackgroundColor = Color.Transparent,
				BorderWidth = 1,
				BorderRadius = 1,
			};
			AbsoluteLayout.SetLayoutFlags(PassCodeTwoBackButton, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(PassCodeTwoBackButton, new Rectangle(0.37, 0.35, 0.2, 0.1));
			MainLayout.Children.Add(PassCodeTwoBackButton);

			PasscodeTwoImage = new DarkIceImage {
				Aspect = Aspect.AspectFit,
			};
			PassCodeImageList.Add(PasscodeTwoImage);

			AbsoluteLayout.SetLayoutFlags(PasscodeTwoImage, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(PasscodeTwoImage, new Rectangle(0.4, 0.35, 0.05, 0.1));
			MainLayout.Children.Add(PasscodeTwoImage);

			var PassCodeThreeBackButton = new Button {
				Text = "",
				BorderColor = Color.FromRgb (171, 146, 91),
				BackgroundColor = Color.Transparent,
				BorderWidth = 1,
				BorderRadius = 1
			};
			AbsoluteLayout.SetLayoutFlags(PassCodeThreeBackButton, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(PassCodeThreeBackButton, new Rectangle(0.64, 0.35, 0.2, 0.1));
			MainLayout.Children.Add(PassCodeThreeBackButton);

			PasscodeThreeImage = new DarkIceImage {
				Aspect = Aspect.AspectFit,
			};
			PassCodeImageList.Add(PasscodeThreeImage);

			AbsoluteLayout.SetLayoutFlags(PasscodeThreeImage, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(PasscodeThreeImage, new Rectangle(0.62, 0.35, 0.05, 0.1));
			MainLayout.Children.Add(PasscodeThreeImage);

			var PassCodeFourBackButton = new Button {
				Text = "",
				BorderColor = Color.FromRgb (171, 146, 91),
				BackgroundColor = Color.Transparent,
				BorderWidth = 1,
				BorderRadius = 1
			};
			AbsoluteLayout.SetLayoutFlags(PassCodeFourBackButton, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(PassCodeFourBackButton, new Rectangle(0.91, 0.35, 0.2, 0.1));
			MainLayout.Children.Add(PassCodeFourBackButton);

			PasscodeFourImage = new DarkIceImage {
				Aspect = Aspect.AspectFit,
			};
			PassCodeImageList.Add(PasscodeFourImage);

			AbsoluteLayout.SetLayoutFlags(PasscodeFourImage, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(PasscodeFourImage, new Rectangle(0.85, 0.35, 0.05, 0.1));
			MainLayout.Children.Add(PasscodeFourImage);
		}
        #endregion
    }
}
