using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections;
using System.IO;
using System.Threading;

namespace WelfareManipulation
{


	public class Program
	{
		static void Main(string[] args)
		{
			DateTime time = DateTime.Now;
			/*
			 * The simulation fixes the value of voters/candidates, and varies the other
			 * value within a range. Here you uncomment the parameter to fix, enter the value
			 * to fix it at, and the range in which to vary the other parameter.
			 */
			//Simulations.AxisNames fixedAxisName = Simulations.AxisNames.Candidates;
			Simulations.AxisNames fixedAxisName = Simulations.AxisNames.Voters;
			int numberOfFixedAxis = 10;
			IEnumerable<int> variableAxisRange = Enumerable.Range(3, 98);

			/*
			 * How many profiles to generate for a given number of voters, candidates.
			 */
			int iterations = 10000;

			/*
			 * Comment out the utilities you're not interested in.
			 */
			UtilityMeasure[] utilities = new UtilityMeasure[] {
                UtilityMeasure.BordaUtility,
                UtilityMeasure.RawlsUtility,
                UtilityMeasure.NashUtility
            };

			/*
			 * Uncomment the culture you want to draw profiles from. 
			 * MallowsSushi is only well defined for 10 candidates.
			 * SkatingBag is only well defined for 30 candidates.
			 */
			//StatisticalCulture culture = StatisticalCulture.ImpartialCulture;
			//StatisticalCulture culture = StatisticalCulture.EuclideanOne;
			//StatisticalCulture culture = StatisticalCulture.EuclideanTwo;
			//StatisticalCulture culture = StatisticalCulture.EuclideanFive;
			//StatisticalCulture culture = StatisticalCulture.MallowsPointFive;
			//StatisticalCulture culture = StatisticalCulture.MallowsPointEight;
			//StatisticalCulture culture = StatisticalCulture.MixedMallowsTwo;
			//StatisticalCulture culture = StatisticalCulture.MallowsSushi;
			//StatisticalCulture culture = StatisticalCulture.SkatingBag;
			/*
			 * Comment out the voting functions you are not interested in.
			 */
			VotingFunctions.VotingRule[] votingFunctions = {
				/*BordaSincere, BordaManip,
				PluralitySincere, PluralityManip,
				Geometric0p5Sincere, Geometric0p5Manip,
				Geometric0p65Sincere, Geometric0p65Manip,
				Geometric0p8Sincere, Geometric0p8Manip,
				Geometric1p2Sincere, Geometric1p2Manip,
				Geometric1p5Sincere, Geometric1p5Manip,
				Geometric2Sincere, Geometric2Manip,
				FiveApprovalSincere, FiveApprovalManip,
				FiveBordaSincere, FiveBordaManip,
				HalfApprovalSincere, HalfApprovalManip,
				HalfBordaSincere, HalfBordaManip,
				QuarterApprovalSincere, QuarterApprovalManip,
				QuarterBordaSincere, QuarterBordaManip,
				NashSincere, NashManip,
				GenAntipSincere, GenAntipManip,
				GenPlurSincere, GenPlurManip,
                VetoSincere, VetoManip,*/
				CopelandSincere, CopelandManip
            };

			/*
			 * By default, results are stored in .\results
			 */
			System.IO.Directory.CreateDirectory("results");
			string[] fileNames;

			StatisticalCulture[] cultures = new StatisticalCulture[]
			{
                StatisticalCulture.ImpartialCulture,
				StatisticalCulture.EuclideanOne,
				StatisticalCulture.EuclideanTwo,
				StatisticalCulture.EuclideanFive,
				StatisticalCulture.MallowsPointEight,
				StatisticalCulture.MallowsPointFive,
				StatisticalCulture.MixedMallowsTwo,
                //StatisticalCulture.MallowsSushi,
                //StatisticalCulture.SkatingBag
            };

			foreach (StatisticalCulture culture in cultures)
			{
				Console.WriteLine(culture.Name);
                foreach (VotingFunctions.VotingRule rule in votingFunctions)
                {
                    Dictionary<int, double>[] results =
                        Simulations.GenerateUtilitySeries(
                            iterations,
                            numberOfFixedAxis,
                            variableAxisRange,
                            culture,
                            utilities,
                            rule,
                            fixedAxisName,
                            out fileNames
                        );
                    for (int j = 0; j < utilities.Length; j++)
                    {
                        File.WriteAllText("results\\" + fileNames[j], Utilities.DictionaryToString(results[j]));
                    }
                    Console.WriteLine(rule.Name + " done. Time taken: " + (DateTime.Now - time));
                    time = DateTime.Now;
                }
            }
			Console.WriteLine("All done.");
			Console.ReadLine();
		}

