using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppClient.MVVM.Helper
{
    public class Helpers
    {
        public static string[] ReturnNoVideo(string[] fileList)
        {
            var result = new List<string>();

            foreach (var file in fileList)
            {
                string ext = Path.GetExtension(file);

                if (!(ext.ToLower().Contains(".mp4") || ext.ToLower().Contains(".mp5") || ext.ToLower().Contains(".mkv") || ext.ToLower().Contains("m4v")))
                    result.Add(file);
            }

            return fileList = result.ToArray();
        }
    }
}
