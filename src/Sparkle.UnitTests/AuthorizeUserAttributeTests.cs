
namespace Sparkle.UnitTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Sparkle.Entities.Networks;
    using Sparkle.Filters;
    using Sparkle.Infrastructure.Constants;
    using Sparkle.Services.Networks;
    using Sparkle.Services.Networks.Mocks;
    using Sparkle.UnitTests.Stubs;
    using Sparkle.WebBase;
    using System;
    using System.Collections.Generic;
    using System.Security.Principal;

    [TestClass]
    public class AuthorizeUserAttributeTests
    {
        [TestMethod]
        public void UnauthenticatedAreDenied()
        {
            IPrincipal user = new GenericPrincipal(new GenericIdentity(""), null);
            var target = new AuthorizeUserAttribute();
            var httpContext = new BasicHttpContext(
                user: user);
            var result = target.IsAuthorized(httpContext);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void AuthenticatedAreAllowed()
        {
            // prepare
            IPrincipal user = new GenericPrincipal(new GenericIdentity("remi"), null);

            // mock
            var services = new BasicServiceFactory(null);
            var people = new Mock<IPeopleService>();
            people.Setup(s => s.IsActive(It.IsAny<User>())).Returns(true);
            services.People = people.Object;
            var target = new AuthorizeUserAttribute();
            var httpContext = new BasicHttpContext(
                user: user,
                session: new BasicHttpSessionState());
            httpContext.Items[typeof(IServiceFactory).Name] = services;
            httpContext.Session[SessionConstants.Me] = new User
            {
                Username = "remi",
            };

            // execute
            var result = target.IsAuthorized(httpContext);

            // verify
            Assert.IsTrue(result);
        }

        ////[TestMethod]
        ////public void UnauthorizedRequestReturn403IfAuthenticated()
        ////{
        ////    IPrincipal user = new GenericPrincipal(new GenericIdentity("remi"), null);
        ////    var target = new AuthorizeUserAttribute();
        ////    var httpContext = new BasicHttpContext(
        ////        user: user);
        ////    var filterContext = new AuthorizationContext();
        ////    filterContext.HttpContext = httpContext;
        ////    target.TestHandleUnauthorizedRequest(filterContext);
        ////    Assert.IsInstanceOfType(filterContext.Result, typeof(HttpStatusCodeResult));
        ////    var result = (HttpStatusCodeResult)filterContext.Result;
        ////    Assert.AreEqual(403, result.StatusCode);
        ////}
    }
}
