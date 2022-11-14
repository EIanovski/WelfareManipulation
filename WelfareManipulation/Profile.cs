using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;


namespace WelfareManipulation
{
	public class Profile
	{
		public int NumberOfVoters;
		public int NumberOfCandidates;

		public IEnumerable<int> Voters
		{
			get
			{
				return Enumerable.Range(0, NumberOfVoters);
			}
		}

		public IEnumerable<int> Candidates
		{
			get
			{
				return Enumerable.Range(0, NumberOfCandidates);
			}
		}

		private int[,] _profile;
		private int[,] _tournamentMatrix = null;

		public int[] GetVoter(int voterNumber)
		{
			return Enumerable.Range(0, NumberOfCandidates)
					.Select(x => _profile[voterNumber, x])
					.ToArray();
		}

		internal int VoterIRanks(int voter, int candidate)
		{
			for (int i = 0; i < NumberOfCandidates; i++)
			{
				if (AgentsIthChoice(voter, i) == candidate)
				{
					return i;
				}
			}
			throw new Exception("Voter " + voter + " does not have candidate " + candidate + " in ranking");
		}

		internal void AddOrRemoveTournamentMatrixEntry(int voter, IEnumerable<int> newPrefs = null)
		{
			HashSet<int> lowSet = new HashSet<int>(Candidates);
			bool removingPrefs = newPrefs == null;
            IEnumerable<int> prefs = removingPrefs ? GetVoter(voter) : newPrefs;
            foreach (int candidate in prefs)
            {
                lowSet.Remove(candidate);
                foreach (int lowCandidate in lowSet)
                {
					if (removingPrefs)
					{
                        _tournamentMatrix[candidate, lowCandidate]--;
                    } else
					{
                        _tournamentMatrix[candidate, lowCandidate]++;
                    }
                }
            }
        }

		private void UpdateTournamentMatrix(int voter, IEnumerable<int> newPrefs)
		{
			AddOrRemoveTournamentMatrixEntry(voter);
			AddOrRemoveTournamentMatrixEntry(voter, newPrefs);
        }

        public void SetVoter(int voterNumber, IEnumerable<int> newPrefs)
        {
            if (_tournamentMatrix != null)
            {
                UpdateTournamentMatrix(voterNumber, newPrefs);
            }
            int i = 0;
            foreach (int candidate in newPrefs)
            {
                _profile[voterNumber, i] = candidate;
                i++;
            }
        }

        public int AgentsIthChoice(int agent, int i)
		{
			return _profile[agent, i];
		}

		public bool AgentIPrefers(int agent, int x, int y)
		{
			for (int pos = 0; pos < NumberOfCandidates; pos++)
			{
				if (AgentsIthChoice(agent, pos) == x)
				{
					return true;
				}
				if (AgentsIthChoice(agent, pos) == y)
				{
					return false;
				}
			}
			throw new ArgumentException("No such candidates");
		}

        public int HowManyPrefer(int firstCandidate, int secondCandidate)
        {
            if (_tournamentMatrix == null)
            {
                BuildTournamentMatrix();
            }
            return _tournamentMatrix[firstCandidate, secondCandidate];
        }

        private void BuildTournamentMatrix()
        {
            _tournamentMatrix = new int[NumberOfCandidates, NumberOfCandidates];
            for (int agent = 0; agent < NumberOfVoters; agent++)
            {
                HashSet<int> unseenCandidates = new HashSet<int>(Candidates);
                for (int candidateIndex = 0; candidateIndex < NumberOfCandidates; candidateIndex++)
                {
                    int candidate = AgentsIthChoice(agent, candidateIndex);
                    unseenCandidates.Remove(candidate);
                    foreach (int worseCandidate in unseenCandidates)
                    {
                        _tournamentMatrix[candidate, worseCandidate]++;
                    }
                }
            }
        }


