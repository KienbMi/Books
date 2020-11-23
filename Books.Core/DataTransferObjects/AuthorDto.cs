using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Books.Core.Entities;

namespace Books.Core.DataTransferObjects
{
    public class AuthorDto
    {
        public int Id { get; set; }

        public string Author { get; set; }

        public int BookCount      
            => (Books == null) ? 0 : Books.Count;       

        public List<Book> Books { get; set; }

        public string Publishers
        {
            get
            {
                if (Books == null)
                {
                    return string.Empty;
                }
                else
                {
                    StringBuilder sb = new StringBuilder();

                    var publishers = Books.GroupBy(b => b.Publishers)
                        .Select(grp => new
                        {
                            Name = grp.Key,
                            Count = grp.Count()
                        });

                    foreach (var publisher in publishers)
                    {
                        if (sb.Length > 0)
                        {
                            sb.Append(" ,");
                        }
                        sb.Append($"{publisher.Name} ({publisher.Count})");
                    }
                    return sb.ToString();
                }
            }
        }
    }
}
