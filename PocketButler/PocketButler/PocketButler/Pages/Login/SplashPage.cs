using System;
using Xamarin.Forms;
using PocketButler.Controls;
using System.Threading.Tasks;
using System.Threading;

namespace PocketButler
{
	public class SplashPage : BasePage
	{
		public SplashPage ()
		{
			BackgroundColor = Color.White;

			Content = new DarkIceImage {
				Source = ImageSource.FromFile("splash.png"),
				Aspect = Aspect.Fill,
			};
		}

		protected override async void OnAppearing ()
		{
			base.OnAppearing ();

			await Task.Run (() => {
				Thread.Sleep (1000);
			});

			App.PageLoaderManager.StartApp ();
		}
	}
}

