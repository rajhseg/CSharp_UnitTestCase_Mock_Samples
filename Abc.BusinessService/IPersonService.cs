using ABC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abc.BusinessService
{
	public interface IPersonService
	{
		Task AddPerson(Person person);

		Task<Person> GetPerson(int id);

		Task<IEnumerable<Person>> GetAll();

		Task DeletePerson(int id);

		Task UpdatePerson(Person person);
	}
}
