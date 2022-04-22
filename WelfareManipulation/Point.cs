using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WelfareManipulation
{
	public class Point
	{
		private double[] _coordinates;

		public override string ToString()
		{
			StringBuilder s = new StringBuilder("(");
			for (int i = 0; i < Dimension; i++)
			{
				s.Append(_coordinates[i] + ", ");
			}
			s.Remove(s.Length - 3, 2);
			s.Append(")");
			return s.ToString();
		}

		public int Dimension
		{
			get
			{
				return _coordinates.Length;
			}
		}

		public Point(double[] coordinates)
		{
			_coordinates = coordinates;
		}

		public double GetCoordinate(int coordinate)
		{
			return _coordinates[coordinate];
		}

		public static Point GenerateRandomPoint(int dimension)
		{
			double[] coordinates = new double[dimension];
			for (int i = 0; i < dimension; i++)
			{
				coordinates[i] = ThreadLocalRandom.NextDouble();
			}
			return new Point(coordinates);
		}

		public double DistanceTo(Point other)
		{
			Debug.Assert(Dimension == other.Dimension);
			double radicand = 0;
			for (int i = 0; i < Dimension; i++)
			{
				radicand += Math.Pow(GetCoordinate(i) - other.GetCoordinate(i), 2);
			}
			return Math.Sqrt(radicand);
		}
	}
}
