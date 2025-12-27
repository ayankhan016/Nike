using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using P2WebMVC.Data;
using P2WebMVC.Interfaces;
using P2WebMVC.Models.DomainModels;
using P2WebMVC.Types;

namespace P2WebMVC.Controllers
{
    public class AdminController : Controller
    {
        private readonly SqlDbContext dbContext;
        private readonly ITokenService tokenService;
        private readonly ICloudinaryService cloudinaryService;

        public AdminController(ITokenService tokenService, SqlDbContext dbContext, ICloudinaryService cloudinaryService)
        {
            this.tokenService = tokenService;
            this.dbContext = dbContext;
            this.cloudinaryService = cloudinaryService;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult> Dashboard()
        {
            try
            {
                Guid? userId = HttpContext.Items["UserId"] as Guid?;
                var user = await dbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);

                if (user == null) return RedirectToAction("Login", "User");

                return user.Role switch
                {
                    Role.User => RedirectToAction("Dashboard", "User"),
                    Role.StoreKeeper => RedirectToAction("Dashboard", "StoreKeeper"),
                    Role.Admin => await AdminDashboardAsync(),
                    _ => RedirectToAction("Login", "User"),
                };
            }
            catch (System.Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View("Error");
            }
        }

        private async Task<ActionResult> AdminDashboardAsync()
        {
            ViewBag.TotalProducts = await dbContext.Products.CountAsync();
            ViewBag.TotalOrders = await dbContext.Orders.CountAsync();
            ViewBag.TotalUsers = await dbContext.Users.CountAsync();
            return View("Dashboard");
        }

        // ---------- CREATE ----------
        [Authorize]
        [HttpGet]
        public ActionResult CreateProduct()
        {
            ViewBag.CategoryList = new SelectList(Enum.GetValues(typeof(ProductCategory)));
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> CreateProduct(Product product, IFormFile image)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    // ViewBag.CategoryList = new SelectList(Enum.GetValues(typeof(ProductCategory)));
                    ViewBag.ErrorMessage = "All required fields must be filled.";
                    return View(product);
                }

                if (image == null || image.Length == 0)
                {
                    ViewBag.CategoryList = new SelectList(Enum.GetValues(typeof(ProductCategory)));
                    ViewBag.ErrorMessage = "Please upload an image.";
                    return View(product);
                }

                product.ProductImage = await cloudinaryService.UploadImageAsync(image);
                await dbContext.Products.AddAsync(product);
                await dbContext.SaveChangesAsync();

                TempData["SuccessMessage"] = "Product created successfully!";
                return RedirectToAction("ProductList");
            }
            catch (System.Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View("Error");
            }
        }

        // ---------- READ (LIST) ----------
        [Authorize]
        [HttpGet]
        public async Task<ActionResult> ProductList()
        {
            var products = await dbContext.Products.ToListAsync();
            return View(products);
        }

        // ---------- UPDATE ----------
        [Authorize]
        [HttpGet]
        public async Task<ActionResult> EditProduct(Guid id)
        {
            var product = await dbContext.Products.FindAsync(id);
            if (product == null) return NotFound();

            ViewBag.CategoryList = new SelectList(Enum.GetValues(typeof(ProductCategory)), product.Category);
            return View(product);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> EditProduct(Product product, IFormFile? image)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.CategoryList = new SelectList(Enum.GetValues(typeof(ProductCategory)), product.Category);
                return View(product);
            }

            var dbProduct = await dbContext.Products.FindAsync(product.ProductId);
            if (dbProduct == null) return NotFound();

            dbProduct.ProductName = product.ProductName;
            dbProduct.ProductPrice = product.ProductPrice; // Fixed typo
            dbProduct.Category = product.Category;
            dbProduct.ProductDescription = product.ProductDescription;

            if (image != null && image.Length > 0)
            {
                dbProduct.ProductImage = await cloudinaryService.UploadImageAsync(image);
            }

            await dbContext.SaveChangesAsync();
            TempData["SuccessMessage"] = "Product updated successfully!";
            return RedirectToAction("ProductList");
        }

        // ---------- DELETE ----------
        [Authorize]
        [HttpPost]
        public async Task<ActionResult> DeleteProduct(Guid id)
        {
            var product = await dbContext.Products.FindAsync(id);
            if (product == null) return NotFound();

            dbContext.Products.Remove(product);
            await dbContext.SaveChangesAsync();

            TempData["SuccessMessage"] = "Product deleted successfully!";
            return RedirectToAction("ProductList");
        }
    }
}
