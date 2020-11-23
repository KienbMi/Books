using Books.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using Books.Core.DataTransferObjects;

namespace Books.Core.Contracts
{
    public interface IAuthorRepository
    {
        Task<IEnumerable<Author>> GetAllAsync();
        Task<IEnumerable<AuthorDto>> GetAllDtosAsync();
        Task<AuthorDto> GetDtoByIdAsync(int authorId);
        void Add(Author author);
    }
}