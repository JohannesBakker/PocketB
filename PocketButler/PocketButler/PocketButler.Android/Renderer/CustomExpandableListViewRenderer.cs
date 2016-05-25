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

[assembly: ExportRenderer (typeof(CustomExpandableListView), typeof(CustomExpandableListViewRenderer))]
namespace PocketButler.Droid.Renderer
{
	public class CustomExpandableListViewRenderer : ViewRenderer
	{
		bool _disposed;
		static public CustomExpandableListView listViewControl;
		static global::Android.Widget.ExpandableListView expandableListView;
		static ExpandableListAdapter adapter;
		static long lastEventTickTime = 0;

		public CustomExpandableListViewRenderer ()
		{
		}

		protected override void OnElementChanged (ElementChangedEventArgs<View> e)
		{
			base.OnElementChanged (e);

			var listViewElement = Element as CustomExpandableListView;
			listViewControl = listViewElement;

			Android.Widget.RelativeLayout relativeNativeControl = new Android.Widget.RelativeLayout (base.Context);
			LinearLayout pagerLayout = new LinearLayout (base.Context);
			pagerLayout.Orientation = Orientation.Vertical;

			expandableListView = new global::Android.Widget.ExpandableListView(base.Context);

			Android.Views.ViewGroup.LayoutParams layout_params = new Android.Views.ViewGroup.LayoutParams (Android.Views.ViewGroup.LayoutParams.MatchParent, Android.Views.ViewGroup.LayoutParams.MatchParent);
			expandableListView.LayoutParameters = layout_params;

			adapter = new ExpandableListAdapter (base.Context, listViewControl.ListData);
			expandableListView.SetAdapter(adapter);
			expandableListView.SetGroupIndicator(null);

			pagerLayout.AddView (expandableListView);
			relativeNativeControl.AddView (pagerLayout);

			SetNativeControl (relativeNativeControl);

			if (listViewControl.IsExpandDefault) {
				if (listViewControl != null && listViewControl.ListData != null) {
					for (int i = 0; i < listViewControl.ListData.Count; i++)
						expandableListView.ExpandGroup (i);
				}
			}
		}

		protected override void OnElementPropertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged (sender, e);

			switch (e.PropertyName) {
			case "ListData":
				//expandableListView.SetAdapter(new ExpandableListAdapter (base.Context, listViewControl.ListData));
				adapter.items = listViewControl.ListData;
				adapter.NotifyDataSetChanged ();
				if (listViewControl.IsExpandDefault) {
					if (listViewControl != null && listViewControl.ListData != null) {
						for (int i = 0; i < listViewControl.ListData.Count; i++)
							expandableListView.ExpandGroup (i);
					}
				}
				break;
			// TODO: Need to add in navigation
			default:
				System.Diagnostics.Debug.WriteLine ("Property change for {0} has not been implemented.", e.PropertyName);
				break;
			}
		}

		public class ExpandableListAdapter : BaseExpandableListAdapter
		{
			public List<ExpandableTableListItem> items;
			Context context;

			public ExpandableListAdapter(Context context, List<ExpandableTableListItem> items) : base()
			{
				this.context = context;
				this.items = items;
			}

			public override Java.Lang.Object GetChild(int groupPosition, int childPosition)
			{
				return items[groupPosition].Items[childPosition].MenuName;
				//return null;
			}

			public override long GetChildId(int groupPosition, int childPosition)
			{
				return childPosition;
			}

			public override int GetChildrenCount(int groupPosition)
			{
				return items[groupPosition].Count;
			}

