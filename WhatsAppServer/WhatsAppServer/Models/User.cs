﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppServer.Models
{
    public class User
    {
        public long ID { get; set; }
        public byte[] ProfileImageByte { get; set; }
        public string ProfileImagePath { get; set; }
        public string Name { get; set; }
        public string About { get; set; }
        public string PhoneNumber { get; set; }
    }
}