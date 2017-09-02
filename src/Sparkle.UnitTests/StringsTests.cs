
namespace Sparkle.UnitTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Sparkle.UI;
    using Sparkle.UnitTests.Mocks;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Threading;

    [TestClass]
    public class StringsTests
    {
        private CultureInfo enNe = new CultureInfo("en");    // english 
        private CultureInfo enUs = new CultureInfo("en-us"); // english USA
        private CultureInfo enUk = new CultureInfo("en-gb"); // english UK
        private CultureInfo enAu = new CultureInfo("en-au"); // english Australia
        private CultureInfo deNe = new CultureInfo("de");    // german
        private CultureInfo deDe = new CultureInfo("de-de"); // german Germany
        private CultureInfo deCh = new CultureInfo("de-ch"); // german Switzerland
        private CultureInfo frNe = new CultureInfo("fr");    // french
        private CultureInfo frFr = new CultureInfo("fr-fr"); // french France
        private CultureInfo frCa = new CultureInfo("fr-ca"); // french Canada
        private CultureInfo esNe = new CultureInfo("es");    // spanish 
        private CultureInfo esEs = new CultureInfo("es-es"); // spanish Spain
        private CultureInfo esMx = new CultureInfo("es-mx"); // spanish Mexico

        [TestInitialize]
        public void Init()
        {
            Thread.CurrentThread.CurrentCulture = enUs;
            Thread.CurrentThread.CurrentUICulture = enUs;
        }

        public TestContext TestContext { get; set; }

        #region Level 0 logic

        [TestMethod]
        public void App1_GetBestCulture_L0_FromNull()
        {
            var strings = CreateApp1();
            Assert.AreEqual(enUs, strings.GetBestCulture(null));
        }

        #endregion

        #region Level 1 logic

        [TestMethod]
        public void App1_GetBestCulture_L1_1_FullySupportedRegion()
        {
            var strings = CreateApp1();
            Assert.AreEqual(enUs, strings.GetBestCulture(enUs), "Failed: 1. Fully supported sub-culture");
        }

        [TestMethod]
        public void App1_GetBestCulture_L1_2_NeutralToAnyRegion()
        {
            var strings = CreateApp1();
            Assert.AreEqual(enUs, strings.GetBestCulture(enNe), "Failed: 2. Neutral not supported, fallbacks to any sub-culture");
        }

        [TestMethod]
        public void App1_GetBestCulture_L1_3_RegionToAnyRegion()
        {
            var strings = CreateApp1();
            Assert.AreEqual(enUs, strings.GetBestCulture(enAu), "Failed: 3. Sub-culture not supported, fallbacks to any other sub-culture");
        }

        [TestMethod]
        public void App1_GetBestCulture_L1_4_NotSupported()
        {
            var strings = CreateApp1();
            Assert.AreEqual(enUs, strings.GetBestCulture(esEs), "Failed: 4. Not supported at all, fallbacks to default");
        }

        [TestMethod]
        public void App1_GetBestCulture_L1_5_NotSupported()
        {
            var strings = CreateApp1();
            Assert.AreEqual(enUs, strings.GetBestCulture(esMx), "Failed: 5. Not supported at all, fallbacks to default");
        }

        [TestMethod]//, Ignore]
        public void App1_GetBestCulture_L1_6_RegionToNeutral()
        {
            var strings = CreateApp1();
            Assert.AreEqual(deNe, strings.GetBestCulture(deDe), "Failed: 6. Sub-culture not supported, fallbacks to neutral culture");
        }

        [TestMethod]//, Ignore]
        public void App1_GetBestCulture_L1_7_FullySupportedNeutral()
        {
            var strings = CreateApp1();
            Assert.AreEqual(deNe, strings.GetBestCulture(deNe), "Failed: 7. Fully supported neutral culture");
        }

        #endregion

        #region Level 2 logic

        #endregion

        #region Pot implementation for simple (formatted) strings

        [TestMethod]
        public void App1_T_Simple()
        {
            var strings = Strings.Load(TestContext.TestDeploymentDir, "App1", "en-us");
            
            Assert.AreEqual("Lang: English US (default)", strings.T(      "Lang"), "wrong for default");
            Assert.AreEqual("Lang: English US (default)", strings.T(enUs, "Lang"), "wrong for en-us");
            Assert.AreEqual("Lang: English UK",           strings.T(enUk, "Lang"), "wrong for en-uk");
            Assert.AreEqual("Lang: French France",        strings.T(frFr, "Lang"), "wrong for fr-fr");
            Assert.AreEqual("Lang: German",               strings.T(deNe,   "Lang"), "wrong for ge");
        }

        [TestMethod]
        public void App1_T_Format()
        {
            var strings = Strings.Load(Environment.CurrentDirectory, "App1", "en-us");
            string param = "xZ4h";

            Assert.AreEqual("App1 xZ4h en-us (default)", strings.T(      "Lang1", param), "wrong for default");
            Assert.AreEqual("App1 xZ4h en-us (default)", strings.T(enUs, "Lang1", param), "wrong for en-us");
            Assert.AreEqual("App1 xZ4h en-gb",           strings.T(enUk, "Lang1", param), "wrong for en-uk");
            Assert.AreEqual("App1 xZ4h fr-fr",           strings.T(frFr, "Lang1", param), "wrong for fr-fr");
            Assert.AreEqual("App1 xZ4h de",              strings.T(deNe,   "Lang1", param), "wrong for ge");
        }

        [TestMethod]
        public void App1_T_FormatWithIndexes()
        {
            var strings = Strings.Load(Environment.CurrentDirectory, "App1", "en-us");
            string param = "xZ4h";

            Assert.AreEqual("App1 xZ4h en-us (default)", strings.T(      "Lang {0}", param), "wrong for default");
            Assert.AreEqual("App1 xZ4h en-us (default)", strings.T(enUs, "Lang {0}", param), "wrong for en-us");
            Assert.AreEqual("App1 xZ4h en-gb",           strings.T(enUk, "Lang {0}", param), "wrong for en-uk");
            Assert.AreEqual("App1 xZ4h fr-fr",           strings.T(frFr, "Lang {0}", param), "wrong for fr-fr");
            Assert.AreEqual("App1 xZ4h de",              strings.T(deNe,   "Lang {0}", param), "wrong for ge");
        }

        #endregion

        #region Pot implementation for single/plural (formatted) strings

        [TestMethod]
        public void App1_M_Simple()
        {
            var strings = Strings.Load(Environment.CurrentDirectory, "App1", "en-us");
            string sKey = "Open contact";
            string pKey = "Open contacts";
            int one = 1, many = 3;

            // singular
            Assert.AreEqual("Open contact (us)",    strings.M(      sKey, pKey, one), "wrong for singular default");
            Assert.AreEqual("Open contact (us)",    strings.M(enUs, sKey, pKey, one), "wrong for singular en-us");
            Assert.AreEqual("Open contact (uk)",    strings.M(enUk, sKey, pKey, one), "wrong for singular en-uk");
            Assert.AreEqual("Ouvrir contact",       strings.M(frFr, sKey, pKey, one), "wrong for singular fr-fr");
            Assert.AreEqual("öffnen Kontakt (de)",  strings.M(deNe,   sKey, pKey, one), "wrong f singularor ge");
            // plural
            Assert.AreEqual("Open contacts (us)",   strings.M(      sKey, pKey, many), "wrong for plural default");
            Assert.AreEqual("Open contacts (us)",   strings.M(enUs, sKey, pKey, many), "wrong for plural en-us");
            Assert.AreEqual("Open contacts (uk)",   strings.M(enUk, sKey, pKey, many), "wrong for plural en-uk");
            Assert.AreEqual("Ouvrir contacts",      strings.M(frFr, sKey, pKey, many), "wrong for plural fr-fr");
            Assert.AreEqual("öffnen Kontakts (de)", strings.M(deNe, sKey, pKey, many), "wrong for plural ge");
        }

        [TestMethod]
        public void App1_M_SimpleWithCount()
        {
            var strings = Strings.Load(Environment.CurrentDirectory, "App1", "en-us");
            string sKey = "Open {0} contact";
            string pKey = "Open {0} contacts";
            int one = 1, many = 3;

            // singular
            Assert.AreEqual("Open 1 contact (us)",    strings.M(      sKey, pKey, one), "wrong for singular default");
            Assert.AreEqual("Open 1 contact (us)",    strings.M(enUs, sKey, pKey, one), "wrong for singular en-us");
            Assert.AreEqual("Open 1 contact (uk)",    strings.M(enUk, sKey, pKey, one), "wrong for singular en-uk");
            Assert.AreEqual("Ouvrir 1 contact",       strings.M(frFr, sKey, pKey, one), "wrong for singular fr-fr");
            Assert.AreEqual("öffnen 1 Kontakt (de)",  strings.M(deNe, sKey, pKey, one), "wrong for singular ge");
            // plural
            Assert.AreEqual("Open 3 contacts (us)",   strings.M(      sKey, pKey, many), "wrong for plural default");
            Assert.AreEqual("Open 3 contacts (us)",   strings.M(enUs, sKey, pKey, many), "wrong for plural en-us");
            Assert.AreEqual("Open 3 contacts (uk)",   strings.M(enUk, sKey, pKey, many), "wrong for plural en-uk");
            Assert.AreEqual("Ouvrir 3 contacts",      strings.M(frFr, sKey, pKey, many), "wrong for plural fr-fr");
            Assert.AreEqual("öffnen 3 Kontakts (de)", strings.M(deNe, sKey, pKey, many), "wrong for plural ge");
        }

        [TestMethod]
        public void App1_M_FormatWithCount()
        {
            var strings = Strings.Load(Environment.CurrentDirectory, "App1", "en-us");
            string sKey = "Open {0} contact {1}";
            string pKey = "Open {0} contacts {1}";
            int one = 1, many = 3;
            string param = "now";

            // singular
            Assert.AreEqual("Open 1 contact now (us)",    strings.M(      sKey, pKey, one, param), "wrong for singular default");
            Assert.AreEqual("Open 1 contact now (us)",    strings.M(enUs, sKey, pKey, one, param), "wrong for singular en-us");
            Assert.AreEqual("Open 1 contact now (uk)",    strings.M(enUk, sKey, pKey, one, param), "wrong for singular en-uk");
            Assert.AreEqual("Ouvrir 1 contact now",       strings.M(frFr, sKey, pKey, one, param), "wrong for singular fr-fr");
            Assert.AreEqual("öffnen 1 Kontakt now (de)",  strings.M(deNe, sKey, pKey, one, param), "wrong for singular ge");
            // plural
            Assert.AreEqual("Open 3 contacts now (us)",   strings.M(      sKey, pKey, many, param), "wrong for plural default");
            Assert.AreEqual("Open 3 contacts now (us)",   strings.M(enUs, sKey, pKey, many, param), "wrong for plural en-us");
            Assert.AreEqual("Open 3 contacts now (uk)",   strings.M(enUk, sKey, pKey, many, param), "wrong for plural en-uk");
            Assert.AreEqual("Ouvrir 3 contacts now",      strings.M(frFr, sKey, pKey, many, param), "wrong for plural fr-fr");
            Assert.AreEqual("öffnen 3 Kontakts now (de)", strings.M(deNe, sKey, pKey, many, param), "wrong for plural ge");
        }

        #endregion

        #region Pot implementation for application fallback

        [TestMethod]
        public void Fallback_T_Simple()
        {
            var strings = Strings.Load(Environment.CurrentDirectory, "App1", "en-us");
            strings.Fallback = Strings.Load(Environment.CurrentDirectory, "default", "en-us");
            
            Assert.AreEqual("Fallback: English US (default)", strings.T(      "LangF"), "wrong for default");
            Assert.AreEqual("Fallback: English US (default)", strings.T(enUs, "LangF"), "wrong for en-us");
            Assert.AreEqual("Fallback: English UK",           strings.T(enUk, "LangF"), "wrong for en-uk");
            Assert.AreEqual("Fallback: French France",        strings.T(frFr, "LangF"), "wrong for fr-fr");
            Assert.AreEqual("Fallback: German",               strings.T(deNe,   "LangF"), "wrong for ge");
        }

        #endregion

        private static TestStrings CreateApp1()
        {
            return TestStrings.Load(Environment.CurrentDirectory, "App1", "en-us");
        }
    }
}
