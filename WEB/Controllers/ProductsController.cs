using BLL.QueueServices;
using DAL.Context;
using DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace WEB.Controllers
{
    [Authorize]
    public class ProductsController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly AppDbContext _appDbContext;
        private readonly RabbitMQClientPublisher _rabbitmqClientPublisher;

        public ProductsController(UserManager<AppUser> userManager, AppDbContext appDbContext, RabbitMQClientPublisher rabbitmqClientPublisher)
        {
            _userManager = userManager;
            _appDbContext = appDbContext;
            _rabbitmqClientPublisher= rabbitmqClientPublisher;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CreateProductExcel()
        {
            var user = _userManager.FindByNameAsync(User.Identity.Name).Result;

            var uniq = Guid.NewGuid().ToString();
            var publicPath = $"/_uploads/product-excel/{uniq}.xlsx";
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "_uploads", "product-excel", $"{uniq}.xlsx");

            var userFile = new UserFile
            {
                AddedDate= DateTime.Now,
                FileName= $"{uniq}.xlsx",
                PublicPath= publicPath,
                FilePath= filePath,
                UserId = user.Id
            };

            var inserted = _appDbContext.UserFiles.Add(userFile);

            if (_appDbContext.SaveChanges() > 0)
            {
                var msg = JsonConvert.SerializeObject(inserted.Entity, new JsonSerializerSettings { 
                    ReferenceLoopHandling= ReferenceLoopHandling.Ignore,
                });
                _rabbitmqClientPublisher.DirectPublish(msg, "excel-operations", "excel-direct", "excel-products");
                TempData["StartCreatingExcel"] = true;
            }
            else
            {
                TempData["StartCreatingExcel"] = false;
            }

            return RedirectToAction("Files");
        }

        public IActionResult Files()
        {
            var user = _userManager.FindByNameAsync(User.Identity.Name).Result;

            var files = _appDbContext.UserFiles.Where(x => x.UserId == user.Id).OrderByDescending(x => x.Id).ToList();

            return View(files);
        }
    }
}
