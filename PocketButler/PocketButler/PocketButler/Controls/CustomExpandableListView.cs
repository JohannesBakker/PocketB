using System;
using Xamarin.Forms;
using System.Collections.Generic;

namespace PocketButler
{
	public class ExpandableTableListItem
	{
		public int Count { get; set; }
		public String ItemName { get; set; }
		public String Price { get; set; }
		public String MenuId { get; set; }
		public List<PaymentTableItem> Items { get; set; }

		public bool IsExpanded { get; set; }
	}

	public class CustomExpandableListView : View
	{
		public List<ExpandableTableListItem> ListData {
			get {
				return (List<ExpandableTableListItem>)GetValue (ExpandableListDataProperty);
			}

			set {
				this.SetValue (ExpandableListDataProperty, value);
			}
		}
		public Utils.PaymentDeleteClickedDelegate DeleteDelegate { get; set; }
		public Utils.PaymentEditItemClickedDelegate EditDelegate { get; set; }

		public bool IsExpandDefault { get; set; }
		public bool DisableEdit { get; set; }

		public CustomExpandableListView ()
		{
			DisableEdit = false;
		}

		public static readonly BindableProperty ExpandableListDataProperty =
			BindableProperty.Create<CustomExpandableListView, List<ExpandableTableListItem>> (
				p => p.ListData, null);
	}
}

