using BookStore.Models;

namespace BookStore.DataAccess.Repository.IRepository
{
    public interface IBookRepository : IRepository<Book>
    {
        void Update(Book obj);
        void Save();
    }
}
