using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WelfareManipulation
{
	public class Manipulation
	{
        public enum ManipulationAlgorithm
        {
            GreedySearch,
            NormalForm,
            OptimisedGreedy
        }

        public static int OptimalCopelandOutcomeViaGreedySearch(Profile profile, int manipulator = 0)
        {
            return GreedySearch(
                profile,
                (p, candidate) => p.CopelandScore(candidate),
                manipulator);
        }

        private static int OptimalScoringRuleOutcomeViaGreedySearch(
            Profile profile,
            double[] scoringVector,
            int manipulator = 0)
        {
            return GreedySearch(
                profile,
                (p, candidate) => VotingFunctions.FindScoringRuleScoreOfCandidate(p, candidate, scoringVector),
                manipulator);
        }

        private static int OptimalScoringRuleOutcomeViaGreedySearch(
            Profile profile,
            BigDecimal[] scoringVector,
            int manipulator = 0)
        {
            return GreedySearch(
                profile,
                (p, candidate) => VotingFunctions.FindScoringRuleScoreOfCandidate(p, candidate, scoringVector),
                manipulator);
        }

        /*
		 * As per Bartholdi, Tovey, and Trick (1989), this algorithm will find
		 * the optimal vote for any voting function that can be represented as
		 * a monotone scoring function. This includes scoring rules, Copeland, MinMax.
		 * 
		 * 1. Put preferred c on top.
		 * 2. See if it is possible to put some d in the next free position, 
		 * such that d's score is less than c's.
		 * 3. Continue until ballot is complete or no such d exists.
		 */
        private const int INVALID_WINNER = -1;

        private static int GreedySearch(
            Profile profile,
            Func<Profile, int, double> scoreFunction,
            int manipulator = 0)
        {
            int[] sincerePrefs = profile.GetVoter(manipulator);
            foreach (int bestChoice in sincerePrefs)
            {
                int[] strategicVote = new int[profile.NumberOfCandidates];
                strategicVote[0] = bestChoice;
                var remainingCandidates = new HashSet<int>(profile.Candidates);
                remainingCandidates.Remove(bestChoice);
                int winner = GreedySearch(profile, scoreFunction, remainingCandidates, strategicVote, 1, manipulator);
                if (winner != INVALID_WINNER)
                {
                    return winner;
                }
            }
            throw new Exception("The code was unable to find any winner. This should never happen, since the sincere winner should win in the original profile.");
        }

        private static int GreedySearch(
            Profile profile,
            Func<Profile, int, double> scoreFunction,
            HashSet<int> remainingCandidates,
            int[] strategicVote,
            int indexToFill,
            int manipulator = 0)
        {

            for (int i = indexToFill; i < profile.NumberOfCandidates; i++)
            {
                bool foundSafe = false;
                foreach (int candidate in remainingCandidates)
                {
                    if (SafeToRank(candidate, i, profile, strategicVote, manipulator, scoreFunction, remainingCandidates))
                    {
                        strategicVote[i] = candidate;
                        remainingCandidates.Remove(candidate);
                        foundSafe = true;
                        break;
                    }
                }
                if (!foundSafe)
                {
                    break;
                }
            }
            if (remainingCandidates.Count == 0)
            {
                return strategicVote[0];
            }
            return INVALID_WINNER;

        }

        private static int GreedySearch(
            Profile profile,
            Func<Profile, int, BigDecimal> scoreFunction,
            int manipulator = 0)
        {
            int[] sincerePrefs = profile.GetVoter(manipulator);
            foreach (int bestChoice in sincerePrefs)
            {
                int[] strategicVote = new int[profile.NumberOfCandidates];
                strategicVote[0] = bestChoice;
                var remainingCandidates = new HashSet<int>(profile.Candidates);
                remainingCandidates.Remove(bestChoice);
                int winner = GreedySearch(profile, scoreFunction, remainingCandidates, strategicVote, 1, manipulator);
                if (winner != INVALID_WINNER)
                {
                    return winner;
                }
            }
            throw new Exception("The code was unable to find any winner. This should never happen, since the sincere winner should win in the original profile.");
        }

        private static int GreedySearch(
            Profile profile,
            Func<Profile, int, BigDecimal> scoreFunction,
            HashSet<int> remainingCandidates,
            int[] strategicVote,
            int indexToFill,
            int manipulator = 0)
        {

            for (int i = indexToFill; i < profile.NumberOfCandidates; i++)
            {
                bool foundSafe = false;
                foreach (int candidate in remainingCandidates)
                {
                    if (SafeToRank(candidate, i, profile, strategicVote, manipulator, scoreFunction, remainingCandidates))
                    {
                        strategicVote[i] = candidate;
                        remainingCandidates.Remove(candidate);
                        foundSafe = true;
                        break;
                    }
                }
                if (!foundSafe)
                {
                    break;
                }
            }
            if (remainingCandidates.Count == 0)
            {
                return strategicVote[0];
            }
            return INVALID_WINNER;

        }


        private static bool SafeToRank(
            int candidate,
            int pos,
            Profile profile,
            int[] strategicVote,
            int manipulator,
            Func<Profile, int, double> scoreFunction,
            HashSet<int> remainingCandidates)
        {
            int[] sincerePrefs = profile.GetVoter(manipulator);
            strategicVote[pos] = candidate;
            int i = pos + 1;
            foreach (int fillerCandidate in remainingCandidates)
            {
                if (fillerCandidate != candidate)
                {
                    strategicVote[i] = fillerCandidate;
                    i++;
                }
            }
            profile.SetVoter(manipulator, strategicVote);
            bool safe = scoreFunction(profile, strategicVote[0]) > scoreFunction(profile, strategicVote[pos])
                || (scoreFunction(profile, strategicVote[0]) == scoreFunction(profile, strategicVote[pos])
                && strategicVote[0] < strategicVote[pos]);
            profile.SetVoter(manipulator, sincerePrefs);
            return safe;
        }

        private static bool SafeToRank(
            int candidate,
            int pos,
            Profile profile,
            int[] strategicVote,
            int manipulator,
            Func<Profile, int, BigDecimal> scoreFunction,
            HashSet<int> remainingCandidates)
        {
            int[] sincerePrefs = profile.GetVoter(manipulator);
            strategicVote[pos] = candidate;
            int i = pos + 1;
            foreach (int fillerCandidate in remainingCandidates)
            {
                if (fillerCandidate != candidate)
                {
                    strategicVote[i] = fillerCandidate;
                    i++;
                }
            }
            profile.SetVoter(manipulator, strategicVote);
            bool safe = scoreFunction(profile, strategicVote[0]) > scoreFunction(profile, strategicVote[pos])
                || (scoreFunction(profile, strategicVote[0]) == scoreFunction(profile, strategicVote[pos])
                && strategicVote[0] < strategicVote[pos]);
            profile.SetVoter(manipulator, sincerePrefs);
            return safe;
        }


        /*
		 * The optimal strategy for a single manipulator under a scoring rule is:
		 * 1. Find the sincere winner
		 * 2. Calculate the candidate's scores without the manipulator
		 * 3. For every candidate c better than the sincere winner, rank
		 * c first and the other candidates in the order of ascending scores.
		 * If it is possible to elect c, it is possible to do so in this manner.
		 */
        private static int NormalFormManipulation(
            Profile profile,
            Dictionary<int, double> scoresWithoutManipulator,
            int sincereWinner,
            Func<IEnumerable<int>, int> manipulationOutcome,
            int manipulator = 0)
        {
            /*
			 * Order candidates by score
			 */
            List<int> strategicVote = new List<int>(profile.Candidates);
            OrderCandidatesByAscendingScore(strategicVote, scoresWithoutManipulator);

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

                int newWinner = manipulationOutcome(strategicVote);
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

        private static int NormalFormManipulation(
            Profile profile,
            Dictionary<int, BigDecimal> scoresWithoutManipulator,
            int sincereWinner,
            Func<IEnumerable<int>, int> manipulationOutcome,
            int manipulator = 0)
        {
            /*
			 * Order candidates by score
			 */
            List<int> strategicVote = new List<int>(profile.Candidates);
            OrderCandidatesByAscendingScore(strategicVote, scoresWithoutManipulator);

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

                int newWinner = manipulationOutcome(strategicVote);
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

        public static int OptimalCopelandOutcome(
            Profile profile,
            int manipulator = 0,
            ManipulationAlgorithm algo = ManipulationAlgorithm.OptimisedGreedy)
        {
            if (algo == ManipulationAlgorithm.GreedySearch)
            {
                return OptimalCopelandOutcomeViaGreedySearch(profile, manipulator);
            }
            if (algo == ManipulationAlgorithm.OptimisedGreedy)
            {
                return OptimisedCopelandGreedySearch(profile, manipulator);
            }
            throw new Exception("Algorithm not implemented");
        }

        internal static HashSet<int> FindIrrelevantCopelandCandidates(
            Profile profile,
            int[] strategicVote,
            int badCandidateIndex,
            int manipulator,
            out bool dontBother)
        {
            int[] sincerePreferences = profile.GetVoter(manipulator);
            profile.SetVoter(manipulator, strategicVote);
            dontBother = false;
            HashSet<int> irrelevantCandidates = new HashSet<int>(profile.Candidates);
            double preferredWinnerScore = profile.CopelandScore(strategicVote[0]);
            for (int i = badCandidateIndex; i < profile.NumberOfCandidates; i++)
            {
                int badCandidate = strategicVote[i];
                double badCandidateScore = profile.CopelandScore(badCandidate);
                if (badCandidateScore - PossibleCopelandScoreDecrease(
                    profile,
                    i,
                    strategicVote) > preferredWinnerScore)
                {
                    dontBother = true;
                    break;
                }
                if (badCandidateScore + PossibleCopelandScoreIncrease(
                    profile,
                    i,
                    strategicVote) >= preferredWinnerScore)
                {
                    irrelevantCandidates.Remove(badCandidate);
                }

            }
            profile.SetVoter(manipulator, sincerePreferences);
            return irrelevantCandidates;
        }

        internal static double PossibleCopelandScoreDecrease(
            Profile profile,
            int candidateIndex,
            int[] strategicVote)
        {
            double halfVoters = profile.NumberOfVoters / 2.0;
            double possibleDecrease = 0;
            int candidate = strategicVote[candidateIndex];
            for (int i = candidateIndex + 1; i < profile.NumberOfCandidates; i++)
            {
                int otherCandidate = strategicVote[i];
                if (profile.HowManyPrefer(candidate, otherCandidate) == halfVoters
                    || profile.HowManyPrefer(candidate, otherCandidate) == halfVoters + 1)
                {
                    possibleDecrease += 0.5;
                }
                if (profile.HowManyPrefer(candidate, otherCandidate) > halfVoters
                    && profile.HowManyPrefer(candidate, otherCandidate) < halfVoters + 1)
                {
                    possibleDecrease++;
                }
            }
            return possibleDecrease;
        }

        internal static double PossibleCopelandScoreIncrease(
            Profile profile,
            int candidateIndex,
            int[] strategicVote)
        {
            double halfVoters = profile.NumberOfVoters / 2.0;
            double possibleIncrease = 0;
            int candidate = strategicVote[candidateIndex];
            for (int i = candidateIndex - 1; i > 0; i--)
            {
                int otherCandidate = strategicVote[i];
                if (profile.HowManyPrefer(candidate, otherCandidate) == halfVoters
                    || profile.HowManyPrefer(candidate, otherCandidate) == halfVoters - 1)
                {
                    possibleIncrease += 0.5;
                }
                if (profile.HowManyPrefer(candidate, otherCandidate) < halfVoters
                    && profile.HowManyPrefer(candidate, otherCandidate) > halfVoters - 1)
                {
                    possibleIncrease++;
                }
            }
            return possibleIncrease;
        }


        private static int OptimisedCopelandGreedySearch(
            Profile profile,
            int manipulator = 0)
        {
            int[] sincerePrefs = profile.GetVoter(manipulator);

            int[] strategicVote = new int[profile.NumberOfCandidates];

            for (int i = 0; i < profile.NumberOfCandidates; i++)
            {
                sincerePrefs.CopyTo(strategicVote, 0);
                int bestChoice = sincerePrefs[i];
                strategicVote[i] = strategicVote[0];
                strategicVote[0] = bestChoice;
                bool dontBother;
                HashSet<int> irrelevantCandidates =
                    FindIrrelevantCopelandCandidates(
                        profile,
                        strategicVote,
                        i + 1,
                        manipulator,
                        out dontBother);
                if (dontBother)
                {
                    continue;
                }
                var remainingCandidates = new HashSet<int>(profile.Candidates);
                remainingCandidates.ExceptWith(irrelevantCandidates);
                irrelevantCandidates.Remove(bestChoice);
                int j = 1;
                foreach (int irrelevantCandidate in irrelevantCandidates)
                {
                    strategicVote[j] = irrelevantCandidate;
                    j++;
                }
                int winner = GreedySearch(
                    profile,
                    (p, candidate) => p.CopelandScore(candidate),
                    new HashSet<int>(remainingCandidates),
                    strategicVote,
                    j,
                    manipulator);
                if (winner != INVALID_WINNER)
                {
                    return winner;
                }
            }
            throw new Exception("The code was unable to find any winner. This should never happen, since the sincere winner should win in the original profile.");
        }

        internal static int OptimalSimpsonOutcome(
            Profile profile,
            int manipulator = 0,
            ManipulationAlgorithm algo = ManipulationAlgorithm.GreedySearch)
        {
            if (algo == ManipulationAlgorithm.GreedySearch)
            {
                return OptimalSimpsonOutcomeViaGreedySearch(profile, manipulator);
            }
            throw new Exception("Algorithm not implemented");
        }

        public static ManipulationAlgorithm[] ImplementedSimpsonManipulationAlgorithms =
        {
            ManipulationAlgorithm.GreedySearch
        };

        private static int OptimalSimpsonOutcomeViaGreedySearch(Profile profile, int manipulator)
        {
            return GreedySearch(
                profile,
                (p, candidate) => p.MaxMinScore(candidate),
                manipulator);
        }

        public static int OptimalScoringRuleOutcome(
            Profile profile,
            double[] scoringVector,
            int manipulator = 0,
            ManipulationAlgorithm algo = ManipulationAlgorithm.NormalForm)
        {
            if (algo == ManipulationAlgorithm.GreedySearch)
            {
                return OptimalScoringRuleOutcomeViaGreedySearch(profile, scoringVector, manipulator);
            }
            if (algo == ManipulationAlgorithm.NormalForm)
            {
                return OptimalScoringRuleOutcomeViaNormalForm(profile, scoringVector, manipulator);
            }
            throw new Exception("Algorithm not implemented");
        }

        private static int OptimalScoringRuleOutcomeViaNormalForm(
            Profile profile,
            double[] scoringVector,
            int manipulator = 0)
        {
            /*
			 * Find the sincere winner
			 */
            Dictionary<int, double> scores = VotingFunctions.FindScoringRuleScores(profile, scoringVector);
            int sincereWinner = VotingFunctions.WinnerFromScoringDict(scores);

            /*
			 * Scores without the manipulator
			 */
            RemoveManipulatorScores(scores, profile, scoringVector, manipulator);
            return NormalFormManipulation(
                profile,
                scores,
                sincereWinner,
                (strategicVote) => ScoringRuleManipulationOutcome(scores, scoringVector, strategicVote),
                manipulator);
        }

        public static int OptimalScoringRuleOutcome(
            Profile profile,
            BigDecimal[] scoringVector,
            int manipulator = 0,
            ManipulationAlgorithm algo = ManipulationAlgorithm.NormalForm)
        {
            if (algo == ManipulationAlgorithm.GreedySearch)
            {
                return OptimalScoringRuleOutcomeViaGreedySearch(profile, scoringVector, manipulator);
            }
            if (algo == ManipulationAlgorithm.NormalForm)
            {
                return OptimalScoringRuleOutcomeViaNormalForm(profile, scoringVector, manipulator);
            }
            throw new Exception("Algorithm not implemented");
        }

        private static int OptimalScoringRuleOutcomeViaNormalForm(
            Profile profile,
            BigDecimal[] scoringVector,
            int manipulator = 0)
        {
            /*
			 * Find the sincere winner
			 */
            Dictionary<int, BigDecimal> scores = VotingFunctions.FindScoringRuleScores(profile, scoringVector);
            int sincereWinner = VotingFunctions.WinnerFromScoringDict(scores);

            /*
			 * Scores without the manipulator
			 */
            RemoveManipulatorScores(scores, profile, scoringVector, manipulator);
            return NormalFormManipulation(
                profile,
                scores,
                sincereWinner,
                (strategicVote) => ScoringRuleManipulationOutcome(scores, scoringVector, strategicVote),
                manipulator);
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

		public static int ScoringRuleManipulationOutcome(
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

		public static int ScoringRuleManipulationOutcome(
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
