using System;
using System.Collections.Generic;
using System.Linq;

namespace WelfareManipulation
{
	public class VotingFunctions
	{
		public static double[] GenerateTruncBordaVector(int m, int k)
		{
			double[] vector = new double[m];
			for (int i = k; i >= 0; i--)
			{
				vector[i] = k - i;
			}
			return vector;
		}

		public static double[] GenerateNashVector(int m, int n)
		{
			double[] vector = new double[m];
			for (int i = 0; i < m - 1; i++)
			{
				vector[i] = Math.Log(m - i - 1);
			}
			vector[m - 1] = -n * Math.Log(m - 1);
			return vector;
		}

		public static double[] GeneratekApprovalVector(int m, int k)
		{
			double[] vector = new double[m];
			for (int i = 0; i < k; i++)
			{
				vector[i] = 1;
			}
			return vector;
		}

		public static BigDecimal[] GenerateGeometricVector(int m, double p)
		{
			BigDecimal[] vector = new BigDecimal[m];

			if (p > 1)
			{
				for (int i = 0; i < m; i++)
				{
					vector[i] = BigDecimal.Pow(p, m - i - 1);
				}
			}
			else if (p == 1)
			{
				for (int i = 0; i < m; i++)
				{
					vector[i] = m - i - 1;
				}
			}
			else if (p < 1)
			{
				for (int i = 0; i < m; i++)
				{
					vector[i] = 1 - BigDecimal.Pow(p, m - i - 1);
				}
			}
			return vector;

		}


		public static IEnumerable<int> MaxKeysInDict(Dictionary<int, double> dict)
		{
			List<int> maxKeys = new List<int>();
			double maxValue = Double.NegativeInfinity;
			foreach (int key in dict.Keys)
			{
				double value = dict[key];
				if (value > maxValue)
				{
					maxValue = value;
					maxKeys = new List<int> { key };
				}
				else if (value == maxValue)
				{
					maxKeys.Add(key);
				}
			}
			return maxKeys;
		}

		public static IEnumerable<int> MaxKeysInDict(Dictionary<int, BigDecimal> dict)
		{
			List<int> maxKeys = new List<int>();
			BigDecimal maxValue = 0;
			foreach (int key in dict.Keys)
			{
				BigDecimal value = dict[key];
				if (value > maxValue)
				{
					maxValue = value;
					maxKeys = new List<int> { key };
				}
				else if (value == maxValue)
				{
					maxKeys.Add(key);
				}
			}
			return maxKeys;
		}


		public static int WinnerFromScoringDict(Dictionary<int, double> scores)
		{
			/*
			 * Min() for lexicographic tie-breaking.
			 */
			return MaxKeysInDict(scores).Min();
		}

		public static int WinnerFromScoringDict(Dictionary<int, BigDecimal> scores)
		{
			return MaxKeysInDict(scores).Min();
		}

        public static int FindUniqueCopelandWinner(Profile profile)
        {
            return WinnerFromScoringDict(FindCopelandScores(profile));
        }

        public static int FindUniqueScoringWinner(Profile profile, double[] scoringVector)
		{
			return WinnerFromScoringDict(FindScoringRuleScores(profile, scoringVector));
		}

		public static int FindUniqueScoringWinner(Profile profile, BigDecimal[] scoringVector)
		{
			return WinnerFromScoringDict(FindScoringRuleScores(profile, scoringVector));
		}

		public static Dictionary<int, double> FindCopelandScores(Profile profile)
		{
			var scores = new Dictionary<int, double>();
            foreach (int candidate in profile.Candidates)
            {
                scores[candidate] = profile.CopelandScore(candidate);
            }
			return scores;
        }

        public static double FindScoringRuleScoreOfCandidate(
            Profile profile,
            int candidate,
            double[] scoringVector)
        {
            double score = 0;
            foreach (int voter in profile.Voters)
            {
                score += scoringVector[profile.VoterIRanks(voter, candidate)];
            }
            return score;
        }

        public static BigDecimal FindScoringRuleScoreOfCandidate(
            Profile profile,
            int candidate,
            BigDecimal[] scoringVector)
        {
            BigDecimal score = 0;
            foreach (int voter in profile.Voters)
            {
                score += scoringVector[profile.VoterIRanks(voter, candidate)];
            }
            return score;
        }

        public static Dictionary<int, double> FindScoringRuleScores(Profile profile, double[] scoringVector)
		{
			var scores = new Dictionary<int, double>();
			foreach (int candidate in profile.Candidates)
			{
				scores[candidate] = 0;
			}

			foreach (int voter in profile.Voters)
			{
				for (int position = 0; position < profile.NumberOfCandidates; position++)
				{
					int candidate = profile.AgentsIthChoice(voter, position);
					scores[candidate] += scoringVector[position];
				}
			}
			return scores;
		}

		public static Dictionary<int, BigDecimal> FindScoringRuleScores(Profile profile, BigDecimal[] scoringVector)
		{
			var scores = new Dictionary<int, BigDecimal>();
			foreach (int candidate in profile.Candidates)
			{
				scores[candidate] = 0;
			}

			foreach (int voter in profile.Voters)
			{
				for (int position = 0; position < profile.NumberOfCandidates; position++)
				{
					int candidate = profile.AgentsIthChoice(voter, position);
					scores[candidate] += scoringVector[position];
				}
			}
			return scores;
		}


		public class VotingRule
		{
			public string Name;

			public Func<Profile, int> FindWinner;

			public VotingRule(string name, Func<Profile, int> findWinner)
			{
				Name = name;
				FindWinner = findWinner;
			}
		}
	}
}