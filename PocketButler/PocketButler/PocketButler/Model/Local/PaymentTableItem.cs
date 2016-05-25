using System;
using SQLite;

namespace PocketButler
{
	public class PaymentTableItem
	{
		[PrimaryKey, AutoIncrement]
		public int ID { get; set; }

		[MaxLength(256)]
		public String RestaurantId { get; set; }

		[MaxLength(512)]
		public String RestaurantName { get; set; }

		[MaxLength(512)]
		public String MenuId { get; set; }

		[MaxLength(512)]
		public String MenuName { get; set; }

		[MaxLength(512)]
		public String ItemPrice { get; set; }

		[MaxLength(512)]
		public String Customization { get; set; }
	}

	public class PaymentExtraItem
	{
		[PrimaryKey, AutoIncrement]
		public int ID { get; set; }

		[MaxLength(256)]
		public int TableItemId { get; set; }

		[MaxLength(256)]
		public String ExtraId { get; set; }

		[MaxLength(512)]
		public String Name { get; set; }

		[MaxLength(512)]
		public String Price { get; set; }

		[MaxLength(512)]
		public String Currency_Symbol { get; set; }

		[MaxLength(512)]
		public String Currency_Symbol_Is_Right { get; set; }

		[MaxLength(512)]
		public String Is_Default { get; set; }
	}

	public class PaymentTypeItem
	{
		[PrimaryKey, AutoIncrement]
		public int ID { get; set; }

		[MaxLength(256)]
		public int TableItemId { get; set; }

		[MaxLength(512)]
		public String TypeID { get; set; }

		[MaxLength(512)]
		public String Name { get; set; }

		[MaxLength(512)]
		public String Price { get; set; }

		[MaxLength(512)]
		public String Currency_Symbol { get; set; }

		[MaxLength(512)]
		public String Currency_Symbol_Is_Right { get; set; }

	}
}

