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

         string filename = null;
         if (args.Length == 1) filename = args[0];

          var mainForm = new TrafficCounter(new AppSettings(), filename);

          Application.Run(mainForm);
      }
   }
}
