using System;
using Xamarin.Forms.Platform.Android;
using Android.Support.V4.View;
using Xamarin.Forms;
using PocketButler;
using PocketButler.Droid.Renderer;
using Android.Widget;
using Android.Content;
using System.Collections.Generic;
using Android.App;
using UrlImageViewHelper;
using ViewPagerIndicator;
using System.Threading;
using Xamarin;

[assembly: ExportRenderer (typeof(SliderControl), typeof(SliderControlRenderer))]
namespace PocketButler.Droid.Renderer
{
	public class SliderControlRenderer : ViewRenderer
	{
		bool _disposed;

		ViewPager _viewPager;
		public CircleIndicatorView circleIndicator;
		public SliderControl slidercontrol;
		public static bool is_online;
		public static global::Android.Widget.ExpandableListView CurListView = null;
		public static ExpandableListAdapter ListViewAdapter = null;

		public static global::Android.Widget.ExpandableListView[] ListViewArr;
		public static ExpandableListAdapter[] ListViewAdapterArr;
		static long lastEventTickTime = 0;

		public class MyPageChangeListener : Java.Lang.Object, ViewPager.IOnPageChangeListener
		{
			Context _context;
			ViewPager _viewPager;
			CircleIndicatorView _circleIndicator;
			SliderControl _slidercontrol;

			public MyPageChangeListener (Context context, CircleIndicatorView circleIndicator, SliderControl sliderControl, ViewPager viewPager)
			{
				_context = context;	
				_circleIndicator = circleIndicator;
				_slidercontrol = sliderControl;
				_viewPager = viewPager;
			}

			#region IOnPageChangeListener implementation
			public void OnPageScrollStateChanged (int state)
			{
				if (state == ViewPager.ScrollStateIdle)
				{
					if (_viewPager.Adapter.Count > 1) {
						int curr = _viewPager.CurrentItem;
						int lastReal = _viewPager.Adapter.Count - 2;
						if (curr == 0) {
							_viewPager.SetCurrentItem(lastReal, false);
						} else if (curr > lastReal) {
							_viewPager.SetCurrentItem(1, false);
						}
					}
				}
			}

			public void OnPageScrolled (int position, float positionOffset, int positionOffsetPixels)
			{
				if (_viewPager.Adapter.Count > 1)
					position--;
				_circleIndicator.ChangeEnablePosition (position);
				_slidercontrol.CategoryIndex = position;
			}

			public void OnPageSelected (int position)
			{

			}
			#endregion
		}

		public SliderControlRenderer ()
		{
		}

		protected override void OnElementChanged (ElementChangedEventArgs<View> e)
		{
			base.OnElementChanged (e);

			var sliderElement = Element as SliderControl;
			slidercontrol = sliderElement;
			is_online = slidercontrol.is_online;

			ListViewArr = new ExpandableListView[sliderElement.ListItems.Count + 2];
			ListViewAdapterArr = new ExpandableListAdapter[sliderElement.ListItems.Count + 2];

			_viewPager = new ViewPager (base.Context);
			circleIndicator = new CircleIndicatorView (base.Context, sliderElement.ListItems.Count);
			SlidePagerAdapter adapter = new SlidePagerAdapter (base.Context, slidercontrol); 
			adapter.CategoryClicked = slidercontrol.CategoryClicked;
			adapter.CategorySubItemClicked = slidercontrol.CategorySubItemClicked;
			adapter.CategoryFavItemClicked = slidercontrol.CategoryFavItemClicked;
			adapter.CategoryAddItemClicked = slidercontrol.CategoryAddItemClicked;
			_viewPager.Adapter = adapter;
			_viewPager.SetFadingEdgeLength (0);
			_viewPager.OverScrollMode = global::Android.Views.OverScrollMode.Never;
			_viewPager.PageScrolled += (object sender, ViewPager.PageScrolledEventArgs ex) => {
				circleIndicator.ChangeEnablePosition(ex.Position);
			};
			_viewPager.PageScrollStateChanged += (object sender, ViewPager.PageScrollStateChangedEventArgs ex1) => {
				if (ex1.State == ViewPager.ScrollStateIdle)
				{
					circleIndicator.ChangeEnablePosition(_viewPager.CurrentItem);
					/*int curr = _viewPager.CurrentItem;
                    int lastReal = _viewPager.Adapter.Count - 2;
                    if (curr == 0) {
                        _viewPager.SetCurrentItem(lastReal, false);
                    } else if (curr > lastReal) {
                        _viewPager.SetCurrentItem(1, false);
                    }*/					
				}
			};
			
			TitlePageIndicator pageIndicator = new TitlePageIndicator (base.Context);
			pageIndicator.SetViewPager (_viewPager, (sliderElement.ListItems.Count > 1) ? 1 : 0);
			pageIndicator.SetOnPageChangeListener (new MyPageChangeListener (base.Context, circleIndicator, slidercontrol, _viewPager));

			Android.Widget.RelativeLayout relativeNativeControl = new Android.Widget.RelativeLayout (base.Context);
			LinearLayout pagerLayout = new LinearLayout (base.Context);
			pagerLayout.Orientation = Orientation.Vertical;

			pagerLayout.AddView (pageIndicator);
			pagerLayout.AddView (_viewPager);
			pagerLayout.AddView (circleIndicator);

			Android.Widget.LinearLayout.LayoutParams param = (Android.Widget.LinearLayout.LayoutParams)_viewPager.LayoutParameters;
			param.Weight = 1.0f;
			_viewPager.LayoutParameters = param;

			relativeNativeControl.AddView (pagerLayout);

			/*relativeNativeControl.AddView (circleIndicator);

            Android.Widget.RelativeLayout.LayoutParams param = (Android.Widget.RelativeLayout.LayoutParams)circleIndicator.LayoutParameters;
            param.BottomMargin = (int)(Resources.DisplayMetrics.Density * 35);
            circleIndicator.LayoutParameters = param;*/

			SetNativeControl (relativeNativeControl);

			circleIndicator.ChangeEnablePosition (0);
		}

