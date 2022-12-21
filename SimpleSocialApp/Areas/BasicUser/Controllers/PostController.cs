using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using SimpleSocial.DataAccess.Repository;
using SimpleSocial.DataAccess.Repository.IRepository;
using SimpleSocial.Models;
using SimpleSocial.Models.ViewModels;
using SimpleSocialApp.Models;
using System.Diagnostics;
using System.Security.Claims;

namespace SimpleSocialApp.Areas.User.Controllers
{
    [Area("BasicUser")]
    public class PostController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;

        public PostController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? id) 
        {
            PostVM postVM = new() {
                Post = new()
            };

            if (id == null || id==0)
            {
                return View(postVM);
            }
            else 
            {
                postVM.Post = _unitOfWork.Post.GetFirstOrDefault(u=>u.Id==id);
                return View(postVM);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]

        public IActionResult Upsert(PostVM obj, IFormFile? file) 
        {
            if (ModelState.IsValid)
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                obj.Post.ApplicationUserId = claim.Value;

                string wwwRootPath = _hostEnvironment.WebRootPath;
                if (file != null) 
                {
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(wwwRootPath, @"images\posts");
                    var extension = Path.GetExtension(file.FileName);
                    if (obj.Post.ImageUrl != null) 
                    {
                        var oldImagePath = Path.Combine(wwwRootPath, obj.Post.ImageUrl.Trim('\\'));                      
                        if (System.IO.File.Exists(oldImagePath)) 
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }              
                    using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension),FileMode.Create)) 
                    {
                        file.CopyTo(fileStreams);
                    }
                    obj.Post.ImageUrl=@"\images\posts\" + fileName + extension;
                   
                }
                if (obj.Post.Id == 0) 
                {
                    TempData["success"] = obj.Post.Title + " posted successfully!";
                    _unitOfWork.Post.Add(obj.Post);
                    _unitOfWork.ApplicationUser.UpdateNoOfPosts(_unitOfWork.ApplicationUser.GetFirstOrDefault(u=>u.Id==obj.Post.ApplicationUserId));
                }
                else
                {
                    TempData["success"] = obj.Post.Title + " updated successfully!";
                    _unitOfWork.Post.Update(obj.Post);
                }               
                _unitOfWork.Save();
                return RedirectToAction("Index");

            }
            return View(obj);
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            var postList = _unitOfWork.Post.GetAll(includeProperties:"ApplicationUser");
            return Json(new {data=postList});
        }

        [HttpDelete]
        public IActionResult Delete(int? id) 
        {
            var obj = _unitOfWork.Post.GetFirstOrDefault(u=>u.Id==id);
            if (obj == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            var oldImagePath = Path.Combine(_hostEnvironment.WebRootPath, obj.ImageUrl.Trim('\\'));
            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }

            TempData["success"] = obj.Title + " deleted successfully!";
            _unitOfWork.Post.Remove(obj);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Delete Successful" });
        }
        #endregion
    }
}