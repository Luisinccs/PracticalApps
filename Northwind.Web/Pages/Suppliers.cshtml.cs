using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Packt.Shared;

namespace Northwind.Web.Pages;

public class SuppliersModel : PageModel {

    private NorthwindContext db;

    public IEnumerable<Supplier>? Suppliers {get; set;}

    [BindProperty]
    public Supplier? Supplier {get; set;}

    public SuppliersModel(NorthwindContext injectedContext){
        db = injectedContext;
    }

    public void OnGet() {
        ViewData["Title"] = "Northwind B2B - Suppliers";

        Suppliers = db.Suppliers.OrderBy(c => c.Country).ThenBy(c => c.CompanyName);
        // Suppliers = new[] {"Alpha, Co", "Beta Limieted", "Gamma Corp"};
    }

    public IActionResult OnPost() {
        if ((Supplier is not null) && ModelState.IsValid) {
            // Adds the supplier to the existing table
            db.Suppliers.Add(Supplier);
            db.SaveChanges();
            return RedirectToPage("/suppliers");
        } else {
            return Page();
        }
    }

}