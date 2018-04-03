using DataAccess.Models.Create;
using DataAccess.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.SqlClient;

namespace DataAccess.Repositories
{
    public class NoticeRepository : Repository<notice>, IRepository<CreateNotice, notice,int>
    {

        public Task<notice> GetByIdAsync(int id)
        {
            return Task.Factory.StartNew(() =>
            {
                return context.notice.Where(x => x.id == id)
                    .Include(n => n.community)
                    .Include(n =>n.community.admins)
                    .FirstOrDefault();
            });
        }
        public Task<IEnumerable<notice>> GetAllAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                return (IEnumerable<notice>)context.notice
                    .Include(n => n.community)
                    .Include(n => n.community.admins)
                    .ToList();
            });
        }

        public Task<int> PostAsync(CreateNotice item)
        {
            return Task.Factory.StartNew(() =>
            {
                var result = new notice() { communityId = item.communityId, description = item.description, initialDate = item.initialDate, title=item.title};

                context.notice.Add(result);

                try
                {
                    context.SaveChanges();
                    return result.id;
                }
                catch (Exception e)
                {
                    throw new ArgumentException(e.InnerException.Message);
                }

            });
        }

        public Task<int> PutAsync(notice item)
        {
            return saveInfo(item, item.id); 
        }

        public Task<int> DeleteAsync(notice id)
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    context.notice.Remove(id);
                    return context.SaveChanges();
                }
                catch (SqlException ex)
                {
                    throw new ArgumentException(ex.Message);
                }

            }); 
        }
    }
}
