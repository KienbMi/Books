﻿using Books.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Utils;

namespace Books.ImportConsole
{
    public static class ImportController
    {
        const string Filename = "books.csv";

        const int Idx_Author = 0;
        const int Idx_Title = 1;
        const int Idx_Publishes = 2;
        const int Idx_Isbn = 3;
        
        public static IEnumerable<Book> ReadBooksFromCsv()
        {
            string[][] matrix = MyFile.ReadStringMatrixFromCsv(Filename, false);

            //Twain, Mark; 1,000,000 Pound Bank Note: For a Dear Friend; Ebury Press; 91783992
            //Random House Disney; 101 Dalmatians; Disney Press; 736421459

            var authors = matrix
                .GroupBy(_ => _[Idx_Author])
                .Select(grp => new Author 
                { 
                    Name = grp.First()[Idx_Author]            
                })
                .ToDictionary(_ => _.Name);

            var books = matrix
                .Select(b => new Book
                {
                    Isbn = b[Idx_Isbn],
                    Publishers = b[Idx_Publishes],
                    Title = b[Idx_Title]
                })
                .ToDictionary(_ => _.Isbn);

            var bookAuthor = matrix
                .Select(b => new BookAuthor
                {
                    Author = authors[b[Idx_Author]],
                    Book = books[b[Idx_Isbn]]
                });

            return books.Values;
        }
    }
}