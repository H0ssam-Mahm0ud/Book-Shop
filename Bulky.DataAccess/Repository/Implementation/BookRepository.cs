using BookStore.DataAccess.Data;
using BookStore.DataAccess.Repository.IRepository;
using BookStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.DataAccess.Repository.Implementation
{
    public class BookRepository : Repository<Book>, IBookRepository
    {
        private ApplicationDbContext _db;
        public BookRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }


        public void Save()
        {
            _db.SaveChanges();
        }

        public void Update(Book book)
        {
            var bookFromDb = _db.Books.FirstOrDefault(u => u.Id == book.Id);
            if (bookFromDb != null)
            {
                bookFromDb.Title = book.Title;
                bookFromDb.ISBN = book.ISBN;
                bookFromDb.Price = book.Price;
                bookFromDb.Price50 = book.Price50;
                bookFromDb.ListPrice = book.ListPrice;
                bookFromDb.Price100 = book.Price100;
                bookFromDb.Description = book.Description;
                bookFromDb.CategoryId = book.CategoryId;
                bookFromDb.Author = book.Author;
                if (book.ImageUrl != null)
                {
                    bookFromDb.ImageUrl = book.ImageUrl;
                }
            }
        }
    }
}
