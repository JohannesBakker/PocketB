using System;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Threading;

namespace PocketButler
{
	public class CustomNavigationPage: NavigationPage
	{
		public bool Animated { get; private set; }

		public CustomNavigationPage(Page page) : base(page)
		{
		}

		// Analysis disable once MethodOverloadWithOptionalParameter
		public async Task PushAsync(Page page, bool animated = true)
		{
			Animated = animated;
			await base.PushAsync(page);
			await Task.Run(delegate {
				Thread.Sleep(5);
			});
		}

		// Analysis disable once MethodOverloadWithOptionalParameter
		public async Task<Page> PopAsync(bool animated = true)
		{
			Animated = animated;
			var task = await base.PopAsync();
			await Task.Run(delegate {
				Thread.Sleep(5);
			});
			return task;
		}
	}
}

