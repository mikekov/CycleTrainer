using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CycleApp
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			context_ = new ActivityDataSource(Dispatcher);
			map_ds_ = new MapDataSource();
			stats_ds_ = new StatisticsDataSource();
			settings_ = new SettingsDataSource();

			stats_ds_.CurrentMapPosition += (sender, args) => { map_ds_.CurrentPosition = new GMap.NET.PointLatLng(args.Position.X, args.Position.Y); };

			context_.ActivityChanged += delegate
			{
				map_ds_.SetMap(context_.CurrentSamples);
				stats_ds_.SetPlot(context_.CurrentSamples);
			};

			//gmap_.MapProvider = GMap.NET.MapProviders.OpenCycleLandscapeMapProvider.Instance;
			//			gmap_.MapProvider = GMap.NET.MapProviders.GoogleMapProvider.Instance;
			//			gmap_.MapProvider = GMap.NET.MapProviders.BingMapProvider.Instance;
			//			gmap_.MapProvider = GMap.NET.MapProviders.OpenCycleMapProvider.Instance;
			//			gmap_.MapProvider = GMap.NET.MapProviders.GoogleTerrainMapProvider.Instance;
			//			gmap_.MapProvider = GMap.NET.MapProviders.GoogleSatelliteMapProvider.Instance;
			gmap_.ShowCenter = false;
			gmap_.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionWithoutCenter;
			//gmap_.MarkersEnabled = true;
			//gmap_.Markers
			//gmap_.Position

			this.DataContext = context_;
			map_panel_.DataContext = map_ds_;
			map_buttons_.DataContext = map_ds_;
			graphs_.DataContext = stats_ds_;
			stats_.DataContext = stats_ds_;
			settings_panel_.DataContext = settings_;

			var s = Properties.Settings.Default;

			settings_.MaxHeartRate = s.MaxHeartRate;
			stats_ds_.YourMaxHeartRate = s.MaxHeartRate;

			context_.ActivitiesFolder = s.PwxDirectory;

			list_box_.SelectionChanged += OnSelectionChanged;
		}

		void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			foreach (CycleTrainer.Activity item in e.AddedItems)
			{
				map_ds_.AddRoute(item.Samples, Brushes.Red, Brushes.Blue);
			}

			foreach (CycleTrainer.Activity item in e.RemovedItems)
			{
				map_ds_.RemoveRoute(item.Samples);
			}
		}

		protected override void OnSourceInitialized(EventArgs e)
		{
			base.OnSourceInitialized(e);

			WindowStartupLocation = WindowStartupLocation.Manual;

			var s = Properties.Settings.Default;

			var rect = s.WindowBounds;
			Left = rect.Left;
			Top = rect.Top;
			Width = rect.Width;
			Height = rect.Height;

			WindowState = s.WindowState;

			context_.SavedListWidth = s.SplitterPos;
			context_.ListVisible = s.ShowActivities;
			context_.MapHeight = new GridLength(s.MapSplitterPos);

			//gmap_.Zoom = s.MapZoom;
			map_ds_.MapZoom = s.MapZoom;
			map_ds_.IsMapPinned = s.PinMap;
			gmap_.Position = new GMap.NET.PointLatLng(s.MapLat, s.MapLong);

			context_.CurrentActivity = context_.Files.Count > 0 ? context_.Files.Last() : null;
		}

		protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
		{
			base.OnClosing(e);

			if (e.Cancel)
				return;

			var s = Properties.Settings.Default;

			s.WindowBounds = RestoreBounds;
			s.WindowState = WindowState;

			s.ShowActivities = context_.ListVisible;
			s.SplitterPos = context_.SavedListWidth;
			s.MapSplitterPos = context_.MapHeight.Value;

			s.PinMap = map_ds_.IsMapPinned;
			s.MapZoom = map_ds_.MapZoom;
			s.MapLat = gmap_.Position.Lat;
			s.MapLong = gmap_.Position.Lng;

			s.PwxDirectory = context_.ActivitiesFolder;

			s.MaxHeartRate = settings_.MaxHeartRate;

			s.Save();
		}

		ActivityDataSource context_;
		MapDataSource map_ds_;
		StatisticsDataSource stats_ds_;
		SettingsDataSource settings_;
	}
}
