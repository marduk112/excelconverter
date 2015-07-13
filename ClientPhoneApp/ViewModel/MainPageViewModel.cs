using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.CompilerServices;
using Windows.Storage;
using ClassLibrary;
using ClientPhoneApp.Annotations;
using Microsoft.Practices.Prism.Commands;


namespace ClientPhoneApp.ViewModel
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        public DelegateCommand DownloadButtonDelegateCommand { get; private set; }

        public string Result
        {
            get { return _result; }
            private set
            {
                _result = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public MainPageViewModel()
        {
            Result = "Result";
            DownloadButtonDelegateCommand = new DelegateCommand(DownloadButton);
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private async void DownloadButton()
        {
            using (var myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
            using (var fileStream = new IsolatedStorageFileStream("archive.zip", FileMode.Create, myIsolatedStorage))
            using (var writer = new BinaryWriter(fileStream))
            {
                var client = new RestClient();
                var archive = await client.GetArchiveFileChecksumCheckAsync() as byte[];
                if (archive != null)
                {
                    writer.Write(archive);
                    Debug.WriteLine(archive.Length);
                    Result = "OK";
                }
                else
                {
                    Result = "ERROR";
                }
            }
        }

        private string _result;
    }
}
