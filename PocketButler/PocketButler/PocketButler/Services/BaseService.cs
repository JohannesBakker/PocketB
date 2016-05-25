using System;
using PocketButler.Globals;

namespace PocketButler
{
	public class BaseService
	{
		public static AppText AppText;

		public static bool HasSuccessResult(BaseResult result)
		{
			try{
				if (result == null)
					return false;

				if (result.status.Equals ("error"))
					return false;
			}
			catch (Exception ex) {
				return false;
			}

			return true;
		}
	}
}