		protected override void OnElementPropertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged (sender, e);

			switch (e.PropertyName) {

			// TODO: Need to add in navigation

			case "NavRight":
				{
					int scrollToPage = _viewPager.CurrentItem + 1;
					// Center things again
					_viewPager.SetCurrentItem(scrollToPage, true);
				}
				break;

			case "NavLeft":
				{
					int scrollToPage = _viewPager.CurrentItem - 1;
					// Center things again
					_viewPager.SetCurrentItem (scrollToPage, true);
				}
				break;
			case "CategoryIndexChanged":
				{
					_viewPager.SetCurrentItem (slidercontrol.CategoryIndex, true);
				}
				break;
			case "RefreshFlag":
				{
					/*
					SlidePagerAdapter adapter = new SlidePagerAdapter (base.Context, slidercontrol); 
					adapter.CategoryClicked = slidercontrol.CategoryClicked;
					adapter.CategorySubItemClicked = slidercontrol.CategorySubItemClicked;
					adapter.CategoryFavItemClicked = slidercontrol.CategoryFavItemClicked;
					adapter.CategoryAddItemClicked = slidercontrol.CategoryAddItemClicked;
					_viewPager.Adapter = adapter;
					_viewPager.SetFadingEdgeLength (0);
					_viewPager.OverScrollMode = global::Android.Views.OverScrollMode.Never;*/
					for (int i = 0; i < ListViewAdapterArr.Length; i++) {
						try{
							if (ListViewAdapterArr[i] != null)
							{
								ListViewAdapterArr[i].NotifyDataSetChanged();
							}
						}
						catch (Exception ex) {
							Insights.Report (ex);
						}
					}
				}
				break;
			default:
				System.Diagnostics.Debug.WriteLine ("Property change for {0} has not been implemented.", e.PropertyName);
				break;
			}
		}

		class SlidePagerAdapter : PagerAdapter, TitleProvider{

			global::Android.Content.Context Context;
			SliderControl Element;
			public PocketButler.Utils.CategoryItemClickedDelegate CategoryClicked;
			public PocketButler.Utils.CategorySubItemClickedDelegate CategorySubItemClicked;
			public PocketButler.Utils.CategoryFavItemClickedDelegate CategoryFavItemClicked;
			public PocketButler.Utils.CategoryAddItemClickedDelegate CategoryAddItemClicked;

			public SlidePagerAdapter(global::Android.Content.Context context, SliderControl element){
				Context = context;
				Element = element;
			}

			public override int GetItemPosition (Java.Lang.Object @object)
			{
				return base.GetItemPosition (@object);
			}

