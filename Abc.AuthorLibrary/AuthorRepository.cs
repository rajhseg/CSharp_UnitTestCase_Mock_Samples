using ABC.BusinessBase;
using ABC.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abc.AuthorLibrary
{
    public class AuthorRepository : Repository<Author>, IAuthorRepository
    {
        private readonly DbConnectionProvider _connectionProvider;

        public AuthorRepository(DbContext context, DbConnectionProvider connectionProvider): base(context)
        {
            this._connectionProvider = connectionProvider;
        }

        public async Task UpdateAuthorName(int id, string name)
        {
            var data = await GetById(id);
            if (data != null)
            {
                data.Name = name;
                await Update(data);
            }

            throw new Exception("Not valid");
        }

        public async Task<IEnumerable<Author>> GetAllWithBooks()
        {
            AbcContext? context = this._context as AbcContext;

            if (context == null)
                return Enumerable.Empty<Author>();

            return await context.Authors.Include(x => x.Books).ToListAsync();
        }

        public async Task<int> GetPersonId(Guid uniqueId)
        {
            int personId = 0;

            using (var conn = this._connectionProvider.GetConnection())
            {
                conn.Open();

                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "Exec GetAll";
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    var guidParam = cmd.CreateParameter();
                    guidParam.ParameterName = "@guid";
                    guidParam.Value = uniqueId;
                    guidParam.DbType = System.Data.DbType.Guid;
                    cmd.Parameters.Add(guidParam);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            personId = int.Parse(reader["Id"].ToString());
                        }
                    }
                }
            }

            return personId;
        }
    }
}
