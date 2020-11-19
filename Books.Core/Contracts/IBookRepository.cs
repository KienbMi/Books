using Books.Core.Entities;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Books.Core.Contracts
{
    public interface IBookRepository
    {
        Task AddRangeAsync(IEnumerable<Book> books);
        Task<IEnumerable<Book>> GetAllAsync();
        Task<IEnumerable<Book>> GetWithFilterAsync(string filterText);
        void Delete(Book book);
        Task<Book> GetByIdAsync(int bookId);
        Task<IEnumerable<string>> GetAllPublishersAsync();
        void SetModified(Book book);
        void Add(Book book);
    }
}