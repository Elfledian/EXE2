using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FastWork.GenericRepository
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<TEntity> Repository<TEntity>()
            where TEntity : class;

        Task<int> Complete();
        int CompleteV2();
        Task<int> SaveChangesAsync();
    }
}
