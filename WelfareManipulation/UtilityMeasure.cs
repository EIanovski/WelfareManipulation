using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WelfareManipulation
{
	public class UtilityMeasure
	{
		public string Name;

		public Func<Profile, int, double> UtilityFunction;

		public UtilityMeasure(string name, Func<Profile, int, double> utilityFunction)
		{
			Name = name;
			UtilityFunction = utilityFunction;
		}
	}
}
