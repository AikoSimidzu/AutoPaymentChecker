using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace PaymentChecker
{
    class NSFormats
    {
        public static string NewPayments(string PID, string HWID, string Sum)
        {
            File.AppendAllText("PaymentLog.log", $"---New payment---\n\tPayment ID: {PID}\n\tHWID: {HWID}\n\tMoney: +{Sum}\n{AddBase(HWID)}\n");
            return $"New payment!\n\tPayment ID: {PID}\n\tHWID: {HWID}\n\tMoney: +{Sum} RUB\n{AddBase(HWID)}\n";
        }

        private static string AddBase(string HWID)
        {
            string Domain = "http://f0482296.xsph.ru/", Pass = "Zarya3";

            string Add = $"{Domain}/drawlicense.php?hwid={HWID}&p={Pass}&date=OK";

            using (WebClient wc = new WebClient())
            {
                try
                {
                    if (!wc.DownloadString(Add).Contains("OK"))
                    {
                        return "Incorrect panel password!!";
                    }
                    else
                    {
                        return "HWID added in base!";
                    }
                }
                catch { return "Please try again!"; }
            }
            
        }
    }
}
