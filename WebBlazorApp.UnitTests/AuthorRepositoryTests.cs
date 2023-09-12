using Abc.AuthorLibrary;
using ABC.BusinessBase;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace WebBlazorApp.UnitTests
{
    public class AuthorRepositoryTests
    {
        private readonly DbContextOptions<AbcContext> options;

        public AuthorRepositoryTests()
        {
            options = new DbContextOptionsBuilder<AbcContext>().UseInMemoryDatabase(databaseName: "sample").Options;
        }

        [Fact]
        public async Task AddEntity_Test()
        {
            using(var context = new AbcContext(options))
            {
                AuthorRepository repo = new AuthorRepository(context);
                await repo.Add(new ABC.Models.Author { Id = 1, Name = "ABC" });
                await context.SaveChangesAsync();                

                Assert.Equal(1, context.Authors.Where(x=>x.Id==1).Count());
            }
        }

        [Fact]
        public async Task UpdateEntity_Test()
        {
            using(var context = new AbcContext(options))
            {
                AuthorRepository repo = new AuthorRepository(context);

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
            using(var context = new AbcContext(options))
            {
                AuthorRepository repo = new AuthorRepository(context);
                await repo.Add(new ABC.Models.Author { Id = 4, Name = "ABC" });
                await context.SaveChangesAsync();

                var ele = await repo.GetById(4);
                await repo.Delete(ele);
                await context.SaveChangesAsync();

                Assert.Equal(0, context.Authors.Where(x=>x.Id==4).Count());
            }
        }

        [Fact]
        public async Task GetDataEntity_Test()
        {
            using(var context = new AbcContext(options))
            {
                AuthorRepository repo = new AuthorRepository(context);
                await repo.Add(new ABC.Models.Author { Id = 2, Name = "ABC" });
                await context.SaveChangesAsync();

                var result = (await repo.GetData(x => x.Id == 2)).ToList();

                Assert.Single(result);
            }
        }
    }
}
