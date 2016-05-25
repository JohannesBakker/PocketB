using System;
using System.Runtime.Serialization;

namespace PocketButler
{
	public class SimpleResult
	{
		[DataMember(Name = "result")]
		public BaseResult result {get;set;}
	}
}

