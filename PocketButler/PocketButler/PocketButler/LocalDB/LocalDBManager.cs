using System;
using SQLite;
using System.Collections.Generic;
using System.Linq;
using Xamarin;

namespace PocketButler
{
	public class LocalDBManager : SQLiteConnection
	{
		public LocalDBManager(String dbPath) : base(dbPath)
		{
			CreateTables ();
		}

		#region PRIVATE METHODS
		private void CreateTables()
		{
			CreateTable<PaymentTableItem> ();
			CreateTable<PaymentExtraItem> ();
			CreateTable<PaymentTypeItem> ();
		}
		#endregion

		#region PUBLIC METHODS
		public List<PaymentTableItem> GetPaymentItems(String restaurantId = "", bool isGetOriginal = false)
		{
			List<PaymentTableItem> resultItems;
			if (restaurantId == "") {
				resultItems = (from s in Table<PaymentTableItem> ()
				               select s).ToList ();
			} else {
				resultItems = (from s in Table<PaymentTableItem>() where s.RestaurantId == restaurantId select s).ToList();
			}

			if (isGetOriginal == true)
				return resultItems;

			foreach (PaymentTableItem paymentTable in resultItems) {
				List<PaymentExtraItem> extras = GetPaymentExtraItems (paymentTable.ID);
				String extra = "";
				double dExtra = 0.0;
				double dItemPrice = 0.0;
				double.TryParse (paymentTable.ItemPrice, out dItemPrice);

				foreach (PaymentExtraItem extraItem in extras) {
					extra += extraItem.Name + ",";
					double.TryParse (extraItem.Price, out dExtra);
					dItemPrice += dExtra;
				}

				List<PaymentTypeItem> types = GetPaymentTypeItems (paymentTable.ID);
				foreach (PaymentTypeItem typeItem in types) {
					extra += typeItem.Name + ",";
					double.TryParse (typeItem.Price, out dExtra);
					dItemPrice += dExtra;
				}

				paymentTable.ItemPrice = dItemPrice.ToString ("0.00");

				if (!extra.Equals ("")) {
					extra = extra.Substring (0, extra.Length - 1);
				}

				if (paymentTable.Customization.Equals (""))
					paymentTable.Customization = extra;
				else {
					if(!extra.Equals(""))
						paymentTable.Customization += "," + extra;
				}
			}

			return resultItems;
		}

		public List<PaymentExtraItem> GetPaymentExtraItems(int tableId){
			List<PaymentExtraItem> extras = (from s in Table<PaymentExtraItem> ()
				where s.TableItemId == tableId
				select s).ToList ();
			return extras;
		}

		public int GetSameTypeCount(String restaurantId, String item_id)
		{
			int retValue = 0;
			try{
				List<PaymentTableItem> items = (from s in Table<PaymentTableItem> ()
					where (s.RestaurantId.Equals(restaurantId) && s.MenuId.Equals(item_id))
					select s).ToList ();
				retValue = items.Count;
			}
			catch (Exception ex) {
				Insights.Report (ex);
			}

			return retValue;
		}

		public PaymentTableItem GetPaymentItemWithId(int uid)
		{
			try{
				PaymentTableItem item = (from s in Table<PaymentTableItem>() where s.ID == uid select s).FirstOrDefault();

				/*List<PaymentExtraItem> extras = GetPaymentExtraItems(item.ID);
				String extra = "";
				double dExtra = 0.0;
				double dItemPrice = 0.0;
				double.TryParse (item.ItemPrice, out dItemPrice);

				foreach (PaymentExtraItem extraItem in extras) {
					extra += extraItem.Name + ",";
					double.TryParse (extraItem.Price, out dExtra);
					dItemPrice += dExtra;
				}
				item.ItemPrice = dItemPrice.ToString("0.00");

				if (!extra.Equals ("")) {
					extra = extra.Substring (0, extra.Length - 1);
				}
				item.Customization = extra;*/

				return item;
			}
			catch (Exception ex) {
				Insights.Report (ex);
			}

			return null;
		}

		public void AddPaymentItem(String restaurantId, String restaurantName, String menuId, String menuName, String price, int count = 1)
		{
			AddPaymentItem (restaurantId, restaurantName, menuId, menuName, price, null, null, "", count);
		}

		public void AddPaymentItem(String restaurantId, String restaurantName, String menuId, String menuName, String price, String customisation, int count = 1)
		{
			AddPaymentItem (restaurantId, restaurantName, menuId, menuName, price, null, null, customisation, count);
		}

		public void AddPaymentItem(String restaurantId, String restaurantName, String menuId, String menuName, String price, List<MenuExtraItem> extras, int count = 1)
		{
			AddPaymentItem (restaurantId, restaurantName, menuId, menuName, price, extras, null, "", count);
		}

		public void AddPaymentItem(String restaurantId, String restaurantName, String menuId, String menuName, String price, List<MenuExtraItem> extras, String customisation, int count = 1)
		{
			AddPaymentItem (restaurantId, restaurantName, menuId, menuName, price, extras, null, customisation, count);
		}

