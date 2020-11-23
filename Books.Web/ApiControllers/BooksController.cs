using Books.Core.Contracts;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Books.Web.ApiControllers
{
    /// <summary>
    /// API to managed the Books application.
    /// </summary>
    [Produces("application/json")]
    [Route("api/books")]
    public class BooksController : Controller
    {
        private readonly IUnitOfWork _uow;

        public BooksController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        /// <summary>
        /// Bind books by title.
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        // GET: api/books/<title>
        [Route("{title}")]
        [HttpGet]
        public async Task<IActionResult> GetBooks(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                NotFound();
            }

            var book = (await _uow.Books.GetByTitleAsync(title))
                .Select(b => new
                { 
                    authorNames = b.Authors,
                    title = b.Title,
                    publishers = b.Publishers,
                    isbn = b.Isbn
                });

            if (book == null)
            {
                NotFound();
            }

            return Ok(book);
        }
    }
}
