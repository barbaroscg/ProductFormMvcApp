using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProductFormMvcApp.Models;

namespace ProductFormMvcApp.Controllers;

public class HomeController : Controller
{
    

    public IActionResult Index(string searchString, string category)
    {
        var products = Repository.Products;
        
        if(!String.IsNullOrEmpty(searchString)){
            ViewBag.SearchString = searchString;
            products = products.Where(p => p.Name.ToLower().Trim().Contains(searchString)).ToList();
        }

        if(!String.IsNullOrEmpty(category) && category != "0"){
            products = products.Where(p => p.CategoryId == int.Parse(category)).ToList();
        }

        // ViewBag.Categories = new SelectList(Repository.Categories, "CategoryId", "Name", category);

        var model = new ProductViewModel {
            Products = products,
            Categories = Repository.Categories,
            SelectedCategory = category
        };


        
        return View(model);
    }

    public IActionResult Create()
    {
        ViewBag.Categories = Repository.Categories;
        return View();
    }

    [HttpPost]
    public IActionResult Create(Product model)
    {
        return View();
    }

   
}
