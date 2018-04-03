using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    interface IRepository<TCreate,TDetail,TParameter>
    {
        Task<IEnumerable<TDetail>> GetAllAsync();
        Task<TDetail> GetByIdAsync(TParameter id);
        Task<TParameter> PostAsync(TCreate item);
        Task<TParameter> PutAsync(TDetail item);
        Task<int> DeleteAsync(TDetail item);
    }
}
