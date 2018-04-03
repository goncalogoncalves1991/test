using DataAccess.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class EventSubscribersRepository : Repository<eventSubscribers>, IRepository<object, eventSubscribers, int>
    {
        public Task<int> DeleteAsync(eventSubscribers item)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<eventSubscribers>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<eventSubscribers> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<int> PostAsync(object item)
        {
            throw new NotImplementedException();
        }

        public Task<int> PutAsync(eventSubscribers item)
        {
            return saveInfo(item,0);
        }

        public Task<eventSubscribers> GetByIdAsync(int eventId, string id)
        {
            return Task.Factory.StartNew(() => context.eventSubscribers.Where(sub => (sub.eventId==eventId && sub.userId==id))
                .FirstOrDefault());
        }
    }
}
