﻿ using GOTech;
 using Microsoft.VisualStudio.TestTools.UnitTesting;
 using Moq;
 using System.Web;
 using System.Web.Mvc;
 using System.Web.Routing;

/* 
 * Name: Jo Lim
 * Date: Apr 5, 2019
 * Last Modified: Apr 5, 2019
 * Description: This class unit tests Routing in CustomersController
 * */
namespace GOTechUnitTest.RoutingTest
{
    [TestClass]
    public class CustomersControllerRoutingTest
    {
        private Mock<HttpRequestBase> _request;
        private Mock<HttpContextBase> _context;
        private Mock<HttpResponseBase> _response;
        private RouteCollection _routes;

        [TestInitialize]
        public void Initialize()
        {
            _context = new Mock<HttpContextBase>();
            _request = new Mock<HttpRequestBase>();
            _response = new Mock<HttpResponseBase>();
            _routes = new RouteCollection();
        }

        // This reusable method constructs http context
        private void CreateHttpContext(string targetUrl = null,
                                                                        string httpMethod = "GET")
        {
            _request.Setup(m => m.AppRelativeCurrentExecutionFilePath)
                 .Returns(targetUrl);

            _request.Setup(m => m.HttpMethod).Returns(httpMethod);

            _response.Setup(m => m.ApplyAppPathModifier(
            It.IsAny<string>())).Returns<string>(s => s);

            _context.Setup(m => m.Request).Returns(_request.Object);

            _context.Setup(m => m.Response).Returns(_response.Object);

            RouteConfig.RegisterRoutes(_routes);
        }

        // https://{application base path}/External
        [TestMethod]
        public void CustomerIndexNoParameter()
        {
            // ARRANGE
            CreateHttpContext("~/External");

            // ACT
            RouteData routeData = _routes.GetRouteData(_context.Object);

            // ASSERT
            Assert.IsNotNull(routeData);
            Assert.AreEqual("Customers", routeData.Values["controller"]);
            Assert.AreEqual("Index", routeData.Values["action"]);
            Assert.AreEqual(UrlParameter.Optional, routeData.Values["email"]);
        }

        // https://{application base path}/External/Details
        // However, in this case, this routing is going nowhere. "email" is NOT an optional parameter in this action method
        [TestMethod]
        public void CustomerDetailsNoParameter()
        {
            // ARRANGE
            CreateHttpContext("~/External/Details");

            // ACT
            RouteData routeData = _routes.GetRouteData(_context.Object);

            // ASSERT
            Assert.AreEqual("Customers", routeData.Values["controller"]);
            Assert.AreEqual("Details", routeData.Values["action"]);
        }

        // https://{application base path}/External/Details/random@google.com
        [TestMethod]
        public void CustomerDetailsRandomParameter()
        {
            // ARRANGE
            CreateHttpContext("~/External/Details/random@gmail.com");

            // ACT
            RouteData routeData = _routes.GetRouteData(_context.Object);

            // ASSERT
            Assert.IsNotNull(routeData);
            Assert.AreEqual("Customers", routeData.Values["controller"]);
            Assert.AreEqual("Details", routeData.Values["action"]);
            Assert.IsTrue(UrlParameter.Equals("random@gmail.com", routeData.Values["email"]));
        }

        [TestMethod]
        public void CustomerEditNoParameterGET()
        {
            // ARRANGE
            CreateHttpContext("~/External/Edit");

            // ACT
            RouteData routeData = _routes.GetRouteData(_context.Object);

            // ASSERT
            Assert.AreEqual("Customers", routeData.Values["controller"]);
            Assert.AreEqual("Edit", routeData.Values["action"]);
        }

        // https://{application base path}/External/Details/random@google.com
        [TestMethod]
        public void CustomerEditRandomParameterGET()
        {
            // ARRANGE
            CreateHttpContext("~/External/Edit/random@gmail.com");

            // ACT
            RouteData routeData = _routes.GetRouteData(_context.Object);

            // ASSERT
            Assert.IsNotNull(routeData);
            Assert.AreEqual("Customers", routeData.Values["controller"]);
            Assert.AreEqual("Edit", routeData.Values["action"]);
            Assert.IsTrue(UrlParameter.Equals("random@gmail.com", routeData.Values["email"]));
        }

        // https://{application base path}/External/Details/random@google.com
        // POST method
        [TestMethod]
        public void CustomerEditRandomParameterPost()
        {
            // ARRANGE
            CreateHttpContext("~/External/Edit/random@gmail.com", "POST");

            // ACT
            RouteData routeData = _routes.GetRouteData(_context.Object);

            // ASSERT
            Assert.IsNotNull(routeData);
            Assert.AreEqual("Customers", routeData.Values["controller"]);
            Assert.AreEqual("Edit", routeData.Values["action"]);
            Assert.IsTrue(UrlParameter.Equals("random@gmail.com", routeData.Values["email"]));
        }

        // https://{application base path}/External/Details/random@google.com
        [TestMethod]
        public void CustomerDeleteRandomParameterGET()
        {
            // ARRANGE
            CreateHttpContext("~/External/Delete/random@gmail.com");

            // ACT
            RouteData routeData = _routes.GetRouteData(_context.Object);

            // ASSERT
            Assert.IsNotNull(routeData);
            Assert.AreEqual("Customers", routeData.Values["controller"]);
            Assert.AreEqual("Delete", routeData.Values["action"]);
            Assert.IsTrue(UrlParameter.Equals("random@gmail.com", routeData.Values["email"]));
        }

        // https://{application base path}/External/Details/random@google.com
        // POST method
        [TestMethod]
        public void CustomerDeleteConfirmedRandomParameterPost()
        {
            // ARRANGE
            CreateHttpContext("~/External/Delete/random@gmail.com", "POST");

            // ACT
            RouteData routeData = _routes.GetRouteData(_context.Object);

            // ASSERT
            Assert.IsNotNull(routeData);
            Assert.AreEqual("Customers", routeData.Values["controller"]);
            Assert.AreEqual("Delete", routeData.Values["action"]);
            Assert.IsTrue(UrlParameter.Equals("random@gmail.com", routeData.Values["email"]));
        }
    }
}