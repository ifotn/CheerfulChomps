using CheerfulChomps.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace CheerfulChompsTests
{
    [TestClass]
    public class HomeControllerTests
    {
        [TestMethod]
        public void IndexLoadsView()
        {
            // arrange - set up required vars / objs
            var controller = new HomeController();

            // act - call method and store result.
            // Must cast IActionResult return type to ViewResult to see the View Name
            var result = (ViewResult)controller.Index();

            // assert - evaluate if result matches our expectation
            Assert.AreEqual("Index", result.ViewName);
        }

        [TestMethod]
        public void PrivacyLoadsView()
        {
            // arrange - set up required vars / objs
            var controller = new HomeController();

            // act - call method and store result.
            // Must cast IActionResult return type to ViewResult to see the View Name
            var result = (ViewResult)controller.Privacy();

            // assert - evaluate if result matches our expectation
            Assert.AreEqual("Privacy", result.ViewName);
        }
    }
}