			public override Java.Lang.Object InstantiateItem (global::Android.Views.ViewGroup container, int position)
			{
				global::Android.Widget.ExpandableListView view = new global::Android.Widget.ExpandableListView(Context);
				//view.TranscriptMode = TranscriptMode.AlwaysScroll;
				//view.SetClipChildren (false);
				CurListView = view;

				if (Element.ListItems.Count > 0) {
					int realPosition = position;
					if (Count == 1) {
						realPosition = position;
					} else {
						if (position == 0)
							realPosition = Element.ListItems.Count - 1;
						else if (position == Count - 1)
							realPosition = 0;
						else
							realPosition = position - 1;
					}

					List<SubCategory> _subcategoryList = new List<SubCategory> ();
					MenuCategory _category = Element.ListItems [realPosition];
					if (_category.is_subcat_exists.Equals("1")){
						foreach (SubCategory _subcategory in _category.subcategories) {
							_subcategoryList.Add (_subcategory);
						}

						if (_category.is_menuitem_exist.Equals ("1")) {
							SubCategory _subcategory = new SubCategory ();
							_subcategory.sub_category_id = "";
							_subcategory.sub_category_name = "General";
							_subcategory.menuitems = _category.menuitems;

							_subcategoryList.Add (_subcategory);
						}
					}
					else if (_category.is_menuitem_exist.Equals ("1")) {
						SubCategory _subcategory = new SubCategory ();
						_subcategory.sub_category_id = "";
						_subcategory.sub_category_name = "";
						_subcategory.menuitems = _category.menuitems;

						_subcategoryList.Add (_subcategory);
					}


					ListViewAdapter = new ExpandableListAdapter (view.Context, _subcategoryList, _category.category_id, CategorySubItemClicked, CategoryFavItemClicked, CategoryAddItemClicked);
					view.SetAdapter(ListViewAdapter);
					view.SetGroupIndicator(null);
					view.GroupCollapse += (object sender, ExpandableListView.GroupCollapseEventArgs e) => {
						var senderView = sender as ExpandableListView;
						int selected = e.GroupPosition;
					};

					view.GroupExpand += (object sender, ExpandableListView.GroupExpandEventArgs e) => {
						var senderView = sender;
						int selected = e.GroupPosition;
					};

					if (_subcategoryList.Count > 0)
						view.ExpandGroup (0);

					//if (CategoryClicked != null)
					//	CategoryClicked.Invoke(_category.category_id, _category.category_name);
				}

				ListViewArr [position] = view;
				ListViewAdapterArr [position] = ListViewAdapter;
				container.AddView (view);
				return view;
			}

			public string GetTitle (int position)
			{
				if (Element.ListItems.Count <= 0) {
					return "";
				} else if (Element.ListItems.Count == 1) {
					return Element.ListItems [position].category_name;
				}
				else {
					if (position == 0)
						return Element.ListItems [Element.ListItems.Count - 1].category_name;
					else if (position == Count - 1)
						return Element.ListItems [0].category_name;

					return Element.ListItems [position - 1].category_name;
				}
			}

			class GlobalLayoutListener : Java.Lang.Object, global::Android.Views.ViewTreeObserver.IOnGlobalLayoutListener
			{
				Action on_global_layout;

				public GlobalLayoutListener (Action onGlobalLayout)
				{
					on_global_layout = onGlobalLayout;
				}

				public void OnGlobalLayout ()
				{
					on_global_layout ();
				}
			}

			public override int Count {
				get {
					if (Element == null || Element.ListItems == null)
						return 0;
					if (Element.ListItems.Count == 0)
						return 0;
					if (Element.ListItems.Count == 1)
						return 1;
					return Element.ListItems.Count + 2;
				}
			}

			public override bool IsViewFromObject (global::Android.Views.View view, Java.Lang.Object @object)
			{
				return view == @object;
			}

			public override void DestroyItem (global::Android.Views.ViewGroup container, int position, Java.Lang.Object @object)
			{
				((ViewPager)container).RemoveView ((global::Android.Views.View)@object);
				//base.DestroyItem (container, position, @object);
			}
		}

		public class ExpandableListAdapter : BaseExpandableListAdapter
		{
			List<SubCategory> items;
			Context context;
			String category_id;
			public PocketButler.Utils.CategorySubItemClickedDelegate CategorySubItemClicked;
			public PocketButler.Utils.CategoryFavItemClickedDelegate CategoryFavItemClicked;
			public PocketButler.Utils.CategoryAddItemClickedDelegate CategoryAddItemClicked;

