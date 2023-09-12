using Abc.BusinessService;
using Abc.UnitOfWorkLibrary;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using WebBlazorApp.Models;

namespace WebBlazorApp.Pages.Components.BookPage
{
    [Authorize(Roles ="User, Admin")]
	public partial class ListBook
	{
        [Inject]
        IBookService _bookService { get; set; }

        [Inject]
        IUnitOfWork unitOfWork { get; set; }

        [Inject]
        NavigationManager navigationManager { get; set; }

        private IEnumerable<BookModel> Books { get; set; }

        private ModelPopup modelObj { get; set; }

        private int BookId { get; set; } = 0;

        
        private bool isClickedEdit = false;


        protected override Task OnInitializedAsync()
        {
            LoadData();
            return base.OnInitializedAsync();
        }

        private async Task LoadThisComponent()
        {
            BookId = 0;
            await LoadData();       
        }

        private async Task LoadData()
        {
            var booksObj = await this._bookService.GetAllBooks();

            List<BookModel> models = new List<BookModel>();

            foreach (var item in booksObj)
            {
                var _book = new BookModel
                {
                    Id = item.Id,
                    Title = item.Title,
                    Description = item.Description,
                    AuthorName = item.Author.Name,
                    AuthorId = item.AuthorId
                };
                models.Add(_book);
            }

            this.Books = models;

            StateHasChanged();
        }

        private void AddBook()
        {
            navigationManager.NavigateTo("/book/addbook");
        }

        private void rowClicked(int id)
        {
            if (isClickedEdit)
            {
                isClickedEdit = false;
                return;
            }

            BookId = id;
            StateHasChanged();
        }

        private void EditBookData(int id)
        { 
           isClickedEdit = true;                
           this.navigationManager.NavigateTo($"/book/edit/{id}");
        }

        private void DeleteData<T>(DeleteModel<T> bookModal)
        {
            this.modelObj.ShowModal(bookModal.Model, bookModal.PopupHeader, bookModal.PopupMessage);
        }

        private async Task ModalCallbackMethod(ModalObj obj)
        {
            if (obj.IsConfirm)
            {
                BookModel bookObj = obj.Data as BookModel;
                if (bookObj != null)
                {                   
                    try
                    {
                        await this._bookService.DeleteBook(bookObj.Id);                         
                        await LoadData();
                    }
                    catch (Exception)
                    {                           
                        throw;
                    }                                          
                }
            }
        }
    }
}
