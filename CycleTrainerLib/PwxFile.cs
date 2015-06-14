using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

// Import *.pwx activity file from Timex CycleTrainer GPS unit

namespace CycleTrainer
{
	public class PwxFile : Activity
	{
		public PwxFile(string dir, string file, DateTime creation)
		{
			Directory = dir;
			FileName = file;
			var match = Regex.Match(file, @"^.*(\d{4})_(\d{2})_(\d{2})_(\d{2})_(\d{2})_(\d{2})\..*$");
			if (match.Success)
			{
				var g = match.Groups[0];
				CreationTime = new DateTime(
					int.Parse(match.Groups[1].Captures[0].Value),
					int.Parse(match.Groups[2].Captures[0].Value),
					int.Parse(match.Groups[3].Captures[0].Value),
					int.Parse(match.Groups[4].Captures[0].Value),
					int.Parse(match.Groups[5].Captures[0].Value),
					int.Parse(match.Groups[6].Captures[0].Value));
			}
			else
				CreationTime = creation;
		}

		public string FileName { get; private set; }
		public string FilePath { get { return Directory + FileName; } }
		public string Directory { get; private set; }
		public DateTime CreationTime { get; private set; }
		public override string ToString()
		{
			return FileName;
		}

		public void Rename(string new_file_name)
		{
			FileName = new_file_name;
		}

		public Samples Samples
		{
			get
			{
				if (samples_ != null)
					return samples_;
				samples_ = Import(FilePath);
				return samples_;
			}
		}

		Samples samples_;

		public static Samples Import(string path)
		{
			var settings = new XmlReaderSettings();
			settings.IgnoreComments = true;
			settings.IgnoreWhitespace = true;

			var samples = new List<Sample>();
			DateTime start_time = DateTime.MinValue;
			double duration = 0, duration_stopped = 0;
			double distance = 0;
			double work = 0;

			using (var reader = XmlReader.Create(path, settings))
			{
				bool skipped = false;

				while (skipped || reader.Read())
				{
					skipped = false;

					if (reader.IsStartElement("time"))
					{
						reader.Read();
						var time= reader.ReadContentAsString();
						DateTime.TryParse(time, null, DateTimeStyles.RoundtripKind, out start_time);
					}
					else if (reader.IsStartElement("segment"))
					{
						if (reader.Read())
						{
							for (bool read= true; read; )
							{
								switch (reader.Name)
								{
								//case "beginning":
								//	reader.ReadElementContentAsDouble();
								//	break;
								//case "duration":
								//	duration = reader.ReadElementContentAsDouble();
								//	break;
								//case "durationstopped":
								//	duration_stopped = reader.ReadElementContentAsDouble();
								//	break;
								//case "dist":
								//	distance = reader.ReadElementContentAsDouble();
								//	break;
								case "work":
									work += reader.ReadElementContentAsDouble();
									break;
								case "name":
									reader.Skip();
									break;
								case "summarydata":
									if (reader.IsStartElement())
										reader.Read();
									else
										read = false;
									break;
								default:
									reader.Skip();
									break;
								}
							}
						}
					}
					else if (reader.IsStartElement("summarydata"))
					{
						if (reader.Read())
						{
							for (bool read = true; read; )
							{
								switch (reader.Name)
								{
								case "beginning":
									reader.ReadElementContentAsDouble();
									break;
								case "duration":
									duration = reader.ReadElementContentAsDouble();
									break;
								case "durationstopped":
									duration_stopped = reader.ReadElementContentAsDouble();
									break;
								case "dist":
									distance = reader.ReadElementContentAsDouble();
									break;
								case "work":
									work = reader.ReadElementContentAsDouble();
									break;
								default:
									read = false;
									break;
								}
							}
						}
					}
					else if (reader.IsStartElement("sample"))
					{
						double to = double.NaN;
						double hr = double.NaN;
						double spd = 0;
						double cad = double.NaN;
						double dist = 0;
						double lat = double.NaN, lon = double.NaN, alt = double.NaN;
						double power = 0;

						if (reader.Read())
						{
							for (bool read = true; read; )
							{
								switch (reader.Name)
								{
								case "timeoffset":
									to = reader.ReadElementContentAsDouble();
									break;
								case "hr":
									double h= reader.ReadElementContentAsInt();
									if (h > 0)
										hr = h;
									break;
								case "spd":
									spd = reader.ReadElementContentAsDouble();
									break;
								case "cad":
									cad = reader.ReadElementContentAsInt();
									break;
								case "dist":
									dist = reader.ReadElementContentAsDouble();
									break;
								case "lat":
									lat = reader.ReadElementContentAsDouble();
									break;
								case "lon":
									lon = reader.ReadElementContentAsDouble();
									break;
								case "alt":
									alt = reader.ReadElementContentAsDouble();
									break;

								case "extension":
									reader.Skip();
									read = false;
									//skipped = true;
									double temperature = double.NaN;
									samples.Add(new Sample(to, hr, spd, cad, dist, lat, lon, alt, temperature, power));
									break;

								//case "gpsstatus":
								//	var stat= reader.ReadElementContentAsInt();
								//	if (stat != 3)
								//	{ }
								//	break;

								default:
									break;
								}
							}
						}
					}
					else
					{
						//System.Diagnostics.Debug.WriteLine("Element: '" + reader.Name + "' type: " + reader.NodeType);
						//var s= reader.ReadString();
					}
				}
			}

			return new Samples(samples, start_time, duration, duration_stopped, distance, work, 0);
		}
	}
}
