using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.Mvvm;

namespace CycleApp
{
	public class SettingsDataSource : BindableBase
	{
		public double MaxHeartRate
		{
			get { return max_hr_; }
			set { SetProperty(ref max_hr_, value); }
		}

		double max_hr_;
	}
}