			public ExpandableListAdapter(Context context, List<SubCategory> items, String categoryid, PocketButler.Utils.CategorySubItemClickedDelegate CategorySubItemClicked,
				PocketButler.Utils.CategoryFavItemClickedDelegate CategoryFavItemClicked, PocketButler.Utils.CategoryAddItemClickedDelegate CategoryAddItemClicked) : base()
			{
				this.context = context;
				this.items = items;
				this.category_id = categoryid;
				this.CategorySubItemClicked = CategorySubItemClicked;
				this.CategoryFavItemClicked = CategoryFavItemClicked;
				this.CategoryAddItemClicked = CategoryAddItemClicked;
			}

			public override Java.Lang.Object GetChild(int groupPosition, int childPosition)
			{
				return items[groupPosition].menuitems[childPosition].item_name;
				//return null;
			}

			public override long GetChildId(int groupPosition, int childPosition)
			{
				return childPosition;
			}

			public override int GetChildrenCount(int groupPosition)
			{
				if (items [groupPosition].menuitems == null)
					return 0;

				return items[groupPosition].menuitems.Count;
			}

			public override Android.Views.View GetChildView(int groupPosition, int childPosition, bool isLastChild, Android.Views.View convertView, Android.Views.ViewGroup parent)
			{
				var view = convertView;

				if (view == null)
				{
					var inflater = context.GetSystemService(Context.LayoutInflaterService) as Android.Views.LayoutInflater;
					view = inflater.Inflate(Resource.Layout.EntryItem, null);
				}

				//setup childview
				var item = items [groupPosition].menuitems[childPosition];

				if (String.IsNullOrEmpty (item.item_image) == false) {
					try{
						view.FindViewById<ImageView> (Resource.Id.imgItem).SetUrlDrawable(item.item_image);
					}
					catch (Exception ex) {
						Insights.Report (ex);
					}
				}

				try{
					String itemName = String.IsNullOrEmpty(item.item_name) ? "" : item.item_name;
					String itemPrice = String.IsNullOrEmpty(item.item_price) ? "" : item.item_price;
					itemPrice = String.IsNullOrEmpty(item.currency_symbol) ? itemPrice : itemPrice + " " + item.currency_symbol;

					view.FindViewById<TextView> (Resource.Id.lblItemName).Text = itemName;
					view.FindViewById<TextView> (Resource.Id.lblItemPrice).Text = itemPrice;

					view.FindViewById<ImageView> (Resource.Id.imgItem).Click += (object sender, EventArgs e) => {
						long curTickTime = DateTime.Now.Ticks / 1000 / 1000;
						if (Math.Abs(curTickTime - lastEventTickTime) < 3)
							return;
						lastEventTickTime = curTickTime;
						var parentView = ((sender as Android.Views.View).Parent) as Android.Widget.RelativeLayout;
						var quickBarView = parentView.FindViewById<Android.Widget.RelativeLayout>(Resource.Id.RLQuickAddBar);
						quickBarView.Visibility = (quickBarView.Visibility == Android.Views.ViewStates.Visible) ? Android.Views.ViewStates.Gone : Android.Views.ViewStates.Visible;

						if (quickBarView.Visibility == Android.Views.ViewStates.Visible)
						{
							var curView = parentView;//CurListView.GetChildAt(groupPosition, childPosition);
							Android.Graphics.Rect r = new Android.Graphics.Rect(0, 0, curView.Width, curView.Height);
							double height = curView.Height * 2;

							try{
								CurListView.GetChildVisibleRect(curView, r, null);
								Android.Graphics.Rect rList = new Android.Graphics.Rect();
								CurListView.GetLocalVisibleRect(rList);

								if (r.Top + height >= rList.Bottom)
								{
									//CurListView.SmoothScrollToPosition(CurListView.LastVisiblePosition);
									//CurListView.CollapseGroup(groupPosition);
									//CurListView.ExpandGroup(groupPosition);
									//CurListView.SmoothScrollBy(100, 100);
								}
							}
							catch (Exception ex)
							{
								Insights.Report (ex);
							}
						}
					};

					var ImgFavourite = view.FindViewById<ImageView> (Resource.Id.imgFavourites);
					ImgFavourite.SetTag (Resource.String.group_position, groupPosition);
					ImgFavourite.SetTag (Resource.String.child_position, childPosition);
					ImgFavourite.SetImageResource(item.is_favourite.Equals("1") ? Resource.Drawable.favorite_on : Resource.Drawable.favorite_off);
					ImgFavourite.Click += (object sender, EventArgs e) => {
						if (CategoryFavItemClicked != null)
						{
							long curTickTime = DateTime.Now.Ticks / 1000 / 1000;
							if (Math.Abs(curTickTime - lastEventTickTime) < 3)
								return;
							lastEventTickTime = curTickTime;

							int gPosition = (int)((sender as Android.Views.View).GetTag(Resource.String.group_position));
							int cPosition = (int)((sender as Android.Views.View).GetTag(Resource.String.child_position));
							MenuItem data = items[gPosition].menuitems[cPosition];
							CategoryFavItemClicked.Invoke(category_id, data.item_id, data);
							if (data.is_favourite.Equals("1"))
							{
								data.is_favourite = "0";
								var imgView = sender as ImageView;
								imgView.SetImageResource(Resource.Drawable.favorite_off);
							}
							else
							{
								data.is_favourite = "1";
								var imgView = sender as ImageView;
								imgView.SetImageResource(Resource.Drawable.favorite_on);
							}
						}
					};

					if (is_online == true) {
						int count = App._DbManager.GetSameTypeCount (Globals.Config.RestaurantId, item.item_id);
						if (count > 0)
							view.FindViewById<TextView> (Resource.Id.txtItemCount).Text = "Quantity Added : " + count.ToString ();
						else
							view.FindViewById<TextView> (Resource.Id.txtItemCount).Text = "";

						var ButtonQuickAdd = view.FindViewById<Android.Widget.Button> (Resource.Id.btnQuickAdd);
						ButtonQuickAdd.SetTag (Resource.String.group_position, groupPosition);
						ButtonQuickAdd.SetTag (Resource.String.child_position, childPosition);
						ButtonQuickAdd.Click += async (object sender, EventArgs e) => {
							if (CategoryAddItemClicked != null) {
								long curTickTime = DateTime.Now.Ticks / 1000 / 1000;
								if (Math.Abs(curTickTime - lastEventTickTime) < 3)
									return;
								lastEventTickTime = curTickTime;

								int gPosition = (int)((sender as Android.Views.View).GetTag(Resource.String.group_position));
								int cPosition = (int)((sender as Android.Views.View).GetTag(Resource.String.child_position));
								MenuItem data = items[gPosition].menuitems[cPosition];
								await CategoryAddItemClicked.Invoke (category_id, data.item_id, data);
								count = App._DbManager.GetSameTypeCount (Globals.Config.RestaurantId, data.item_id);
								var parentView = ((sender as Android.Views.View).Parent) as Android.Widget.RelativeLayout;
								if (count > 0)
									parentView.FindViewById<TextView> (Resource.Id.txtItemCount).Text = "Quantity Added : " + count.ToString ();
								else
									parentView.FindViewById<TextView> (Resource.Id.txtItemCount).Text = "";								

								var quickBarView = parentView.FindViewById<Android.Widget.RelativeLayout>(Resource.Id.RLQuickAddBar);
								quickBarView.Visibility = Android.Views.ViewStates.Visible;

								if (quickBarView.Visibility == Android.Views.ViewStates.Visible)
								{
									var curView = parentView;//CurListView.GetChildAt(groupPosition, childPosition);
									Android.Graphics.Rect r = new Android.Graphics.Rect(0, 0, curView.Width, curView.Height);
									double height = curView.Height * 2;

									try{
										CurListView.GetChildVisibleRect(curView, r, null);
										Android.Graphics.Rect rList = new Android.Graphics.Rect();
										CurListView.GetLocalVisibleRect(rList);

										if (r.Top + height >= rList.Bottom)
										{
											//CurListView.SmoothScrollToPosition(CurListView.LastVisiblePosition);
											//CurListView.CollapseGroup(groupPosition);
											//CurListView.ExpandGroup(groupPosition);
											//CurListView.SmoothScrollBy(100, 100);
										}
									}
									catch (Exception ex)
									{
										Insights.Report (ex);
									}
								}
							}
						};
					} else {
						view.FindViewById<TextView> (Resource.Id.txtItemCount).Visibility = Android.Views.ViewStates.Invisible;
						view.FindViewById<Android.Widget.Button> (Resource.Id.btnQuickAdd).Visibility = Android.Views.ViewStates.Invisible;
					}

					var RootRelativeView = (Android.Widget.RelativeLayout)view;
					RootRelativeView.SetTag (Resource.String.group_position, groupPosition);
					RootRelativeView.SetTag (Resource.String.child_position, childPosition);
					RootRelativeView.Click += (object sender, EventArgs e) => {
						if (CategorySubItemClicked != null)
						{
							long curTickTime = DateTime.Now.Ticks / 1000 / 1000;
							if (Math.Abs(curTickTime - lastEventTickTime) < 3)
								return;
							lastEventTickTime = curTickTime;

							int gPosition = (int)((sender as Android.Views.View).GetTag(Resource.String.group_position));
							int cPosition = (int)((sender as Android.Views.View).GetTag(Resource.String.child_position));
							MenuItem data = items[gPosition].menuitems[cPosition];
							if (data.IsPopularCategory == true)
								CategorySubItemClicked.Invoke(data.Category_Id, data.item_id, data);
							else
								CategorySubItemClicked.Invoke(category_id, data.item_id, data);
						}
					};
				}
				catch (Exception ex) {
				}

				return view;
			}

