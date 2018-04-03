using DataAccess.Models.Create;
using DataAccess.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class CommunitySocialRepository : Repository<CommunitySocialRepository>, IRepository<CreateCommunitySocial, CommunitySocialRepository, int>
    {
        public Task<IEnumerable<CommunitySocialRepository>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<CommunitySocialRepository> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<int> PostAsync(CreateCommunitySocial item)
        {
            return Task.Factory.StartNew(() =>
            {
                var result = new CommunitySocialNetwork() { Acess_Token=item.token,CommunityId=item.communityId, provider=item.social, Url=item.url };

                context.CommunitySocialNetwork.Add(result);

                try
                {
                    context.SaveChanges();
                    return result.CommunityId;
                }
                catch (Exception e)
                {
                    throw new ArgumentException(e.InnerException.Message);
                }

            });
        }

        public Task<int> PutAsync(CommunitySocialRepository item)
        {
            throw new NotImplementedException();
        }

        public Task<int> DeleteAsync(CommunitySocialRepository item)
        {
            throw new NotImplementedException();
        }

    }
}
