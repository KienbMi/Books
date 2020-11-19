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
using System.Windows.Input;

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
        private Author _selectedAuthor;
        private string _selectedPublisher;
        private BookAuthor _selectedBookAuthor;

        [Required(ErrorMessage ="Titel muss angegeben werden")]
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

        public Author SelectedAuthor
        {
            get => _selectedAuthor;
            set
            {
                _selectedAuthor = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<BookAuthor> BookAuthors
        {
            get => _bookAuthors;
            set
            {
                _bookAuthors = value;
                OnPropertyChanged();
            }
        }

        public BookAuthor SelectedBookAuthor
        {
            get => _selectedBookAuthor;
            set
            {
                _selectedBookAuthor = value;
                OnPropertyChanged();
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
            }
        }

        public ICommand CmdAddAuthor { get; set; }
        public ICommand CmdDeleteAuthor { get; set; }
        public ICommand CmdSaveBook { get; set; }

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

            var allAuthorsInDb = await uow.Authors.GetAllAsync();
            AllAuthors = new ObservableCollection<Author>(allAuthorsInDb);

            var allPublishersInDb = await uow.Books.GetAllPublishersAsync();
            AllPublishers = new ObservableCollection<string>(allPublishersInDb);

            _title = _book.Title;
            OnPropertyChanged(nameof(Title));
            _isbn = _book.Isbn;
            OnPropertyChanged(nameof(Isbn));
            Validate();
            SelectedPublisher = _book.Publishers;
            BookAuthors = new ObservableCollection<BookAuthor>(_book.BookAuthors);
        }

        private void LoadCommands()
        {
            CmdAddAuthor = new RelayCommand(_ => AddAuthor(), _ => SelectedAuthor != null);
            CmdDeleteAuthor = new RelayCommand(_ => DeleteAuthor(), _ => SelectedBookAuthor != null);
            CmdSaveBook = new RelayCommand(async _ => await SaveBookAsync(), _ => IsValid);
        }

        private void AddAuthor()
        {
            var newBookAuthor = new BookAuthor
            {
                Book = _book,
                Author = SelectedAuthor
            };
            BookAuthors.Add(newBookAuthor);
            OnPropertyChanged(nameof(BookAuthors));
        }

        private void DeleteAuthor()
        {
            BookAuthors.Remove(SelectedBookAuthor);
            OnPropertyChanged(nameof(BookAuthors));
        }

        private async Task SaveBookAsync()
        {
            await using UnitOfWork uow = new UnitOfWork();

            var bookInDb = await uow.Books.GetByIdAsync(_book.Id);
            bookInDb.Title = Title;
            bookInDb.Publishers = SelectedPublisher;
            bookInDb.Isbn = Isbn;

            bookInDb.BookAuthors.Clear();
            foreach (var bookAuthor in BookAuthors)
            {
                bookInDb.BookAuthors.Add(bookAuthor);
            }
            uow.Books.SetModified(bookInDb);

            try
            {
                await uow.SaveChangesAsync();
                Controller.CloseWindow(this);
            }
            catch (ValidationException validationException)
            {
                if(validationException is IEnumerable<string> properties)
                {
                    foreach (var property in properties)
                    {
                        Errors.Add(property, new List<string> { validationException.ValidationResult.ErrorMessage });
                    }
                }
                else
                {
                    DbError = validationException.ValidationResult.ToString();
                }
            }
        }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            ValidationResult result = new IsbnValidation().GetValidationResult(Isbn, new ValidationContext(Isbn));

            if (result != null && result != ValidationResult.Success)
            {
                yield return new ValidationResult($"Isbn {Isbn} ist ungültig", new string[] { nameof(Isbn) });
            }
        }
    }
}
