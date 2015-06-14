using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.Practices.Prism.Mvvm;

namespace CycleApp
{
	class ActivityDataSource : BindableBase
	{
		public ActivityDataSource(Dispatcher dispatcher)
		{
			activities_.Dispatcher = dispatcher;
		}

		public string ActivitiesFolder
		{
			set
			{
				current_ = null;
				activities_.ActivitiesDirectory = value;
				OnPropertyChanged("Files");
			}
			get
			{
				return activities_.ActivitiesDirectory;
			}
		}

		public IReadOnlyCollection<CycleTrainer.Activity> Files
		{
			get { return activities_.Files; }
		}

		public CycleTrainer.Activity CurrentActivity
		{
			get { return current_; }
			set
			{
				if (SetProperty(ref current_, value))
					Import(current_);
			}
		}

		public CycleTrainer.Samples CurrentSamples
		{
			get { return samples_; }
		}

		public bool ListVisible
		{
			get { return list_visible_; }
			set
			{
				if (SetProperty(ref list_visible_, value))
					OnPropertyChanged("ListWidth");	// list width depends on visibility, so trigger it
			}
		}

		public double SavedListWidth
		{
			get { return list_width_.Value; }
			set { ListWidth = new GridLength(value); }
		}

		public GridLength ListWidth
		{
			get { return list_visible_ ? list_width_ : new GridLength(0); }
			set { SetProperty(ref list_width_, value); }
		}

		public GridLength MapHeight
		{
			get { return map_height_; }
			set { SetProperty(ref map_height_, value); }
		}

		public ICommand SaveAsGpx
		{
			get
			{
				return new DelegateCommand(
					sender => CycleTrainer.GPX.Export(samples_, Path.ChangeExtension(file_path_, ".gpx"), true),
					sender => samples_ != null && file_path_ != null
				);
			}
		}

		public ICommand SelectFolder
		{
			get
			{
				return new DelegateCommand(sender => SelectActivityFolder());
			}
		}

		void SelectActivityFolder()
		{
			try
			{
				var dlg = new Microsoft.Win32.OpenFileDialog
				{
					Title = "Select Activities Folder",
					Filter = "Activity Files|*.fit;*.pwx", // Filter files by extension 
					InitialDirectory = string.IsNullOrEmpty(activities_.ActivitiesDirectory) ? System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal) : activities_.ActivitiesDirectory,
					CheckPathExists = true
				};

				// Show open file dialog box
				bool? result = dlg.ShowDialog();

				// Process open file dialog box results 
				if (result == true)
				{
					// Open document
					string filename = dlg.FileName;
					ActivitiesFolder = Path.GetDirectoryName(filename);
					CurrentActivity = Files.First(f => f.FilePath == filename);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		CycleTrainer.Activities activities_ = new CycleTrainer.Activities();
		CycleTrainer.Activity current_ = null;
		CycleTrainer.Samples samples_ = null;
		string file_path_;
		bool list_visible_ = true;
		GridLength list_width_ = new GridLength(200);
		GridLength map_height_ = new GridLength(300);

		void Import(CycleTrainer.Activity file)
		{
			samples_ = null;
			file_path_ = null;

			if (file == null)
				return;

			samples_ = file.Samples;
			file_path_ = file.FilePath;

			if (ActivityChanged != null)
				ActivityChanged(this, EventArgs.Empty);
		}

		public event EventHandler ActivityChanged;
	}


}
