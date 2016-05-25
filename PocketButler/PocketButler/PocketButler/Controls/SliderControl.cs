using System;
using Xamarin.Forms;
using System.Collections.Generic;

namespace PocketButler
{
	public class SliderControl : View
	{
		public List<MenuCategory> ListItems { get; set; }

		public PocketButler.Utils.CategoryItemClickedDelegate CategoryClicked { get; set; }
		public PocketButler.Utils.CategorySubItemClickedDelegate CategorySubItemClicked { get; set; }
		public PocketButler.Utils.CategoryFavItemClickedDelegate CategoryFavItemClicked { get; set; }
		public PocketButler.Utils.CategoryAddItemClickedDelegate CategoryAddItemClicked { get; set; }
		public bool is_online { get; set; }
		public SliderControl ()
		{
		}

		public void NotifyItemSelected(int selectedIndex){
			try{
				//if(ListItems[selectedIndex].Tapped != null){
				//	ListItems[selectedIndex].Tapped.Invoke();
				//}
			}catch(IndexOutOfRangeException ex) {
				System.Diagnostics.Debug.WriteLine (ex.Message);
			}
		}

		public static readonly BindableProperty NavRightProperty =
			BindableProperty.Create<SliderControl, bool> (
				p => p.NavRight, false);

		public bool NavRight {
			get {
				return (bool)GetValue (NavRightProperty);
			}

			set {
				this.SetValue (NavRightProperty, value);
			}
		}

		public static readonly BindableProperty NavLeftProperty =
			BindableProperty.Create<SliderControl, bool> (
				p => p.NavLeft, false);

		public bool NavLeft {
			get {
				return (bool)GetValue (NavLeftProperty);
			}

			set {
				this.SetValue (NavLeftProperty, value);
			}
		}

		public static readonly BindableProperty CategoryIndexChangedProperty =
			BindableProperty.Create<SliderControl, bool> (
				p => p.CategoryIndexChanged, false);

		public bool CategoryIndexChanged {
			get {
				return (bool)GetValue (CategoryIndexChangedProperty);
			}

			set {
				this.SetValue (CategoryIndexChangedProperty, value);
			}
		}

		public int CategoryIndex = -1;

		public static readonly BindableProperty RefreshFlagProperty =
			BindableProperty.Create<SliderControl, bool> (
				p => p.RefreshFlag, false);

		public bool RefreshFlag {
			get {
				return (bool)GetValue (RefreshFlagProperty);
			}

			set {
				this.SetValue (RefreshFlagProperty, value);
			}
		}
	}
}

