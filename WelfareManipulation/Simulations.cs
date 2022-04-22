using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WelfareManipulation
{
	public class Simulations
	{
		public enum AxisNames
		{
			Voters,
			Candidates
		}

		public static double ParallelAdd(ref double location1, double value)
		{
			double newCurrentValue = location1;
			while (true)
			{
				double currentValue = newCurrentValue;
				double newValue = currentValue + value;
				newCurrentValue = Interlocked.CompareExchange(ref location1, newValue, currentValue);
				if (newCurrentValue == currentValue)
					return newValue;
			}
		}

		public static Dictionary<int, double>[] GenerateUtilitySeries(
			int iterations,
			int fixedAxisNumber,
			IEnumerable<int> variableAxisRange,
			StatisticalCulture culture,
			UtilityMeasure[] utilities,
			VotingFunctions.VotingRule votingRule,
			AxisNames fixedAxis,
			out string[] fileNames)
		{
			int numberOfUtilities = utilities.Length;
			fileNames = new string[numberOfUtilities];
			Dictionary<int, double>[] results = new Dictionary<int, double>[numberOfUtilities];
			for (int j = 0; j < numberOfUtilities; j++)
			{
				results[j] = new Dictionary<int, double>();
			}
			foreach (int variableAxisNumber in variableAxisRange)
			{
				double[] utilitiesSoFar = new double[numberOfUtilities];
				Parallel.For(0, iterations, () => new double[numberOfUtilities], (i, state, subtotal) =>
				{
					Profile p;
					if (fixedAxis == AxisNames.Voters)
					{
						p = culture.GenerateProfile(fixedAxisNumber, variableAxisNumber);
					}
					else
					{
						p = culture.GenerateProfile(variableAxisNumber, fixedAxisNumber);
					}

					int winner = votingRule.FindWinner(p);
					return subtotal;
				},
					(subtotal) =>
					{
						for (int j = 0; j < numberOfUtilities; j++)
						{
							ParallelAdd(ref utilitiesSoFar[j], subtotal[j]);
						}
					}
					);
				for (int j = 0; j < numberOfUtilities; j++)
				{
					utilitiesSoFar[j] /= iterations;
					results[j][variableAxisNumber] = utilitiesSoFar[j];
				}
			}
			for (int j = 0; j < numberOfUtilities; j++)
			{
				string fileName = votingRule.Name
				+ "_iterations=" + iterations
				+ "_" + fixedAxis.ToString() + "=" + fixedAxisNumber
				+ "_" + culture.Name
				+ "_" + utilities[j].Name;
				fileNames[j] = fileName;
			}

			return results;
		}
	}
}
