using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WelfareManipulation
{
	public class Manipulation
	{
		/*
		 * The optimal strategy for a single manipulator under a scoring rule is:
		 * 1. Find the sincere winner
		 * 2. Calculate the candidate's scores without the manipulator
		 * 3. For every candidate c better than the sincere winner, rank
		 * c first and the other candidates in the order of ascending scores.
		 * If it is possible to elect c, it is possible to do so in this manner.
		 */
		public static int OptimalScoringRuleOutcome(
			Profile profile,
			double[] scoringVector,
			int manipulator = 0,
			Dictionary<int, double> scores = null)
		{
			/*
			 * Find the sincere winner
			 */
			if (scores == null)
			{
				scores = VotingFunctions.FindScoringRuleScores(profile, scoringVector);
			}
			int sincereWinner = VotingFunctions.WinnerFromScoringDict(scores);

			/*
			 * Scores without the manipulator
			 */
			RemoveManipulatorScores(scores, profile, scoringVector, manipulator);

			/*
			 * Order candidates by score
			 */
			List<int> strategicVote = new List<int>(profile.Candidates);
			OrderCandidatesByAscendingScore(strategicVote, scores);

			Dictionary<int, int> indicesInOrder = FindIndicesInList(strategicVote);

			/*
			 * Iterate over candidates better than the sincere winner, starting with the best
			 */
			int potentialWinnerPosition = 0;
			int potentialWinner = profile.AgentsIthChoice(manipulator, potentialWinnerPosition);
			while (potentialWinner != sincereWinner)
			{
				int oldPosition = indicesInOrder[potentialWinner];
				strategicVote.RemoveAt(oldPosition);
				strategicVote.Insert(0, potentialWinner);

				int newWinner = ManipulationOutcome(scores, scoringVector, strategicVote);
				if (newWinner != sincereWinner && profile.AgentIPrefers(manipulator, newWinner, sincereWinner))
				{
					return newWinner;
				}

				strategicVote.RemoveAt(0);
				strategicVote.Insert(oldPosition, potentialWinner);

				potentialWinnerPosition++;
				potentialWinner = profile.AgentsIthChoice(manipulator, potentialWinnerPosition);
			}

			return sincereWinner;
		}

		public static int OptimalScoringRuleOutcome(
			Profile profile,
			BigDecimal[] scoringVector,
			int manipulator = 0,
			Dictionary<int, BigDecimal> scores = null)
		{
			if (scores == null)
			{
				scores = VotingFunctions.FindScoringRuleScores(profile, scoringVector);
			}
			int sincereWinner = VotingFunctions.WinnerFromScoringDict(scores);

			RemoveManipulatorScores(scores, profile, scoringVector, manipulator);

			List<int> strategicVote = new List<int>(profile.Candidates);
			OrderCandidatesByAscendingScore(strategicVote, scores);

			Dictionary<int, int> indicesInOrder = FindIndicesInList(strategicVote);

			int potentialWinnerPosition = 0;
			int potentialWinner = profile.AgentsIthChoice(manipulator, potentialWinnerPosition);
			while (potentialWinner != sincereWinner)
			{
				int oldPosition = indicesInOrder[potentialWinner];
				strategicVote.RemoveAt(oldPosition);
				strategicVote.Insert(0, potentialWinner);

				int newWinner = ManipulationOutcome(scores, scoringVector, strategicVote);
				if (newWinner != sincereWinner && profile.AgentIPrefers(manipulator, newWinner, sincereWinner))
				{
					return newWinner;
				}

				strategicVote.RemoveAt(0);
				strategicVote.Insert(oldPosition, potentialWinner);

				potentialWinnerPosition++;
				potentialWinner = profile.AgentsIthChoice(manipulator, potentialWinnerPosition);
			}

			return sincereWinner;
		}

		public static Dictionary<int, int> FindIndicesInList(IEnumerable<int> list)
		{
			Dictionary<int, int> indicesInOrder = new Dictionary<int, int>();
			int index = 0;
			foreach (int element in list)
			{
				indicesInOrder[element] = index;
				index++;
			}
			return indicesInOrder;
		}

		public static void RemoveManipulatorScores(
			Dictionary<int, double> scores,
			Profile profile,
			double[] scoringVector,
			int manipulator)
		{
			for (int position = 0; position < profile.NumberOfCandidates; position++)
			{
				int candidate = profile.AgentsIthChoice(manipulator, position);
				scores[candidate] -= scoringVector[position];
			}
		}

		public static void RemoveManipulatorScores(
			Dictionary<int, BigDecimal> scores,
			Profile profile,
			BigDecimal[] scoringVector,
			int manipulator)
		{
			for (int position = 0; position < profile.NumberOfCandidates; position++)
			{
				int candidate = profile.AgentsIthChoice(manipulator, position);
				scores[candidate] -= scoringVector[position];
			}
		}

		public static int ManipulationOutcome(
			Dictionary<int, double> scores,
			double[] scoringVector,
			IEnumerable<int> vote)
		{
			int position = 0;
			foreach (int candidate in vote)
			{
				scores[candidate] += scoringVector[position];
				position++;
			}
			int newWinner = VotingFunctions.WinnerFromScoringDict(scores);
			position = 0;
			foreach (int candidate in vote)
			{
				scores[candidate] -= scoringVector[position];
				position++;
			}
			return newWinner;
		}

		public static int ManipulationOutcome(
			Dictionary<int, BigDecimal> scores,
			BigDecimal[] scoringVector,
			IEnumerable<int> vote)
		{
			int position = 0;
			foreach (int candidate in vote)
			{
				scores[candidate] += scoringVector[position];
				position++;
			}
			int newWinner = VotingFunctions.WinnerFromScoringDict(scores);
			position = 0;
			foreach (int candidate in vote)
			{
				scores[candidate] -= scoringVector[position];
				position++;
			}
			return newWinner;
		}

		public static void OrderCandidatesByAscendingScore(List<int> candidates, Dictionary<int, double> scores)
		{
			candidates.Sort((x, y) => {
				if (scores[x] > scores[y])
				{
					return 1;
				}
				/*
				 * Lexicographic tie-breaking
				 */
				else if (scores[x] == scores[y])
				{
					if (x < y)
					{
						return 1;
					}
					else
					{
						return -1;
					}
				}
				else
				{
					return -1;
				}
			});
		}

		public static void OrderCandidatesByAscendingScore(List<int> candidates, Dictionary<int, BigDecimal> scores)
		{
			candidates.Sort((x, y) => {
				if (scores[x] > scores[y])
				{
					return 1;
				}
				else if (scores[x] == scores[y])
				{
					if (x < y)
					{
						return 1;
					}
					else
					{
						return -1;
					}
				}
				else
				{
					return -1;
				}
			});
		}
	}
}
