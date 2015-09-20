using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using CycleTrainer;
using Microsoft.Practices.Prism.Mvvm;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace CycleApp
{
	class StatisticsDataSource : BindableBase
	{
		public StatisticsDataSource()
		{
			ElevationHysterises = 0.0; // 9.0 for Timex Cycle Trainer, 0.0 for Garmin Edge;
		}

		class VertAxis : OxyPlot.Axes.LinearAxis
		{
			public VertAxis()
			{
				ShowLabels = true;
			}

			public bool ShowLabels { set; get; }

			public override void GetTickValues(
			out IList<double> majorLabelValues, out IList<double> majorTickValues, out IList<double> minorTickValues)
			{
				if (ShowLabels)
				{
					base.GetTickValues(out majorLabelValues, out majorTickValues, out minorTickValues);
					majorLabelValues = minorTickValues;
				}
				else
				{
					minorTickValues = empty;
					majorTickValues = empty;
					majorLabelValues = empty;
				}
			}

			List<double> empty= new List<double>();
		}

		OxyPlot.Series.LineSeries CreateSeries(string name, OxyPlot.OxyColor color, CycleTrainer.Samples samples, Func<CycleTrainer.Sample, double> GetY, bool shaded)
		{
			var series= shaded ? new OxyPlot.Series.AreaSeries() : new OxyPlot.Series.LineSeries();

			series.Title = name;
			series.Color = color;
			series.MarkerType = OxyPlot.MarkerType.None;
				//MarkerSize = 1,
				//MarkerStroke = OxyColors.White,
				//MarkerFill = OxyColors.Blue,
				//MarkerStrokeThickness = 1.0,
			series.StrokeThickness = 1.0;
			series.CanTrackerInterpolatePoints = false;
			series.Smooth = false;
				//LineJoin = OxyPenLineJoin.


			double x = samples.Date.TimeOfDay.TotalSeconds;
			double min = double.MaxValue;
//			double area_min_y = 0;

			foreach (var s in samples.Trip)
			{
				var y= GetY(s);
				if (!double.IsNaN(y) && y < min)
					min = y;
			}

			foreach (var s in samples.Trip)
			{
				var y= GetY(s);
				if (double.IsNaN(y))
				{
					if (series is OxyPlot.Series.AreaSeries)
						series.Points.Add(new DataPoint(x + s.TimeOffset, min));	// hack
					else
						series.Points.Add(DataPoint.Undefined);
				}
				else
				{
					series.Points.Add(new DataPoint(x + s.TimeOffset, y));

					//if (y < min)
					//	min = y;
				}
			}

			var area = series as OxyPlot.Series.AreaSeries;
			if (area != null)
			{
				area.Fill = OxyColor.FromAColor(60, color);
				area.Color2 = OxyColors.Transparent;

				foreach (var s in samples.Trip)
				{
					var y= GetY(s);
					if (double.IsNaN(y))
						area.Points2.Add(new DataPoint(x + s.TimeOffset, min));	// hack
					else
						area.Points2.Add(new DataPoint(x + s.TimeOffset, min));
				}

				area.ConstantY2 = min;
			}

			//series.Smooth = true;

			return series;
		}

		class TrackerAnnotation : OxyPlot.Annotations.LineAnnotation
		{
			public TrackerAnnotation()
			{
				Fields = null;
			}

			public OxyPlot.Annotations.TextAnnotation[] Fields { get; set; }

			//protected override IList<ScreenPoint> GetScreenPoints()
			//{
			//	var points= new List<DataPoint>();

			//	if (Fields == null)
			//	{
			//		points.Add(new DataPoint(X, this.ActualMinimumY));
			//		points.Add(new DataPoint(X, this.ActualMaximumY));
			//	}
			//	else
			//	{
			//		points.Add(new DataPoint(X, this.ActualMinimumY));
			//		points.Add(new DataPoint(X, this.ActualMaximumY));

			//		foreach (var field in Fields)
			//		{
			//			//field.Min
			//		}
			//	}

			//	// transform to screen coordinates
			//	return points.Select(p => this.Transform(p)).ToList();
			//}
		};

		static CycleTrainer.Point GetPosition(CycleTrainer.Samples samples, int index)
		{
			if (samples != null && index >= 0 && index < samples.Trip.Length)
				return new CycleTrainer.Point(samples.Trip[index].Latitude, samples.Trip[index].Longitude);
			else
				return new CycleTrainer.Point(0, 0);
		}

		const double MS_TO_KMH= 3.6;			// m/s to km/h
		const double J_TO_CAL= 0.238902957619;	// 1 joule is equal to 0.238902957619 calories

		public void SetPlot(CycleTrainer.Samples samples)
		{
			RefreshStats(samples);

			var model = new OxyPlot.PlotModel { LegendSymbolLength = 24, LegendFontSize = 10, TitleFontSize = 12, DefaultFontSize = 10 };

			if (samples == null || samples.Trip.Length == 0)
			{
				PlotModel = model;

				if (CurrentMapPosition != null)
					CurrentMapPosition(this, new Args(GetPosition(samples, 0)));

				return;
			}

			var data = new OxyPlot.Series.LineSeries[]
			{
				CreateSeries("Heart Rate", OxyColors.Red, samples, (s) => s.HeartRate == 0.0 ? double.NaN : s.HeartRate, true),
				CreateSeries("Cadence", OxyColor.FromRgb(0,72,255), samples, (s) => double.IsNaN(s.Cadence) || s.Cadence == 0.0 ? double.NaN : s.Cadence, false),
				CreateSeries("Altitude", OxyColors.Green, samples, (s) => s.Altitude, true),
				CreateSeries("Speed", OxyColors.Gray, samples, (s) => s.Speed * MS_TO_KMH, true),	// m/s to km/h
				CreateSeries("Temperature", OxyColors.Orange, samples, (s) => s.Temperature, true)
			};

			foreach (var s in data)
				model.Series.Add(s);

			model.PlotMargins = new OxyThickness(0);
			model.Padding = new OxyThickness(0, 2, 0, 26);
			model.PlotAreaBorderThickness = new OxyThickness(0.0);
			model.PlotAreaBorderColor = OxyColor.FromHsv(0, 0, 0.7);

			// calculate min/max in each series
			((IPlotModel)model).Update(true);
			model.Axes.Clear();

			var line_color = OxyColor.FromHsv(0, 0, 0.6);

			//var axes = new LinearAxis[data.Length];
			var tips = new OxyPlot.Annotations.TextAnnotation[data.Length];

			int i = 0;
			double gap = 0.1 * (1.0 / data.Length);
			foreach (var series in data)
			{
				series.YAxisKey = (i + 1).ToString();

				var axis = new VertAxis()
				{
					//Position = (i & 1) == 0 ? AxisPosition.Left : AxisPosition.Right,
					Position = OxyPlot.Axes.AxisPosition.Left,
					PositionAtZeroCrossing = false,
					TickStyle = OxyPlot.Axes.TickStyle.Outside,
					TicklineColor = line_color,
					AxislineColor = line_color,
					AxislineStyle = LineStyle.Solid,
					MajorGridlineStyle = LineStyle.Solid,
					MajorGridlineColor = OxyColor.FromAColor(0x40, series.Color),
					MinorGridlineStyle = LineStyle.Solid,
					MinorGridlineColor = OxyColor.FromAColor(0x18, series.Color),
					StartPosition = (double)i / data.Length + gap,
					EndPosition = (double)(i + 1) / data.Length,
					Key = series.YAxisKey,
					Minimum = series.MinY,
					Maximum = series.MaxY,
					IsZoomEnabled = false,
					IsPanEnabled = false
				};

				model.Axes.Add(axis);

				var b = Colors.White; // plot.BackColor;

				tips[i] = new OxyPlot.Annotations.TextAnnotation
				{
					TextPosition = new DataPoint(0, axis.Minimum),
					Text = string.Empty,
					Background = OxyColor.FromArgb(0xc0, b.R, b.G, b.B),
					Stroke = OxyColors.Undefined,
					TextColor = series.Color,
					//VerticalAlignment = OxyPlot.VerticalAlignment.Middle,
					//VerticalAlignment = OxyPlot.VerticalAlignment.Middle,
					YAxisKey = series.YAxisKey,
					Tag = series,
					Layer = OxyPlot.Annotations.AnnotationLayer.AboveSeries
				};

				model.Annotations.Add(tips[i]);

				i++;
			}

			var x = samples.Date.TimeOfDay.TotalSeconds;

			var x_axis = new OxyPlot.Axes.TimeSpanAxis()
			{
				StringFormat = "hh:mm",
				Maximum = x,
				Minimum = x,
				Position = OxyPlot.Axes.AxisPosition.Bottom,
				PositionAtZeroCrossing = false,
				TickStyle = OxyPlot.Axes.TickStyle.Outside,
				TicklineColor = line_color,
				AxislineColor = line_color,
				AxislineStyle = LineStyle.Solid
			};

//			var sample_axis = new OxyPlot.Axes.LinearAxis()
//			{
//				Minimum = 0,
//				Maximum = samples.Trip.Length,
//				Position = OxyPlot.Axes.AxisPosition.Top,
//				PositionAtZeroCrossing = false,
//				AxislineColor = OxyColors.Transparent
////				IsAxisVisible = false
//			};

			if (samples.Trip.Length > 0)
				x_axis.Maximum += samples.Trip.Last().TimeOffset;

			model.Axes.Add(x_axis);
			//model.Axes.Add(sample_axis);
			/*
						reset_zoom_btn_.Click += delegate
						{
							plot.Zoom(x_axis, x_axis.Minimum, x_axis.Maximum);
						};

						const double zoom= 2.0;

						zoom_in_btn_.Click += delegate
						{
							plot.ZoomAt(x_axis, zoom);
						};

						zoom_out_btn_.Click += delegate
						{
							plot.ZoomAt(x_axis, 1 / zoom);
						};
			*/
			var la= new TrackerAnnotation
			{
				Type = OxyPlot.Annotations.LineAnnotationType.Vertical,
				Color = OxyColors.Gray,
				ClipByYAxis = false,
				X = 0,
				Layer = OxyPlot.Annotations.AnnotationLayer.BelowSeries,
				Fields = tips
			};
			model.Annotations.Add(la);
			/*
						plot.MouseLeave += delegate
						{
							//la.show = false;
							la.X = -1;
							foreach (var tip in tips)
								tip.Position = new DataPoint(-1, tip.Position.Y);

							model.RefreshPlot(false);
						};
			*/
			//var ta= new TextAnnotation { Position = new DataPoint(0, 2), Text = "Test" };
			//model.Annotations.Add(ta);

			//model.MouseDown += (sender, e) =>
			//{
			//	var itm= e.HitTestResult.Item;
			//	e.Handled = false;
			//};

			model.MouseMove += (sender, e) =>
			{
				e.Handled = false;

				//if (la.XAxis == null || la.YAxis == null)
				//	model.RefreshPlot(false);

				if (la.XAxis == null || la.YAxis == null)
					return;

				var x_pos= la.InverseTransform(e.Position).X;
				la.X = x_pos;

				//var itm= e.HitTestResult.Item;

				foreach (var tip in tips)
				{
					var s= tip.Tag as OxyPlot.Series.LineSeries;
					var y_val= GetNearestPointVal(s, e.Position, la.XAxis, la.YAxis);
					if (double.IsNaN(y_val))
					{
						tip.TextPosition = new DataPoint(x_pos, 0);
						tip.Text = string.Empty;
					}
					else
					{
						tip.TextPosition = new DataPoint(x_pos, tip.TextPosition.Y);
						tip.Text = double.IsNaN(y_val) ? "-" : y_val.ToString("0.#");
					}
				}

				//model.RefreshPlot(false);
				model.InvalidatePlot(false);

				if (CurrentMapPosition != null)
				{
					var time_offset = x_axis.InverseTransform(e.Position.X) - x_axis.ActualMinimum;
					var index = samples.Trip.BinarySearch(s => s.TimeOffset, time_offset);
					if (index < 0)
						index = -index;
					//var sample_index = (int)(sample_axis.InverseTransform(e.Position.X) + 0.5);
//System.Diagnostics.Debug.WriteLine(e.Position.X + "   " + sample_index);
					CurrentMapPosition(this, new Args(GetPosition(samples, index)));
				}
			};

			//model.TrackerChanged += (sender, e) =>
			//{
			//	model.Subtitle = e.HitResult != null ? "Tracker item index = " + e.HitResult.Index : "Not tracking";
			//	model.InvalidatePlot(false);
			//};

			PlotModel = model;
		}

		static bool IsValidPoint(DataPoint pt, OxyPlot.Axes.Axis xaxis, OxyPlot.Axes.Axis yaxis)
		{
			return !double.IsNaN(pt.X) && !double.IsInfinity(pt.X) && !double.IsNaN(pt.Y) && !double.IsInfinity(pt.Y) &&
					(xaxis != null && xaxis.IsValidValue(pt.X)) && (yaxis != null && yaxis.IsValidValue(pt.Y));
		}

		static double GetNearestPointVal(OxyPlot.Series.DataPointSeries points, ScreenPoint point, Axis xaxis, Axis yaxis)
		{
			var spn= default(ScreenPoint);
			DataPoint dpn= default(DataPoint);

			double minimumDistance = double.MaxValue;
			int i= 0;
			foreach (DataPoint p in points.Points)
			{
				if (!IsValidPoint(p, points.XAxis, points.YAxis))
				{
					i++;
					continue;
				}

				var sp= //OxyPlot.Axes.Axis.Transform(p, points.XAxis, points.YAxis);
					new ScreenPoint(xaxis.Transform(p.X), yaxis.Transform(p.Y));
				double d2= Math.Abs((sp - point).X); //.LengthSquared;

				if (d2 < minimumDistance)
				{
					dpn = p;
					spn = sp;
					minimumDistance = d2;
				}

				i++;
			}

			if (minimumDistance < double.MaxValue)
				return dpn.Y;

			return double.NaN;
		}

		public OxyPlot.PlotModel PlotModel
		{
			get { return plot_model_; }
			set
			{
				if (plot_model_ != value)
				{
					plot_model_ = value;
					OnPropertyChanged("PlotModel");
				}
			}
		}

		OxyPlot.PlotModel plot_model_= new OxyPlot.PlotModel();
#if false
		void SetHeartZones(CycleTrainer.Samples samples, double max)
		{
			var model= new PlotModel { Title = "Heart Rate", LegendSymbolLength = 6, LegendFontSize = 10, TitleFontSize = 10, DefaultFontSize = 10 };

			if (samples == null || samples.Trip.Length == 0)
			{
				HeartRatePlotModel = model;
				return;
			}

			var zones= new double[] { 50, 60, 70, 80, 90 };
			int zone_count= zones.Length;
			string[] labels= { "<50%", "Warm Up", "Fitness", "Aerobic", "Anaerobic", "Red Line" };
			OxyColor[] bands= { OxyColors.LimeGreen, OxyColors.YellowGreen, OxyColors.Yellow, OxyColors.Orange, OxyColors.OrangeRed, OxyColors.Red };

			var bins= new List<HistogramBin>(zone_count + 1);

			for (int i= 0; i < labels.Length; ++i)
			{
				bins.Add(new HistogramBin { Label = labels[i] });

				model.Annotations.Add(new RectangleAnnotation { MinimumY = i - 0.5, MaximumY = i + 0.5, /*Text = labels[i],*/ Fill = OxyColor.FromAColor(50, bands[i]), Layer = AnnotationLayer.BelowAxes });
			}

			double tick= 2;
			//if (samples.Trip.Length > 1)
			//{
			//	//samples.Trip.Aggregate((a, b) => a);

			//	var alt= samples.Trip.Select(s => s.Altitude).Where(a => !double.IsNaN(a));
			//	// check if empty or 1 el
			//	var pairs= alt.Zip(alt.Skip(1), (a, b) => Tuple.Create(a, b));
			//	// calculate total ascent and descent
			//	var up_down= pairs.Aggregate(Tuple.Create(0.0, 0.0), (acc, p) => p.Item1 > p.Item2 ? Tuple.Create(acc.Item1, p.Item1 - p.Item2) : Tuple.Create(p.Item2 - p.Item1, acc.Item2));
			//}

			foreach (var s in samples.Trip)
			{
				var hr= s.HeartRate;
				if (hr <= 0.0 || double.IsNaN(hr))
					continue;

				var bin= Array.BinarySearch(zones, hr * 100 / max);
				if (bin < 0)
					bin = ~bin;
				else
					bin++;

				if (bin < 0)
					bin = 0;
				else if (bin >= bins.Count)
					bin = bins.Count - 1;

				bins[bin].Value += tick;
			}

			var series = new BarSeries { ItemsSource = bins, ValueField = "Value", FillColor = OxyColor.FromRgb(0x69, 0x77, 0x81), StrokeThickness = 0, StrokeColor = OxyColors.Transparent };
			model.Series.Add(series);

			var gray= OxyColor.FromHsv(0, 0, 0.65);

			model.PlotMargins = new OxyThickness(0);
			model.Padding = new OxyThickness(0, 0, 12, 0);
			model.PlotAreaBorderThickness = new OxyThickness(0.0);
			model.PlotAreaBorderColor = OxyColor.FromHsv(0, 0, 0.7);

			model.Axes.Add(new CategoryAxis
			{
				Position = AxisPosition.Left, ItemsSource = bins, LabelField = "Label", GapWidth = 1,
				TicklineColor = gray, MajorTickSize = 5, AxisTickToLabelDistance = 1, AxislineStyle = LineStyle.Solid, AxislineColor = gray,
				IsZoomEnabled = false, IsPanEnabled = false
			});

			const double step= 10;
			model.Axes.Add(new HorzAxis { Position = AxisPosition.Right, MinimumPadding = 0, AbsoluteMinimum = 0, MajorGridlineStyle = LineStyle.Solid, MajorGridlineColor = gray, MajorTickSize = 4, MinorTickSize = 0, AxisTickToLabelDistance = 2, Minimum = zones.First() - step, Maximum = zones.Last() + step, MajorStep = step, /*StringFormat = "0 %",*/ FontSize = 8, AxislineStyle = LineStyle.Solid, AxislineColor = gray, TicklineColor = gray, IsZoomEnabled = false, IsPanEnabled = false });

			model.Axes.Add(new TimeSpanAxis { Position = AxisPosition.Bottom, MinimumPadding = 0, AbsoluteMinimum = 0, TicklineColor = gray, IsZoomEnabled = false, IsPanEnabled = false });

			HeartRatePlotModel = model;
		}
#endif
		void SetHeartZonesHistogram(CycleTrainer.Samples samples, double max)
		{
			var model= new PlotModel { Title = "Heart Rate", LegendSymbolLength = 6, LegendFontSize = 10, TitleFontSize = 10, DefaultFontSize = 10, TitlePadding = 16 };

			if (samples == null || samples.Trip.Length == 0)
			{
				HeartRatePlotModel = model;
				return;
			}

			const double step= 2;
			var zones2= new List<double>(25);
			for (int i= 40; i <= 100; i += (int)step)
				zones2.Add(i);
			var zones= zones2.ToArray();
			int zone_count= zones.Length;
			string[] labels= { "<50%", "Warm Up", "Fitness", "Aerobic", "Anaerobic", "Red Line" };
			OxyColor[] bands= { OxyColors.LimeGreen, OxyColors.YellowGreen, OxyColors.Yellow, OxyColors.Orange, OxyColors.OrangeRed, OxyColors.Red };

			var bins= new List<HistogramBin>(zone_count + 1);

			Func<int, double> pos= (i) => (double)(i * (zone_count - 1)) / labels.Length + 0.5;

			for (int i= 0; i < labels.Length; ++i)
			{
				model.Annotations.Add(new RectangleAnnotation { MinimumX = pos(i), MaximumX = pos(i + 1) /* MinimumY = i - 0.5, MaximumY = i + 0.5*/, /*Text = labels[i],*/ Fill = OxyColor.FromAColor(50, bands[i]), Layer = AnnotationLayer.BelowAxes });
			}

			for (int i= 0; i < zone_count + 1; ++i)
				bins.Add(new HistogramBin { Label = string.Empty });


			//if (samples.Trip.Length > 1)
			//{
			//	//samples.Trip.Aggregate((a, b) => a);

			//	var alt= samples.Trip.Select(s => s.Altitude).Where(a => !double.IsNaN(a));
			//	// check if empty or 1 el
			//	var pairs= alt.Zip(alt.Skip(1), (a, b) => Tuple.Create(a, b));
			//	// calculate total ascent and descent
			//	var up_down= pairs.Aggregate(Tuple.Create(0.0, 0.0), (acc, p) => p.Item1 > p.Item2 ? Tuple.Create(acc.Item1, p.Item1 - p.Item2) : Tuple.Create(p.Item2 - p.Item1, acc.Item2));
			//}

			double time = 0;
			foreach (var s in samples.Trip)
			{
				var hr= s.HeartRate;
				if (hr <= 0.0 || double.IsNaN(hr))
					continue;

				// 0.01 to move max heart rate just below 'max'
				var bin= Array.BinarySearch(zones, (hr - 0.01) * 100 / max);
				if (bin < 0)
					bin = ~bin;
				else
					bin++;

				if (bin < 0)
					bin = 0;
				else if (bin >= bins.Count)
					bin = bins.Count - 1;

				bins[bin].Value += s.TimeOffset - time;

				time = s.TimeOffset;
			}

			var series = new ColumnSeries { ItemsSource = bins, ValueField = "Value", FillColor = OxyColor.FromRgb(0x69, 0x77, 0x81), StrokeThickness = 0, StrokeColor = OxyColors.Transparent };
			model.Series.Add(series);

			var gray= OxyColor.FromHsv(0, 0, 0.65);

			model.PlotMargins = new OxyThickness(0);
			model.Padding = new OxyThickness(0, 0, 12, 10);
			model.PlotAreaBorderThickness = new OxyThickness(0.0);
			model.PlotAreaBorderColor = OxyColor.FromHsv(0, 0, 0.7);

			// axis required by column series
			model.Axes.Add(new CategoryAxis
			{
				Position = AxisPosition.Bottom, ItemsSource = bins, LabelField = "Label", GapWidth = 0,
				TicklineColor = gray, MajorTickSize = 5, AxisTickToLabelDistance = 1, AxislineStyle = LineStyle.Solid, AxislineColor = gray, TickStyle = TickStyle.None, IsZoomEnabled = false, IsPanEnabled = false
			});

			// axis at the top with '%'
			model.Axes.Add(new HorzAxis { Position = AxisPosition.Top, MinimumPadding = 0, MaximumPadding = 0, AbsoluteMinimum = 0, MajorGridlineStyle = LineStyle.Solid, MajorGridlineColor = gray, MajorTickSize = 4, MinorTickSize = 0, AxisTickToLabelDistance = 2, Minimum = zones.First() - step, Maximum = zones.Last() + step, MajorStep = 10, /*StringFormat = "0 %",*/ FontSize = 8, AxislineStyle = LineStyle.Solid, AxislineColor = gray, TicklineColor = gray, IsAxisVisible = true, IsZoomEnabled = false, IsPanEnabled = false });

	//		model.Axes.Add(new TimeSpanAxis { Position = AxisPosition.Left, MinimumPadding = 0, AbsoluteMinimum = 0, TicklineColor = gray, TickStyle = TickStyle.None });

			// axis at the left
			model.Axes.Add(new VertAxis { Position = AxisPosition.Left, MinimumPadding = 0, AbsoluteMinimum = 0, TicklineColor = gray, TickStyle = TickStyle.None, ShowLabels = false, MaximumPadding = 0.05, IsZoomEnabled = false, IsPanEnabled = false });

			HeartRatePlotModel = model;
		}

		public class HistogramBin
		{
			public string Label { get; set; }
			public double Value { get; set; }
		}

		class HorzAxis : LinearAxis
		{
			public HorzAxis()
			{
				base.LabelFormatter = x => x < 50 || x > 90 ? string.Empty : x.ToString("0") + "%";
			}

			public override void GetTickValues(
			out IList<double> majorLabelValues, out IList<double> majorTickValues, out IList<double> minorTickValues)
			{
				base.GetTickValues(out majorLabelValues, out majorTickValues, out minorTickValues);
				//	majorLabelValues.Clear();
			}

			//public override string FormatValue(double x)
			//{
			//    if (x < 50 || x > 90)
			//        return string.Empty;
			//    return base.FormatValue(x) + "%";
			//}
		}

		public OxyPlot.PlotModel HeartRatePlotModel
		{
			get { return heart_rate_model_; }
			set
			{
				if (heart_rate_model_ != value)
				{
					heart_rate_model_ = value;
					OnPropertyChanged("HeartRatePlotModel");
				}
			}
		}

		OxyPlot.PlotModel heart_rate_model_= new OxyPlot.PlotModel();

		void RefreshStats(CycleTrainer.Samples samples)
		{
			double max_hr= your_max_hr_;
			SetHeartZonesHistogram(samples, max_hr);
			SetStatistics(samples, max_hr);
		}

		struct ValueDuration
		{
			public ValueDuration(double v, double d)
			{
				value = v;
				duration = d;
			}
			public double value;
			public double duration;
		}

		static IEnumerable<ValueDuration> GetNumbersWithDuration(CycleTrainer.Samples samples, Func<CycleTrainer.Sample, double> select, int min_length = 1, Func<double, bool> filter_in = null)
		{
			if (filter_in == null)
				filter_in = (double x) => true;

			var nums = samples.Trip.Where(s => { var x = select(s); return !double.IsNaN(x) && filter_in(x); }).
				Select2((prev, el) => new ValueDuration(select(el), el.TimeOffset - prev.TimeOffset));

			if (min_length > 0) // check if there's enough elements
			{
				var it = nums.GetEnumerator();
				int n = 0;
				while (it.MoveNext())
				{
					n++;
					if (n == min_length)
						return nums;
				}
				return null;    // sequence too short
			}

			return nums;
		}

		static IEnumerable<double> GetNumbers(CycleTrainer.Samples samples, Func<CycleTrainer.Sample, double> select, int min_length= 1, Func<double, bool> filter_in= null)
		{
			if (filter_in == null)
				filter_in = (double x) => true;

			var nums= samples.Trip.Select(select).Where(x => !double.IsNaN(x) && filter_in(x));

			if (min_length > 0)	// check if there's enough elements
			{
				var it= nums.GetEnumerator();
				int n= 0;
				while (it.MoveNext())
				{
					n++;
					if (n == min_length)
						return nums;
				}
				return null;	// sequence too short
			}

			return nums;
		}

		public double ElevationHysterises { get; set; }
		CycleTrainer.Samples samples_;

		void SetStatistics(CycleTrainer.Samples samples, double max_hr)
		{
			samples_ = samples;
			string empty= "-";
			Date = new DateTime();
			Duration = empty;
			Ascent = empty;
			Descent = empty;
			Elevation = empty;
			TotalDistance = empty;
			Calories = empty;
			MaxHeartRate = empty;
			AvgHeartRate = empty;
			Cadence = empty;
			MaxSpeed = empty;
			AvgSpeed = empty;
			Product = samples.Product;
			Manufacturer = samples.Manufacturer;

			if (samples == null || samples.Trip.Length == 0)
			{
				return;
			}
			else
			{
				//activity_label_.Text = samples.Date.ToShortDateString();
				Date = samples.Date;

				//var spd= GetNumbers(samples, s => s.Speed);
				var spd = GetNumbersWithDuration(samples, s => s.Speed);
				if (spd != null)
				{
					// average weighted
					var avg = spd.Reduce((v, acc) => new ValueDuration(acc.value + v.value * v.duration, acc.duration + v.duration), new ValueDuration(0, 0));
					if (avg.duration > 0)
						AvgSpeed = (avg.value / avg.duration * MS_TO_KMH).ToString("0.0") + " km/h";
//					var avg = spd.Average(v => v.value);
//					if (avg > 0)
//						AvgSpeed = (avg * MS_TO_KMH).ToString("0.0") + " km/h";
					var max= spd.Max(v => v.value);
					if (max > 0)
						MaxSpeed = (max * MS_TO_KMH).ToString("0.0") + " km/h";
				}

				var hr = GetNumbersWithDuration(samples, s => s.HeartRate, 1, h => h > 0.0);
				if (hr != null)
				{
					var min = hr.Min(v => v.value);
					MinHeartRate = min.ToString("0") + " (" + (min / max_hr).ToString("0%") + ")";
					var max = hr.Max(v => v.value);
					MaxHeartRate = max.ToString("0") + " (" + (max / max_hr).ToString("0%") + ")";

					// average weighted
					var avg = hr.Reduce((v, acc) => new ValueDuration(acc.value + v.value * v.duration, acc.duration + v.duration), new ValueDuration(0, 0));
					if (avg.duration > 0)
						AvgHeartRate = (avg.value / avg.duration).ToString("0.0");
				}

				var dr= samples.Duration - samples.DurationStopped;
				if (dr > 0)
					Duration = TimeSpan.FromSeconds(dr).ToString(@"hh\:mm\:ss");

				var alt= GetNumbers(samples, s => s.Altitude, 2);
				if (alt != null)
				{
					var altitude= alt.First();
					var hysterises= ElevationHysterises;
					var descent= 0.0;
					var ascent= 0.0;

					foreach (var a in alt)
					{
						var delta= a - altitude;

						if (Math.Abs(delta) >= hysterises)
						{
							if (delta > 0)
								ascent += delta;
							else
								descent -= delta;

							altitude = a;
						}
					}

					Ascent = ascent.ToString("0") + " m";
					Descent = descent.ToString("0") + " m";
/*
					var pairs= alt.Zip(alt.Skip(1), (a, b) => Tuple.Create(a, b));

					// calculate total ascent and descent
					var up_down= pairs.Aggregate(Tuple.Create(0.0, 0.0), (acc, p) => p.Item1 > p.Item2 ? Tuple.Create(acc.Item1, acc.Item2 + p.Item1 - p.Item2) : Tuple.Create(acc.Item1 + p.Item2 - p.Item1, acc.Item2));

					Ascent = up_down.Item1.ToString("0") + " m";
					Descent = up_down.Item2.ToString("0") + " m"; */
					Elevation = (alt.Max() - alt.Min()).ToString("0") + " m";
				}

				//var time= GetNumbers(samples, s => s.TimeOffset, 2);
				//if (time != null)
				//{
				//	var deltas= time.Zip(time.Skip(1), (a, b) => b - a);
				//	var big= deltas.Where(d => d > 3.0);
				//if (!big.IsEmpty)
				//	big.First();
				//}

				//Cadence = "-";
				var cad= GetNumbers(samples, s => s.Cadence, 1, c => c > 0.0);
				if (cad != null)
				{
					// average cadence
					var c= cad.Average();
					if (c > 0)
						Cadence = c.ToString("0.0") + " rpm";
					/*
					// calc median
					var c= cad.OrderBy(x => x).ToArray();
					if (c.Length > 0)
						Cadence = c[c.Length / 2].ToString("0") + " rpm"; */
				}

				TotalDistance = (samples.Distance / 1000).ToString("0.000") + " km";

				Calories = (samples.Work * J_TO_CAL).ToString("0");
			}
		}

		DateTime date_;

		public DateTime Date
		{
			get { return date_; }
			set { SetProperty(ref date_, value); }
		}

		string duration_;

		public string Duration
		{
			get { return duration_; }
			set { SetProperty(ref duration_, value); }
		}

		string ascent_;

		public string Ascent
		{
			get { return ascent_; }
			set { SetProperty(ref ascent_, value); }
		}

		string descent_;

		public string Descent
		{
			get { return descent_; }
			set { SetProperty(ref descent_, value); }
		}

		string elevation_;

		public string Elevation
		{
			get { return elevation_; }
			set { SetProperty(ref elevation_, value); }
		}

		string total_distance_;

		public string TotalDistance
		{
			get { return total_distance_; }
			set { SetProperty(ref total_distance_, value); }
		}

		string calories_;

		public string Calories
		{
			get { return calories_; }
			set { SetProperty(ref calories_, value); }
		}

		string min_heart_rate_;

		public string MinHeartRate
		{
			get { return min_heart_rate_; }
			set { SetProperty(ref min_heart_rate_, value); }
		}

		string max_heart_rate_;

		public string MaxHeartRate
		{
			get { return max_heart_rate_; }
			set { SetProperty(ref max_heart_rate_, value); }
		}

		string avg_heart_rate_;

		public string AvgHeartRate
		{
			get { return avg_heart_rate_; }
			set { SetProperty(ref avg_heart_rate_, value); }
		}

		string cadence_;

		public string Cadence
		{
			get { return cadence_; }
			set { SetProperty(ref cadence_, value); }
		}

		string max_speed_;

		public string MaxSpeed
		{
			get { return max_speed_; }
			set { SetProperty(ref max_speed_, value); }
		}

		string avg_speed_;

		public string AvgSpeed
		{
			get { return avg_speed_; }
			set { SetProperty(ref avg_speed_, value); }
		}

		public class Args : EventArgs
		{
			public Args(CycleTrainer.Point pos)
			{
				Position = pos;
			}

			public CycleTrainer.Point Position
			{
				get;
				private set;
			}
		}

		public event EventHandler<Args> CurrentMapPosition;

		string manufacturer_;

		public string Manufacturer
		{
			get { return manufacturer_; }
			set { SetProperty(ref manufacturer_, value); }
		}

		string product_;

		public string Product
		{
			get { return product_; }
			set { SetProperty(ref product_, value); }
		}

		double your_max_hr_ = 180;
		public double YourMaxHeartRate
		{
			get { return your_max_hr_; }
			set { SetProperty(ref your_max_hr_, value); }
		}
	}
}
