using Abc.AuthorLibrary;
using Abc.BusinessService;
using Abc.UnitOfWorkLibrary;
using ABC.BusinessBase;
using ABC.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace WebBlazorApp.UnitTests
{
    public class AuthorServiceTests
    {
        private readonly IGenericTransactionService transactionService;
        private readonly Mock<IUnitOfWork> unitOfWorkMock;
        private readonly Mock<AuthorRepository> authorRepositoryMock;
        private readonly AuthorService service;
        private readonly AbcContext context;

        public AuthorServiceTests()
        {
            var options = new DbContextOptionsBuilder<AbcContext>().UseInMemoryDatabase(databaseName: "sample")
                .ConfigureWarnings(x=>x.Ignore(InMemoryEventId.TransactionIgnoredWarning)).Options;

            context = new AbcContext(options);
            
            unitOfWorkMock = new Mock<IUnitOfWork>();
            authorRepositoryMock = new Mock<AuthorRepository>(context);
            transactionService = new GenericTransactionService(unitOfWorkMock.Object);
            service = new AuthorService(authorRepositoryMock.Object, transactionService);
        }

        [Fact]
        public async Task AddAuthor_Test()
        {
            unitOfWorkMock.Setup(x => x.BeginTransactionAsync()).ReturnsAsync(context.Database.BeginTransaction());                        

            await service.AddAuthor(new ABC.Models.Author { Id = 11111, Name = "ADF" });
            await context.SaveChangesAsync();
            
            unitOfWorkMock.Verify(x => x.CommitTransactionAsync(It.IsAny<IDbContextTransaction>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAuthor_Test()
        {
            unitOfWorkMock.Setup(x => x.BeginTransactionAsync()).ReturnsAsync(context.Database.BeginTransaction());

            await service.AddAuthor(new ABC.Models.Author { Id = 21111, Name = "ADF" });
            await context.SaveChangesAsync();

            await service.DeleteAuthor(21111);
            await context.SaveChangesAsync();

            unitOfWorkMock.Verify(x => x.CommitTransactionAsync(It.IsAny<IDbContextTransaction>()), Times.AtLeast(2));
        }

        [Fact]
        public async Task UpdateAuthor_Test()
        {
            unitOfWorkMock.Setup(x => x.BeginTransactionAsync()).ReturnsAsync(context.Database.BeginTransaction());

            await service.AddAuthor(new ABC.Models.Author { Id = 31111, Name = "ADF" });
            await context.SaveChangesAsync();

            var author = (await service.GetAuthors(x => x.Id == 31111)).First();
            author.Name = "BCG";

            var updateResult = await service.UpdateAuthor(author);
            await context.SaveChangesAsync();

            Assert.Equal("BCG", updateResult.Name);
            unitOfWorkMock.Verify(x => x.CommitTransactionAsync(It.IsAny<IDbContextTransaction>()), Times.AtLeast(2));
        }
    }
}
