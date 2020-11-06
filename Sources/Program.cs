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
        static void Main(string[] args)
        {
            if (Settings.Check("Settings.cfg") == false)
            {
                Console.ReadKey();
                Environment.Exit(Environment.ExitCode);
            }

            string name = "ContentType";
            HttpRequest req = new HttpRequest();
            Console.WriteLine("Connect...");
            try
            {                
                req.AddHeader("Accept", "application/json");
                req.AddHeader("Authorization", string.Format("Bearer {0}", Settings.GetQiwiToken("Settings.cfg")));
                string text = req.Get("https://edge.qiwi.com/person-profile/v1/profile/current", null).ToString();
                req.Close();
                Console.WriteLine("Success!");
                Console.WriteLine("ContractID: " + Helper.Pars(text, "{\"contractId\":", ",", 0));

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
                        req.AddHeader("Authorization", string.Format("Bearer {0}", Settings.GetQiwiToken("Settings.cfg")));
                        string textP = req.Get("https://edge.qiwi.com/payment-history/v2/persons/" + Helper.Pars(text, "{\"contractId\":", ",", 0) + "/payments?rows=15", null).ToString();
                        req.Close();

                        myjs.RootObject newQiwi = JsonConvert.DeserializeObject<myjs.RootObject>(textP);
                        foreach (var ck in newQiwi.data)
                        {
                            if (ck.type == "IN" && ck.sum.MSC() == "RUB" && ck.sum.amount == 100)
                            {
                                string a;
                                a = $"ID операции: {ck.trmTxnId} Статус: {ck.statusText} Сумма: {ck.sum.amount} {ck.sum.MSC()} {ck.mcomment()}\n";

                                if (!tempTickets.Contains(a))
                                {
                                    string ns = NSFormats.NewPayments(ck.trmTxnId, Helper.Pars(ck.mcomment(), "{HWID=\"", "\"}"), ck.sum.amount.ToString());
                                    if (ns.Length < 2)
                                    {
                                        ns = NSFormats.NewPayments(ck.trmTxnId, Helper.Pars(ck.mcomment(), "{HWID=", "}"), ck.sum.amount.ToString());
                                    }

                                    if (Settings.UseTelegramBot("Settings.cfg") == true)
                                    {
                                        Telegram.Send(Settings.TelegramToken("Settings.cfg"), Settings.TelegramChatID("Settings.cfg"), ns);
                                    }
                                    File.AppendAllText("Tickets.log", a);
                                    Console.WriteLine(ns);
                                }
                            }
                        }
                    }
                    catch { Console.WriteLine("Error! (Tickets)"); }
                    Thread.Sleep(1500);
                }
            }
            catch
            { Console.WriteLine("Error!"); }
        }
    }
}
