
namespace Sparkle.UnitTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Sparkle.Entities.Networks;
    using Sparkle.Filters;
    using Sparkle.Infrastructure.Constants;
    using Sparkle.UnitTests.Stubs;
    using Sparkle.WebBase;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Principal;
    using System.Text;
    using System.Web;

    [TestClass]
    public class AuthorizeByNetworkAccessAttributeTests
    {
        [TestMethod]
        public void Unauthenticated_AreDenied()
        {
            var flag = NetworkAccessLevel.Disabled;
            var sessionContent = new Dictionary<string, object>();
            IPrincipal user = null;
            var session = new NetworkSessionService(sessionContent);
            var target = new AuthorizeByNetworkAccessAttribute(flag);
            var httpContext = new BasicHttpContext(
                session: new BasicHttpSessionState(sessionContent),
                user: user);
            var result = target.IsAuthorized(httpContext);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void SingleAbsentFlag_UserIsDenied()
        {
            var flag = NetworkAccessLevel.SparkleStaff;
            var sessionContent = new Dictionary<string, object>();
            var user = new GenericPrincipal(new GenericIdentity("username"), null);
            sessionContent.Add(SessionConstants.Me, new User
            {
                NetworkAccess = NetworkAccessLevel.User,
            });
            var session = new NetworkSessionService(sessionContent);
            var target = new AuthorizeByNetworkAccessAttribute(flag);
            var httpContext = new BasicHttpContext(
                session: new BasicHttpSessionState(sessionContent),
                user: user);
            var result = target.IsAuthorized(httpContext);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void SinglePresentFlag_UserIsAllowed()
        {
            var flag = NetworkAccessLevel.SparkleStaff;
            var sessionContent = new Dictionary<string, object>();
            var user = new GenericPrincipal(new GenericIdentity("username"), null);
            sessionContent.Add(SessionConstants.Me, new User
            {
                NetworkAccess = NetworkAccessLevel.User | NetworkAccessLevel.SparkleStaff,
            });
            var session = new NetworkSessionService(sessionContent);
            var target = new AuthorizeByNetworkAccessAttribute(flag);
            var httpContext = new BasicHttpContext(
                session: new BasicHttpSessionState(sessionContent),
                user: user);
            var result = target.IsAuthorized(httpContext);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void MultipleAbsentFlag_UserIsDenied()
        {
            var flag = NetworkAccessLevel.SparkleStaff | NetworkAccessLevel.NetworkAdmin;
            var sessionContent = new Dictionary<string, object>();
            var user = new GenericPrincipal(new GenericIdentity("username"), null);
            sessionContent.Add(SessionConstants.Me, new User
            {
                NetworkAccess = NetworkAccessLevel.User,
            });
            var session = new NetworkSessionService(sessionContent);
            var target = new AuthorizeByNetworkAccessAttribute(flag);
            var httpContext = new BasicHttpContext(
                session: new BasicHttpSessionState(sessionContent),
                user: user);
            var result = target.IsAuthorized(httpContext);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void MultiplePresentFlag_UserIsAllowed()
        {
            var flag = NetworkAccessLevel.SparkleStaff | NetworkAccessLevel.NetworkAdmin;
            var sessionContent = new Dictionary<string, object>();
            var user = new GenericPrincipal(new GenericIdentity("username"), null);
            sessionContent.Add(SessionConstants.Me, new User
            {
                NetworkAccess = NetworkAccessLevel.User | NetworkAccessLevel.NetworkAdmin,
            });
            var session = new NetworkSessionService(sessionContent);
            var target = new AuthorizeByNetworkAccessAttribute(flag);
            var httpContext = new BasicHttpContext(
                session: new BasicHttpSessionState(sessionContent),
                user: user);
            var result = target.IsAuthorized(httpContext);
            Assert.IsTrue(result);
        }
    }
}
