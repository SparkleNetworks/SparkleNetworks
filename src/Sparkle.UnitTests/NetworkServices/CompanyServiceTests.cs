
namespace Sparkle.UnitTests.NetworkServices
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Sparkle.Services.Internals;
    using Sparkle.Services.Main.Networks;
    using Sparkle.Services.Networks;
    using Sparkle.Services.Networks.Mocks;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class CompanyServiceTests
    {
        public static CompanyService GetService()
        {
            var services = new MainServiceFactory(
                connectionString: "",
                emailTemplateProvider: () => null,
                syslogger: () => Sparkle.Infrastructure.SysLogger.NewEmpty,
                parallelismMode: ContextParallelismMode.None,
                cache: new BasicServiceCache());
            return (CompanyService)((IServiceFactory)services).Company;
        }

        [TestClass]
        public class FindEmailDomainFromNameAndEmailMethod
        {
            [TestMethod]
            public void EasyMatch_Succeeds()
            {
                string name = "Wazeo";
                string email = "kevin@wazeo.co.uk";
                string expectedDomain = "wazeo.co.uk";

                var service = GetService();
                string result = service.FindEmailDomainFromNameAndEmail(name, email);

                Assert.AreEqual(expectedDomain, result);
            }

            [TestMethod]
            public void NoMatch_Succeeds()
            {
                string name = "Wakeo";
                string email = "kevin@wazeo.co.uk";
                string expectedDomain = null;

                var service = GetService();
                string result = service.FindEmailDomainFromNameAndEmail(name, email);

                Assert.IsNull(expectedDomain, result);
            }

            [TestMethod]
            public void OneWordInNameMatches_Succeeds()
            {
                string name = "Wakeo Consulting";
                string email = "kevin@wazeo.co.uk";
                string expectedDomain = null;

                var service = GetService();
                string result = service.FindEmailDomainFromNameAndEmail(name, email);

                Assert.AreEqual(expectedDomain, result);
            }

            [TestMethod]
            public void ManyWordsInNameMatch_Succeeds()
            {
                string name = "Wakeo Consulting";
                string email = "kevin@wazeo-consulting.co.uk";
                string expectedDomain = "wazeo-consulting.co.uk";

                var service = GetService();
                string result = service.FindEmailDomainFromNameAndEmail(name, email);

                Assert.AreEqual(expectedDomain, result);
            }

            [TestMethod]
            public void AwakeIT_Succeeds()
            {
                string name = "Awake'AT";
                string email = "contact@awakeat-groupe.com";
                string expectedDomain = "awakeat-groupe.com";

                var service = GetService();
                string result = service.FindEmailDomainFromNameAndEmail(name, email);

                Assert.AreEqual(expectedDomain, result);
            }

            [TestMethod]
            public void ByBeez_Succeeds()
            {
                string name = "Buuz";
                string email = "contact@bybuuz.com";
                string expectedDomain = "bybuuz.com";

                var service = GetService();
                string result = service.FindEmailDomainFromNameAndEmail(name, email);

                Assert.AreEqual(expectedDomain, result);
            }

            [TestMethod]
            public void CITC_Succeeds()
            {
                string name = "ABCD-EXXXFID";
                string email = "ndefaut@ABCD-EXXXFID.com";
                string expectedDomain = "ABCD-EXXXFID.com";

                var service = GetService();
                string result = service.FindEmailDomainFromNameAndEmail(name, email);

                Assert.AreEqual(expectedDomain, result);
            }

            [TestMethod]
            public void DSDSystem_Succeeds()
            {
                string name = "DSD System";
                string email = "contact@dsdsystem.com";
                string expectedDomain = "dsdsystem.com";

                var service = GetService();
                string result = service.FindEmailDomainFromNameAndEmail(name, email);

                Assert.AreEqual(expectedDomain, result);
            }

            [TestMethod]
            public void Kayak_Succeeds()
            {
                string name = "Kahghak";
                string email = "contact@kahghak.fr";
                string expectedDomain = "kahghak.fr";

                var service = GetService();
                string result = service.FindEmailDomainFromNameAndEmail(name, email);

                Assert.AreEqual(expectedDomain, result);
            }

            [TestMethod]
            public void PlanetNemo_Succeeds()
            {
                string name = "Planet Memo";
                string email = "contact@planetmemo.com";
                string expectedDomain = "planetmemo.com";

                var service = GetService();
                string result = service.FindEmailDomainFromNameAndEmail(name, email);

                Assert.AreEqual(expectedDomain, result);
            }

            [TestMethod]
            public void GmailPlus_IsForbiden()
            {
                string name = "Gmail Plus";
                string email = "gmplus@gmail.com";

                var service = GetService();
                string result = service.FindEmailDomainFromNameAndEmail(name, email);

                Assert.IsNull(result);
            }

            [TestMethod]
            public void TheHotMail_IsForbiden()
            {
                string name = "the hot mail";
                string email = "alexyz@hotmail.com";

                var service = GetService();
                string result = service.FindEmailDomainFromNameAndEmail(name, email);

                Assert.IsNull(result);
            }

            [TestMethod]
            public void LaPosteNet_IsForbiden()
            {
                string name = "laposte point net";
                string email = "contact@laposte.net";

                var service = GetService();
                string result = service.FindEmailDomainFromNameAndEmail(name, email);

                Assert.IsNull(result);
            }
        }
    }
}
