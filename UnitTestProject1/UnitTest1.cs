using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WelfareManipulation;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tests
{

	[TestClass]

	public class ParallelTests
	{
		[TestMethod]
		public void TestParallelAdd()
		{
			double x = 0;
			Parallel.For(0, 1000, (i, state) =>
			{
				Simulations.ParallelAdd(ref x, 1);
			}
			);
			Assert.AreEqual(1000, x);
		}

		[TestMethod]
		public void TestUtilitiySeriesUnanimous()
		{
			Profile p = new Profile(
				new int[,]
				{
					{ 0, 1, 2 },
                    { 0, 1, 2 },
                    { 0, 1, 2 }
                }
				);
			string[] filenames;
            Dictionary<int, double>[] utilities = Simulations.GenerateUtilitySeries(
				10,
				3,
				Enumerable.Range(3,1),
				StatisticalCulture.GetConstantCulture(p),
				new UtilityMeasure[] {UtilityMeasure.BordaUtility},
				Program.BordaSincere,
				Simulations.AxisNames.Voters,
				out filenames);
			Assert.AreEqual(100, utilities[0][3]);
        }

        [TestMethod]
        public void TestUtilitiySeriesDissenting()
        {
            Profile p = new Profile(
                new int[,]
                {
                    { 2, 1, 0 },
                    { 0, 1, 2 },
                    { 0, 1, 2 }
                }
                );
            string[] filenames;
            Dictionary<int, double>[] utilities = Simulations.GenerateUtilitySeries(
                10,
                3,
                Enumerable.Range(3, 1),
                StatisticalCulture.GetConstantCulture(p),
                new UtilityMeasure[] { UtilityMeasure.BordaUtility },
                Program.BordaSincere,
                Simulations.AxisNames.Voters,
                out filenames);
			double welfare = (40.0 / 60) * 100;
            Assert.AreEqual(welfare, utilities[0][3]);
        }
    }

    [TestClass]
	public class ProfileTests
	{
		[TestMethod]
		public void ICProfileRightDimensions()
		{
			int numberOfAgents = 3;
			int numberOfCanidates = 4;
			Profile prof = Profile.GenerateICProfile(numberOfAgents, numberOfCanidates);
			Assert.AreEqual(prof.NumberOfCandidates, numberOfCanidates);
			Assert.AreEqual(prof.NumberOfVoters, numberOfAgents);

			numberOfAgents = 4;
			numberOfCanidates = 3;
			prof = Profile.GenerateICProfile(numberOfAgents, numberOfCanidates);
			Assert.AreEqual(prof.NumberOfCandidates, numberOfCanidates);
			Assert.AreEqual(prof.NumberOfVoters, numberOfAgents);
		}

		[TestMethod]
		public void EuclideanProfileRightDimensions()
		{
			int numberOfAgents = 3;
			int numberOfCanidates = 4;
			int dimension = 1;
			Profile prof = Profile.GenerateEuclideanProfile(numberOfAgents, numberOfCanidates, dimension);
			Assert.AreEqual(prof.NumberOfCandidates, numberOfCanidates);
			Assert.AreEqual(prof.NumberOfVoters, numberOfAgents);

			numberOfAgents = 4;
			numberOfCanidates = 3;
			dimension = 2;
			prof = Profile.GenerateEuclideanProfile(numberOfAgents, numberOfCanidates, dimension);
			Assert.AreEqual(prof.NumberOfCandidates, numberOfCanidates);
			Assert.AreEqual(prof.NumberOfVoters, numberOfAgents);
		}

		[TestMethod]
		public void TestBordaUtility()
		{
			Profile profile = new Profile(
				new int[,] {
					{ 6,0,5,8,2,4,9,1,7,3 },
					{ 2,1,7,9,8,0,6,4,5,3 },
					{ 1,4,2,6,8,7,0,3,5,9 },
					{ 1,4,3,7,2,9,5,8,0,6},
					{ 2,3,8,4,9,7,0,1,6,5},
					{8,3,6,2,7,0,5,1,4,9 },
					{ 0,6,8,9,3,7,4,1,2,5 },
					{4,3,9,2,7,8,0,6,5,1 },
					{5,1,8,6,2,4,0,9,7,3 }
				});

			Assert.AreEqual(38, profile.BordaUtility(0), "0 has 38 utility.");
			Assert.AreEqual(42, profile.BordaUtility(1), "1 has 42 utility.");
			Assert.AreEqual(53, profile.BordaUtility(2), "2 has 53 utility.");
			Assert.AreEqual(38, profile.BordaUtility(3), "3 has 38 utility.");
			Assert.AreEqual(45, profile.BordaUtility(4), "4 has 45 utility.");
		}

		[TestMethod]
		public void TestNormalisedBorda()
		{
			Profile profile = new Profile(
				new int[,] {
					{ 6,0,5,8,2,4,9,1,7,3 },
					{ 2,1,7,9,8,0,6,4,5,3 },
					{ 1,4,2,6,8,7,0,3,5,9 },
					{ 1,4,3,7,2,9,5,8,0,6},
					{ 2,3,8,4,9,7,0,1,6,5},
					{8,3,6,2,7,0,5,1,4,9 },
					{ 0,6,8,9,3,7,4,1,2,5 },
					{4,3,9,2,7,8,0,6,5,1 },
					{5,1,8,6,2,4,0,9,7,3 }
				});
			int maxBorda = 9 * 9;
			Assert.AreEqual(100.0 * 38 / maxBorda, profile.NormalisedBordaUtility(0), "0 has 38 utility.");
			Assert.AreEqual(100.0 * 42 / maxBorda, profile.NormalisedBordaUtility(1), "1 has 42 utility.");
			Assert.AreEqual(100.0 * 53 / maxBorda, profile.NormalisedBordaUtility(2), "2 has 53 utility.");
			Assert.AreEqual(100.0 * 38 / maxBorda, profile.NormalisedBordaUtility(3), "3 has 38 utility.");
			Assert.AreEqual(100.0 * 45 / maxBorda, profile.NormalisedBordaUtility(4), "4 has 45 utility.");
		}

		[TestMethod]
		public void TestNashSmall()
		{
			Profile profile = new Profile(
				new int[,] {
					{ 0,1,2,3 },
					{ 0,1,2,3 },
					{ 0,1,2,3 },
				});
			int agents = 3;
			double root = 1.0 / agents;
			Assert.AreEqual(3, profile.RootNashUtility(0), "0 has 27 utility.");
			Assert.AreEqual(2, profile.RootNashUtility(1), "1 has 8 utility.");
			Assert.AreEqual(1, profile.RootNashUtility(2), "2 has 1 utility.");
			Assert.AreEqual(0, profile.RootNashUtility(3), "3 has 0 utility.");
		}

		[TestMethod]
		public void TestNormalisedNashSmall()
		{
			Profile profile = new Profile(
				new int[,] {
					{ 0,1,2,3 },
					{ 0,1,2,3 },
					{ 0,1,2,3 },
				});
			int agents = 3;
			double root = 1.0 / agents;
			double error = Math.Pow(0.1, 10);
			Assert.IsTrue(Math.Abs(100.0 * 3 / 3 - profile.NormalisedNashUtility(0)) < error, "0 has 27 utility.");
			Assert.AreEqual(100.0 * 2 / 3, profile.NormalisedNashUtility(1), "1 has 8 utility.");
			Assert.AreEqual(100.0 * 1 / 3, profile.NormalisedNashUtility(2), "2 has 1 utility.");
			Assert.AreEqual(0, profile.NormalisedNashUtility(3), "3 has 0 utility.");
		}

		[TestMethod]
		public void TestNashUtility()
		{
			Profile profile = new Profile(
				new int[,] {
					{ 6,0,5,8,2,4,9,1,7,3 },
					{ 2,1,7,9,8,0,6,4,5,3 },
					{ 1,4,2,6,8,7,0,3,5,9 },
					{ 1,4,3,7,2,9,5,8,0,6},
					{ 2,3,8,4,9,7,0,1,6,5},
					{8,3,6,2,7,0,5,1,4,9 },
					{ 0,6,8,9,3,7,4,1,2,5 },
					{4,3,9,2,7,8,0,6,5,1 },
					{5,1,8,6,2,4,0,9,7,3 }
				});
			int agents = 9;
			double root = 1.0 / agents;
			double error = Math.Pow(0.1, 10);
			Assert.IsTrue(Math.Abs(Math.Pow(93312, root) - profile.RootNashUtility(0)) < error, "0 has 93312 utility.");
			Assert.AreEqual(0, profile.RootNashUtility(1), "1 has 0 utility.");
			Assert.IsTrue(Math.Abs(Math.Pow(2551500, root) - profile.RootNashUtility(2)) < error, "2 has 2551500 utility.");
			Assert.AreEqual(0, profile.RootNashUtility(3), "3 has 0 utility.");
			Assert.IsTrue(Math.Abs(Math.Pow(331776, root) - profile.RootNashUtility(4)) < error, "4 has 331776 utility.");
		}

		[TestMethod]
		public void TestNormalisedNash()
		{
			Profile profile = new Profile(
				new int[,] {
					{ 6,0,5,8,2,4,9,1,7,3 },
					{ 2,1,7,9,8,0,6,4,5,3 },
					{ 1,4,2,6,8,7,0,3,5,9 },
					{ 1,4,3,7,2,9,5,8,0,6},
					{ 2,3,8,4,9,7,0,1,6,5},
					{8,3,6,2,7,0,5,1,4,9 },
					{ 0,6,8,9,3,7,4,1,2,5 },
					{4,3,9,2,7,8,0,6,5,1 },
					{5,1,8,6,2,4,0,9,7,3 }
				});
			int agents = 9;
			double root = 1.0 / agents;
			double error = Math.Pow(0.1, 10);
			double maxNash = 9;
			Assert.IsTrue(Math.Abs(100.0 * Math.Pow(93312, root) / maxNash - profile.NormalisedNashUtility(0)) < error, "0 has 93312 utility.");
			Assert.AreEqual(0, profile.NormalisedNashUtility(1), "1 has 0 utility.");
			Assert.IsTrue(Math.Abs(100.0 * Math.Pow(2551500, root) / maxNash - profile.NormalisedNashUtility(2)) < error, "2 has 2551500 utility.");
			Assert.AreEqual(0, profile.NormalisedNashUtility(3), "3 has 0 utility.");
			Assert.IsTrue(Math.Abs(100.0 * Math.Pow(331776, root) / maxNash - profile.NormalisedNashUtility(4)) < error, "4 has 331776 utility.");
		}

		[TestMethod]
		public void TestRawlsUtility()
		{
			Profile profile = new Profile(
				new int[,] {
					{ 6,0,5,8,2,4,9,1,7,3 },
					{ 2,1,7,9,8,0,6,4,5,3 },
					{ 1,4,2,6,8,7,0,3,5,9 },
					{ 1,4,3,7,2,9,5,8,0,6},
					{ 2,3,8,4,9,7,0,1,6,5},
					{8,3,6,2,7,0,5,1,4,9 },
					{ 0,6,8,9,3,7,4,1,2,5 },
					{4,3,9,2,7,8,0,6,5,1 },
					{5,1,8,6,2,4,0,9,7,3 }
				});

			Assert.AreEqual(1, profile.RawlsUtility(0), "0 has 1 utility.");
			Assert.AreEqual(0, profile.RawlsUtility(1), "1 has 0 utility.");
			Assert.AreEqual(1, profile.RawlsUtility(2), "2 has 1 utility.");
			Assert.AreEqual(0, profile.RawlsUtility(3), "3 has 0 utility.");
			Assert.AreEqual(1, profile.RawlsUtility(4), "4 has 1 utility.");
			Assert.AreEqual(2, profile.RawlsUtility(8), "8 has 2 utility.");
		}

		[TestMethod]
		public void TestNormalisedRawls()
		{
			Profile profile = new Profile(
				new int[,] {
					{ 6,0,5,8,2,4,9,1,7,3 },
					{ 2,1,7,9,8,0,6,4,5,3 },
					{ 1,4,2,6,8,7,0,3,5,9 },
					{ 1,4,3,7,2,9,5,8,0,6},
					{ 2,3,8,4,9,7,0,1,6,5},
					{8,3,6,2,7,0,5,1,4,9 },
					{ 0,6,8,9,3,7,4,1,2,5 },
					{4,3,9,2,7,8,0,6,5,1 },
					{5,1,8,6,2,4,0,9,7,3 }
				});

			Assert.AreEqual(100.0 / 9, profile.NormalisedRawlsUtility(0), "0 has 1 utility.");
			Assert.AreEqual(0, profile.NormalisedRawlsUtility(1), "1 has 0 utility.");
			Assert.AreEqual(100.0 / 9, profile.NormalisedRawlsUtility(2), "2 has 1 utility.");
			Assert.AreEqual(0, profile.NormalisedRawlsUtility(3), "3 has 0 utility.");
			Assert.AreEqual(100.0 / 9, profile.NormalisedRawlsUtility(4), "4 has 1 utility.");
			Assert.AreEqual(200.0 / 9, profile.NormalisedRawlsUtility(8), "8 has 2 utility.");
		}

		[TestMethod]
		public void TestAgentIPrefers()
		{
			int[,] profileArray = { { 0, 1, 2, 3 }, { 1, 2, 0, 3 }, { 2, 0, 1, 3 } };
			Profile profile = new Profile(profileArray);

			Assert.IsTrue(profile.AgentIPrefers(0, 1, 2));
			Assert.IsFalse(profile.AgentIPrefers(2, 1, 2));
		}

		[TestMethod]
		public void TestAgentIChoice()
		{
			int[,] profileArray = { { 0, 1, 2, 3 }, { 1, 2, 0, 3 }, { 2, 0, 1, 3 } };
			Profile profile = new Profile(profileArray);

			Assert.AreEqual(2, profile.AgentsIthChoice(1, 1));
		}


		[TestMethod]
		public void ProfileFromArray()
		{
			int[,] profileArray = { { 0, 1, 2, 3 }, { 1, 2, 0, 3 }, { 2, 0, 1, 3 } };
			Profile profile = new Profile(profileArray);

			Assert.AreEqual(profileArray.GetLength(0), profile.NumberOfVoters);
			Assert.AreEqual(profileArray.GetLength(1), profile.NumberOfCandidates);

			for (int agent = 0; agent < profileArray.GetLength(0); agent++)
			{
				for (int candidate = 0; candidate < profileArray.GetLength(1); candidate++)
				{
					Assert.AreEqual(profile.AgentsIthChoice(agent, candidate), profileArray[agent, candidate]);
				}
			}
		}

		[TestMethod]
		public void GetVoters()
		{
			int[,] profileArray = { { 0, 1, 2, 3 }, { 1, 2, 0, 3 }, { 2, 0, 1, 3 } };
			Profile profile = new Profile(profileArray);

			Assert.IsTrue(new HashSet<int>(profile.Voters).SetEquals(Enumerable.Range(0, profileArray.GetLength(0))));
		}

		[TestMethod]
		public void GetCandidates()
		{
			int[,] profileArray = { { 0, 1, 2, 3 }, { 1, 2, 0, 3 }, { 2, 0, 1, 3 } };
			Profile profile = new Profile(profileArray);

			Assert.IsTrue(new HashSet<int>(profile.Candidates).SetEquals(Enumerable.Range(0, profileArray.GetLength(1))));
		}

		[TestMethod]
		public void ProfileFromNestedList()
		{
			//{ { 0, 1, 2, 3 }, { 1, 2, 0, 3 }, { 2, 0, 1, 3} }
			List<List<int>> profileList = new List<List<int>>() {
				new List<int>() { 0, 1, 2, 3 },
				new List<int>() { 1, 2, 0, 3 },
				new List<int>() { 2, 0, 1, 3 }
				};

			Profile profile = new Profile(profileList);

			Assert.AreEqual(profileList.Count, profile.NumberOfVoters);
			Assert.AreEqual(profileList[0].Count, profile.NumberOfCandidates);

			for (int agent = 0; agent < profileList.Count; agent++)
			{
				for (int candidate = 0; candidate < profileList[0].Count; candidate++)
				{
					Assert.AreEqual(profile.AgentsIthChoice(agent, candidate), profileList[agent][candidate]);
				}
			}
		}
	}

	[TestClass]
	public class EuclidenPointsTests
	{
		public void RandomPointHasRightDimensions()
		{
			int dimension = 5;
			Point point = Point.GenerateRandomPoint(dimension);
			Assert.AreEqual(point.Dimension, dimension);

			dimension = 7;
			point = Point.GenerateRandomPoint(dimension);
			Assert.AreEqual(point.Dimension, dimension);
		}

		public void OneDimensionZeroDistance()
		{
			Point firstPoint = new Point(new double[] { 0.5 });
			Point secondPoint = new Point(new double[] { 0.5 });

			Assert.AreEqual(firstPoint.DistanceTo(firstPoint), 0);
			Assert.AreEqual(secondPoint.DistanceTo(secondPoint), 0);
			Assert.AreEqual(firstPoint.DistanceTo(secondPoint), 0);
			Assert.AreEqual(firstPoint.DistanceTo(secondPoint), secondPoint.DistanceTo(firstPoint));
		}

		public void OneDimensionDistancePositive()
		{
			Point firstPoint = new Point(new double[] { 0.5 });
			Point secondPoint = new Point(new double[] { 0.7 });

			Assert.AreEqual(firstPoint.DistanceTo(secondPoint), 0.2);
			Assert.AreEqual(firstPoint.DistanceTo(secondPoint), secondPoint.DistanceTo(firstPoint));
		}

		public void OneDimensionDistanceNegative()
		{
			Point firstPoint = new Point(new double[] { -0.5 });
			Point secondPoint = new Point(new double[] { -0.7 });

			Assert.AreEqual(firstPoint.DistanceTo(secondPoint), 0.2);
			Assert.AreEqual(firstPoint.DistanceTo(secondPoint), secondPoint.DistanceTo(firstPoint));
		}

		public void OneDimensionDistanceMixed()
		{
			Point firstPoint = new Point(new double[] { 0.5 });
			Point secondPoint = new Point(new double[] { -0.7 });

			Assert.AreEqual(firstPoint.DistanceTo(secondPoint), 1.2);
			Assert.AreEqual(firstPoint.DistanceTo(secondPoint), secondPoint.DistanceTo(firstPoint));
		}

		public void ThreeDimensionZeroDistance()
		{
			Point firstPoint = new Point(new double[] { 0.5, -0.5, 0 });
			Point secondPoint = new Point(new double[] { 0.5, -0.5, 0 });

			Assert.AreEqual(firstPoint.DistanceTo(firstPoint), 0);
			Assert.AreEqual(secondPoint.DistanceTo(secondPoint), 0);
			Assert.AreEqual(firstPoint.DistanceTo(secondPoint), 0);
			Assert.AreEqual(firstPoint.DistanceTo(secondPoint), secondPoint.DistanceTo(firstPoint));
		}

		public void ThreeDimensionDistancePositive()
		{
			Point firstPoint = new Point(new double[] { 0.5, 0.2, 0.3 });
			Point secondPoint = new Point(new double[] { 0.7, 0.5, 0.4 });

			double distance = Math.Sqrt(0.2 * 0.2 + 0.3 * 0.3 + 0.1 * 0.1);
			Assert.AreEqual(firstPoint.DistanceTo(secondPoint), distance);
			Assert.AreEqual(firstPoint.DistanceTo(secondPoint), secondPoint.DistanceTo(firstPoint));
		}

		public void ThreeDimensionDistanceNegative()
		{
			Point firstPoint = new Point(new double[] { -0.5, -0.2, -0.3 });
			Point secondPoint = new Point(new double[] { -0.7, -0.5, -0.4 });

			double distance = Math.Sqrt(0.2 * 0.2 + 0.3 * 0.3 + 0.1 * 0.1);
			Assert.AreEqual(firstPoint.DistanceTo(secondPoint), distance);
			Assert.AreEqual(firstPoint.DistanceTo(secondPoint), secondPoint.DistanceTo(firstPoint));
		}

		public void ThreeDimensionDistanceMixed()
		{
			Point firstPoint = new Point(new double[] { 0.5, -0.2, 0.3 });
			Point secondPoint = new Point(new double[] { -0.7, 0.5, -0.4 });

			double distance = Math.Sqrt(1.2 * 1.2 + 0.7 * 0.7 + 0.7 * 0.7);
			Assert.AreEqual(firstPoint.DistanceTo(secondPoint), distance);
			Assert.AreEqual(firstPoint.DistanceTo(secondPoint), secondPoint.DistanceTo(firstPoint));
		}

	}

	[TestClass]
	public class ManipulationTests
	{
		[TestMethod]
		public void TestCandidateOrdering()
		{
			var scores = new Dictionary<int, double> {
				{0, 2 },
				{1, 0 },
				{2, -1 },
				{3, 0 },
				{4, 5 },
				{5, 6 }
			};
			List<int> candidates = new List<int> { 0, 1, 2, 3, 4, 5 };
			Manipulation.OrderCandidatesByAscendingScore(candidates, scores);

			Assert.AreEqual(2, candidates.ElementAt(0));
			Assert.AreEqual(3, candidates.ElementAt(1));
			Assert.AreEqual(1, candidates.ElementAt(2));
			Assert.AreEqual(0, candidates.ElementAt(3));
			Assert.AreEqual(4, candidates.ElementAt(4));
			Assert.AreEqual(5, candidates.ElementAt(5));
		}

		[TestMethod]
		public void TestTwoApprovalManipulation()
		{
			Profile p = new Profile(new int[,] {
				{ 0, 3, 2, 1 },
				{0, 1, 3, 2 },
				{0, 1, 2, 3 },
				{2, 1, 0, 3 },
				{2, 1, 3, 0},
				{3, 2, 1, 0}
			});
			double[] twoApproval = VotingFunctions.GeneratekApprovalVector(4, 2);
			int sincereWinner = VotingFunctions.FindUniqueScoringWinner(p, twoApproval);
			Assert.AreEqual(1, sincereWinner);

			int twoManipulates = Manipulation.OptimalScoringRuleOutcome(p, twoApproval, 2);
			Assert.AreEqual(0, twoManipulates);

			int threeCannot = Manipulation.OptimalScoringRuleOutcome(p, twoApproval, 3);
			Assert.AreEqual(sincereWinner, threeCannot);
		}

		[TestMethod]
		public void TestBordaLarge()
		{
			Profile profile = new Profile(
				new int[,] {
					{ 6,0,5,8,2,4,9,1,7,3 },
					{ 2,1,7,9,8,0,6,4,5,3 },
					{ 1,4,2,6,8,7,0,3,5,9 },
					{ 1,4,3,7,2,9,5,8,0,6},
					{ 2,3,8,4,9,7,0,1,6,5},
					{8,3,6,2,7,0,5,1,4,9 },
					{ 0,6,8,9,3,7,4,1,2,5 },
					{4,3,9,2,7,8,0,6,5,1 },
					{5,1,8,6,2,4,0,9,7,3 }
				});
			double[] bordaScores = VotingFunctions.GenerateTruncBordaVector(10, 9);
			int oldWinner = VotingFunctions.FindUniqueScoringWinner(profile, bordaScores);
			Assert.AreEqual(2, oldWinner);

			int oneManipulates = Manipulation.OptimalScoringRuleOutcome(profile, bordaScores, 0);
			Assert.AreEqual(8, oneManipulates);

			int twoManipulates = Manipulation.OptimalScoringRuleOutcome(profile, bordaScores, 1);
			Assert.AreEqual(oldWinner, twoManipulates);

			int threeManipulates = Manipulation.OptimalScoringRuleOutcome(profile, bordaScores, 2);
			Assert.AreEqual(oldWinner, threeManipulates);

			int fourManipulates = Manipulation.OptimalScoringRuleOutcome(profile, bordaScores, 3);
			Assert.AreEqual(oldWinner, fourManipulates);

			int fiveManipulates = Manipulation.OptimalScoringRuleOutcome(profile, bordaScores, 4);
			Assert.AreEqual(oldWinner, fiveManipulates);

			int sixManipulates = Manipulation.OptimalScoringRuleOutcome(profile, bordaScores, 5);
			Assert.AreEqual(8, sixManipulates);

			int sevenManipulates = Manipulation.OptimalScoringRuleOutcome(profile, bordaScores, 6);
			Assert.AreEqual(8, sevenManipulates);

			int eightManipulates = Manipulation.OptimalScoringRuleOutcome(profile, bordaScores, 7);
			Assert.AreEqual(oldWinner, eightManipulates);

			int nineManipulates = Manipulation.OptimalScoringRuleOutcome(profile, bordaScores, 8);
			Assert.AreEqual(8, nineManipulates);
		}

		[TestMethod]
		public void TestManipulationOutcome()
		{
			var scores = new Dictionary<int, double> {
				{0, 2 },
				{1, 0 },
				{2, -1 },
				{3, 0 },
				{4, 5 },
				{5, 6 }
			};
			var scoringVector = new double[] { 1, 0, 0, 0, 0, 0 };
			List<int> vote = new List<int> { 4, 3, 5, 2, 1, 0 };
			Assert.AreEqual(4, Manipulation.ManipulationOutcome(scores, scoringVector, vote));
		}

		[TestMethod]
		public void TestBorda3CandidatesManipulation()
		{
			Profile p = new Profile(new int[,] {
				{0, 1, 2 },
				{1, 0, 2 },
				{2, 0, 1 },
				{1, 2, 0 }
			});
			int oldWinner = VotingFunctions.FindUniqueScoringWinner(p, new double[] { 2, 1, 0 });
			Assert.AreEqual(1, oldWinner);
			int newWinner = Manipulation.OptimalScoringRuleOutcome(
				p,
				new double[] { 2, 1, 0 },
				2);
			Assert.AreEqual(0, newWinner);
		}

		[TestMethod]
		public void TestFindIndicesInList()
		{
			List<int> list = new List<int> { 9, 4, 5, 2, 3, 1, 0 };
			Dictionary<int, int> indices = Manipulation.FindIndicesInList(list);

			Assert.AreEqual(0, indices[9]);
			Assert.AreEqual(1, indices[4]);
			Assert.AreEqual(2, indices[5]);
			Assert.AreEqual(3, indices[2]);
			Assert.AreEqual(4, indices[3]);
			Assert.AreEqual(5, indices[1]);
			Assert.AreEqual(6, indices[0]);
		}

		[TestMethod]
		public void TestRemoveManipulatorScores()
		{
			Profile profile = new Profile(new int[,]
			{ { 0, 1, 2, 3, 4 },
			{ 2, 1, 3, 4, 0 },
			{0, 4, 3, 1, 2 },
			{ 4, 3, 2, 1, 0 },
			});
			double[] scoringVector = { 5, 3, 2, 1, 0 };
			Dictionary<int, double> scores = VotingFunctions.FindScoringRuleScores(profile, scoringVector);
			Manipulation.RemoveManipulatorScores(scores, profile, scoringVector, 1);

			Profile newProfile = new Profile(new int[,]
			{ { 0, 1, 2, 3, 4 },
			{0, 4, 3, 1, 2 },
			{ 4, 3, 2, 1, 0 },
			});
			Dictionary<int, double> newScores = VotingFunctions.FindScoringRuleScores(newProfile, scoringVector);
			foreach (int candidate in profile.Candidates)
			{
				Assert.AreEqual(newScores[candidate], scores[candidate]);
			}
		}
	}

	[TestClass]
	public class MallowsTests
	{
		[TestMethod]
		public void TestNormalisationConstant()
		{
			double dispersion = 0.5;
			int terms = 5;
			double answer = 1.9375;
			double constant = Mallows.ComputeNormalisationConstant(dispersion, terms);
			Assert.AreEqual(answer, constant);
		}

		[TestMethod]
		public void ProbabilitiesDispersionHalf()
		{
			int[] ranking = new int[] { 0, 1, 2, 3 };

			Mallows model = new Mallows(new double[] { 0.5 },
				new double[] { 1 },
				new List<int[]>(new int[][] { ranking }));

			double[][] insertionProbabilities = model.InsertionPobabilities[0];
			double[][] expectedProbabilities = new double[4][];
			expectedProbabilities[0] = new double[] { 1 };
			expectedProbabilities[1] = new double[] { 0.5 / 1.5, 1 };
			expectedProbabilities[2] = new double[] { 0.25 / 1.75, 0.75 / 1.75, 1 };
			expectedProbabilities[3] = new double[] { 0.125 / 1.875, 0.375 / 1.875, 0.875 / 1.875, 1 };
			for (int i = 0; i < 4; i++)
			{
				double[] currentRow = insertionProbabilities[i];
				int length = currentRow.Length;
				for (int j = 0; j < length; j++)
				{
					Assert.AreEqual(expectedProbabilities[i][j], insertionProbabilities[i][j]);
				}
			}
		}

		[TestMethod]
		public void UniformProbabilities()
		{
			int[] ranking = new int[] { 0, 1, 2, 3 };

			Mallows model = new Mallows(new double[] { 1 },
				new double[] { 1 },
				new List<int[]>(new int[][] { ranking }));

			double[][] insertionProbabilities = model.InsertionPobabilities[0];
			double[][] expectedProbabilities = new double[4][];
			expectedProbabilities[0] = new double[] { 1 };
			expectedProbabilities[1] = new double[] { 1.0 / 2, 1 };
			expectedProbabilities[2] = new double[] { 1.0 / 3, 2.0 / 3, 1 };
			expectedProbabilities[3] = new double[] { 1.0 / 4, 2.0 / 4, 3.0 / 4, 1 };
			for (int i = 0; i < 4; i++)
			{
				double[] currentRow = insertionProbabilities[i];
				int length = currentRow.Length;
				for (int j = 0; j < length; j++)
				{
					Assert.AreEqual(expectedProbabilities[i][j], insertionProbabilities[i][j]);
				}
			}
		}

	}

	[TestClass]
	public class ScoringRuleTests
	{
		[TestMethod]
		public void TestPluralityScores()
		{
			Profile profile = new Profile(new int[,]
			{ { 0, 1, 2, 3, 4 },
			{ 2, 1, 3, 4, 0 },
			{0, 4, 3, 1, 2 },
			{ 4, 3, 2, 1, 0 },
			});
			double[] scoringVector = { 1, 0, 0, 0, 0 };
			Dictionary<int, double> pluralityScores = VotingFunctions.FindScoringRuleScores(profile, scoringVector);
			Assert.AreEqual(2, pluralityScores[0], "0 has 2 points.");
			Assert.AreEqual(0, pluralityScores[1], "1 has 0 points.");
			Assert.AreEqual(1, pluralityScores[2], "2 has 1 point.");
			Assert.AreEqual(0, pluralityScores[3], "3 has 0 points.");
			Assert.AreEqual(1, pluralityScores[4], "4 has 2 point.");
		}

		[TestMethod]
		public void TestConvexGeometricScores()
		{
			BigDecimal[] geometric = VotingFunctions.GenerateGeometricVector(5, 1.5);
			double[] answers = { 5.0625, 3.375, 2.25, 1.5, 1 };

			double roundingError = Math.Pow(0.1, 10);

			for (int i = 0; i < 5; i++)
			{
				Assert.IsTrue(Math.Abs(answers[i] - (double)geometric[i]) < roundingError);
			}
		}

		[TestMethod]
		public void TestNashScores()
		{
			double[] nash = VotingFunctions.GenerateNashVector(5, 7);
			double[] answers = { Math.Log(4), Math.Log(3), Math.Log(2), Math.Log(1), -7 * Math.Log(4) };


			for (int i = 0; i < 5; i++)
			{
				Assert.IsTrue(answers[i] == nash[i]);
			}
		}

		[TestMethod]
		public void TestConcaveGeometricScores()
		{
			BigDecimal[] geometric = VotingFunctions.GenerateGeometricVector(5, 0.8);
			double[] answers = { 0.5904, 0.488, 0.36, 0.2, 0 };

			double roundingError = Math.Pow(0.1, 10);

			for (int i = 0; i < 5; i++)
			{
				Assert.IsTrue(Math.Abs(answers[i] - (double)geometric[i]) < roundingError);
			}
		}

		[TestMethod]
		public void TestpOneIsBorda()
		{
			int m = 10;
			BigDecimal[] geometricPOne = VotingFunctions.GenerateGeometricVector(m, 1);
			double[] borda = VotingFunctions.GenerateTruncBordaVector(m, m - 1);

			AuxiliaryFunctions.AssertArraysEqual(geometricPOne, borda);
		}

		[TestMethod]
		public void TestGeometricPrecisionBig()
		{
			var vector = VotingFunctions.GenerateGeometricVector(100, 2);
			Assert.IsTrue(vector[0] > vector[1]);
			Assert.IsTrue(vector[0] == 2 * vector[1]);
		}

		[TestMethod]
		public void TestGeometricPrecisionSmall()
		{
			var vector = VotingFunctions.GenerateGeometricVector(100, 0.5);
			Assert.IsTrue(vector[1] < 1);
		}

		[TestMethod]
		public void TestBordaScores()
		{
			Profile profile = new Profile(
				new int[,] {
					{ 6,0,5,8,2,4,9,1,7,3 },
					{ 2,1,7,9,8,0,6,4,5,3 },
					{ 1,4,2,6,8,7,0,3,5,9 },
					{ 1,4,3,7,2,9,5,8,0,6},
					{ 2,3,8,4,9,7,0,1,6,5},
					{8,3,6,2,7,0,5,1,4,9 },
					{ 0,6,8,9,3,7,4,1,2,5 },
					{4,3,9,2,7,8,0,6,5,1 },
					{5,1,8,6,2,4,0,9,7,3 }
				});

			double[] scoringVector = { 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 };
			Dictionary<int, double> bordaScores = VotingFunctions.FindScoringRuleScores(profile, scoringVector);
			Assert.AreEqual(38, bordaScores[0], "0 has 38 points.");
			Assert.AreEqual(42, bordaScores[1], "1 has 42 points.");
			Assert.AreEqual(53, bordaScores[2], "2 has 53 points.");
			Assert.AreEqual(38, bordaScores[3], "3 has 38 points.");
			Assert.AreEqual(45, bordaScores[4], "4 has 45 points.");
			Assert.AreEqual(25, bordaScores[5], "5 has 25 points.");
			Assert.AreEqual(42, bordaScores[6], "6 has 42 points.");
			Assert.AreEqual(37, bordaScores[7], "7 has 37 points.");
			Assert.AreEqual(52, bordaScores[8], "8 has 52 points.");
			Assert.AreEqual(33, bordaScores[9], "9 has 33 points.");
		}

		[TestMethod]
		public void TestGenerateBordaScores()
		{
			double[] fullBorda = VotingFunctions.GenerateTruncBordaVector(5, 4);
			double[] actualVector = { 4, 3, 2, 1, 0 };

			AuxiliaryFunctions.AssertArraysEqual(fullBorda, actualVector);

			double[] truncBorda = VotingFunctions.GenerateTruncBordaVector(5, 2);
			actualVector = new double[] { 2, 1, 0, 0, 0 };

			AuxiliaryFunctions.AssertArraysEqual(truncBorda, actualVector);
		}

		[TestMethod]
		public void TestGeneratekApprovalScores()
		{
			double[] twoApproval = VotingFunctions.GeneratekApprovalVector(5, 2);
			double[] actualVector = { 1, 1, 0, 0, 0 };
			AuxiliaryFunctions.AssertArraysEqual(twoApproval, actualVector);
		}

		[TestMethod]
		public void TestUnique2Approval()
		{
			Profile profile = new Profile(
				new int[,] {
					{ 1, 0, 2, 3, 4 },
					{ 1,2,0,3,4 },
					{ 0,4,2,1,3 }
				});
			double[] twoApprovalScores = { 1, 1, 0, 0, 0 };
			int winner = VotingFunctions.FindUniqueScoringWinner(profile, twoApprovalScores);

			Assert.AreEqual(0, winner, "0 wins tie-breaking.");
		}

		[TestMethod]
		public void TestWinnerFromScoringDict()
		{
			Dictionary<int, double> dict = new Dictionary<int, double> {
				{0, -2 },
				{1, 0.5 },
				{3, 5 },
				{5, 4 }
			};
			int winner = VotingFunctions.WinnerFromScoringDict(dict);
			Assert.AreEqual(3, winner, "3 is the unique winner");

			dict = new Dictionary<int, double> {
				{0, -2 },
				{1, -0.5 },
				{3, -0.5 },
				{5, -10 }
			};
			winner = VotingFunctions.WinnerFromScoringDict(dict);
			Assert.AreEqual(1, winner, "1 wins the tie");
		}

		[TestMethod]
		public void TestMaxKeysInDictDouble()
		{
			Dictionary<int, double> dict = new Dictionary<int, double> {
				{0, -2 },
				{1, 0.5 },
				{3, 5 },
				{5, 4 }
			};
			IEnumerable<int> largest = VotingFunctions.MaxKeysInDict(dict);
			Assert.AreEqual(1, largest.Count(), "Unique largest");
			Assert.AreEqual(3, largest.ElementAt(0), "3 has the largest value");

			dict = new Dictionary<int, double> {
				{0, -2 },
				{1, -0.5 },
				{3, -0.5 },
				{5, -10 }
			};
			largest = VotingFunctions.MaxKeysInDict(dict);
			Assert.AreEqual(2, largest.Count(), "Two largest");
			Assert.IsTrue(largest.Contains(3), "3 has the largest value");
			Assert.IsTrue(largest.Contains(1), "1 has the largest value");
		}
	}

	public class AuxiliaryFunctions
	{
		public static void AssertArraysEqual(double[] arr1, double[] arr2)
		{
			Assert.AreEqual(arr1.Length, arr2.Length, "Arrays of different lengths");

			for (int i = 0; i < arr1.Length; i++)
			{
				Assert.AreEqual(arr1[i], arr2[i]);
			}
		}

		public static void AssertArraysEqual(BigDecimal[] arr1, double[] arr2)
		{
			Assert.AreEqual(arr1.Length, arr2.Length, "Arrays of different lengths");

			for (int i = 0; i < arr1.Length; i++)
			{
				Assert.AreEqual(arr1[i], arr2[i]);
			}
		}

		public static void AssertArraysEqual(double[] arr1, BigDecimal[] arr2)
		{
			Assert.AreEqual(arr1.Length, arr2.Length, "Arrays of different lengths");

			for (int i = 0; i < arr1.Length; i++)
			{
				Assert.AreEqual(arr1[i], arr2[i]);
			}
		}

		public static void AssertArraysEqual(BigDecimal[] arr1, BigDecimal[] arr2)
		{
			Assert.AreEqual(arr1.Length, arr2.Length, "Arrays of different lengths");

			for (int i = 0; i < arr1.Length; i++)
			{
				Assert.AreEqual(arr1[i], arr2[i]);
			}
		}
	}


}
