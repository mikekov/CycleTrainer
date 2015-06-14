using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.Mvvm;
using GMap.NET;
using GMap.NET.WindowsPresentation;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Input;


namespace CycleApp
{
	class MapDataSource : BindableBase
	{
		public MapDataSource()
		{
			Markers = new ObservableCollection<GMapMarker>();
			locate_cmd_ = new DelegateCommand(LocateActivityOnMap, CanLocateActivityOnMap);
		}

		public ObservableCollection<GMapMarker> Markers { get; private set; }

		public Point ActivityCenterPosition
		{
			get { return position_; }
			set { position_ = value; OnPropertyChanged("ActivityCenterPosition");/*SetProperty(ref position_, value);*/ }
		}
		Point position_;
		Point activity_position_;

		public double MapZoom
		{
			get { return zoom_; }
			set { SetProperty(ref zoom_, value); }
		}
		double zoom_ = 12;
		int MaxZoom = 17;
		int MinZoom = 1;

		public void RemoveRoute(CycleTrainer.Samples samples)
		{
			var count = Markers.Count;
			for (int i = 0; i < count; ++i)
				if (Markers[i].Tag == samples)
				{
					Markers.RemoveAt(i);
					break;
				}
		}

		public void AddRoute(CycleTrainer.Samples samples, Brush start, Brush end)
		{
			if (samples != null)
			{
				// select samples with valid GPS latitude and longitude
				var valid_samples = samples.Trip.Where(s => !(double.IsNaN(s.Longitude) && double.IsNaN(s.Latitude)));

				var route = valid_samples.Select(s => Tuple.Create(s.Latitude, s.Longitude, s.TimeOffset)).ToList();

				const int PARTS = 2;
				// split trip into parts to paint it in different colors
				if (route.Count > 0)
				{
					// parts are used to color route differently, so start and end look differently
					for (int part = 0; part < PARTS; ++part)
					{
						int from = part * route.Count / PARTS;
						int count = Math.Min(route.Count - from, route.Count / PARTS + 1);
						var points = route.GetRange(from, count);

						var threshold = 30.0; // 30 seconds gap between samples will split route into separate parts

						// detect gaps in recording, split route if needed (this will prevent distant points on a map to be joined by line)
						foreach (var partition in points.Partition((t1, t2) => t2.Item3 - t1.Item3 >= threshold))
						{
							// select longitude and latitude only
							var gps = partition.Select(s => new GMap.NET.PointLatLng(s.Item1, s.Item2)).ToList();

							Markers.Add(new MapRoute(gps, part == 0 ? start : end, samples));
						}
					}

					// mark starting position
					var first = route.First();
					var m = new GMapMarker(new PointLatLng(first.Item1, first.Item2));
					m.Offset = new Point(-3, -3);
					m.Shape = new Ellipse() { Fill = Brushes.Lime, Stroke = Brushes.Green, Width = 12, Height = 12 };
					Markers.Add(m);
					marker_ = m;
				}

				// temporary workaround for GMap not refreshing Markers
				// should be fixed in GMap > 1.7.1
				MapZoom += 0.002;
				MapZoom -= 0.002;
			}
		}

		GMapMarker marker_;
		GMap.NET.PointLatLng current_pos_;

		public GMap.NET.PointLatLng CurrentPosition
		{
			get { return current_pos_; }
			set
			{
				if (current_pos_ != value)
				{
					current_pos_ = value;
					if (marker_ != null)
						marker_.Position = value;
				}
			}
		}

		public void SetMap(CycleTrainer.Samples samples)
		{
			var area = samples != null ? samples.Area : CycleTrainer.Rectangle.Empty;
			Markers.Clear();
			marker_ = null;

			activity_position_ = new Point(area.Y + area.Height / 2, area.X + area.Width / 2);
			locate_cmd_.RaiseCanExecuteChanged();

			if (!pinned_)
				ActivityCenterPosition = activity_position_;
		}

		bool pinned_ = false;

		public bool IsMapPinned
		{
			get { return pinned_; }
			set { SetProperty(ref pinned_, value); }
		}

