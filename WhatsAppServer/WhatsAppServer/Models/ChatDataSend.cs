using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppServer.Models
{
    public class ChatDataSend
    {
        public long CustomID { get; set; }
        public byte[] DataBytes { get; set; }
        public string FileName { get; set; }
        public long FriendID { get; set; }
    }
}