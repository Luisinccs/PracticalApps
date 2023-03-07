using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Northwind.Mvc.Models;
using Packt.Shared;

namespace Northwind.Mvc.Controllers;

public class HomeController : Controller {

    private readonly NorthwindContext db;
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger, NorthwindContext injectedContext) {
        _logger = logger;
        db = injectedContext;
    }

    public IActionResult Index() {

        _logger.LogError("This is a serious error (not really!");
        _logger.LogWarning("This is your first warning!");
        _logger.LogWarning("Secong warning!");
        _logger.LogInformation("I am in the Index method of the HomeController.");

        HomeIndexViewModel model = new(
            VisitorCount: Random.Shared.Next(1, 1001),
            Categories: db.Categories.ToList(),
            Products: db.Products.ToList());

        return View(model);
    }

    public IActionResult Privacy() {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public IActionResult ProductDetail(int? id) {

        if (!id.HasValue) {
            return BadRequest("You must pass a product ID in the route, for example, /Home/ProductDetail/21");
        }

        Product? model = db.Products.SingleOrDefault(p => p.ProductId == id);   

        if (model is null) {
            return NotFound($"ProductID {id} not found.");
        }

        return View(model);
    }

    public IActionResult ModelBinding() {
        return View();
    }

    [HttpPost]
    public IActionResult ModelBinding(Thing thing) {
        return View(thing);
    }
}

