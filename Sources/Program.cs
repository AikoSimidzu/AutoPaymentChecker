using System;
using xNet;
using QLib;
using Newtonsoft.Json;
using System.Threading;
using System.IO;

namespace PaymentChecker
{
    class Program
    {
        //Data
        private static readonly string token = "your qiwi token", Phone = "your qiwi phone", TToken = "your telegram token", CID = "your telegram chatID";
        static void Main(string[] args)
        {
            string name = "ContentType";
            HttpRequest req = new HttpRequest();
            Console.WriteLine("Connect...");
            try
            {                
                req.AddHeader("Accept", "application/json");
                req.AddHeader("Authorization", string.Format("Bearer {0}", token));
                string text = req.Get("https://edge.qiwi.com/person-profile/v1/profile/current", null).ToString(); //Get qiwi profile details
                req.Close();
                Console.WriteLine("Success!"); //If all okay...
                Console.WriteLine("ContractID: " + Helper.Pars(text, "{\"contractId\":", ",", 0));
            }
            catch
            { Console.WriteLine("Error!"); }

            while (true)
            {
                if (!File.Exists("Tickets.log"))
                {
                    File.Create("Tickets.log");
                }

                string tempTickets;
                tempTickets = File.ReadAllText("Tickets.log");
                try
                {
                    req.AddHeader("Accept", "application/json");
                    req.AddHeader(name, "application/json");
                    req.AddHeader("Authorization", string.Format("Bearer {0}", token));
                    string text = req.Get("https://edge.qiwi.com/payment-history/v2/persons/" + Phone + "/payments?rows=15", null).ToString(); //Get checks
                    req.Close();

                    myjs.RootObject newQiwi = JsonConvert.DeserializeObject<myjs.RootObject>(text);
                    foreach (var ck in newQiwi.data)
                    {
                        if (ck.type == "IN" && ck.sum.MSC() == "RUB" && ck.sum.amount == 100) //If the payment is incoming & amount in rubles & check sum == {amount}
                        {
                            string ns = NSFormats.NewPayments(ck.trmTxnId, Helper.Pars(ck.mcomment(), "{HWID=\"", "\"}"), ck.sum.amount.ToString());
                            string a;
                            a = $"ID операции: {ck.trmTxnId} Статус: {ck.statusText} Сумма: {ck.sum.amount} {ck.sum.MSC()} {ck.mcomment()}\n";                            

                            if (!tempTickets.Contains(a))
                            {
                                Telegram.Send(TToken, CID, ns);
                                File.AppendAllText("Tickets.log", a);
                                Console.WriteLine(ns);
                            }
                        }
                    }
                }
                catch { Console.WriteLine("Error! (Tickets)"); }
                Thread.Sleep(300);
            }
        }
    }
}
