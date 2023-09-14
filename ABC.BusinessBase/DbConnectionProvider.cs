using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABC.BusinessBase
{
    public class DbConnectionProvider 
    {
        private readonly DbContext _dbContext;

        public DbConnectionProvider(DbContext context)
        {
            this._dbContext = context;
        }

        public IDbConnection GetConnection()
        {
            var ef = GetProvider();
            if (ef != null && ef.IsInMemory)
            {
                return ef.Provider;
            }
            else
            {
                return this._dbContext.Database.GetDbConnection();
            }
        }

        public virtual EntityContextProvider? GetProvider()
        {
            return null;
        }
    }

    public class EntityContextProvider
    {
        public IDbConnection Provider { get; set; }

        public bool IsInMemory { get; set; }

    }
}
