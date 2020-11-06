namespace PaymentChecker
{
    using QLib;
    using System;
    using System.IO;

    class Settings
    {
        public static bool Check(string path)
        {
            try
            {
                if (!File.Exists(path))
                {
                    File.AppendAllText(path, "QiwiToken[token]\nUseTelegramBot[true or false]\nTelegramToken[Your telegram token]\nTelegramChatID[Your chat ID]");
                    Console.WriteLine($"Please set the settings along the way: {path}");
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public static string GetQiwiToken(string path)
        {
            return Helper.Pars(File.ReadAllText(path), "QiwiToken[", "]");
        }

        public static bool UseTelegramBot(string path)
        {
            string UT;
            UT = Helper.Pars(File.ReadAllText(path), "UseTelegramBot[", "]");
            if (UT == "true")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static string TelegramToken(string path)
        {
            return Helper.Pars(File.ReadAllText(path), "TelegramToken[", "]");
        }

        public static string TelegramChatID(string path)
        {
            return Helper.Pars(File.ReadAllText(path), "TelegramChatID[", "]");
        }
    }
}