        public static VotingFunctions.VotingRule CopelandSincere = new VotingFunctions.VotingRule(
            "CopelandSincere",
            profile => VotingFunctions.FindUniqueCopelandWinner(profile));

        public static VotingFunctions.VotingRule CopelandManip = new VotingFunctions.VotingRule(
            "CopelandManip",
            profile => Manipulation.OptimalCopelandOutcome(profile));

        /*public static VotingFunctions.VotingRule SeqMajSincere = new VotingFunctions.VotingRule(
            "SeqMajSincere",
            profile => VotingFunctions.FindSequentialMajorityWinner(profile));

        public static VotingFunctions.VotingRule SeqMajManip = new VotingFunctions.VotingRule(
            "SeqMajManip",
            profile => Manipulation.OptimalSequentialMajorityOutcome(profile));

        public static VotingFunctions.VotingRule MaxMinSincere = new VotingFunctions.VotingRule(
            "MaxMinSincere",
            profile => VotingFunctions.FindUniqueMaxMinWinner(profile));

        public static VotingFunctions.VotingRule MaxMinManip = new VotingFunctions.VotingRule(
            "MaxMinManip",
            profile => Manipulation.OptimalMaxMinOutcome(profile));*/

        public static VotingFunctions.VotingRule BordaSincere = new VotingFunctions.VotingRule(
			"BordaSincere",
			profile => VotingFunctions.FindUniqueScoringWinner(
				profile,
				VotingFunctions.GenerateTruncBordaVector(profile.NumberOfCandidates, profile.NumberOfCandidates - 1)));

		public static VotingFunctions.VotingRule BordaManip = new VotingFunctions.VotingRule(
			"BordaManip",
			profile => Manipulation.OptimalScoringRuleOutcome(
				profile,
				VotingFunctions.GenerateTruncBordaVector(profile.NumberOfCandidates, profile.NumberOfCandidates - 1)));

		public static VotingFunctions.VotingRule PluralitySincere = new VotingFunctions.VotingRule(
			"PluralitySincere",
			profile => VotingFunctions.FindUniqueScoringWinner(
				profile,
				VotingFunctions.GeneratekApprovalVector(profile.NumberOfCandidates, 1)));

		public static VotingFunctions.VotingRule PluralityManip = new VotingFunctions.VotingRule(
			"PluralityManip",
			profile => Manipulation.OptimalScoringRuleOutcome(
				profile,
				VotingFunctions.GeneratekApprovalVector(profile.NumberOfCandidates, 1)));

		public static VotingFunctions.VotingRule VetoSincere = new VotingFunctions.VotingRule(
			"AntipSincere",
			profile => VotingFunctions.FindUniqueScoringWinner(
				profile,
				VotingFunctions.GeneratekApprovalVector(profile.NumberOfCandidates, profile.NumberOfCandidates - 1)));

		public static VotingFunctions.VotingRule VetoManip = new VotingFunctions.VotingRule(
			"AntipManip",
			profile => Manipulation.OptimalScoringRuleOutcome(
				profile,
				VotingFunctions.GeneratekApprovalVector(profile.NumberOfCandidates, profile.NumberOfCandidates - 1)));

		public static VotingFunctions.VotingRule Geometric1p2Sincere = new VotingFunctions.VotingRule(
			"Geometric1p2Sincere",
			profile => VotingFunctions.FindUniqueScoringWinner(
				profile,
				VotingFunctions.GenerateGeometricVector(profile.NumberOfCandidates, 1.2)));

		public static VotingFunctions.VotingRule Geometric1p2Manip = new VotingFunctions.VotingRule(
			"Geometric1p2Manip",
			profile => Manipulation.OptimalScoringRuleOutcome(
				profile,
				VotingFunctions.GenerateGeometricVector(profile.NumberOfCandidates, 1.2)));

		public static VotingFunctions.VotingRule Geometric1p5Sincere = new VotingFunctions.VotingRule(
			"Geometric1p5Sincere",
			profile => VotingFunctions.FindUniqueScoringWinner(
				profile,
				VotingFunctions.GenerateGeometricVector(profile.NumberOfCandidates, 1.5)));

		public static VotingFunctions.VotingRule Geometric1p5Manip = new VotingFunctions.VotingRule(
			"Geometric1p5Manip",
			profile => Manipulation.OptimalScoringRuleOutcome(
				profile,
				VotingFunctions.GenerateGeometricVector(profile.NumberOfCandidates, 1.5)));

