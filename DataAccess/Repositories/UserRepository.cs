using DataAccess.Models.Create;
using DataAccess.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace DataAccess.Repositories
{
    public class UserRepository : Repository<userInfo>, IRepository<CreateUser, userInfo, string>
    {

        public Task<userInfo> GetByIdAsync(string id)
        {
            return Task.Factory.StartNew(() =>
            {
                return context.userInfo.Where(x => x.id == id)
                    .Include(u => u.eventSubscribers)
                    .Include(u =>u.eventSubscribers.Select(x =>x.@event))
                    .Include(u=> u.member)
                    .Include(u => u.admin)
                    .FirstOrDefault();
            });
        }
        public Task<IEnumerable<userInfo>> GetAllAsync()
        {
            return Task.Factory.StartNew(() =>{
                return (IEnumerable<userInfo>)context.userInfo
                    .Include(u => u.eventSubscribers)
                    .Include(u => u.eventSubscribers.Select(x => x.@event))
                    .Include(u => u.member)
                    .Include(u => u.admin)
                    .ToList();
            });
        }



        public Task<string> PostAsync(CreateUser item)
        {
            return Task.Factory.StartNew(() =>
            {               
                var result = new userInfo() { id= item.id, name=item.name, lastName=item.lastName, email=item.email, local=item.local, registerDate=item.registerDate,picture= item.picture,facebook=item.facebook,googleplus=item.googleplus};

                context.userInfo.Add(result);

                try
                {
                    context.SaveChanges();
                    return item.id;
                }
                catch (Exception e)
                {
                    throw new ArgumentException(e.InnerException.Message);
                }

            });
        }

        public Task<string> PutAsync(userInfo item)
        {
            return  Task.Factory.StartNew(() =>
            {
                try
                {
                    context.SaveChanges();
                    return item.id;
                }
                catch (SqlException ex)
                {
                    throw new ArgumentException(ex.Message);
                }

            });
         
        }


        public Task<int> DeleteAsync(userInfo item)
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    context.userInfo.Remove(item);
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
