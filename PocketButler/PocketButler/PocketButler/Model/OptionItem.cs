using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using System.ComponentModel;

namespace PocketButler.Model
{
	public class OptionItem : INotifyPropertyChanged 
    {
		public event PropertyChangedEventHandler PropertyChanged;

		public int Id{ get; set; }
		public String text;
		public String icon;
		public String detail;

        public OptionItem()
        {
			Text = "";
			Icon = "";
			Detail = "";
        }

		public OptionItem(int id, String text, String detail, String icon)
        {
            Id = id;
			Text = text;
			Icon = icon;
			Detail = detail;
        }

		public String Text {
			get{
				return text;
			}
			set{
				text = value;
				OnPropertyChanged ("Text");
			} 
		}

		public String Detail {
			get{
				return detail;
			}
			set{
				detail = value;
				OnPropertyChanged ("Detail");
			} 
		}

        public String Icon {
			get{
				return icon;
			}
			set{
				icon = value;
				OnPropertyChanged ("Icon");
			}
		}

		protected virtual void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, 
					new PropertyChangedEventArgs(propertyName));
			}
		}
    }
}
