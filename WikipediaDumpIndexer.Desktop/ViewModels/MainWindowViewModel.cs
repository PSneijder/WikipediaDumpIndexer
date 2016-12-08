using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using WikipediaDumpIndexer.Core;
using WikipediaDumpIndexer.Desktop.Utilities;

namespace WikipediaDumpIndexer.Desktop.ViewModels
{
    public sealed class MainWindowViewModel
        : INotifyPropertyChanged
    {
        public ObservableCollection<Tuple<string, float, string[]>> SearchResults { get; set; }
        public string SearchText { get; set; }
        public ICommand SearchCommand { get; set; }

        public MainWindowViewModel()
        {
            SearchCommand = new DelegateCommand(OnSearch);
            SearchResults = new ObservableCollection<Tuple<string, float, string[]>>();
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

        #region INotifyPropertyChanged Implementations

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}