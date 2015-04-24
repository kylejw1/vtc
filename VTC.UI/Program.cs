using System;
using System.Windows.Forms;
using VTC.Settings;

namespace VTC
{
   static class Program
   {
      /// <summary>
      /// The main entry point for the application.
      /// </summary>
      [STAThread]
      static void Main(string[] args)
      {
         Application.EnableVisualStyles();
         Application.SetCompatibleTextRenderingDefault(false);

         string appArgument = null;
         if (args.Length == 1) appArgument = args[0];

          var mainForm = new TrafficCounter(new AppSettings(), appArgument);

          Application.Run(mainForm);
      }
   }
}
