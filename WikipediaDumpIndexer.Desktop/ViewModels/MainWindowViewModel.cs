using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using WikipediaDumpIndexer.Core;
using WikipediaDumpIndexer.Desktop.Utilities;

namespace WikipediaDumpIndexer.Desktop.ViewModels
{
    public sealed class MainWindowViewModel
        : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindowViewModel()
        {
            SearchCommand = new DelegateCommand(OnSearch);
            SearchResults = new ObservableCollection<Tuple<string, float>>();
        }

        private void OnSearch()
        {
            var results = WikiPediaService.Search(SearchText);

            SearchResults.Clear();
            foreach (var result in results)
            {
                SearchResults.Add(result);
            }
        }

        public ObservableCollection<Tuple<string, float>> SearchResults { get; set; }
        public string SearchText { get; set; }
        public ICommand SearchCommand { get; set; }
    }
}