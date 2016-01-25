using SkyXoft.BusinessSolutions.LicenseManager.Protector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LicenseManager
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            string publicKey = "<RSAKeyValue><Modulus>sNNBll27qDwVY2taLYlVRz1qrEe1+xEMSKYDGXojI5znZqQ+VcABzWZbp5Cbjwmw2G5JlrjMMMdkxEPLEC1j5o+tKXGjcJ2M54wjwocudbLzhecby6ZuZLMF3V9IDgr/Nn1AraLPHx1hn9Re2Unzd6rMlxrc3YCxPL1vwjAbPE5vJeoBhTe1TvO0nFMjVqWSfVxH8kPW8xqrBSgOq7akp7fD293T8MdRzsyea6uZe4xy1mgckk8hUDW3J735ISB4QSIy2+f9NfJlju1x/HEz7Lv1bwUlFQNQ0gaquhUK5gmEnoauw7/Ebgc99w0J9gXziaB+Z++K5OJV4Y8RR3NI4Q==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new LicensedFormContext(publicKey, ProcessLicensedResult, String.Empty));
        }

        private static Form ProcessLicensedResult(bool isLicensed, string args)
        {
            return null;
        }
    }
}