			public override Java.Lang.Object GetGroup(int groupPosition)
			{
				return items[groupPosition].sub_category_name;
			}

			public override long GetGroupId(int groupPosition)
			{
				return groupPosition;
			}

			public override Android.Views.View GetGroupView(int groupPosition, bool isExpanded, Android.Views.View convertView, Android.Views.ViewGroup parent)
			{
				var view = convertView;

				if (items.Count <= 1) {
					if (view == null)
					{
						var inflater = context.GetSystemService(Context.LayoutInflaterService) as Android.Views.LayoutInflater;
						view = inflater.Inflate(Resource.Layout.BlankItem, null);
					}
				} else {
					if (view == null)
					{
						var inflater = context.GetSystemService(Context.LayoutInflaterService) as Android.Views.LayoutInflater;
						view = inflater.Inflate(Resource.Layout.GroupHeaderItem, null);
					}

					//setup groupview
					var item = items [groupPosition].sub_category_name;
					view.FindViewById<TextView> (Resource.Id.lblGroupName).Text = item;
					view.FindViewById<TextView> (Resource.Id.lblExpanded).Text = (isExpanded) ? "-" : "+";
					/*
					((Android.Widget.RelativeLayout)view).Click += (object sender, EventArgs e) => {

						String curText = view.FindViewById<TextView> (Resource.Id.lblExpanded).Text;
						view.FindViewById<TextView> (Resource.Id.lblExpanded).Text = curText.Equals("+") ? "-" : "+";
					};*/
				}

				return view;
			}

