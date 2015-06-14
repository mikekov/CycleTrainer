using System;

namespace CycleTrainer
{
	public class Point
	{
		public Point(double x, double y)
		{
			this.x = x;
			this.y = y;
		}

		public double X
		{
			get { return x; }
			set { x = value; }
		}

		public double Y
		{
			get { return y; }
			set { y = value; }
		}

		double x;
		double y;
	}
}
