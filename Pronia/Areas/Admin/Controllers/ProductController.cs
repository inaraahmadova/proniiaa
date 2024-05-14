using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Pronia.DataAccesLayer;
using Pronia.Extensions;
using Pronia.Models;
using Pronia.ViewModels.Products;
using System.Text;
using Pronia.Extensions;

namespace Pronia.Areas.Admin.Controllers;
[Area("Admin")]
public class ProductController(ProniaContext _context, IWebHostEnvironment _env) : Controller
{
    public async Task<IActionResult> Index()
	{
        return View(await _context.Products
            .Select(p => new GetProductAdminVM
            {
                CostPrice = p.CostPrice,
                Discount = p.Discount,
                Id = p.Id,
                ImageUrl = p.ImageUrl,
                Name = p.Name,
                Raiting = p.Raiting,
                SellPrice = p.SellPrice,
                StockCount = p.StockCount,
                Categories = p.ProductCategories.Select(pc=>pc.Category.Name).Bind(','),
                CreatedTime=p.CreatedTime.ToString("dd MMM ddd yyyy"),
                UpdatedTime = p.UpdatedTime.ToString("dd MMM ddd yyyy")
            })
            .ToListAsync());
    }
    public async Task<IActionResult> Create()
    {
        ViewBag.Categories = await _context.Categories
            .Where(s=> !s.IsDeleted)
            .ToListAsync();
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Create(CreateProductVM data)
    {
        if (data.ImageFile != null)
        {
            if (!data.ImageFile.IsValidType("image"))
                ModelState.AddModelError("ImageFile","Fayl sekil formatinda olmalidir.");
            if (!data.ImageFile.IsValidLength(300))
                ModelState.AddModelError("ImageFile","Faylin olcusu 200kb-den cox olmamalidir.");
        }
        bool isImageValid = true;
        StringBuilder sb = new StringBuilder();
        foreach (var img in data.ImageFiles ?? new List<IFormFile>())
        {
            if (!img.IsValidType("image"))
            {
                sb.Append("-" + img.FileName + " Fayli sekil formatinda olmalidir.");
                isImageValid = false;
            }
            if (!img.IsValidLength(300))
            {
                sb.Append("-" + img.FileName + " Faylinin olcusu 200kb-den cox olmamalidir.");
                isImageValid = false;
            }
        }
        if (!isImageValid)
        {
            ModelState.AddModelError("ImageFiles", sb.ToString());
        }
        if (await _context.Categories.ContainsAsync(c => data.CategoryIds.Contains(c.Id)) != data.CategoryIds.Length)
            ModelState.AddModelError("CategoryIds", "Kateqoriya tapilmadi");
        if (!ModelState.IsValid)
        {
            ViewBag.Categories = await _context.Categories
            .Where(s => !s.IsDeleted)
            .ToListAsync();
            return View(data);
        }
        return View(data);
        string fileName = await data.ImageFile.SaveFileAsync(Path.Combine(_env.WebRootPath, "imgs", "products"));
        Product prod = new Product
        {
            CostPrice = data.CostPrice,
            CreatedTime = DateTime.Now,
            Discount = data.Discount,
            ImageUrl = Path.Combine("imgs", "products", fileName),
            IsDeleted = false,
            Name = data.Name,
            Raiting = data.Raiting,
            SellPrice = data.SellPrice,
            StockCount = data.StockCount,
            Images = new List<ProductImage>(),
            ProductCategories = data.CategoryIds.Select(x=>new
            ProductCategory
            {
                CategoryId=x
            }).ToList()
        }; 
        foreach (var img in data.ImageFiles)
        {
            string imgName = await img.SaveFileAsync(Path.Combine(_env.WebRootPath, "imgs", "products"));
            //await _context.ProductImages.AddAsync(new ProductImage
            //{
            //    ImageUrl = Path.Combine("imgs", "products", imgName),
            //    CreatedTime = DateTime.Now,
            //    IsDeleted = false,
            //    Product = prod
            //})
            prod.Images.Add(new ProductImage
            {
                ImageUrl = Path.Combine("imgs", "products", imgName),
                CreatedTime = DateTime.Now,
                IsDeleted = false,
            });
        }
        await _context.Products.AddAsync(prod);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
