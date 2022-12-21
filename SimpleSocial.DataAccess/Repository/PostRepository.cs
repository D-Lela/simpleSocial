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
    public class PostRepository : Repository<Post>, IPostRepository
    {
        private ApplicationDbContext _db;
        public PostRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Post obj)
        {
            var objFromDb = _db.Posts.FirstOrDefault(u => u.Id == obj.Id);
            if (objFromDb != null) 
            {
                objFromDb.Title = obj.Title;
                objFromDb.Description = obj.Description;
                if (obj.ImageUrl != null) 
                {
                    objFromDb.ImageUrl = obj.ImageUrl;
                }
            }
        }
    }
}
