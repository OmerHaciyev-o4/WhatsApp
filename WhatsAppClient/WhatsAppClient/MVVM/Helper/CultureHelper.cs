using PhoneNumbers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppClient.MVVM.Helper
{
    public class CultureHelper
    {
        public static List<string> RegionNameList { get; set; }
        public static List<string> RegionCodeList { get; set; }

        public static void Start()
        {
            RegionNameList = new List<string>();
            RegionCodeList = new List<string>();

            getCountryCodes();
        }

        public static async void getCountryCodes()
        {
            CultureInfo[] cinfo = CultureInfo.GetCultures(CultureTypes.AllCultures & ~CultureTypes.NeutralCultures);
            Dictionary<string, int> countryNameAndCode = new Dictionary<string, int>();

            foreach (CultureInfo cul in cinfo)
            {
                char[] name = cul.Name.ToCharArray();
                if (name.Length >= 2)
                {

                    string twoLetterCode = "" + name[name.Length - 2] + name[name.Length - 1];
                    int code = findCountryCode(twoLetterCode);

                    try
                    {
                        var rigion = new RegionInfo(cul.Name);
                        countryNameAndCode[rigion.EnglishName] = code;
                    }
                    catch (ArgumentException e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                }

            }


            await Task.WhenAll();
            Console.WriteLine("Number of Countries: " + countryNameAndCode.Count);

            foreach (var kvp in countryNameAndCode)
                Console.WriteLine("Country name: {0}, Country code: {1}", kvp.Key, kvp.Value);

            printDictionaryOutput(countryNameAndCode);

        }

        public static int findCountryCode(string countryShortCode)
        {
            PhoneNumberUtil phoneUtil = PhoneNumberUtil.GetInstance();
            return phoneUtil.GetCountryCodeForRegion(countryShortCode.ToUpper());
        }

        public static void printDictionaryOutput(Dictionary<string, int> countryNameAndCode)
        {
            foreach (var mapping in countryNameAndCode.OrderBy(mapping => mapping.Key))
            {
                RegionNameList.Add(mapping.Key);
                RegionCodeList.Add(mapping.Value.ToString());
            }
        }
    }
}