		public static VotingFunctions.VotingRule Geometric0p8Sincere = new VotingFunctions.VotingRule(
			"Geometric0p8Sincere",
			profile => VotingFunctions.FindUniqueScoringWinner(
				profile,
				VotingFunctions.GenerateGeometricVector(profile.NumberOfCandidates, 0.8)));

		public static VotingFunctions.VotingRule Geometric0p8Manip = new VotingFunctions.VotingRule(
			"Geometric0p8Manip",
			profile => Manipulation.OptimalScoringRuleOutcome(
				profile,
				VotingFunctions.GenerateGeometricVector(profile.NumberOfCandidates, 0.8)));

		public static VotingFunctions.VotingRule Geometric0p65Sincere = new VotingFunctions.VotingRule(
			"Geometric0p65Sincere",
			profile => VotingFunctions.FindUniqueScoringWinner(
				profile,
				VotingFunctions.GenerateGeometricVector(profile.NumberOfCandidates, 0.65)));

		public static VotingFunctions.VotingRule Geometric0p65Manip = new VotingFunctions.VotingRule(
			"Geometric0p65Manip",
			profile => Manipulation.OptimalScoringRuleOutcome(
				profile,
				VotingFunctions.GenerateGeometricVector(profile.NumberOfCandidates, 0.65)));

		public static VotingFunctions.VotingRule Geometric2Sincere = new VotingFunctions.VotingRule(
			"Geometric2Sincere",
			profile => VotingFunctions.FindUniqueScoringWinner(
				profile,
				VotingFunctions.GenerateGeometricVector(profile.NumberOfCandidates, 2)));

		public static VotingFunctions.VotingRule Geometric2Manip = new VotingFunctions.VotingRule(
			"Geometric2Manip",
			profile => Manipulation.OptimalScoringRuleOutcome(
				profile,
				VotingFunctions.GenerateGeometricVector(profile.NumberOfCandidates, 2)));

		public static VotingFunctions.VotingRule Geometric0p5Sincere = new VotingFunctions.VotingRule(
			"Geometric0p5Sincere",
			profile => VotingFunctions.FindUniqueScoringWinner(
				profile,
				VotingFunctions.GenerateGeometricVector(profile.NumberOfCandidates, 0.5)));

		public static VotingFunctions.VotingRule Geometric0p5Manip = new VotingFunctions.VotingRule(
			"Geometric0p5Manip",
			profile => Manipulation.OptimalScoringRuleOutcome(
				profile,
				VotingFunctions.GenerateGeometricVector(profile.NumberOfCandidates, 0.5)));

        public static VotingFunctions.VotingRule GenAntipSincere = new VotingFunctions.VotingRule(
            "GenAntipSincere",
            profile => VotingFunctions.FindUniqueScoringWinner(
                profile,
                VotingFunctions.GenerateGeometricVector(profile.NumberOfCandidates, 1.0/ profile.NumberOfVoters)));

        public static VotingFunctions.VotingRule GenAntipManip = new VotingFunctions.VotingRule(
            "GenAntipManip",
            profile => Manipulation.OptimalScoringRuleOutcome(
                profile,
                VotingFunctions.GenerateGeometricVector(profile.NumberOfCandidates, 1.0 / profile.NumberOfVoters)));

        public static VotingFunctions.VotingRule GenPlurSincere = new VotingFunctions.VotingRule(
            "GenPlurSincere",
            profile => VotingFunctions.FindUniqueScoringWinner(
                profile,
                VotingFunctions.GenerateGeometricVector(profile.NumberOfCandidates, profile.NumberOfVoters)));

        public static VotingFunctions.VotingRule GenPlurManip = new VotingFunctions.VotingRule(
            "GenPlurManip",
            profile => Manipulation.OptimalScoringRuleOutcome(
                profile,
                VotingFunctions.GenerateGeometricVector(profile.NumberOfCandidates, profile.NumberOfVoters)));

        public static VotingFunctions.VotingRule FiveBordaSincere = new VotingFunctions.VotingRule(
			"5BordaSincere",
			profile => VotingFunctions.FindUniqueScoringWinner(
				profile,
				VotingFunctions.GenerateTruncBordaVector(profile.NumberOfCandidates, Math.Min(profile.NumberOfCandidates - 1, 5))));

		public static VotingFunctions.VotingRule FiveBordaManip = new VotingFunctions.VotingRule(
			"5BordaManip",
			profile => Manipulation.OptimalScoringRuleOutcome(
				profile,
				VotingFunctions.GenerateTruncBordaVector(profile.NumberOfCandidates, Math.Min(profile.NumberOfCandidates - 1, 5))));