			public override bool IsChildSelectable(int groupPosition, int childPosition)
			{
				return true;
			}

			public override int GroupCount
			{
				get { return items.Count; }
			}

			public override bool HasStableIds
			{
				get { return true; }
			}
		}

		const int IMAGE_START_INDEX = 0x7F7F7F7F;
		public class CircleIndicatorView : Android.Widget.RelativeLayout
		{
			ImageView[] itemArr;
			int count;

			public CircleIndicatorView(Context context, int count) : base(context)
			{
				this.count = count;
				this.SetGravity(Android.Views.GravityFlags.Center);
				LayoutParams layout_params = new LayoutParams(MarginLayoutParams.MatchParent, 30);
				layout_params.AddRule(LayoutRules.AlignParentBottom);
				this.LayoutParameters = layout_params;

				if (count > 0)
				{
					itemArr = new ImageView[count];
					for (int i = 0; i < count; i++)
					{
						itemArr[i] = new ImageView(context);
						itemArr[i].Id = IMAGE_START_INDEX + i;
						itemArr[i].SetImageResource(Resource.Drawable.progress_gray);
						LayoutParams layout_image_params = new LayoutParams(20, 20);
						if (i > 0)
						{
							layout_image_params.AddRule(LayoutRules.RightOf, itemArr[i - 1].Id);
							layout_image_params.LeftMargin = 30;
						}
						itemArr[i].LayoutParameters = layout_image_params;
						this.AddView(itemArr[i]);
					}
				}				
			}

			public void ChangeEnablePosition(int id)
			{
				if (count > 0) {
					if ((id >= 0) && (id < count)) {
						for (int i = 0; i < count; i++)
						{
							itemArr[i].SetImageResource(Resource.Drawable.progress_gray);
						}
						itemArr [id].SetImageResource (Resource.Drawable.progress_yellow);
					}
				}
			}
		}

		#region IDisposable implementation
		protected override void Dispose (bool disposing)
		{
			if (_disposed)
				return;

			if (_viewPager != null)
				_viewPager.Dispose ();

			_disposed = true;
		}

		#endregion
	}
}

