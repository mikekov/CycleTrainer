using System;

namespace CycleTrainer
{
	public interface Activity
	{
		string FileName { get; }
		string FilePath { get; }
		string Directory { get; }
		DateTime CreationTime { get; }
		Samples Samples { get; }

		void Rename(string new_name);
	}
}