        public double CopelandScore(int candidate, double scoreForTies = 0.5)
        {
            double score = 0;
            foreach (int otherCandidate in Candidates)
            {
                if (otherCandidate != candidate)
                {
                    if (HowManyPrefer(candidate, otherCandidate) > NumberOfVoters / 2.0)
                    {
                        score++;
                    }
                    if (HowManyPrefer(candidate, otherCandidate) == NumberOfVoters / 2.0)
                    {
                        score += scoreForTies;
                    }
                }
            }
            return score;
        }


        public Profile(int[,] preferenceMatrix)
		{
			NumberOfVoters = preferenceMatrix.GetLength(0);
			NumberOfCandidates = preferenceMatrix.GetLength(1);
			_profile = preferenceMatrix;
		}

		public Profile(IEnumerable<IEnumerable<int>> preferenceMatrix)
		{
			NumberOfVoters = preferenceMatrix.Count();
			NumberOfCandidates = preferenceMatrix.ElementAt(0).Count();
			_profile = new int[NumberOfVoters, NumberOfCandidates];
			for (int i = 0; i < NumberOfVoters; i++)
			{
				Debug.Assert(NumberOfCandidates == preferenceMatrix.ElementAt(i).Count());

				for (int j = 0; j < NumberOfCandidates; j++)
				{
					_profile[i, j] = preferenceMatrix.ElementAt(i).ElementAt(j);
				}
			}
		}

		public int BordaUtility(int candidate)
		{
			int bordaUtility = 0;
			foreach (int voter in Voters)
			{
				for (int position = 0; position < NumberOfCandidates; position++)
				{
					if (AgentsIthChoice(voter, position) == candidate)
					{
						bordaUtility += NumberOfCandidates - position;
						break;
					}
				}
			}
			return bordaUtility - NumberOfVoters;
		}

		public double NormalisedBordaUtility(int candidate)
		{
			double maxBordaUtility = (NumberOfCandidates - 1) * NumberOfVoters;
			return 100 * BordaUtility(candidate) / maxBordaUtility;
		}

		public double RootNashUtility(int candidate)
		{
			double nash = 1;
			double normalisingRoot = 1.0 / NumberOfVoters;
			foreach (int voter in Voters)
			{
				for (int position = 0; position < NumberOfCandidates; position++)
				{
					if (AgentsIthChoice(voter, position) == candidate)
					{
						int rank = NumberOfCandidates - position - 1;
						nash *= Math.Pow(rank, normalisingRoot);
						break;
					}
				}
			}
			return nash;
		}

		public double NormalisedNashUtility(int candidate)
		{
			double rootNash = RootNashUtility(candidate);
			double maxRoot = Math.Pow(NumberOfCandidates - 1, 1.0 / NumberOfVoters);
			double maxNash = Math.Pow(maxRoot, NumberOfVoters);
			return 100 * rootNash / maxNash;
		}

		public int RawlsUtility(int candidate)
		{
			int lowestPosition = 0;

			foreach (int voter in Voters)
			{
				for (int position = lowestPosition; position < NumberOfCandidates; position++)
				{
					if (AgentsIthChoice(voter, position) == candidate)
					{
						lowestPosition = position;
						break;
					}
				}
			}
			return NumberOfCandidates - lowestPosition - 1;
		}

		public double NormalisedRawlsUtility(int candidate)
		{
			double maxRawls = NumberOfCandidates - 1;
			return 100 * RawlsUtility(candidate) / maxRawls;
		}

