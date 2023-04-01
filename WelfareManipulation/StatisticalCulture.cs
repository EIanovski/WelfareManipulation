using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WelfareManipulation
{
	public class StatisticalCulture
	{
		public static StatisticalCulture EuclideanOne = new StatisticalCulture("EuclideanOne", (n, m) => Profile.GenerateEuclideanProfile(n, m, 1));
		public static StatisticalCulture EuclideanTwo = new StatisticalCulture("EuclideanTwo", (n, m) => Profile.GenerateEuclideanProfile(n, m, 2));
		public static StatisticalCulture EuclideanFive = new StatisticalCulture("EuclideanFive", (n, m) => Profile.GenerateEuclideanProfile(n, m, 5));
		public static StatisticalCulture ImpartialCulture = new StatisticalCulture("IC", (n, m) => Profile.GenerateICProfile(n, m));
		public static StatisticalCulture MallowsPointFive = new StatisticalCulture("MallowsPointFive", (n, m) =>
			Profile.GenerateMallowsProfile(
				n,
				new double[] { 0.5 },
				new double[] { 1 },
				new List<IEnumerable<int>>(new IEnumerable<int>[] { Enumerable.Range(0, m).OrderBy(x => ThreadLocalRandom.Next()).ToArray() })));
		public static StatisticalCulture MallowsPointEight = new StatisticalCulture("MallowsPointEight", (n, m) =>
			Profile.GenerateMallowsProfile(
				n,
				new double[] { 0.8 },
				new double[] { 1 },
				new List<IEnumerable<int>>(new IEnumerable<int>[] { Enumerable.Range(0, m).OrderBy(x => ThreadLocalRandom.Next()).ToArray() })));
		public static StatisticalCulture MixedMallowsTwo = new StatisticalCulture("MixedMallowsTwo", (n, m) =>
			Profile.GenerateMallowsProfile(
				n,
				new double[] { 0.5, 0.5 },
				new double[] { 0.5, 0.5 },
				new List<IEnumerable<int>>(new IEnumerable<int>[] {
					Enumerable.Range(0, m).OrderBy(x => ThreadLocalRandom.Next()).ToArray(),
					Enumerable.Range(0, m).OrderBy(x => ThreadLocalRandom.Next()).ToArray()})));

		/*
		 * The Mixed Mallows model trained for the sushi dataset by Lu and Boutilier (2014).
		 * Paper ``Effective Sampling and Learning for Mallows Models with Pairwise-Preference Data''
		 * Data https://www.preflib.org/data/ED/00014
		 * Only valid for m=10
		 */
		static readonly int[] sushiNames = Enumerable.Range(0, 10).OrderBy(x => ThreadLocalRandom.Next()).ToArray();
		static readonly int FATTY_TUNA = sushiNames[0];
		static readonly int SALMON_ROE = sushiNames[1];
		static readonly int TUNA = sushiNames[2];
		static readonly int SEA_EEL = sushiNames[3];
		static readonly int TUNA_ROLL = sushiNames[4];
		static readonly int SHRIMP = sushiNames[5];
		static readonly int EGG = sushiNames[6];
		static readonly int SQUID = sushiNames[7];
		static readonly int CUCUMBER_ROLL = sushiNames[8];
		static readonly int SEA_URCHIN = sushiNames[9];
		public static StatisticalCulture MallowsSushi = new StatisticalCulture("MallowsSushi",
			(n, m) => Profile.GenerateMallowsProfile(
				n,
				new double[] { 0.66, 0.74, 0.61, 0.64, 0.61, 0.62 },
				new double[] { 0.17, 0.15, 0.17, 0.18, 0.16, 0.18 },
				new List<IEnumerable<int>>(new IEnumerable<int>[] {
				new int[] { FATTY_TUNA, SALMON_ROE, TUNA, SEA_EEL, TUNA_ROLL, SHRIMP, EGG, SQUID, CUCUMBER_ROLL, SEA_URCHIN },
				new int[] { SHRIMP, SEA_EEL, SQUID, EGG, FATTY_TUNA, TUNA, TUNA_ROLL, CUCUMBER_ROLL, SALMON_ROE, SEA_URCHIN },
				new int[] { SEA_URCHIN, FATTY_TUNA, SEA_EEL, SALMON_ROE, SHRIMP, TUNA, SQUID, TUNA_ROLL, EGG, CUCUMBER_ROLL },
				new int[] { FATTY_TUNA, TUNA, SHRIMP, TUNA_ROLL, SQUID, SEA_EEL, EGG, CUCUMBER_ROLL, SALMON_ROE, SEA_URCHIN },
				new int[] { FATTY_TUNA, SEA_URCHIN, TUNA, SALMON_ROE, SEA_EEL, TUNA_ROLL, SHRIMP, SQUID, EGG, CUCUMBER_ROLL },
				new int[] { FATTY_TUNA, SEA_URCHIN, SALMON_ROE, SHRIMP, TUNA, SQUID, TUNA_ROLL, SEA_EEL, EGG, CUCUMBER_ROLL },})));

		/*
		 *  Samples preferences from the skating profile at: https://www.preflib.org/data/ED/00006/00000046
		 *	The preferences are below. The first integer (1) is the number of judges with said preferences.
		 *	Athlete 30 was renamed to athlete 0 in the code for 0 indexing.
		 *	1,28,13,25,2,19,10,7,15,21,30,20,18,4,1,9,17,23,14,8,11,26,24,6,22,16,12,27,5,3,29
		 *	1,25,13,19,28,10,2,7,21,15,30,18,20,9,4,1,17,8,14,11,27,23,16,26,12,6,24,22,5,3,29
		 *	1,25,13,28,19,10,2,7,21,15,20,18,30,1,4,9,14,17,23,8,26,24,6,12,11,16,3,22,27,5,29
		 *	1,28,25,13,19,2,7,10,15,30,21,17,20,18,1,14,23,8,4,9,26,24,6,16,11,27,12,22,3,5,29
		 *	1,25,13,28,19,2,10,7,21,15,20,30,18,4,9,1,17,14,8,23,26,11,6,24,22,16,3,27,12,5,29
		 *	1,25,13,28,19,2,10,7,21,15,18,20,30,4,9,17,1,14,23,11,8,6,24,26,22,12,3,27,16,5,29
		 *	1,25,13,28,19,10,2,7,21,15,30,20,18,4,17,9,1,8,14,23,26,24,11,12,6,22,3,27,16,5,29
		 */
		
		static readonly int[] athleteNames = Enumerable.Range(0, 30).OrderBy(x => ThreadLocalRandom.Next()).ToArray();
		
		public static StatisticalCulture SkatingBag = new StatisticalCulture("Skating",
			(n, m) => Profile.GenerateBagProfile(n,
				new int[][] {
					new int[] {28,13,25,2,19,10,7,15,21,0,20,18,4,1,9,17,23,14,8,11,26,24,6,22,16,12,27,5,3,29}.Select(athlete => athleteNames[athlete]).ToArray(),
					new int[] {25,13,19,28,10,2,7,21,15,0,18,20,9,4,1,17,8,14,11,27,23,16,26,12,6,24,22,5,3,29}.Select(athlete => athleteNames[athlete]).ToArray(),
					new int[] {25,13,28,19,10,2,7,21,15,20,18,0,1,4,9,14,17,23,8,26,24,6,12,11,16,3,22,27,5,29}.Select(athlete => athleteNames[athlete]).ToArray(),
					new int[] {28,25,13,19,2,7,10,15,0,21,17,20,18,1,14,23,8,4,9,26,24,6,16,11,27,12,22,3,5,29}.Select(athlete => athleteNames[athlete]).ToArray(),
					new int[] {25,13,28,19,2,10,7,21,15,20,0,18,4,9,1,17,14,8,23,26,11,6,24,22,16,3,27,12,5,29}.Select(athlete => athleteNames[athlete]).ToArray(),
					new int[] {25,13,28,19,2,10,7,21,15,18,20,0,4,9,17,1,14,23,11,8,6,24,26,22,12,3,27,16,5,29}.Select(athlete => athleteNames[athlete]).ToArray(),
					new int[] {25,13,28,19,10,2,7,21,15,0,20,18,4,17,9,1,8,14,23,26,24,11,12,6,22,3,27,16,5,29}.Select(athlete => athleteNames[athlete]).ToArray(),
				})
			);

        public string Name;

		public Func<int, int, Profile> GenerateProfile;

		public StatisticalCulture(string name, Func<int, int, Profile> generateProfile)
		{
			Name = name;
			GenerateProfile = generateProfile;
		}

		public static StatisticalCulture GetConstantCulture(Profile p)
		{
			return new StatisticalCulture("ConstantCulture", (n, m) => p);
		}
	}
}
