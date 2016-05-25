using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Android.App;

namespace PocketButler.Controls
{
    public class CustomMasterDetailPage : MasterDetailPage
    {
        public CustomMasterDetailPage()
        {

        }

		#region PAGE LOADING
#if __ANDROID__
		ProgressDialog p = null;
		public void ShowLoading()
		{
			p = new ProgressDialog(Forms.Context);
			p.SetMessage("Loading...");
			p.SetCancelable (false);
			p.Show();
		}

		public void HideLoading()
		{
			if (p != null)
			{
				p.Dismiss();
				p = null;
			}
		}
#endif
		#endregion

		public String GetUserID()
		{
			if (Globals.Config.USER_INFO != null)
				return Globals.Config.USER_INFO.user_id;

			return "";
		}

		public String GetUserToken()
		{
			if (Globals.Config.USER_INFO != null)
				return Globals.Config.USER_INFO.user_token;

			return "";
		}
    }
}
