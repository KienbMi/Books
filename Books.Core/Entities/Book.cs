using Books.Core.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Books.Core.Entities
{
    public class Book : EntityObject, IValidatableObject
    {

        public ICollection<BookAuthor> BookAuthors { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Titel muss definiert sein")]
        [MaxLength(200, ErrorMessage = "Titel darf maximal 200 Zeichen lang sein")]
        public string Title { get; set; }

        [Required, MaxLength(100)]
        public string Publishers { get; set; }

        [IsbnValidation]
        [Required, MaxLength(13)]
        public string Isbn { get; set; }

        public Book()
        {
            BookAuthors = new List<BookAuthor>();
        }

        /// <summary>
        /// Eine gültige ISBN-Nummer besteht aus den Ziffern 0, ... , 9,
        /// 'x' oder 'X' (nur an der letzten Stelle)
        /// Die Gesamtlänge der ISBN beträgt 10 Zeichen.
        /// Für die Ermittlung der Prüfsumme werden die Ziffern 
        /// von rechts nach links mit 1 - 10 multipliziert und die 
        /// Produkte aufsummiert. Ist das rechte Zeichen ein x oder X
        /// wird als Zahlenwert 10 verwendet.
        /// Die Prüfsumme muss modulo 11 0 ergeben.
        /// </summary>
        /// <returns>Prüfergebnis</returns>
        public static bool CheckIsbn(string isbn)
        {
            if (isbn == null)
            {
                return false;
                throw new ArgumentNullException(nameof(isbn));
            }

            if (isbn.Length != 10)
            {
                return false;
                throw new Exception("Isbn has not 10 char");
            }

            int sum = 0;

            for (int i = 0; i < isbn.Length; i++)
            {
                int digit = isbn[i] - '0';

                if (i == 9 && char.ToLower(isbn[i]).Equals('x'))
                {
                    sum += 10 * (10 - i); 
                }
                else if (0 <= digit && digit <= 9)
                {
                    sum += digit * (10 - i);
                }
                else
                {
                    return false;
                    throw new Exception("Digit is not between 0 and 9");
                }
            }

            return (sum % 11) == 0;
        }


        public override string ToString()
        {
            return $"{Title} {Isbn} mit {BookAuthors.Count()} Autoren";
        }

        /// <inheritdoc />
        /// <summary>
        /// Jedes Buch muss zumindest einen Autor haben.
        /// Weiters darf ein Autor einem Buch nicht mehrfach zugewiesen
        /// werden.
        /// </summary>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (BookAuthors.Count == 0)
            {
                yield return new ValidationResult("Book must have at least one author", new string[] { nameof(BookAuthors) });
            }
            else if (BookAuthors.Count > 1)
            {
                yield return new ValidationResult($"{BookAuthors.ElementAt(1).Author.Name} are twice authors of the book", new string[] { nameof(BookAuthors) });
            }
            else
            {
                yield return ValidationResult.Success;
            }
        }
    }
}

