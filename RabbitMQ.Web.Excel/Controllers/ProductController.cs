using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Web.Excel.Models;
using RabbitMQ.Web.Excel.Services;
using RabbitMQ_Common;

namespace RabbitMQ.Web.Excel.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private UserManager<IdentityUser> _userManager;
        private readonly AppDbContext _context;
        private readonly RabbitMQProducer _rabbitMQProducer;
        public ProductController(UserManager<IdentityUser> userManager, AppDbContext context, RabbitMQProducer rabbitMQProducer)
        {
            _userManager = userManager;
            _context = context;
            _rabbitMQProducer = rabbitMQProducer;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> CreateProductExcel()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            var fileName = $"product-excel-{Guid.NewGuid().ToString().Substring(1, 10)}";

            UserFile userFile = new()
            {
                UserId = user.Id,
                FileName = fileName,
                FileStatus = FileStatus.Creating

            };
            await _context.AddAsync(userFile);
            await _context.SaveChangesAsync();

            _rabbitMQProducer.Publish(new CreateExcelMessage
            {
                FileId = userFile.Id
            });
            TempData["StartCreatingExcel"] = true;

            return RedirectToAction(nameof(Files));

        }
        public async Task<IActionResult> Files()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            var userFiles = await _context.UserFiles.Where(x => x.UserId == user.Id).OrderByDescending(x=>x.Id).ToListAsync();

            return View(userFiles);
        }
    }
}
