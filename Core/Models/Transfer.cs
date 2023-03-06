using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Core.Models
{
    public class Transfer
    {
        public string uuid { get; set; }
        public DateTime expirationDate { get; set; }
        public string password { get; set; }
        public int uploadNumber { get; set; }
        public string phone { get; set; }
        public string prefixPhone { get; set; }
        public ObservableCollection<File> files { get; set; }
    }
}
