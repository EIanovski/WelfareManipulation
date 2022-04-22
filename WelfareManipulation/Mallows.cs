using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WelfareManipulation
{ 
	public class Mallows
	{
		double[] RankingProbabilities;
		Dictionary<int, IEnumerable<int>> ReferenceRankings = new Dictionary<int, IEnumerable<int>>();
		public Dictionary<int, double[][]> InsertionPobabilities = new Dictionary<int, double[][]>();
		int NumberOfElements;

		public Mallows(IEnumerable<double> dispersions, IEnumerable<double> centerProbabilities, IEnumerable<IEnumerable<int>> referenceRankings)
		{
			NumberOfElements = referenceRankings.ElementAt(0).Count();
			double thresholdProbability = 0;
			RankingProbabilities = new double[NumberOfElements];
			int numberOfRankings = referenceRankings.Count();
			for (int i = 0; i < numberOfRankings; i++)
			{
				thresholdProbability += centerProbabilities.ElementAt(i);
				RankingProbabilities[i] = thresholdProbability;
				ReferenceRankings[i] = referenceRankings.ElementAt(i);
				InsertionPobabilities[i] = CalculateInsertionProbabilities(dispersions.ElementAt(i));
			}
		}

		public double[][] CalculateInsertionProbabilities(double dispersion)
		{
			double[][] insertionProbabilities = new double[NumberOfElements][];
			double normalisationConstant = 0;
			for (int i = 1; i <= NumberOfElements; i++)
			{
				normalisationConstant += Math.Pow(dispersion, i - 1);
				insertionProbabilities[i - 1] = new double[i];
				double thresholdProbability = 0;
				for (int j = 0; j < i; j++)
				{
					thresholdProbability += Math.Pow(dispersion, i - j - 1) / normalisationConstant;
					insertionProbabilities[i - 1][j] = thresholdProbability;
				}
			}
			return insertionProbabilities;
		}

		public static double ComputeNormalisationConstant(double dispersion, double terms)
		{
			double constant = 0;
			for (int i = 0; i < terms; i++)
			{
				constant += Math.Pow(dispersion, i);
			}
			return constant;
		}

		IEnumerable<int> ChooseRanking(out double[][] insertionProbabilities)
		{
			double randomDraw = ThreadLocalRandom.NextDouble();
			for (int i = 0; i < NumberOfElements; i++)
			{
				if (randomDraw < RankingProbabilities[i])
				{
					insertionProbabilities = InsertionPobabilities[i];
					return ReferenceRankings[i];
				}
			}
			throw new Exception("The last entry in rankingProbabilities should be 1");
		}

		/*
		 * Samples an order using the repeated insertion method of Doignon et al. (2004)
		 */
		public IEnumerable<int> Sample()
		{
			double[][] insertionPobabilities;
			IEnumerable<int> referenceRanking = ChooseRanking(out insertionPobabilities);
			List<int> outputRanking = new List<int>();
			for (int i = 0; i < NumberOfElements; i++)
			{
				double randomDraw = ThreadLocalRandom.NextDouble();
				for (int j = 0; j <= i; j++)
				{
					if (randomDraw < insertionPobabilities[i][j])
					{
						outputRanking.Insert(j, referenceRanking.ElementAt(i));
						break;
					}
				}
			}
			return outputRanking;
		}

	}
}
