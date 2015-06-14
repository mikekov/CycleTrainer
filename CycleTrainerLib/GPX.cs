using System;
using System.Xml;

// Export samples in GPX format

namespace CycleTrainer
{
	public static class GPX
	{
		public static void Export(CycleTrainer.Samples samples, string path, bool indent = true)
		{
			using (var writer = XmlWriter.Create(path, new XmlWriterSettings() { CloseOutput = true, Indent = indent, NewLineChars = Environment.NewLine, NewLineHandling = NewLineHandling.Replace, IndentChars = "\t" }))
			{
				writer.WriteStartDocument();

				writer.WriteStartElement("gpx", "http://www.topografix.com/GPX/1/1");
				writer.WriteAttributeString("version", "1.1");
				writer.WriteAttributeString("creator", "CycleTrainer 0.1 by Mike Kowalski");

				writer.WriteAttributeString("xmlns", "xsi", null, "http://www.w3.org/2001/XMLSchema-instance");

				writer.WriteAttributeString("xsi", "schemaLocation", null, "http://www.topografix.com/GPX/1/1 http://www.topografix.com/GPX/1/1/gpx.xsd http://www.garmin.com/xmlschemas/GpxExtensions/v3 http://www.garmin.com/xmlschemas/GpxExtensionsv3.xsd http://www.garmin.com/xmlschemas/TrackPointExtension/v1 http://www.garmin.com/xmlschemas/TrackPointExtensionv1.xsd");
				writer.WriteAttributeString("xmlns", "gpxtpx", null, "http://www.garmin.com/xmlschemas/TrackPointExtension/v1");
				writer.WriteAttributeString("xmlns", "gpxx", null, "http://www.garmin.com/xmlschemas/GpxExtensions/v3");

				//				writer.WriteEndAttribute();
				//				writer.WriteQualifiedName("qloca", "gpx");

				writer.WriteStartElement("trk");

				// name
				writer.WriteStartElement("name");
				writer.WriteValue(samples.Date);
				writer.WriteFullEndElement();

				writer.WriteStartElement("trkseg");

				foreach (var s in samples.Trip)
				{
					if (!s.IsPostionValid)
						continue;

					writer.WriteStartElement("trkpt");

					writer.WriteStartAttribute("lon");
					writer.WriteValue(s.Longitude);
					writer.WriteStartAttribute("lat");
					writer.WriteValue(s.Latitude);

					if (s.IsAltitudeValid)
					{
						writer.WriteStartElement("ele");
						writer.WriteValue(s.Altitude);
						writer.WriteFullEndElement();
					}

					writer.WriteStartElement("time");
					var dt = samples.Date.AddSeconds(s.TimeOffset).ToUniversalTime();
					writer.WriteValue(dt.ToString("u").Replace(' ', 'T'));
					writer.WriteFullEndElement();

					writer.WriteStartElement("extensions");
					writer.WriteStartElement("TrackPointExtension", "gpxtpx");
					if (s.IsHeartRateValid)
					{
						writer.WriteStartElement("hr", "gpxtpx");
						writer.WriteValue((int)s.HeartRate);
						writer.WriteFullEndElement();
					}
					if (s.IsCadenceValid)
					{
						writer.WriteStartElement("cad", "gpxtpx");
						writer.WriteValue((int)s.Cadence);
						writer.WriteFullEndElement();
					}
					writer.WriteFullEndElement();
					writer.WriteFullEndElement();

					writer.WriteFullEndElement(); // trkpt
				}

				writer.WriteEndElement(); // trkseg

				writer.WriteEndElement(); // trk

				writer.WriteEndElement(); // gpx

				writer.WriteEndDocument();
			}
		}
	}
}
