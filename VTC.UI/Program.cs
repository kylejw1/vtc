using System;
using System.Reflection;
using System.Windows.Forms;
using NLog;
using VTC.Settings;

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
         AppDomain.CurrentDomain.UnhandledException += (_, e) => _logger.Error((Exception) e.ExceptionObject, "Unhandled exception");

         _logger.Info("***** Start. v." + Assembly.GetExecutingAssembly().GetName().Version.ToString());
         if (args.Length > 0)
         {
             _logger.Info("Arguments: " + string.Join(";", args));
         }

          string appArgument = null;
         if (args.Length == 1) appArgument = args[0];

          var mainForm = new TrafficCounter(new AppSettings(), appArgument);

          Application.Run(mainForm);
      }
   }
}
