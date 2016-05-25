using System;

namespace PocketButler
{
	public interface IPageLoader
	{
		void StartApp();

		void ShowMainPage();
		void ShowTypeSliderPage(RestaurantInfo info);
		void Logout();
		void StartIntent(String newUrl);
		void FacebookLogin();
	}
}

