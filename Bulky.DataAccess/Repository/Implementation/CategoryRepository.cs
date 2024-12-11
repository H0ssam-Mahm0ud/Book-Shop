using BookStore.DataAccess.Data;
using BookStore.Models;

namespace BookStore.DataAccess.Repository.Implementation
{
    public class CategoryRepository : Repository<Category>, IRepository.ICategoryRepository
    {
        private ApplicationDbContext _db;
        public CategoryRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Save()
        {
            _db.SaveChanges();
        }

        public void Update(Category obj)
        {
            _db.Categories.Update(obj);
        }
    }
}