		public void AddPaymentItem(String restaurantId, String restaurantName, String menuId, String menuName, String price, List<MenuExtraItem> extras, List<MenuTypeItem> types, String customisation, int count = 1)
		{
			for (int i = 0; i < count; i++) {
				PaymentTableItem newItem = new PaymentTableItem { RestaurantId = restaurantId, RestaurantName = restaurantName, MenuId = menuId, MenuName = menuName, ItemPrice = price, Customization = customisation };
				Insert(newItem);

				if (extras != null && extras.Count > 0 && newItem.ID >= 0) {
					foreach (MenuExtraItem extraItem in extras) {
						App._DbManager.AddNewExtraItem(newItem.ID, extraItem.id, extraItem.name, extraItem.unit_price, extraItem.currency_symbol, extraItem.currency_symbol_position_is_right, "false");
					}
				}

				if (types != null && types.Count > 0 && newItem.ID >= 0) {
					foreach (MenuTypeItem typeItem in types) {
						App._DbManager.AddNewTypeItem (newItem.ID, typeItem.id, typeItem.name, typeItem.unit_price, typeItem.currency_symbol, typeItem.currency_symbol_position_is_right);
					}
				}
			}
		}

		public void RemovePaymentItems(String restaurantId = "")
		{
			List<PaymentTableItem> paymentList = GetPaymentItems (restaurantId);
			foreach (PaymentTableItem item in paymentList)
				RemovePaymentItem (item.ID);
		}

		public void UpdateCustomization(int uid, String customization)
		{
			try{
				PaymentTableItem item = (from s in Table<PaymentTableItem>() where s.ID == uid select s).FirstOrDefault();
				item.Customization = customization;
				Update(item);
			}
			catch (Exception ex) {
				Insights.Report (ex);
			}
		}

		public void UpdateExtra(int uid, List<MenuExtraItem> extras)
		{
			try{
				List<PaymentExtraItem> extraItems = GetPaymentExtraItems(uid);
				foreach(PaymentExtraItem item in extraItems){
					Delete<PaymentExtraItem>(item.ID);
				}

				if (extras.Count > 0) {
					foreach (MenuExtraItem extraItem in extras) {
						App._DbManager.AddNewExtraItem(uid, extraItem.id, extraItem.name, extraItem.unit_price, extraItem.currency_symbol, extraItem.currency_symbol_position_is_right, "false");
					}
				}
			}
			catch (Exception ex) {
				Insights.Report (ex);
			}
		}


		public void UpdatePrice(int uid, String price)
		{
			try{
				PaymentTableItem item = (from s in Table<PaymentTableItem>() where s.ID == uid select s).FirstOrDefault();
				item.ItemPrice = price;
				Update(item);
			}
			catch (Exception ex) {
				Insights.Report (ex);
			}
		}

		public void RemovePaymentItem(int uid)
		{
			Delete<PaymentTableItem> (uid);

			try{
				List<PaymentExtraItem> extraItems = GetPaymentExtraItems(uid);
				foreach(PaymentExtraItem item in extraItems){
					Delete<PaymentExtraItem>(item.ID);
				}
			}catch(Exception e){

			}

			try{
				List<PaymentTypeItem> typeItems = GetPaymentTypeItems(uid);
				foreach(PaymentTypeItem item in typeItems){
					Delete<PaymentTypeItem>(item.ID);
				}
			}catch(Exception e){

			}

		}

		public double GetPaymentTotalPrice(String restaurantId = "")
		{
			double dRetTotalPrice = 0.0;

			try{
				List<PaymentTableItem> paymentItems = GetPaymentItems(restaurantId);
				foreach (PaymentTableItem item in paymentItems)
				{
					double dPrice = 0.0;
					double.TryParse(item.ItemPrice, out dPrice);
					dRetTotalPrice += dPrice;
					/*
					//Add Extra Price
					List<PaymentExtraItem> extras = GetPaymentExtraItems(item.ID);
					String extra = "";
					foreach (PaymentExtraItem extraItem in extras) {
						double.TryParse(extraItem.Price, out dPrice);
						dRetTotalPrice += dPrice;
					}*/
				}
			}
			catch (Exception ex) {
				Insights.Report (ex);
			}

			return dRetTotalPrice;
		}

		public void AddNewExtraItem(int paymentItemID, String extraId, String name, String price, String symbol, String symbol_is_right, String is_default)
		{
			PaymentExtraItem newItem = new PaymentExtraItem { TableItemId = paymentItemID, ExtraId = extraId, Name = name, Price = price, Currency_Symbol = symbol, Currency_Symbol_Is_Right = symbol_is_right, Is_Default = is_default };
			Insert(newItem);
		}

		public void UpdateTypes(int uid, List<MenuTypeItem> types)
		{
			try{
				List<PaymentTypeItem> typeItems = GetPaymentTypeItems(uid);
				foreach(PaymentTypeItem item in typeItems){
					Delete<PaymentTypeItem>(item.ID);
				}

				if (types.Count > 0) {
					foreach (MenuTypeItem typeItem in types) {
						App._DbManager.AddNewTypeItem(uid, typeItem.id, typeItem.name, typeItem.unit_price, typeItem.currency_symbol, typeItem.currency_symbol_position_is_right);
					}
				}
			}
			catch (Exception ex) {
				Insights.Report (ex);
			}
		}

		public List<PaymentTypeItem> GetPaymentTypeItems(int tableId){
			List<PaymentTypeItem> extras = (from s in Table<PaymentTypeItem> ()
				where s.TableItemId == tableId
				select s).ToList ();
			return extras;
		}

		public void AddNewTypeItem(int tableId, String typeId, String name, String price, String symbol, String symbol_is_right)
		{
			PaymentTypeItem newItem = new PaymentTypeItem { TableItemId = tableId, TypeID = typeId, Name = name, Price = price , Currency_Symbol = symbol, Currency_Symbol_Is_Right = symbol_is_right};
			Insert(newItem);
		}
		#endregion
	}
}

