using System;
using System.Reflection;
using System.Windows.Forms;
using NLog;
using LicenseManager;
using VTC.Common;

namespace VTC
{
    static class Program
    {
        private static readonly Logger _logger = LogManager.GetLogger("app.global");

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ThreadException += (_, e) => _logger.Error(e.Exception, "Thread exception");
            AppDomain.CurrentDomain.UnhandledException += (_, e) => _logger.Error((Exception)e.ExceptionObject, "Unhandled exception");

            int delayMs = 5000;
            var ss = new SplashScreen(delayMs);
            ss.Show();

            _logger.Info("***** Start. v." + Assembly.GetExecutingAssembly().GetName().Version.ToString());
            if (args.Length > 0)
            {
                _logger.Info("Arguments: " + string.Join(";", args));
            }

            string appArgument = null;
            if (args.Length == 1) appArgument = args[0];


            string publicKey = "<RSAKeyValue><Modulus>sNNBll27qDwVY2taLYlVRz1qrEe1+xEMSKYDGXojI5znZqQ+VcABzWZbp5Cbjwmw2G5JlrjMMMdkxEPLEC1j5o+tKXGjcJ2M54wjwocudbLzhecby6ZuZLMF3V9IDgr/Nn1AraLPHx1hn9Re2Unzd6rMlxrc3YCxPL1vwjAbPE5vJeoBhTe1TvO0nFMjVqWSfVxH8kPW8xqrBSgOq7akp7fD293T8MdRzsyea6uZe4xy1mgckk8hUDW3J735ISB4QSIy2+f9NfJlju1x/HEz7Lv1bwUlFQNQ0gaquhUK5gmEnoauw7/Ebgc99w0J9gXziaB+Z++K5OJV4Y8RR3NI4Q==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

            var context = new LicensedFormContext(publicKey, CreateTrafficCounterForm, appArgument);
            if (null != context.MainForm)
                Application.Run(context);
        }

        private static Form CreateTrafficCounterForm(bool isLicensed, string args)
        {
            return new TrafficCounter(new AppSettings(), isLicensed, args);
        }
    }
}
