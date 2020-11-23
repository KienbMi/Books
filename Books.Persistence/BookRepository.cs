using Books.Core.Contracts;
using Books.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Books.Persistence
{
    public class BookRepository : IBookRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public BookRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Add(Book book)
            => _dbContext.Add(book);

        public async Task AddRangeAsync(IEnumerable<Book> books)
            => await _dbContext.AddRangeAsync(books);

        public void Delete(Book book)
            => _dbContext.Books.Remove(book);

        public async Task<IEnumerable<Book>> GetAllAsync()
            => await _dbContext.Books
                        .Include(_ => _.BookAuthors)
                        .ThenInclude(_ => _.Author)
                        .OrderBy(_ => _.Title)
                        .ToArrayAsync();

        public async Task<IEnumerable<string>> GetAllPublishersAsync()
            => await _dbContext.Books
                        .Select(b => b.Publishers)
                        .OrderBy(_ => _)
                        .Distinct()
                        .ToArrayAsync();

        public async Task<Book> GetByIdAsync(int bookId)
            => await _dbContext.Books
                        .Where(b => b.Id == bookId)
                        .Include(_ => _.BookAuthors)
                        .SingleOrDefaultAsync();

        public async Task<IEnumerable<Book>> GetByTitleAsync(string title)
            => await _dbContext.Books
                .Include(_ => _.BookAuthors)
                .ThenInclude(_ => _.Author)
                .Where(b => b.Title.Contains(title))
                .ToArrayAsync();

        public async Task<IEnumerable<Book>> GetWithFilterAsync(string filterText)          
            => await _dbContext.Books
                        .Where(b => EF.Functions.Like(b.Title, $"%{filterText}%"))
                        .Include(_ => _.BookAuthors)
                        .ThenInclude(_ => _.Author)
                        .OrderBy(_ => _.Title)
                        .ToArrayAsync();

        public void SetModified(Book book)
        {
            _dbContext.Entry(book).State = EntityState.Modified;
        }
    }
}