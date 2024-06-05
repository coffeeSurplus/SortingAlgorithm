using System.Collections.ObjectModel;
using System.Windows;

namespace SortingAlgorithm
{
	public partial class MainWindow : Window
	{
		public int Items { get; set; } = 50;
		public int Speed { get; set; } = 50;

		public bool Mode { get; set; } = true;

		public ObservableCollection<int> ItemList { get; set; } = new(Enumerable.Range(1, 50));

		public MainWindow() => InitializeComponent();

		private readonly Random Random = new();

		private bool IsRunning = false;

		private int CurrentIndex = 0;
		private int NextIndex => CurrentIndex + 1;
		private int MaxIndex;

		private bool Sorted = false;

		private int Comparisons = 0;
		private int Swaps = 0;

		private void ItemsValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) => UpdateItems();

		private void UpdateItems()
		{
			IsRunning = false;

			ItemList.Clear();

			for (int value = 1; value <= Items; value++)
			{
				ItemList.Add(value);
			}

			ShuffleList();
		}

		private void ModeChanged(object sender, RoutedEventArgs e) => ShuffleList();

		private void ShuffleButtonClick(object sender, RoutedEventArgs e) => ShuffleList();

		private void ShuffleList()
		{
			IsRunning = false;

			for (int index1 = 0; index1 < Items; index1++)
			{
				int index2 = Random.Next(Items);
				(ItemList[index1], ItemList[index2]) = (ItemList[index2], ItemList[index1]);
			}

			CurrentIndex = 0;
			MaxIndex = Items - 1;

			Sorted = false;
			Comparisons = 0;
			Swaps = 0;
		}

		private void StartStopButtonClick(object sender, RoutedEventArgs e) => StartStop();

		private void StartStop()
		{
			IsRunning ^= true;

			if (IsRunning)
			{
				if (Sorted)
				{
					ShuffleList();	
				}
				
				SortList();
			}
		}

		private async void SortList()
		{
			if (Mode)
			{
				await BubbleSort();
			}
			else
			{
				await MergeSort(0, Items);

				if (IsRunning)
				{
					Sorted = true;
					MessageBox.Show($"merge sort complete.\ncomparisons: {Comparisons}\nswaps: {Swaps}");
				}

				IsRunning = false;
			}
		}

		private async Task BubbleSort()
		{
			while (IsRunning && MaxIndex != 0)
			{
				await BubbleSortIteration();
			}

			if (MaxIndex == 0)
			{
				Sorted = true;
				MessageBox.Show($"bubble sort complete.\ncomparisons: {Comparisons}\nswaps: {Swaps}");
			}

			IsRunning = false;
		}

		private async Task BubbleSortIteration()
		{
			if (ItemList[CurrentIndex] > ItemList[NextIndex])
			{
				(ItemList[CurrentIndex], ItemList[NextIndex]) = (ItemList[NextIndex], ItemList[CurrentIndex]);
				Swaps++;
			}

			Comparisons++;

			if (CurrentIndex < MaxIndex - 1)
			{
				CurrentIndex++;
			}
			else
			{
				CurrentIndex = 0;
				MaxIndex--;
			}

			await Task.Delay(1000 / Speed);
		}

		private async Task MergeSort(int startIndex, int length)
		{
			if (length != 1)
			{
				int midLength = length / 2;
				int midPoint = startIndex + midLength;

				await MergeSort(startIndex, midLength);
				await MergeSort(midPoint, length - midLength);

				await MergeSortIteration(startIndex, midPoint, length);
			}
		}

		private async Task MergeSortIteration(int startIndex, int midpoint, int length)
		{
			int endIndex = startIndex + length;

			while (IsRunning && startIndex < endIndex && startIndex != midpoint && midpoint != endIndex)
			{
				if (ItemList[midpoint] < ItemList[startIndex])
				{
					int temp = ItemList[midpoint];

					for (int index = midpoint; index > startIndex; index--)
					{
						ItemList[index] = ItemList[index - 1];
						Swaps++;
						await Task.Delay(1000 / Speed);
					}

					ItemList[startIndex] = temp;
					Swaps++;

					midpoint++;
				}

				Comparisons++;
				startIndex++;
			}
		}
	}
}