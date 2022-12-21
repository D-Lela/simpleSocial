using SimpleSocial.DataAccess.Data;
using SimpleSocial.DataAccess.Repository.IRepository;
using SimpleSocial.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSocial.DataAccess.Repository
{
    public class ApplicationUserRepository : Repository<ApplicationUser> ,IApplicationUserRepository
    {
        private ApplicationDbContext _db;

        public ApplicationUserRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(ApplicationUser appUser)
        {
            var objFromDb = _db.ApplicationUsers.FirstOrDefault(u => u.Id == appUser.Id);
            if (objFromDb != null)
            {
                objFromDb.Name = appUser.Name;
                //objFromDb.Description = appUser.Description;
                //if (appUser.ImageUrl != null)
                //{
                //    objFromDb.ImageUrl = obj.ImageUrl;
                //}
            }
        }
        public void UpdateNoOfPosts(ApplicationUser appUser)
        {
            var objFromDb = _db.ApplicationUsers.FirstOrDefault(u => u.Id == appUser.Id);
            if (objFromDb != null)
            {
                objFromDb.NoOfPosts++;
            }
        }
    }
}
