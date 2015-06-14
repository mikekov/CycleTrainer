using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Dynastream.Fit;

// Import *.fit activity file from Garmin devices

namespace CycleTrainer
{
	public class FitFile : Activity
	{
		public FitFile(string dir, string file, System.DateTime creation)
		{
			Directory = dir;
			FileName = file;
			// 2014-12-07-07-56-15.fit
			var match= Regex.Match(file, @"^.*(\d{4})-(\d{2})-(\d{2})-(\d{2})-(\d{2})-(\d{2})\..*$");
			if (match.Success)
			{
				var g= match.Groups[0];
				CreationTime = new System.DateTime(
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
		public System.DateTime CreationTime { get; private set; }
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

		Samples samples_ = null;

		static Samples Import(string file_path)
		{
			var samples = new List<Sample>();
			double start_time = 0.0;

			using (FileStream fitSource = new FileStream(file_path, FileMode.Open))
			{
				Decode decoder = new Decode();
				MesgBroadcaster mesgBroadcaster = new MesgBroadcaster();

				// Connect the Broadcaster to our event (message) source (in this case the Decoder)
				decoder.MesgEvent += mesgBroadcaster.OnMesg;
				decoder.MesgDefinitionEvent += mesgBroadcaster.OnMesgDefinition;

				// Subscribe to message events of interest by connecting to the Broadcaster
				mesgBroadcaster.MesgEvent += new MesgEventHandler((object sender, MesgEventArgs e) => OnMesg(sender, e, samples, ref start_time));
				mesgBroadcaster.MesgDefinitionEvent += new MesgDefinitionEventHandler(OnMesgDefn);

				mesgBroadcaster.FileIdMesgEvent += new MesgEventHandler(OnFileIDMesg);
				mesgBroadcaster.UserProfileMesgEvent += new MesgEventHandler(OnUserProfileMesg);

				bool status = decoder.IsFIT(fitSource) && decoder.CheckIntegrity(fitSource);
				// Process the file
				if (status)
				{
					// Decoding...
					decoder.Read(fitSource);
				}
				else
				{
					try
					{
						// Integrity Check Failed
						// Attempting to decode...
						decoder.Read(fitSource);
					}
					catch (FitException )
					{
						//
					}
				}
			}

			double duration = 0;
			var start = System.DateTime.Now;
			double distance = 0.0;
			double power = 0.0;
			if (samples.Count > 1)
			{
				duration = samples.Last().TimeOffset - samples.First().TimeOffset;
				start = new System.DateTime(1989, 12, 31, 0, 0, 0, DateTimeKind.Utc).AddSeconds(start_time).ToLocalTime();
				distance = samples.Last().Distance;
				power = samples.Sum(s => s.Power);
			}

			return new Samples(samples, start, duration, 0, distance, 0, power);
		}

		static void OnMesgDefn(object sender, MesgDefinitionEventArgs e)
		{
		//	Console.WriteLine("OnMesgDef: Received Defn for local message #{0}, global num {1}", e.mesgDef.LocalMesgNum, e.mesgDef.GlobalMesgNum);
		//	Console.WriteLine("\tIt has {0} fields and is {1} bytes long", e.mesgDef.NumFields, e.mesgDef.GetMesgSize());
		}

		const int RECORD = 20;
		const int SESSION = 18;

		static void OnMesg(object sender, MesgEventArgs e, List<Sample> samples, ref double start_time)
		{
			//Console.WriteLine("OnMesg: Received Mesg with global ID#{0}, its name is {1}", e.mesg.Num, e.mesg.Name);
			if (e.mesg.Num == RECORD)	// data record in Garmin Edge 800
			{
				double time_offset = 0.0;
				double hr = double.NaN, spd = 0, cad = 0, dist = 0, lat = double.NaN, lon = double.NaN, alt = double.NaN, power = 0;
				double div = Math.Pow(2.0, 31.0);
				double temperature = double.NaN;

				for (int i = 0; i < e.mesg.GetNumFields(); ++i)
				{
					if (e.mesg.fields[i].GetNumValues() < 1)
						continue;

					int j = 0;
					var value = e.mesg.fields[i].GetValue(j);

					switch (e.mesg.fields[i].GetName())
					{
					case "Timestamp":
						time_offset = Convert.ToDouble(value);
						if (start_time == 0.0)
							start_time = time_offset;
						time_offset -= start_time;
						break;
					case "PositionLat":
						lat = Convert.ToDouble(value) * 180.0 / div;
						break;
					case "PositionLong":
						lon = Convert.ToDouble(value) * 180.0 / div;
						break;
					case "Distance":
						dist = Convert.ToDouble(value);
						break;
					case "Altitude":
						alt = Convert.ToDouble(value);
						break;
					case "HeartRate":
						hr = Convert.ToDouble(value);
						break;
					case "Temperature":
						temperature = Convert.ToDouble(value);
						break;
					case "Speed":
						spd = Convert.ToDouble(value);
						break;
					case "Cadence":
						cad = Convert.ToDouble(value);
						break;
					case "Power":
						power = Convert.ToDouble(value);
						break;
					}
				}

				var sample = new Sample(time_offset, hr, spd, cad, dist, lat, lon, alt, temperature, power);
				samples.Add(sample);
			}
			else if (e.mesg.Num == SESSION)
			{
				//
			}

//			for (int i = 0; i < e.mesg.GetNumFields(); ++i)
//			{
//				for (int j = 0; j < e.mesg.fields[i].GetNumValues(); ++j)
//				{
////					Console.WriteLine("\tField{0} Index{1} (\"{2}\" Field#{4}) Value: {3} (raw value {5})", i, j, e.mesg.fields[i].GetName(), e.mesg.fields[i].GetValue(j), e.mesg.fields[i].Num, e.mesg.fields[i].GetRawValue(j));
//				}
//			}

			//if (mesgCounts.ContainsKey(e.mesg.Num) == true)
			//{
			//	mesgCounts[e.mesg.Num]++;
			//}
			//else
			//{
			//	mesgCounts.Add(e.mesg.Num, 1);
			//}
		}

		static void OnFileIDMesg(object sender, MesgEventArgs e)
		{
			//Console.WriteLine("FileIdHandler: Received {1} Mesg with global ID#{0}", e.mesg.Num, e.mesg.Name);
			//FileIdMesg myFileId = (FileIdMesg)e.mesg;
			//try
			//{
			//	Console.WriteLine("\tType: {0}", myFileId.GetType());
			//	Console.WriteLine("\tManufacturer: {0}", myFileId.GetManufacturer());
			//	Console.WriteLine("\tProduct: {0}", myFileId.GetProduct());
			//	Console.WriteLine("\tSerialNumber {0}", myFileId.GetSerialNumber());
			//	Console.WriteLine("\tNumber {0}", myFileId.GetNumber());
			//	Dynastream.Fit.DateTime dtTime = new Dynastream.Fit.DateTime(myFileId.GetTimeCreated().GetTimeStamp());

			//}
			//catch (FitException exception)
			//{
			//	Console.WriteLine("\tOnFileIDMesg Error {0}", exception.Message);
			//	Console.WriteLine("\t{0}", exception.InnerException);
			//}
		}

		static void OnUserProfileMesg(object sender, MesgEventArgs e)
		{
			//Console.WriteLine("UserProfileHandler: Received {1} Mesg, it has global ID#{0}", e.mesg.Num, e.mesg.Name);
			//UserProfileMesg myUserProfile = (UserProfileMesg)e.mesg;
			//try
			//{
			//	Console.WriteLine("\tFriendlyName \"{0}\"", Encoding.UTF8.GetString(myUserProfile.GetFriendlyName()));
			//	Console.WriteLine("\tGender {0}", myUserProfile.GetGender().ToString());
			//	Console.WriteLine("\tAge {0}", myUserProfile.GetAge());
			//	Console.WriteLine("\tWeight {0}", myUserProfile.GetWeight());
			//}
			//catch (FitException exception)
			//{
			//	Console.WriteLine("\tOnUserProfileMesg Error {0}", exception.Message);
			//	Console.WriteLine("\t{0}", exception.InnerException);
			//}
		}

	}
}
