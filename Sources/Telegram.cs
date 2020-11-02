namespace PaymentChecker
{
    using System;
    using System.Net;
    class Telegram
    {
        public static void Send(string token, string chatid, string log)
        {
            try
            {
                using (WebClient wc = new WebClient())
                {
                    wc.DownloadString($"https://api.telegram.org/bot{token}/sendMessage?chat_id={chatid}&text={log}");
                }
            }
            catch { Console.WriteLine("---Cant send log in telegram---"); }
        }
    }
}