		internal void Print()
		{
			StringBuilder text = new StringBuilder();
			text.Append(@"\begin{tabular}{");
			for (int i = 0; i < NumberOfVoters; i++)
			{
				text.Append("c ");
			}
			text.Append(@"}");
			text.Append(Environment.NewLine);

			for (int i = 0; i < NumberOfVoters; i++)
			{
				text.Append(i);
				if (i < NumberOfVoters - 1)
				{
					text.Append(@" & ");
				}
			}
			text.Append(@"\\");
			text.Append(Environment.NewLine);
			text.Append(@"\hline");
			text.Append(Environment.NewLine);
			for (int candidateIndex = 0; candidateIndex < NumberOfCandidates; candidateIndex++)
			{
				for (int voter = 0; voter < NumberOfVoters; voter++)
				{
					text.Append(AgentsIthChoice(voter, candidateIndex));
					if (voter < NumberOfVoters - 1)
					{
						text.Append(@" & ");
					}
				}
				text.Append(@"\\");
				text.Append(Environment.NewLine);
			}
			text.Append(@"\end{tabular}");
			Console.Write(text);
		}

		public static Profile GenerateICProfile(int numberOfAgents, int numberOfCandidates)
		{
			var profile = new int[numberOfAgents, numberOfCandidates];

			for (int i = 0; i < numberOfAgents; i++)
			{
				for (int j = 0; j < numberOfCandidates; j++)
				{
					profile[i, j] = j;
				}
				Utilities.ShuffleRow(profile, i);
			}
			return new Profile(profile);
		}

		public static Profile GenerateEuclideanProfile(int numberOfAgents, int numberOfCandidates, int dimension)
		{
			List<Point> agents = GenerateIdealPoints(numberOfAgents, dimension);
			List<Point> candidatePoints = GenerateIdealPoints(numberOfCandidates, dimension);
			List<List<int>> profile = new List<List<int>>();
			foreach (Point agent in agents)
			{
				List<int> agentPreferences = new List<int>();

				List<double> distancesToCanidates = new List<double>();
				foreach (Point candidate in candidatePoints)
				{
					distancesToCanidates.Add(agent.DistanceTo(candidate));
				}
				List<(int, Point)> candidates = new List<(int, Point)>();
				for (int candidateIndex = 0; candidateIndex < numberOfCandidates; candidateIndex++)
				{
					candidates.Add((candidateIndex, candidatePoints[candidateIndex]));
				}
				candidates.Sort((first, second) =>
				distancesToCanidates[first.Item1] > distancesToCanidates[second.Item1] ? 1 :
				distancesToCanidates[first.Item1] == distancesToCanidates[second.Item1] ? 0 :
				-1);
				foreach ((int, Point) candidate in candidates)
				{
					agentPreferences.Add(candidate.Item1);
				}
				profile.Add(agentPreferences);
			}
			Profile p = new Profile(profile);
			return p;
		}

		/*
		 * Samples preferences from prefsInBag, with replacement
		 */
		public static Profile GenerateBagProfile(
			int numberOfVoters,
			int[][] prefsInBag)
		{
			int numPrefs = prefsInBag.Length;
			IEnumerable<int>[] profile = new IEnumerable<int>[numberOfVoters];
			for (int i = 0; i < numberOfVoters; i++)
			{
				int nextPref = ThreadLocalRandom.Next(numPrefs);
				profile[i] = prefsInBag[nextPref];
			}
			return new Profile(profile);
		}

		public static Profile GenerateMallowsProfile(
			int numberOfVoters,
			IEnumerable<double> dispersions,
			IEnumerable<double> centerProbabilities,
			IEnumerable<IEnumerable<int>> referenceRankings)
		{

			IEnumerable<int>[] profile = new IEnumerable<int>[numberOfVoters];
			Mallows model = new Mallows(dispersions, centerProbabilities, referenceRankings);
			for (int i = 0; i < numberOfVoters; i++)
			{
				profile[i] = model.Sample();
			}
			return new Profile(profile);
		}

		public static List<Point> GenerateIdealPoints(int numberOfPoints, int dimension)
		{
			List<Point> points = new List<Point>();
			for (int i = 0; i < numberOfPoints; i++)
			{
				points.Add(Point.GenerateRandomPoint(dimension));
			}
			return points;
		}
	}
}
