using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppClient.MVVM.Models
{
    public class UserLow
    {
        public long ID { get; set; }
        public string ProfileImagePath { get; set; }
        public string Name { get; set; }
        public string About { get; set; }
        public string PhoneNumber { get; set; }
    }
}