using Books.Core.Contracts;
using Books.Core.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Books.Core.DataTransferObjects;
using Microsoft.EntityFrameworkCore;

namespace Books.Persistence
{
    public class AuthorRepository : IAuthorRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public AuthorRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Add(Author author)
            => _dbContext.Add(author);

        public async Task<IEnumerable<Author>> GetAllAsync()
            => await _dbContext.Authors
                    .OrderBy(a => a.Name)
                    .ToArrayAsync();

        public async Task<IEnumerable<AuthorDto>> GetAllDtosAsync()
            => await _dbContext.Authors
                    .OrderBy(a => a.Name)
                    .Select(a => new AuthorDto
                    {
                        Id = a.Id,
                        Author = a.Name,
                        Books = a.BookAuthors.Select(ba => ba.Book).ToList()
                    })
                    .ToArrayAsync();

        public async Task<AuthorDto> GetDtoByIdAsync(int authorId)
            => await _dbContext.Authors
                    .Where(a => a.Id == authorId)
                    .Select(a => new AuthorDto
                    {
                        Id = a.Id,
                        Author = a.Name,
                        Books = a.BookAuthors.Select(ba => ba.Book).ToList()
                    })
                    .SingleOrDefaultAsync();
    }
}