using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using WebBlazorApp.Models;
using Abc.BusinessService;
using Abc.UnitOfWorkLibrary;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Authorization;
using ABC.Models;

namespace WebBlazorApp.Pages.Components.BookPage
{
    [Authorize(Roles ="User, Admin")]
	public partial class AddBook
	{
        [Inject]
        IUnitOfWork unitOfWork { get; set; }

        [Inject]
        IBookService bookService { get; set; }

        [Inject]
        IAuthorService authorService { get; set; }

        [Inject]
        NavigationManager navigationManager { get; set; }


        public BookModel Book { get; set; }
        
        public IEnumerable<SelectListItem> Authors { get; set; }

        protected override Task OnInitializedAsync()
        {
            LoadData();
            return base.OnInitializedAsync();
        }

        private async Task LoadData()
        {
            var listData = await this.authorService.GetAllAuthors();
            this.Authors = listData.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).ToList();
            this.Book = new BookModel { };
            StateHasChanged();
        }

        private async Task AddAuthor()
        {
            if (Book.Title != "" && Book.Description != "" && Book.AuthorId > 0)
            {
                try
                {
                    await this.bookService.AddBook(new ABC.Models.Book { Title = Book.Title, Description = Book.Description, AuthorId = Book.AuthorId });
                    navigationManager.NavigateTo("/book");
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        private void Back()
        {
            navigationManager.NavigateTo("/book");
        }
    }
}
