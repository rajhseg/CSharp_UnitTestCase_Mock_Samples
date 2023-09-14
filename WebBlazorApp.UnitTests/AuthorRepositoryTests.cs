using Abc.AuthorLibrary;
using ABC.BusinessBase;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace WebBlazorApp.UnitTests
{
    public class AuthorRepositoryTests
    {
        private bool isSqlite = false;

        public AuthorRepositoryTests()
        {

        }

        private AbcContext GetContext()
        {
            DbContextOptions<AbcContext> options;
            var builder = new DbContextOptionsBuilder<AbcContext>();

            /// Sqlite supports query, not stored procedure

            if (isSqlite)
            {
                options = builder.UseSqlite("DataSource=:memory:").Options;
            }
            else
            {
                options = builder.UseInMemoryDatabase(databaseName: "sample").Options;
            }

            var context = new AbcContext(options);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            return context;
        }

        private Mock<AbcContext> GetMockContext()
        {
            DbContextOptions<AbcContext> options;
            var builder = new DbContextOptionsBuilder<AbcContext>();
            options = builder.UseInMemoryDatabase(databaseName: "sample").Options;

            return new Mock<AbcContext>(options);
        }

        private void UseSqlQuery()
        {
            isSqlite = true;
        }

        [Fact]
        public async Task AddEntity_Test()
        {
            using (var context = GetContext())
            {
                Mock<DbConnectionProvider> connProvider = new Mock<DbConnectionProvider>(context);
                AuthorRepository repo = new AuthorRepository(context, connProvider.Object);
                await repo.Add(new ABC.Models.Author { Id = 1, Name = "ABC" });
                await context.SaveChangesAsync();

                Assert.Equal(1, context.Authors.Where(x => x.Id == 1).Count());
            }
        }

        [Fact]
        public async Task UpdateEntity_Test()
        {
            using (var context = GetContext())
            {
                Mock<DbConnectionProvider> connProvider = new Mock<DbConnectionProvider>(context);
                AuthorRepository repo = new AuthorRepository(context, connProvider.Object);

                await repo.Add(new ABC.Models.Author { Id = 3, Name = "ABC" });
                await context.SaveChangesAsync();

                var ele = await repo.GetById(3);
                ele.Name = "UpdatedName";

                await repo.Update(ele);
                await context.SaveChangesAsync();

                var _author = context.Authors.First();

                Assert.NotNull(_author);
                Assert.Equal("UpdatedName", _author.Name);
            }
        }

        [Fact]
        public async Task DeleteEntity_Test()
        {
            using (var context = GetContext())
            {
                Mock<DbConnectionProvider> connProvider = new Mock<DbConnectionProvider>(context);
                AuthorRepository repo = new AuthorRepository(context, connProvider.Object);
                await repo.Add(new ABC.Models.Author { Id = 4, Name = "ABC" });
                await context.SaveChangesAsync();

                var ele = await repo.GetById(4);
                await repo.Delete(ele);
                await context.SaveChangesAsync();

                Assert.Equal(0, context.Authors.Where(x => x.Id == 4).Count());
            }
        }

        [Fact]
        public async Task GetDataEntity_Test()
        {
            using (var context = GetContext())
            {
                Mock<DbConnectionProvider> connProvider = new Mock<DbConnectionProvider>(context);
                AuthorRepository repo = new AuthorRepository(context, connProvider.Object);
                await repo.Add(new ABC.Models.Author { Id = 2, Name = "ABC" });
                await context.SaveChangesAsync();

                var result = (await repo.GetData(x => x.Id == 2)).ToList();

                Assert.Single(result);
            }
        }

        [Fact]
        public async Task Mock_EFCore_DBConnection_StoredProcedure_Execute_Test()
        {
            using (var context = GetContext())
            {
                // Create mock for DBConnection
                var connectionMock = new Mock<IDbConnection>();
                connectionMock.Setup(c => c.State).Returns(ConnectionState.Closed); 
                connectionMock.Setup(c => c.Open());

                int rowIndex = 0;
                int expectedRow = 1;

                // Create mock for DataReader
                var readerMock = new Mock<DbDataReader>();
                readerMock.Setup(r => r.Read()).Returns(() =>
                {
                    rowIndex++;
                    return rowIndex <= expectedRow;
                });

                readerMock.Setup(r => r["Id"]).Returns(2);

                var paramMock = new Mock<IDbDataParameter>();
                var parametersMock = new Mock<IDataParameterCollection>();

                // Create a mock for SqlCommand
                var commandMock = new Mock<IDbCommand>();
                commandMock.SetupGet(c => c.Connection).Returns(connectionMock.Object);
                commandMock.Setup(c => c.ExecuteReader()).Returns(readerMock.Object);
                commandMock.Setup(c => c.CreateParameter()).Returns(paramMock.Object);
                commandMock.SetupGet(c => c.Parameters).Returns(parametersMock.Object);

                connectionMock.Setup(c => c.CreateCommand()).Returns(commandMock.Object);

                Mock<DbConnectionProvider> connProvider = new Mock<DbConnectionProvider>(context);

                EntityContextProvider entityContextProvider = new EntityContextProvider();
                entityContextProvider.Provider = connectionMock.Object;
                entityContextProvider.IsInMemory = true;

                connProvider.Setup(x => x.GetProvider()).Returns(entityContextProvider);

                AuthorRepository repo = new AuthorRepository(context, connProvider.Object);

                Guid uniqueId = Guid.NewGuid();
                var result = await repo.GetPersonId(uniqueId);

                Assert.Equal(2, result);
            }
        }    

        private IDbConnection GetDBConnection()
        {

            var mockDataReader = new Mock<IDataReader>();
            mockDataReader.SetupSequence(x => x.Read()).Returns(true).Returns(true).Returns(false);
            mockDataReader.SetupGet(x => x["Id"]).Returns(() => 1);

            var mockParameters = new Mock<IDataParameterCollection>();

            var mockCommand = new Mock<IDbCommand>();
           // mockCommand.SetupGet(x => x.Parameters).Returns(mockParameters.Object);
            mockCommand.Setup(x => x.ExecuteReader(CommandBehavior.CloseConnection)).Returns(mockDataReader.Object);

            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(x => x.CreateCommand()).Returns(mockCommand.Object);

            return mockConnection.Object;

        }
    }
}
