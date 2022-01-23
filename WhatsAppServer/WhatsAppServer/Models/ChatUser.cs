using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppServer.Models
{
    public class ChatUser
    {
        public long ID { get; set; }

        public ObservableCollection<byte[]> ImageByteList { get; set; }
        public ObservableCollection<byte[]> FileByteList { get; set; }
        public ObservableCollection<byte[]> SoundByteList { get; set; }
        public ObservableCollection<string> ImageNameList { get; set; }
        public ObservableCollection<string> FileNameList { get; set; }
        public ObservableCollection<string> SoundNameList { get; set; }
    }
}