using System;
using System.Collections.Generic;
using System.Text;

namespace WelfareManipulation
{
	internal class Utilities
	{
		internal static void ShuffleRow(int[,] array, int row)
		{
			int n = array.GetLength(1);
			while (n > 1)
			{
				n--;
				int i = ThreadLocalRandom.Next(n + 1);
				int temp = array[row, i];
				array[row, i] = array[row, n];
				array[row, n] = temp;
			}
		}

		public static string DictionaryToString(Dictionary<int, double> dict)
		{
			StringBuilder str = new StringBuilder();
			foreach (int key in dict.Keys)
			{
				str.Append(key + " " + dict[key] + Environment.NewLine);
			}
			return str.ToString();
		}
	}
}