using Microsoft.VisualStudio.TestTools.UnitTesting;
using LicenseManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LicenseManager.Tests
{
    [TestClass()]
    public class LicenseManagerControllerTests
    {
        private const string ValidLicenseKey = "D2DF-1C8F-6A5C-40F0-8E5F-041B-9A8A";
        private const string InvalidLicenseKey = "D2DF-1C8F-6A5C-40F0-8E5F-041B-9999";

        [TestMethod()]
        public void SetLicenseKeyTest()
        {
            var model = new LicenseManagerModel();
            var controller = new LicenseManagerController(model, null);

            Assert.IsTrue(controller.TrySetLicenseKey(ValidLicenseKey));

            Assert.IsFalse(controller.TrySetLicenseKey(InvalidLicenseKey));
        }
    }
}