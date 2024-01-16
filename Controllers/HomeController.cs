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
            products = products.Where(p => p.Name.ToLowerInvariant().Trim().Contains(searchString)).ToList();
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
        ViewBag.Categories = new SelectList(Repository.Categories, "CategoryId", "Name");
        return View();
    }

     public IActionResult Edit(int? id)
    {
        if(id == null)
        {
            return NotFound();
        }
        var entity = Repository.Products.FirstOrDefault( p => p.ProductId == id);
        if(entity == null)
        {
            return NotFound();
        }
         ViewBag.Categories = new SelectList(Repository.Categories, "CategoryId", "Name");
         return View(entity);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Product model, IFormFile imageFile)
    {

        var extension = "";

        if(imageFile != null)
        {
            var allowedExtensions = new[] {".jpg", ".jpeg", ".png"};
            //dosya adı bulur
             extension = Path.GetExtension(imageFile.FileName); //xyz.jpg
            if(!allowedExtensions.Contains(extension))
            {
                ModelState.AddModelError("", "Geçerli bir resim tipi seçiniz.");
            }
        }

        if(ModelState.IsValid)
        {
            if(imageFile != null)
            {
                 //dosyaya random isim ataması yapar
                var randomFileName = string.Format($"{Guid.NewGuid()}{extension}");
                 //dosyanın kaydedileceği yer
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", randomFileName);
                //burada FileStream Classı asenkron olarak çalışacaktır. Ve sıfırdan yeni bir dosya oluşumu sağlayacaktır. Bu kütüphanenin işlevini tıpklı daha önce gördüğümüz Ado.Net veri çekim işlemleri gibi bir lifecycle arasına eklemek için using blokları kullandık.
                using(var stream = new FileStream(path, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }
            
            model.Image = randomFileName;
            model.ProductId = Repository.Products.Count +1;
            Repository.CreateProduct(model);
            return RedirectToAction("Index");
            }
        }
        ViewBag.Categories = new SelectList(Repository.Categories, "CategoryId", "Name");
        return View(model);
       
    }


    [HttpPost]
    public async Task<IActionResult> Edit(int id,Product model, IFormFile? imageFile)
    {
        if(id != model.ProductId)
        {
            return NotFound();
        }

  
        if(ModelState.IsValid)
        {
            if (imageFile != null)
            {
                var extension = Path.GetExtension(imageFile.FileName); //xyz.jpg
                                                                       //dosyaya random isim ataması yapar
                var randomFileName = string.Format($"{Guid.NewGuid()}{extension}");
                //dosyanın kaydedileceği yer
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", randomFileName);
                //burada FileStream Classı asenkron olarak çalışacaktır. Ve sıfırdan yeni bir dosya oluşumu sağlayacaktır. Bu kütüphanenin işlevini tıpklı daha önce gördüğümüz Ado.Net veri çekim işlemleri gibi bir lifecycle arasına eklemek için using blokları kullandık.
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }
            model.Image = randomFileName;
            }
            
           
            Repository.EditProduct(model);
            return RedirectToAction("Index");
            
        }
        ViewBag.Categories = new SelectList(Repository.Categories, "CategoryId", "Name");
        return View(model);
       
    }

    public IActionResult Delete(int? id)
    {
        if(id == null)
        {
            return NotFound();
        }

        var entity = Repository.Products.FirstOrDefault(p => p.ProductId == id);
        if(entity == null)
        {
            return NotFound();
        }

        // Repository.DeleteProduct(entity);
        return View("DeleteConfirm", entity);
    }

    [HttpPost]
    public IActionResult Delete(int id, int ProductId)
    {
        if(id!= ProductId)
        {
            return NotFound();
        }
         var entity = Repository.Products.FirstOrDefault(p => p.ProductId == ProductId);
         if(entity == null)
         {
              return NotFound();
         }

         Repository.DeleteProduct(entity);
         return RedirectToAction("Index");

    }

   
}
