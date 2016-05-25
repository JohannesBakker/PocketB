using System;
using Xamarin.Forms;

namespace PocketButler.Controls
{
	public class DarkIceImage : Image
	{

		public DarkIceImage ()
		{
		}

		public string ImageName { get; set; }

		public int Tag { get; set; }
		public bool IsEnablePadding { get; set; }
		public Action Tapped { get; set; }
		public bool IsDefaultImage { get; set; }
		public bool IsShowBorder { get; set; }

		public String RestaurantId {
			get {
				return (String)GetValue (RestaurantIdProperty);
			}

			set {
				this.SetValue (RestaurantIdProperty, value);
			}
		}

		public String MenuId {
			get {
				return (String)GetValue (MenuIdProperty);
			}

			set {
				this.SetValue (MenuIdProperty, value);
			}
		}

		public bool IsShortCut {
			get {
				return (bool)GetValue (IsShortCutProperty);
			}

			set {
				this.SetValue (IsShortCutProperty, value);
			}
		}

		public Utils.FavDeleteClickedDelegate DeleteDelegate { get; set; }

		public bool IsSwipeAction { get; set; }
		public bool IsShortCutList { get; set; }
		public static readonly BindableProperty RestaurantIdProperty =
			BindableProperty.Create<DarkIceImage, String> (
				p => p.RestaurantId, "");

		public static readonly BindableProperty MenuIdProperty =
			BindableProperty.Create<DarkIceImage, String> (
				p => p.MenuId, "");

		public static readonly BindableProperty IsShortCutProperty =
			BindableProperty.Create<DarkIceImage, bool> (
				p => p.IsShortCut, false);

		public static readonly BindableProperty ShouldDisposeImageProperty =
			BindableProperty.Create<DarkIceImage, bool> (
				p => p.ShouldDisposeImage, false);


		public bool ShouldDisposeImage {
			get {
				return (bool)GetValue (ShouldDisposeImageProperty);
			}

			set {
				this.SetValue (ShouldDisposeImageProperty, value);
			}
		}

		public bool disableDispose { get; set; }
		public static readonly BindableProperty WidthRequestImageProperty = 
			BindableProperty.Create<DarkIceImage, double>(
				p => p.WidthRequestImage, 0);

		public double WidthRequestImage{
			get {
				return (double)GetValue (WidthRequestImageProperty);
			}
			set {
				this.SetValue (WidthRequestImageProperty, value);
			}
		}

		public static readonly BindableProperty DataItemIDProperty =
			BindableProperty.Create<DarkIceImage, String> (
				p => p.ItemID, "");

		public String ItemID {
			get {
				return (String)GetValue (DataItemIDProperty);
			}

			set {
				this.SetValue (DataItemIDProperty, value);
			}
		}

		public static readonly BindableProperty DataItemTypeProperty =
			BindableProperty.Create<DarkIceImage, String> (
				p => p.ItemID, "0");

		public String ItemType {
			get {
				return (String)GetValue (DataItemTypeProperty);
			}

			set {
				this.SetValue (DataItemTypeProperty, value);
			}
		}

	}

	public static class DarkIceImageExtensions
	{

		public static void DisposeImageIfNotNull (this DarkIceImage item)
		{
			if (item != null)
				item.ShouldDisposeImage = true;
		}
	}
}