			public override Android.Views.View GetChildView(int groupPosition, int childPosition, bool isLastChild, Android.Views.View convertView, Android.Views.ViewGroup parent)
			{
				var view = convertView;

				if (view == null)
				{
					var inflater = context.GetSystemService(Context.LayoutInflaterService) as Android.Views.LayoutInflater;
					view = inflater.Inflate(Resource.Layout.OrderItem, null);
				}

				//setup childview
				var item = items [groupPosition].Items[childPosition];
				string customizedString = "Item not customized";

				if (String.IsNullOrEmpty (item.Customization) == false) {
					customizedString = "";
					String[] customs = item.Customization.Split (',');
					if (customs != null && customs.Length > 0) {
						customizedString = "Customized with: ";
						for (int i = 0; i < customs.Length; i++) {
							if(i != customs.Length - 1)
								customizedString += customs[i] + ", ";
							else
								customizedString += customs[i];
						}
					}
				}

				view.FindViewById<TextView> (Resource.Id.lblMenuName).Text = item.MenuName;
				view.FindViewById<TextView> (Resource.Id.lblMenuPrice).Text = "$" + item.ItemPrice;
				view.FindViewById<TextView> (Resource.Id.lblCustomized).Text = customizedString;

				var imageItem = view.FindViewById<ImageView> (Resource.Id.imgItem);
				var rootItem = (Android.Widget.RelativeLayout)(view.FindViewById<Android.Widget.RelativeLayout>(Resource.Id.RLElementRoot));
				imageItem.SetTag (Resource.String.tag_position, item.RestaurantId + ";" + item.MenuId + ";" + item.ID);
				rootItem.SetTag (Resource.String.tag_position, item.RestaurantId + ";" + item.MenuId + ";" + item.ID);

				rootItem.Click += (object sender, EventArgs e) => {
					if (listViewControl.EditDelegate != null)
					{
						long curTickTime = DateTime.Now.Ticks / 1000 / 1000;
						if (Math.Abs(curTickTime - lastEventTickTime) < 3)
							return;
						lastEventTickTime = curTickTime;
						var relativeView = sender as Android.Widget.RelativeLayout;
						string tagValue = (string)relativeView.GetTag(Resource.String.tag_position);
						string[] tagValues = tagValue.Split(';');
						listViewControl.EditDelegate.Invoke(tagValues[0], tagValues[1], int.Parse(tagValues[2]));

					}
				};

				if (listViewControl.DisableEdit) {
					imageItem.Visibility = Android.Views.ViewStates.Gone;
				} else {
					imageItem.Visibility = Android.Views.ViewStates.Visible;
					imageItem.Click += (object sender, EventArgs e) => {
						if (listViewControl.EditDelegate != null)
						{
							long curTickTime = DateTime.Now.Ticks / 1000 / 1000;
							if (Math.Abs(curTickTime - lastEventTickTime) < 3)
								return;
							lastEventTickTime = curTickTime;
							var imageView = sender as ImageView;
							string tagValue = (string)imageView.GetTag(Resource.String.tag_position);
							string[] tagValues = tagValue.Split(';');
							listViewControl.DeleteDelegate.Invoke(tagValues[0], tagValues[1], int.Parse(tagValues[2]));

						}
					};
				}
					
				return view;
			}

			public override Java.Lang.Object GetGroup(int groupPosition)
			{
				return items[groupPosition].ItemName;
			}

			public override long GetGroupId(int groupPosition)
			{
				return groupPosition;
			}

			public override Android.Views.View GetGroupView(int groupPosition, bool isExpanded, Android.Views.View convertView, Android.Views.ViewGroup parent)
			{
				var view = convertView;

				if (view == null) {
					var inflater = context.GetSystemService(Context.LayoutInflaterService) as Android.Views.LayoutInflater;
					view = inflater.Inflate(Resource.Layout.OrderHeaderItem, null);
				}

				//setup groupview
				var item = items [groupPosition];
				view.FindViewById<TextView> (Resource.Id.lblItemName).Text = item.ItemName;
				view.FindViewById<TextView> (Resource.Id.lblItemCount).Text = "" + item.Count;
				view.FindViewById<TextView> (Resource.Id.btnMenuPrice).Text = "$" + item.Price;
				view.FindViewById<TextView> (Resource.Id.lblExpanded).Text = (isExpanded) ? "-" : "+";

				Android.Widget.RelativeLayout rlRoot = view.FindViewById<Android.Widget.RelativeLayout> (Resource.Id.RLElementRoot);
				rlRoot.SetTag (Resource.String.tag_position, groupPosition);

				rlRoot.Click += (object sender, EventArgs e) => {
					String curText = view.FindViewById<TextView> (Resource.Id.lblExpanded).Text;
					Android.Widget.RelativeLayout rl = sender as Android.Widget.RelativeLayout;

					if (curText.Equals("+"))
						expandableListView.ExpandGroup((int)rl.GetTag(Resource.String.tag_position));
					else
						expandableListView.CollapseGroup((int)rl.GetTag(Resource.String.tag_position));

					//String curText = view.FindViewById<TextView> (Resource.Id.lblExpanded).Text;
					//view.FindViewById<TextView> (Resource.Id.lblExpanded).Text = curText.Equals("+") ? "-" : "+";
				};
	
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

		#region IDisposable implementation
		protected override void Dispose (bool disposing)
		{
			if (_disposed)
				return;

			_disposed = true;
		}

		#endregion
	}
}

