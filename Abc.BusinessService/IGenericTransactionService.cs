using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abc.BusinessService
{
	public interface IGenericTransactionService
	{
        Task ProcessTask(Func<Task> method);
    }
}