		public static VotingFunctions.VotingRule FiveApprovalSincere = new VotingFunctions.VotingRule(
			"5ApprovalSincere",
			profile => VotingFunctions.FindUniqueScoringWinner(
				profile,
				VotingFunctions.GeneratekApprovalVector(profile.NumberOfCandidates, Math.Min(profile.NumberOfCandidates - 1, 5))));

		public static VotingFunctions.VotingRule FiveApprovalManip = new VotingFunctions.VotingRule(
			"5ApprovalManip",
			profile => Manipulation.OptimalScoringRuleOutcome(
				profile,
				VotingFunctions.GeneratekApprovalVector(profile.NumberOfCandidates, Math.Min(profile.NumberOfCandidates - 1, 5))));

		public static VotingFunctions.VotingRule HalfBordaSincere = new VotingFunctions.VotingRule(
			"HalfBordaSincere",
			profile => VotingFunctions.FindUniqueScoringWinner(
				profile,
				VotingFunctions.GenerateTruncBordaVector(profile.NumberOfCandidates, Math.Min(profile.NumberOfCandidates - 1, profile.NumberOfCandidates / 2))));

		public static VotingFunctions.VotingRule HalfBordaManip = new VotingFunctions.VotingRule(
			"HalfBordaManip",
			profile => Manipulation.OptimalScoringRuleOutcome(
				profile,
				VotingFunctions.GenerateTruncBordaVector(profile.NumberOfCandidates, Math.Min(profile.NumberOfCandidates - 1, profile.NumberOfCandidates / 2))));

		public static VotingFunctions.VotingRule HalfApprovalSincere = new VotingFunctions.VotingRule(
			"HalfApprovalSincere",
			profile => VotingFunctions.FindUniqueScoringWinner(
				profile,
				VotingFunctions.GeneratekApprovalVector(profile.NumberOfCandidates, Math.Min(profile.NumberOfCandidates - 1, profile.NumberOfCandidates / 2))));

		public static VotingFunctions.VotingRule HalfApprovalManip = new VotingFunctions.VotingRule(
			"HalfApprovalManip",
			profile => Manipulation.OptimalScoringRuleOutcome(
				profile,
				VotingFunctions.GeneratekApprovalVector(profile.NumberOfCandidates, Math.Min(profile.NumberOfCandidates - 1, profile.NumberOfCandidates / 2))));

		public static VotingFunctions.VotingRule QuarterBordaSincere = new VotingFunctions.VotingRule(
			"QuarterBordaSincere",
			profile => VotingFunctions.FindUniqueScoringWinner(
				profile,
				VotingFunctions.GenerateTruncBordaVector(profile.NumberOfCandidates, Math.Min(profile.NumberOfCandidates - 1, profile.NumberOfCandidates / 4))));

		public static VotingFunctions.VotingRule QuarterBordaManip = new VotingFunctions.VotingRule(
			"QuarterBordaManip",
			profile => Manipulation.OptimalScoringRuleOutcome(
				profile,
				VotingFunctions.GenerateTruncBordaVector(profile.NumberOfCandidates, Math.Min(profile.NumberOfCandidates - 1, profile.NumberOfCandidates / 4))));

		public static VotingFunctions.VotingRule QuarterApprovalSincere = new VotingFunctions.VotingRule(
			"QuarterApprovalSincere",
			profile => VotingFunctions.FindUniqueScoringWinner(
				profile,
				VotingFunctions.GeneratekApprovalVector(profile.NumberOfCandidates, Math.Min(profile.NumberOfCandidates - 1, profile.NumberOfCandidates / 4))));

		public static VotingFunctions.VotingRule QuarterApprovalManip = new VotingFunctions.VotingRule(
			"QuarterApprovalManip",
			profile => Manipulation.OptimalScoringRuleOutcome(
				profile,
				VotingFunctions.GeneratekApprovalVector(profile.NumberOfCandidates, Math.Min(profile.NumberOfCandidates - 1, profile.NumberOfCandidates / 4))));

		public static VotingFunctions.VotingRule NashSincere = new VotingFunctions.VotingRule(
			"NashSincere",
			profile => VotingFunctions.FindUniqueScoringWinner(
				profile,
				VotingFunctions.GenerateNashVector(profile.NumberOfCandidates, profile.NumberOfVoters)));

		public static VotingFunctions.VotingRule NashManip = new VotingFunctions.VotingRule(
			"NashManip",
			profile => Manipulation.OptimalScoringRuleOutcome(
				profile,
				VotingFunctions.GenerateNashVector(profile.NumberOfCandidates, profile.NumberOfVoters)));
	}
}
