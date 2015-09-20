using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CycleTrainer
{
	// single sample/event from activity file
	public class Sample
	{
		public Sample(double to, double hr, double spd, double cad, double dist, double lat, double lon, double alt, double temperature, double power)
		{
			TimeOffset = to;
			HeartRate = hr;
			Speed = spd;
			Cadence = cad;
			Distance = dist;
			Latitude = lat;
			Longitude = lon;
			Altitude = alt;
			Temperature = temperature;
			Power = power;
		}

		public double TimeOffset { get; private set; }	// s
		public double HeartRate { get; private set; }	// bpm
		public double Speed { get; private set; }	// ?
		public double Cadence { get; private set; }	// rpm
		public double Distance { get; private set; }	// m
		public double Latitude { get; private set; }	// deg
		public double Longitude { get; private set; }	// deg
		public double Altitude { get; private set; }	// m
		public double Temperature { get; private set; }	// C
		public double Power { get; private set; }	// W

		public bool IsPostionValid
		{
			get { return !double.IsNaN(Latitude) && !double.IsNaN(Longitude) && (Latitude != 0.0 || Longitude != 0.0); }
		}

		public bool IsAltitudeValid
		{
			get { return !double.IsNaN(Altitude); }
		}

		public bool IsHeartRateValid
		{
			get { return !double.IsNaN(HeartRate); }
		}

		public bool IsCadenceValid
		{
			get { return !double.IsNaN(Cadence); }
		}

		// status ...
	}

	// collection of samples
	public class Samples
	{
		public Samples(List<Sample> samples, DateTime date_time, double duration, double duration_stopped, double distance, double work, double power, string manufacturer, string product)
		{
			Trip = samples.ToArray();
			Date = date_time;
			Duration = duration;
			Distance = distance;
			DurationStopped = duration_stopped;
			Work = work;
			Power = power;
			Manufacturer = manufacturer;
			Product = product;

			var min_lat = double.NaN;
			var max_lat = double.NaN;
			var min_lon = double.NaN;
			var max_lon = double.NaN;

			foreach (var s in Trip)
			{
				if (double.IsNaN(s.Latitude) || double.IsNaN(s.Longitude) || (s.Latitude == 0.0 && s.Longitude == 0.0))
					continue;

				min_lat = min(min_lat, s.Latitude);
				max_lat = max(max_lat, s.Latitude);

				min_lon = min(min_lon, s.Longitude);
				max_lon = max(max_lon, s.Longitude);
			}

			if (double.IsNaN(min_lat) || double.IsNaN(max_lat) ||
				double.IsNaN(min_lon) || double.IsNaN(max_lon))
				Area = Rectangle.Empty;
			else
				Area = Rectangle.FromLTRB((float)min_lon, (float)min_lat, (float)max_lon, (float)max_lat);
		}

		static double min(double l1, double l2)
		{
			if (double.IsNaN(l1))
				return l2;

			return Math.Min(l1, l2);
		}

		static double max(double l1, double l2)
		{
			if (double.IsNaN(l1))
				return l2;

			return Math.Max(l1, l2);
		}

		public Sample[] Trip { get; private set; }
		public DateTime Date { get; private set; }
		public Rectangle Area { get; private set; }
		public double Duration { get; private set; }
		public double DurationStopped { get; private set; }
		public double Distance { get; private set; }
		public double Work { get; private set; }
		public double Power { get; private set; }
		public string Manufacturer { get; private set; }
		public string Product { get; private set; }
	}
}
