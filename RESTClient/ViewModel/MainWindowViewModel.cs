using System.ComponentModel;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using ClassLibrary;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Win32;
using RESTClient.Annotations;

namespace RESTClient.ViewModel
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public DelegateCommand DownloadButtonDelegateCommand { get; private set; }

        public string Result
        {
            get { return _result; }
            set
            {
                _result = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindowViewModel()
        {
            DownloadButtonDelegateCommand = new DelegateCommand(DownloadButton);
            Result = "Result";
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private string _result;
        private async void DownloadButton()
        {
            var saveFile = new SaveFileDialog {AddExtension = true, Filter = "Zip file | *.zip", DefaultExt = "zip"};
            saveFile.ShowDialog();
            var path = saveFile.FileName;
            var restClient = new RestClient();
            var archive = await restClient.GetArchiveFileChecksumCheckAsync();
            if (archive != null)
            {
                File.WriteAllBytes(path, archive);
                Result = "File was successfully downloaded";
            }
            else
            {
                Result = "Error with archive";
            }
        }
    }
}
