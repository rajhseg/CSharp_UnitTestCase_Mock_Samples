using ABC.BusinessBase;
using ABC.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace Abc.AuthorLibrary
{
	public class PersonRepository : Repository<Person>, IPersonRepository
	{
		public PersonRepository(DbContext context) : base(context)
		{

		}

		public IEnumerable<Person> GetPersonByQuery(Expression<Func<Person, bool>> expression)
		{
			if (expression != null && this._context != null)
			{
				AbcContext abcContext = this._context as AbcContext;

				if(abcContext != null)
				{
					return abcContext.Persons.Where(expression);
				}
			}

			return Enumerable.Empty<Person>();
		}
	}
}
