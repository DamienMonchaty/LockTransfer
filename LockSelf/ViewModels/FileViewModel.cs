using Core.Models;
using System.IO;
using File = Core.Models.File;

namespace LockSelf.ViewModels
{
    public class FileViewModel : BaseViewModel
    {
        private File _file;
        public File File
        {
            get => _file;
            set
            {
                _file = value;
                RaisePropertyChanged("File");
            }
        }

        public FileViewModel(File file)
            => _file = file;
    }
}
