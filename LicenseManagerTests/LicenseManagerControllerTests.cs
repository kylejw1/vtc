using Microsoft.VisualStudio.TestTools.UnitTesting;
using LicenseManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace LicenseManager.Tests
{
    [TestClass()]
    public class LicenseManagerControllerTests
    {
        private const string ValidLicenseKey = "7FB2-E092-B59A-49F8-9BA1-E4A4-1B6F";
        private const string InvalidLicenseKey = "7FB2-E092-B59A-49F8-9BA1-E4A4-9999";

        private const string PublicKey = "<RSAKeyValue><Modulus>sNNBll27qDwVY2taLYlVRz1qrEe1+xEMSKYDGXojI5znZqQ+VcABzWZbp5Cbjwmw2G5JlrjMMMdkxEPLEC1j5o+tKXGjcJ2M54wjwocudbLzhecby6ZuZLMF3V9IDgr/Nn1AraLPHx1hn9Re2Unzd6rMlxrc3YCxPL1vwjAbPE5vJeoBhTe1TvO0nFMjVqWSfVxH8kPW8xqrBSgOq7akp7fD293T8MdRzsyea6uZe4xy1mgckk8hUDW3J735ISB4QSIy2+f9NfJlju1x/HEz7Lv1bwUlFQNQ0gaquhUK5gmEnoauw7/Ebgc99w0J9gXziaB+Z++K5OJV4Y8RR3NI4Q==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
        
        [TestMethod()]
        public void SetLicenseKeyTest()
        {
            var model = new LicenseModel(typeof(LicenseManagerControllerTests), this, PublicKey, ValidLicenseKey);
            model.Activate(ValidLicenseKey, true);
            model.Deactivate();
            
            var kyle = model.IsActivated;
            var controller = new LicenseManagerController(model, null);

            Assert.IsTrue(controller.TrySetLicenseKey(ValidLicenseKey));

            Assert.IsFalse(controller.TrySetLicenseKey(InvalidLicenseKey));
        }
    }
}