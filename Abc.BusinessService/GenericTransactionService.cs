using Abc.UnitOfWorkLibrary;
using ABC.BusinessBase;
using ABC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abc.BusinessService
{
	public class GenericTransactionService : IGenericTransactionService
	{
        private readonly IUnitOfWork _unitOfWork;

        public GenericTransactionService(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

		public async Task ProcessTask(Func<Task> method)
		{
            using (var transaction = await this._unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    await method();
                    await this._unitOfWork.CommitTransactionAsync(transaction);
                }
                catch (Exception ex)
                {
                    await this._unitOfWork.RollbackTransactionAsync(transaction);
                }
                finally
                {
                    await transaction.DisposeAsync();
                }
            }
        }
	}
}
