using ABC.BusinessBase;
using ABC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Abc.AuthorLibrary
{
	public interface IPersonRepository : IReposistory<Person>
	{
		IEnumerable<Person> GetPersonByQuery(Expression<Func<Person, bool>> expression);		
	}
}
