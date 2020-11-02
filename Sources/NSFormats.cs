using System.Net;
using System.IO;

namespace PaymentChecker
{
    class NSFormats
    {
        public static string NewPayments(string PID, string HWID, string Sum)
        {
            File.AppendAllText("PaymentLog.log", $"---New payment---\n\tPayment ID: {PID}\n\tHWID: {HWID}\n\tMoney: +{Sum} RUB\n{AddBase(HWID)}\n");
            return $"New payment!\n\tPayment ID: {PID}\n\tHWID: {HWID}\n\tMoney: +{Sum} RUB\n{AddBase(HWID)}\n";
        }

        private static string AddBase(string HWID)
        {
            string Domain = "http://domain.ru/", Pass = "Your password"; //your panel

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
