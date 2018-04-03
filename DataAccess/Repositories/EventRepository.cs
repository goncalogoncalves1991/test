using DataAccess.Factories;
using DataAccess.Models.Create;
using DataAccess.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Globalization;

namespace DataAccess.Repositories
{
    public class EventRepository : Repository<@event>, IRepository<CreateEvent, @event,int>
    {
        public Task<@event> GetByIdAsync(int id)
        {
            return Task.Factory.StartNew(() => {
                return context.@event.Where(x => x.id == id)
                    .Include(c => c.comment)
                    .Include(c => c.comment.Select(a => a.userInfo))
                    .Include(c =>c.eventSubscribers)
                    .Include(c=>c.eventSubscribers.Select(a => a.userInfo))
                    .Include(c =>c.session)
                    .Include(c => c.tag)
                    .Include(c => c.community)
                    .Include(c=> c.community.admins)
                    .Include(c=> c.community.tag)
                    .Include(c=> c.survey)
                    .Include(c =>c.survey.Select(a=>a.surveyChoice))
                    .FirstOrDefault();
            });
        }
        public Task<IEnumerable<@event>> GetAllAsync()
        {
            return Task.Factory.StartNew(() =>{
                return (IEnumerable<@event>)context.@event
                    .Include(c => c.comment)
                    .Include(c => c.comment.Select(a => a.userInfo))
                    .Include(c => c.eventSubscribers)
                    .Include(c => c.eventSubscribers.Select(a => a.userInfo))
                    .Include(c => c.session)
                    .Include(c => c.tag)
                    .Include(c => c.community)
                    .Include(c => c.community.admins)
                    .ToList();
            });
        }

    
        public async Task<IEnumerable<@event>> GetByLocationAsync(string location)
        {
            return await GetEventsByPropertyAsync(eve => eve.local == location);
        }

        

        //Metodo auxiliar
        private Task<IEnumerable<@event>> GetEventsByPropertyAsync(Func<@event,bool> comparer){
            
            return Task.Factory.StartNew(() => {
                return (IEnumerable<@event>)context.@event.Where(comparer).ToList();
            });
        }

        public Task<IEnumerable<@event>> GetByParamsAsync(CoordinatesRange location, string[] tags)
        {
            return Task.Factory.StartNew(() => {
                var dbEvent = context.@event.AsQueryable();

                if (tags != null) dbEvent = dbEvent.Where(elem => elem.tag.Any(e => tags.Contains(e.name)));
                if (location != null) return dbEvent.AsEnumerable().Where(elem => {

                    var lat = Convert.ToDouble(elem.latitude,new CultureInfo("en-US"));
                    var longi = Convert.ToDouble(elem.longitude, new CultureInfo("en-US"));
                    bool s = lat <= location.MaxLatitude && lat >= location.MinLatitude
                        && longi <= location.MaxLongitude && longi >= location.MinLongitude;
                    return s;
                }).ToList();
                

                return (IEnumerable<@event>)dbEvent.ToList();

            });
        }


        public Task<int> PostAsync(CreateEvent item)
        {
            return Task.Factory.StartNew(() =>
            {
                SqlParameter sponsors;
                SqlParameter tags;
                format_tags_and_sponsors_to_parameter(item.Tags, null, out sponsors, out tags);

                var title = SqlParameterFactory.getVarCharParameter("title", item.title);
                var local = SqlParameterFactory.getVarCharParameter("local", item.local);
                var description = SqlParameterFactory.getVarCharParameter("description", item.description);
                var endDate = SqlParameterFactory.getDateParameter("endDate", item.endDate);
                var initDate = SqlParameterFactory.getDateParameter("initDate", item.initDate);
                var nrTickets = SqlParameterFactory.getIntParameter("nrTickets", item.nrOfTickets);
                var communityId = SqlParameterFactory.getIntParameter("communityId", item.communityId);
                var latitude = SqlParameterFactory.getVarCharParameter("latitude", item.latitude);
                var longitude = SqlParameterFactory.getVarCharParameter("longitude", item.longitude);
                var qrcode = SqlParameterFactory.getVarCharParameter("qrcode", item.qrcode);

                ////Parameter for SP output
                var result = new SqlParameter("result", SqlDbType.Int);
                result.Direction = ParameterDirection.Output;

                try
                {
                    context.Database.ExecuteSqlCommand("EXEC insert_event @communityId,@title,@local,@description,@nrTickets,@initDate,@endDate,@tags,@latitude,@longitude,@qrcode,@result OUTPUT", communityId, title, local, description, nrTickets, initDate, endDate, tags,latitude,longitude, qrcode, result);
                    return Convert.ToInt32(result.Value);
                }
                catch (SqlException ex)
                {
                    throw new ArgumentException(ex.Message);
                }

            });
        }

        public Task<int> PutAsync(@event item)
        {
            return saveInfo(item,item.id);    
        }
       

        public Task<int> DeleteAsync(@event item)
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    context.@event.Remove(item);
                    return context.SaveChanges();
                }
                catch (SqlException ex)
                {
                    throw new ArgumentException(ex.Message);
                }

            }); 
        }

        public Task<string> InsertSubscriber(int eventId, string userId)
        {
            return N_To_N_operation(eventId, userId, "insert into eventSubscribers (eventId,userId) values (@id,@userId)");
        }

        public Task<string> DeleteMember(int eventId, string userId)
        {
            return N_To_N_operation(eventId, userId, "delete from eventSubscribers where eventId=@id and userId=@userId");
        }

        public Task<bool> InsertTag(int eventId, int[] tags)
        {
            return Tags_N_To_N_operations(eventId, tags, "EXEC insert_tag_to_event @tags,@entityId");
        }
        public Task<bool> DeleteTag(int eventId, int[] tags)
        {
            return Tags_N_To_N_operations(eventId, tags, "EXEC delete_tag_to_event @tags,@entityId");
        }

        
    }
}
