using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WelfareManipulation
{
	public class UtilityMeasure
	{
        public static UtilityMeasure BordaUtility =
            new UtilityMeasure("Borda_utility",
                (profile, winner) => profile.NormalisedBordaUtility(winner));
        public static UtilityMeasure RawlsUtility =
            new UtilityMeasure("Rawls_utility",
                (profile, winner) => profile.NormalisedRawlsUtility(winner));
        public static UtilityMeasure NashUtility =
            new UtilityMeasure("Nash_utility",
                (profile, winner) => profile.NormalisedNashUtility(winner));

        public string Name;

		public Func<Profile, int, double> UtilityFunction;

		public UtilityMeasure(string name, Func<Profile, int, double> utilityFunction)
		{
			Name = name;
			UtilityFunction = utilityFunction;
		}
	}
}
