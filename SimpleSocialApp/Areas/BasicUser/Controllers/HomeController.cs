using Microsoft.AspNetCore.Mvc;
using SimpleSocial.DataAccess.Repository.IRepository;
using SimpleSocial.Models;
using SimpleSocialApp.Models;
using System.Diagnostics;

namespace SimpleSocialApp.Areas.User.Controllers
{
    [Area("BasicUser")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Post> allPosts = _unitOfWork.Post.GetAll(includeProperties: "ApplicationUser");
            return View(allPosts);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}