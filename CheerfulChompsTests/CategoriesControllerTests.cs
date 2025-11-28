using CheerfulChomps.Controllers;
using CheerfulChomps.Data;
using CheerfulChomps.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CheerfulChompsTests;

[TestClass]
public class CategoriesControllerTests
{
    // class level vars shared among tests
    private ApplicationDbContext _context;
    CategoriesController controller;

    // this method runs automatically before each test
    [TestInitialize]
    public void TestInitialize()
    {
        // set up db options using unique db name & instantiate db obj
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new ApplicationDbContext(options);

        // populate mock db w/data before tests
        _context.Category.Add(new Category { CategoryId = 38, Name = "Cat 38" });
        _context.Category.Add(new Category { CategoryId = 21, Name = "Cat Twenty One" });
        _context.Category.Add(new Category { CategoryId = 75, Name = "A Fave Category" });
        _context.SaveChanges();

        // instantiate controller using mock db so we can run tests
        controller = new CategoriesController(_context);
    }

    #region "Index"
    [TestMethod]
    public void IndexLoadsView()
    {
        // arrange - set up required vars / objs.  moved to TestInitialize

        // act - call method and store result.
        // Must cast IActionResult return type to ViewResult to see the View Name
        var result = (ViewResult)controller.Index();

        // assert - evaluate if result matches our expectation
        Assert.AreEqual("Index", result.ViewName);
    }

    [TestMethod]
    public void IndexLoadsCategories()
    {
        // act
        var result = (ViewResult)controller.Index();

        // assert - does view data model match the db table data?
        var categories = _context.Category.OrderBy(c => c.Name).ToList();
        CollectionAssert.AreEqual(categories, (List<Category>)result.Model);
    }
    #endregion

    #region "Edit GET"
    
    [TestMethod]
    public void EditGetInvalidId404()
    {
        // act
        var result = (ViewResult)controller.Edit(-1);

        // assert
        Assert.AreEqual("404", result.ViewName);
    }

    [TestMethod]
    public void EditGetValidIdLoadsView()
    {
        // act
        var result = (ViewResult)controller.Edit(38);

        // assert
        Assert.AreEqual("Edit", result.ViewName);
    }

    [TestMethod]
    public void EditGetValidIdLoadsCategory()
    {
        // act
        var result = (ViewResult)controller.Edit(38);

        // assert: does view show correct record from db?
        Assert.AreEqual(_context.Category.Find(38), (Category)result.Model);
    }

    #endregion
}