		public ICommand LocateOnMapCmd
		{
			get { return locate_cmd_; }
		}
		readonly DelegateCommand locate_cmd_;

		bool flip_ = false;
		void LocateActivityOnMap()
		{
			var pos = activity_position_;
			if (flip_)
				pos.X += 0.0000000000001;
			else
				pos.X -= 0.0000000000001;
			flip_ = !flip_;
			ActivityCenterPosition = pos;
		}

		bool CanLocateActivityOnMap()
		{
			return activity_position_.X != 0 && activity_position_.Y != 0;
		}

		public ICommand MapZoomIn
		{
			get { return new DelegateCommand(s => MapZoom++, s => MapZoom < MaxZoom); }
		}

		public ICommand MapZoomOut
		{
			get { return new DelegateCommand(s => MapZoom--, s => MapZoom > MinZoom); }
		}

		public bool IsCycleMapVisible
		{
			get { return cycle_map_visible_; }
			set
			{
				if (SetProperty(ref cycle_map_visible_, value))
				{
					MapProvider = cycle_map_visible_ ?
						(GMap.NET.MapProviders.GMapProvider)GMap.NET.MapProviders.OpenCycleLandscapeMapProvider.Instance :
						(GMap.NET.MapProviders.GMapProvider)GMap.NET.MapProviders.GoogleSatelliteMapProvider.Instance;
				}
			}
		}

		bool cycle_map_visible_ = true;

		public GMap.NET.MapProviders.GMapProvider MapProvider
		{
			get { return map_provider_; }
			set { SetProperty(ref map_provider_, value); }
		}

		GMap.NET.MapProviders.GMapProvider map_provider_ = GMap.NET.MapProviders.OpenCycleLandscapeMapProvider.Instance;
	}


	public class MapRoute : GMapRoute
	{
		public MapRoute(IEnumerable<PointLatLng> points, Brush color, object tag)
			: base(points)
		{
			color_ = color;
			Tag = tag;
		}

		/// <summary>
		/// regenerates shape of route
		/// </summary>
		public override void RegenerateShape(GMapControl map)
		{
			if (map != null)
			{
				if (Points.Count > 1)
				{
					Position = Points[0];

					var localPath = new List<System.Windows.Point>();
					var offset = Map.FromLatLngToLocal(Points[0]);
					foreach (var i in Points)
					{
						var p = Map.FromLatLngToLocal(new PointLatLng(i.Lat, i.Lng));
						localPath.Add(new System.Windows.Point(p.X - offset.X, p.Y - offset.Y));
					}

					var shape = CreatePath(localPath);

					if (this.Shape != null && this.Shape is Path)
					{
						(this.Shape as Path).Data = shape.Data;
					}
					else
					{
						this.Shape = shape;
					}
				}
				else
				{
					this.Shape = null;
				}
			}
		}

		Brush color_;

		public Path CreatePath(List<System.Windows.Point> localPath)
		{
			// Create a StreamGeometry to use to specify myPath.
			StreamGeometry geometry = new StreamGeometry();

			using (StreamGeometryContext ctx = geometry.Open())
			{
				ctx.BeginFigure(localPath[0], false, false);

				// Draw a line to the next specified point.
				ctx.PolyLineTo(localPath, true, true);
			}

			// Freeze the geometry (make it unmodifiable)
			// for additional performance benefits.
			geometry.Freeze();

			// Create a path to draw a geometry with.
			Path path = new Path()
			{
				// Specify the shape of the Path using the StreamGeometry.
				Data = geometry,

				Stroke = color_,
				StrokeThickness = 3,
				StrokeLineJoin = PenLineJoin.Round,
				StrokeStartLineCap = PenLineCap.Flat,
				StrokeEndLineCap = PenLineCap.Flat,
				//StrokeDashArray = new DoubleCollection(new double[]{2, 1}),

				Fill = null,

				Opacity = 1.0,
				IsHitTestVisible = false
			};

			return path;
		}
	}
}
