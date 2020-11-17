using Books.Core.Contracts;
using Books.Core.Entities;
using Books.Persistence;
using Books.Wpf.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Books.Wpf.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {

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

        

        
        
        public MainWindowViewModel() : base(null)
        {
        }

        public MainWindowViewModel(IWindowController windowController) : base(windowController)
        {
            LoadCommands();
        }

        private void LoadCommands()
        {
        }



        /// <summary>
        /// Lädt die gefilterten Buchdaten
        /// </summary>
        public async Task LoadBooks()
        {
            await using IUnitOfWork uow = new UnitOfWork();
            var books = await uow.Books.GetAllAsync();
            Books = new ObservableCollection<Book>(books);
        }

        public static async Task<BaseViewModel> Create(IWindowController controller)
        {
            var model = new MainWindowViewModel(controller);
            await model.LoadBooks();
            return model;
        }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            throw new NotImplementedException();
        }
    }
}
