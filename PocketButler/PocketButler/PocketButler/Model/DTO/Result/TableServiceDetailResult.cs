using System;
using System.Collections.Generic;

namespace PocketButler
{
	public class AreaTable
	{
		public string area { get; set; }
		public string start_no { get; set; }
		public string end_no { get; set; }
		public string has_area { get; set; }
	}

	public class TableserviceSettings
	{
		public string delievery_charges { get; set; }
		public string full_table_service { get; set; }
		public string order_table_service { get; set; }
		public List<AreaTable> area_tables { get; set; }
	}

	public class TableServiceDetailResult : BaseResult
	{
		public TableserviceSettings tableservice_settings { get; set; }
	}
}

