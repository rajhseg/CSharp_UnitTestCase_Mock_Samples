using Abc.AuthorLibrary;
using Abc.UnitOfWorkLibrary;
using ABC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abc.BusinessService
{
	public class PersonService : IPersonService
	{
		private readonly IPersonRepository _personRepository;		
		private readonly IGenericTransactionService _transactionService;

		public PersonService(IPersonRepository personRepository, IGenericTransactionService transactionService)
		{
			this._personRepository = personRepository;			
			this._transactionService = transactionService;
		}

		public async Task AddPerson(Person person)
		{            
			await _transactionService.ProcessTask(async () => { 
				await this._personRepository.Add(person); 
			});
		}

		public async Task DeletePerson(int id)
		{
			Person per = await this._personRepository.GetById(id);

			if (per != null)
			{
				await _transactionService.ProcessTask(async () => { 
					await this._personRepository.Delete(per);
                });
            }
		}

		public async Task<IEnumerable<Person>> GetAll()
		{
			IEnumerable<Person> allPersons = Enumerable.Empty<Person>();

			await _transactionService.ProcessTask(async () =>
			{
                allPersons = await this._personRepository.GetAll();
            });

			return allPersons;
		}

		public async Task<Person> GetPerson(int id)
		{
			Person _person = null;

			await _transactionService.ProcessTask(async () =>
			{
                _person = await this._personRepository.GetById(id);
            });

			return _person;
		}

		public async Task UpdatePerson(Person person)
		{
			await _transactionService.ProcessTask(async () =>
			{
                await this._personRepository.Update(person);
            });			
		}

		public async Task UpdatePer(Person per)
		{

		}
	}
}
