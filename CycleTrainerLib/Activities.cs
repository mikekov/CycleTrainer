using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Threading;


namespace CycleTrainer
{
	public class Activities : IDisposable
	{
		public Activities()
		{
			Dispatcher = null;
			fs_watcher_.EnableRaisingEvents = false;
			fs_watcher_.Deleted += OnFileDeleted;
			fs_watcher_.Created += OnFileCreated;
			fs_watcher_.Changed += OnFileChanged;
			fs_watcher_.Renamed += OnFileRenamed;
		}

		public Dispatcher Dispatcher { get; set; }

		void OnFileRenamed(object sender, RenamedEventArgs e)
		{
			if (Dispatcher == null)
				return;

			Dispatcher.BeginInvoke(() => {
				// rename item in place; first find it by path
				var index = files_.IndexOf(pwx => pwx.FilePath, e.OldFullPath);
				if (index >= 0)
				{
					files_[index].Rename(Path.GetFileName(e.FullPath));
					files_.ItemChanged(index);
				}
			});
		}

		void OnFileChanged(object sender, FileSystemEventArgs e)
		{
			if (Dispatcher == null)
				return;

			Dispatcher.BeginInvoke(() => {
				// add new item if it's not there
				if (!files_.Contains(pwx => pwx.FilePath, e.FullPath))
				{
					var new_item = CreateActivity(e.Name);
					if (new_item != null)
						files_.BinarySearchInsert(pwx => pwx.CreationTime, new_item);
				}
				else
				{
					// refresh current activity if it uses file that has changed
					//todo
				}
			});
		}

		void OnFileCreated(object sender, FileSystemEventArgs e)
		{
			if (Dispatcher == null)
				return;

			Dispatcher.BeginInvoke(() => {
				// add new item
				var new_item = CreateActivity(e.Name);
				if (new_item != null)
					files_.BinarySearchInsert(pwx => pwx.CreationTime, new_item);
			});
		}

		void OnFileDeleted(object sender, FileSystemEventArgs e)
		{
			if (Dispatcher == null)
				return;

			Dispatcher.BeginInvoke(() => {
				files_.Remove(pwx => pwx.FilePath, e.FullPath);
			});
		}

		public string ActivitiesDirectory
		{
			set
			{
				fs_watcher_.EnableRaisingEvents = false;
				files_.Clear();

				string dir = value;

				if (string.IsNullOrWhiteSpace(dir))
					return;

				if (dir.Last() != '\\')
					dir += '\\';

				base_dir_ = dir;
				var idx = base_dir_.Length;

				Directory.EnumerateFiles(base_dir_, "*.*")
					.Select(f => CreateActivity(f))
					.Where(activity => activity != null)
					.OrderBy(p => p.CreationTime)
					.ToList()
					.ForEach(f => files_.Add(f));

				fs_watcher_.Path = value;
				fs_watcher_.EnableRaisingEvents = true;
			}

			get
			{
				return base_dir_;
			}
		}

		Activity CreateActivity(string file_path)
		{
			var idx = base_dir_.Length;

			switch (Path.GetExtension(file_path).ToLower())
			{
			case ".pwx":
				return new PwxFile(base_dir_, file_path.Substring(idx), Directory.GetCreationTime(file_path));

			case ".fit":
				return new FitFile(base_dir_, file_path.Substring(idx), Directory.GetCreationTime(file_path));

			default:
				return null;
			}
		}

		public ObservableCollection<Activity> Files
		{
			get { return files_; }
		}

		ObservablePwxCollection files_ = new ObservablePwxCollection();
		string base_dir_ = string.Empty;
		FileSystemWatcher fs_watcher_ = new FileSystemWatcher();

		public void Dispose()
		{
			fs_watcher_.Dispose();
		}

		class ObservablePwxCollection : ObservableCollection<Activity>
		{
			// that doesn't cut it, list box is not refreshed
			public void ItemChanged(int index)
			{
				base.SetItem(index, this[index]);
			}
		}
	}
}
