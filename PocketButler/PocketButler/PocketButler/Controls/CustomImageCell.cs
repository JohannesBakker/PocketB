using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace PocketButler.Controls
{
    public class CustomImageCell : ImageCell
    {
        public CustomImageCell()
        {
            Height = 55;
			TextColor = Color.White;
			DetailColor = Color.FromRgb (171, 146, 91);
        }
    }
}
