using Books.Core.Entities;
using Books.Core.Validations;
using Books.Persistence;
using Books.Wpf.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Books.Wpf.ViewModels
{
    public class BookEditCreateViewModel : BaseViewModel
    {
        private Book _book;
        private string _title;
        private string _isbn;
        private ObservableCollection<Author> _allAuthors;
        private ObservableCollection<string> _allPublishers;
        private ObservableCollection<BookAuthor> _bookAuthors;
        private string _selectedAuthor;
        private string _selectedPublisher;

        public string Title
        {
            get => _title;
            set 
            {
                _title = value;
                OnPropertyChanged();
                Validate();
            }
        }

        public string Isbn
        {
            get => _isbn;
            set 
            { 
                _isbn = value;
                OnPropertyChanged();
                Validate();
            }
        }

        public ObservableCollection<Author> AllAuthors 
        {
            get => _allAuthors; 
            set
            {
                _allAuthors = value;
                OnPropertyChanged();
            }
        }

        public string SelectedAuthor
        {
            get => _selectedAuthor;
            set 
            { 
                _selectedAuthor = value;
                OnPropertyChanged();
                Validate();
            }
        }

        public ObservableCollection<BookAuthor> BookAuthors
        {
            get => _bookAuthors;
            set 
            { 
                _bookAuthors = value;
                OnPropertyChanged();
                Validate();
            }
        }


        public ObservableCollection<string> AllPublishers
        {
            get => _allPublishers;
            set 
            { 
                _allPublishers = value;
                OnPropertyChanged();
            }
        }

        public string SelectedPublisher
        {
            get => _selectedPublisher;
            set 
            { 
                _selectedPublisher = value;
                OnPropertyChanged();
                Validate();
            }
        }

        public string WindowTitle { get; set; }

        public BookEditCreateViewModel() : base(null)
        {
        }

        public BookEditCreateViewModel(IWindowController windowController, Book book) : base(windowController)
        {
            _book = book;
            LoadCommands();
        }

        public static async Task<BaseViewModel> Create(IWindowController controller, Book book)
        {
            var model = new BookEditCreateViewModel(controller, book);
            await model.LoadDataAsync();
            return model;
        }

        private async Task LoadDataAsync()
        {
            await using UnitOfWork uow = new UnitOfWork();
            //var bookInDb = await uow.Books.GetByIdAsync(_book.Id);

            var allAuthorsInDb = await uow.Authors.GetAllAsync();
            AllAuthors = new ObservableCollection<Author>(allAuthorsInDb);

            var allPublishersInDb = await uow.Books.GetAllPublishersAsync();
            AllPublishers = new ObservableCollection<string>(allPublishersInDb);

            Title = _book.Title;
            Isbn = _book.Isbn;
            SelectedPublisher = _book.Publishers;
            BookAuthors = new ObservableCollection<BookAuthor>(_book.BookAuthors);
        }

        private void LoadCommands()
        {

        }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext = null)
        {
            yield return ValidationResult.Success;
        }
    }
}
