using System;
using System.Reflection;
using System.Windows.Forms;
using NLog;
using LicenseManager;
using VTC.Common;
using VTC.RegionConfiguration;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using VTC.Kernel.RegionConfig;

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

            var cs = new List<CaptureSource.CaptureSource>();
            for(int i = 0; i < 20; i++)
            {
                var c = new CaptureSource.VideoFileCapture(@"C:\vtc\bin\traffic.wmv");
                c.Init(new AppSettings()
                {
                    FrameHeight = 200,
                    FrameWidth = 200
                });
                cs.Add(c);
            }
            
            var rcDal = new FileRegionConfigDAL(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                                        "\\VTC\\regionConfigs.xml");
            var regions = rcDal.LoadRegionConfigList().ToList();
            var view = new RegionConfigSelectorView();
            var model = new RegionConfigSelectorModel(cs, regions);
            view.SetModel(model);

            if (view.ShowDialog() == DialogResult.OK)
            {
                // Save any region config changes
                var regionConfigs = view.GetModel();
                rcDal.SaveRegionConfigList(regionConfigs.RegionConfigs);

                // Get results
                var results = view.GetRegionConfigSelections();
            }
            return;

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
