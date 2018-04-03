using DataAccess.Models.Create;
using DataAccess.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq;
using System.Data.Entity;
using DataAccess.Factories;
using System.Globalization;

namespace DataAccess.Repositories
{
    public class CommunityRepository : Repository<community>, IRepository<CreateCommunity, community,int>
    {        
        public Task<IEnumerable<community>> GetAllAsync()
        {
            return Task.Factory.StartNew(() => (IEnumerable<community>)context.community
               /* .Include(c => c.@event)
                .Include(c=>c.userInfo)
                .Include(c=>c.userInfo1)
                .Include(c=>c.notice)
                .Include(c=>c.sponsor)
                .Include(c=>c.tag)
                .Include(c => c.commentCommunity)
                .Include(c => c.commentCommunity.Select(a => a.userInfo))*/
                .ToList());  
        }

        public Task<community> GetByIdAsync(int id)
        {
            return Task.Factory.StartNew(()=>context.community.Where(comm => comm.id == id)
                .Include(c => c.@event)
                .Include(c => c.admins)
                .Include(c => c.members)
                .Include(c => c.notice)
                .Include(c => c.sponsor)
                .Include(c => c.tag)
                .Include(c => c.comment)
                .Include(c => c.comment.Select(a => a.userInfo))
                .FirstOrDefault());                
        }

        public Task<community> GetByNameAsync(string name)
        {
            return Task.Factory.StartNew(() => context.community.Where(comm =>comm.name == name)
                .Include(c => c.@event)
                .Include(c => c.admins)
                .Include(c => c.members)
                .Include(c => c.notice)
                .Include(c => c.sponsor)
                .Include(c => c.tag)
                .Include(c => c.comment)
                .Include(c => c.comment.Select(a => a.userInfo))
                .Include(c => c.CommunitySocialNetwork)
                .FirstOrDefault());

        }

        public Task<IEnumerable<community>> GetByParamsAsync(CoordinatesRange location,string name, string[] tags)
        {
            return Task.Factory.StartNew(() => {
                var dbCommunity = context.community.AsQueryable();

                if (tags != null) dbCommunity = dbCommunity.Where(elem => elem.tag.Any(e => tags.Contains(e.name)));
                if (name != null) dbCommunity = dbCommunity.Where(elem => elem.name == name);
                if (location != null) return dbCommunity.AsEnumerable().Where(elem => {

                    var lat = Convert.ToDouble(elem.latitude, new CultureInfo("en-US"));
                    var longi = Convert.ToDouble(elem.longitude, new CultureInfo("en-US"));
                    bool s = lat <= location.MaxLatitude && lat >= location.MinLatitude
                        && longi <= location.MaxLongitude && longi >= location.MinLongitude;
                    return s;
                }).ToList();


                return (IEnumerable<community>)dbCommunity.ToList();

            });
        }

        public Task<int> PostAsync(CreateCommunity item)
        {
            return Task.Factory.StartNew(() =>
            {
                    SqlParameter sponsors;
                    SqlParameter tags;
                    format_tags_and_sponsors_to_parameter(item.Tags, item.Sponsors, out sponsors, out tags);

                    var name = SqlParameterFactory.getVarCharParameter("name",item.Name);
                    var local = SqlParameterFactory.getVarCharParameter("local", item.Local);
                    var latitude = SqlParameterFactory.getVarCharParameter("latitude", item.Latitude);
                    var longitude = SqlParameterFactory.getVarCharParameter("longitude", item.Longitude);
                    var description = SqlParameterFactory.getVarCharParameter("description", item.Description);
                    var date = SqlParameterFactory.getDateParameter("date", item.FoundationDate);
                    var avatar = SqlParameterFactory.getVarCharParameter("avatar", item.AvatarLink);//para não alterar o procedimento (muito trabalho...) 
                    var userId = SqlParameterFactory.getVarCharParameter("userId", item.UserId); 

                    ////Parameter for SP output
                    var result = new SqlParameter("result", SqlDbType.Int);
                    result.Direction = ParameterDirection.Output;

                    try
                    {
                        context.Database.ExecuteSqlCommand("EXEC insert_community @name,@local,@description,@date,@avatar,@userId,@latitude,@longitude,@sponsors,@tags,@result OUTPUT", name, local, description, date, avatar, userId, latitude, longitude, sponsors, tags, result);

                        return Convert.ToInt32(result.Value);
                    }
                    catch (SqlException ex)
                    {
                        throw new ArgumentException(ex.Message);
                    }

            });
        }

        public Task<int> PutAsync(community item)
        {
            return saveInfo(item,item.id);              
        
        }

        public Task<int> DeleteAsync(community item)
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    context.community.Remove(item);
                    return context.SaveChanges();
                }
                catch (SqlException ex)
                {
                    throw new ArgumentException(ex.Message);
                }

            }); 
        }

        public Task<string> InsertMember(int communityId, string userId)
        {
            return N_To_N_operation(communityId, userId, "insert into communityMembers (communityId,userId) values (@id,@userId)"); 
        }

        public Task<string> DeleteMember(int communityId, string userId)
        {
            return N_To_N_operation(communityId,userId,"delete from communityMembers where communityId=@id and userId=@userId"); 
        }

        public Task<string> InsertAdmin(int communityId, string userId)
        {

            return N_To_N_operation(communityId, userId, "insert into communityAdmins (communityId,userId) values (@id,@userId)");           
        }

        public Task<string> DeleteAdmin(int communityId, string userId)
        {
            return N_To_N_operation(communityId, userId, "delete from communityAdmins where communityId=@id and userId=@userId");
        }

        public Task<bool> InsertTag(int communityId, int[] tags) {
            return Tags_N_To_N_operations(communityId, tags, "EXEC insert_tag_to_community @tags,@entityId");
        }
        public Task<bool> DeleteTag(int communityId, int[] tags)
        {
            return Tags_N_To_N_operations(communityId, tags, "EXEC delete_tag_to_community @tags,@entityId");
        }
       

        public Task<bool> InsertSponsor(int communityId, int[] sponsors)
        {
            return Sponsors_N_To_N_operations(communityId, sponsors, "EXEC insert_sponsor_to_community @sponsors,@communityId");
        }

        public Task<bool> DeleteSponsor(int communityId, int[] sponsors)
        {
            return Sponsors_N_To_N_operations(communityId, sponsors, "EXEC delete_sponsor_to_community @sponsors,@communityId");
        }

       
    }
}
