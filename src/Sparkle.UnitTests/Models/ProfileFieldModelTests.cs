
namespace Sparkle.UnitTests.Models
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Sparkle.Entities.Networks;
    using Sparkle.Services.Networks.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    [TestClass]
    public class ProfileFieldModelTests
    {
        [TestMethod]
        public void ProfileFieldModel_Serializes()
        {
            var model = new ProfileFieldModel();
            model.Id = (int)ProfileFieldType.City;
            model.Name = ProfileFieldType.City.ToString();
            model.ApplyToUsers = true;
            model.ApplyToCompanies = true;
            model.AvailableValuesCount = 2;
            model.RulesValue = @"{""Value"":{""Type"":""string"",""IsMultiLineText"":true,""StringMaxLength"":1200}}";

            var json = JsonConvert.SerializeObject(model);
            var result = JsonConvert.DeserializeObject<ProfileFieldModel>(json);

            dynamic rules1 = result.Rules;
            Assert.AreEqual(model.Id, result.Id);
            Assert.AreEqual(model.Name, result.Name);
            Assert.AreEqual(model.ApplyToUsers, result.ApplyToUsers);
            Assert.AreEqual(model.ApplyToCompanies, result.ApplyToCompanies);
            Assert.AreEqual(model.AvailableValuesCount, result.AvailableValuesCount);
            Assert.AreEqual("string", (string)rules1.Value.Type);
            Assert.AreEqual(true, (bool)rules1.Value.IsMultiLineText);
            Assert.AreEqual(1200, (int)rules1.Value.StringMaxLength);
        }

        [TestMethod]
        public void ProfileFieldModel_ParseValue_ImplicitString()
        {
            var value = "hello";
            var model = new ProfileFieldModel();
            model.RulesValue = null;

            var result = model.ParseValue(value);

            Assert.AreEqual(value, result);
        }

        [TestMethod]
        public void ProfileFieldModel_ParseValue_ExplicitString()
        {
            var value = "hello";
            var model = new ProfileFieldModel();
            model.RulesValue = @"{""Value"":{""Type"":""string"",""IsMultiLineText"":true,""StringMaxLength"":1200}}";

            var result = model.ParseValue(value);

            Assert.AreEqual(value, result);
        }

        [TestMethod]
        public void ProfileFieldModel_ParseBool_True1()
        {
            var value = "true";
            var model = new ProfileFieldModel();
            model.RulesValue = @"{""Value"":{""Type"":""bool""}}";

            var result = model.ParseValue(value);

            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void ProfileFieldModel_ParseBool_True2()
        {
            var value = "true";
            var model = new ProfileFieldModel();
            model.RulesValue = @"{""Value"":{""Type"":""System.Boolean""}}";

            var result = model.ParseValue(value);

            Assert.AreEqual(true, result);
        }
    }
}
