using DataAccess.Models.Create;
using DataAccess.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.SqlClient;
using DataAccess.Factories;

namespace DataAccess.Repositories
{
    public class CommentRepository : Repository<comment>, IRepository<CreateComment, comment,int>
    {
        public Task<IEnumerable<comment>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<comment> GetByIdAsync(int id)
        {
            return Task.Factory.StartNew(() => context.comment.Where(comm => comm.id == id)
                .Include(c => c.userInfo)
                .Include(c => c.community)
                .Include(c => c.@event)
                //.Include(c => c.community.admins)
                .FirstOrDefault());
        }

        public Task<int> PostAsync(CreateComment item)
        {
            return Task.Factory.StartNew(() =>
            {
                int? res = null;
                var result = new comment() { authorId = item.authorId, eventId = item.eventId == 0 ? res : item.eventId, communityId = item.communityId == 0 ? res : item.communityId, initialDate = item.initialDate, message = item.message };

                context.comment.Add(result);

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

        public Task<comment> PostAsyncV2(CreateComment item)
        {
            return Task.Factory.StartNew(() =>
            {
                int? res = null;
                var result = new comment() { authorId = item.authorId, eventId = item.eventId == 0 ? res : item.eventId, communityId = item.communityId == 0 ? res : item.communityId, initialDate = item.initialDate, message = item.message };

                context.comment.Add(result);

                try
                {
                    context.SaveChanges();
                    return result;
                }
                catch (Exception e)
                {
                    throw new ArgumentException(e.InnerException.Message);
                }

            });
        }




        public Task<int> PutAsync(comment item)
        {
            return saveInfo(item, item.id); 
        }

        public Task<int> DeleteAsync(comment item)
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    context.comment.Remove(item);
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
