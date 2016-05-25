using System;
using System.ComponentModel;
using Xamarin;

namespace PocketButler
{
	public class PaymentInfoViewModel : INotifyPropertyChanged 
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public String price;
		public double TotalPrice { get; set; }

		public PaymentInfoViewModel()
		{
			Price = " $0.00";
			TotalPrice = 0;
		}

		public String Price {
			get{
				return price;
			}
			set{
				price = " $" + value;
				OnPropertyChanged ("Price");
			} 
		}

		protected virtual void OnPropertyChanged(string propertyName)
		{
			try{
				if (PropertyChanged != null)
				{
					PropertyChanged(this, 
						new PropertyChangedEventArgs(propertyName));
				}
			}
			catch (Exception ex){
				Insights.Report (ex);
			}
		}
	}
}

