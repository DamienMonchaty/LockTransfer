using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Models
{
    public class File
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public long KbWeight { get; set; }
        public string FileType { get; set; }
        public string Icon { get; set; }

        public File(string Title, long KbWeight, string FileType, string Icon)
        {
            this.Title = Title;
            this.KbWeight = KbWeight;
            this.FileType = FileType;
            this.Icon = Icon;
        }
    }
}
