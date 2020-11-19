using Books.Core.Contracts;
using Books.Core.Entities;
using Books.Persistence;
using Books.Wpf.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Books.Wpf.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        private string _filterText;
        private Book _selectedBook;
        private ObservableCollection<Book> _books;

        public ObservableCollection<Book> Books
        {
            get => _books;
            set
            {
                _books = value;
                OnPropertyChanged();
            }
        }

        public string FilterText
        {
            get => _filterText;
            set
            {
                _filterText = value;
                OnPropertyChanged();
                _ = LoadBooksAsync();
            }
        }

        public Book SelectedBook
        {
            get => _selectedBook;
            set
            {
                _selectedBook = value;
                OnPropertyChanged();
            }
        }

        public MainWindowViewModel() : base(null)
        {
        }

        public MainWindowViewModel(IWindowController windowController) : base(windowController)
        {
            LoadCommands();
        }

        private void LoadCommands()
        {
            CmdNewBook = new RelayCommand(async _ => await NewBookAsync(), _ => true);
            CmdEditBook = new RelayCommand(async _ => await EditBookAsync(), _ => SelectedBook != null);
            CmdDeleteBook = new RelayCommand(async _ => await DeleteBookAsync(), _ => SelectedBook != null);
        }

        private async Task DeleteBookAsync()
        {
            await using UnitOfWork uow = new UnitOfWork();
            uow.Books.Delete(SelectedBook);
            await uow.SaveChangesAsync();
            await LoadBooksAsync();
        }

        private async Task EditBookAsync()
        {
            var window = await BookEditCreateViewModel.Create(Controller, SelectedBook);
            Controller.ShowWindow(window, true);
            await LoadBooksAsync();
        }

        private async Task NewBookAsync()
        {
            var window = await BookEditCreateViewModel.Create(Controller, null);
            Controller.ShowWindow(window, true);
            await LoadBooksAsync();
        }   

        /// <summary>
        /// Lädt die gefilterten Buchdaten
        /// </summary>
        public async Task LoadBooksAsync()
        {
            await using IUnitOfWork uow = new UnitOfWork();
            var books = await uow.Books.GetWithFilterAsync(FilterText);
            Books = new ObservableCollection<Book>(books);
        }

        public static async Task<BaseViewModel> Create(IWindowController controller)
        {
            var model = new MainWindowViewModel(controller);
            await model.LoadBooksAsync();
            return model;
        }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            throw new NotImplementedException();
        }

        // Commands
        public ICommand CmdNewBook { get; set; }
        public ICommand CmdEditBook { get; set; }
        public ICommand CmdDeleteBook { get; set; }
    }
}
