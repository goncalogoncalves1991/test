using DataAccess.Models.Create;
using DataAccess.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Validation;

namespace DataAccess.Repositories
{
    public class SessionRepository : Repository<session>, IRepository<CreateSession, session,int>
    {
        public Task<session> GetByIdAsync(int id)
        {
            return Task.Factory.StartNew(() =>
            {
                var questions = context.question.Where(e => e.sessionId == id).ToList();
                questions.ForEach(e => context.Entry(e).Reload());//preciso para fazer update dos likes das questions, pq é um procedure q faz esses updates e dbcontext n faz update nas entidades
                //logo teve-se de forçar ir á bd

                var sess=  context.session.Where(x => x.id == id)
                .Include(s => s.@event)
                .Include(s => s.@event.community)
                .Include(s => s.@event.community.admins)
                .Include(s => s.@event.eventSubscribers)
                .Include(s => s.question)
                .Include(s => s.question.Select(a => a.userInfo))
                .Include(s => s.question.Select(q => q.user_like))//para apresentar os id's dos likers nas listas para por ex no android saber se apresento o butao de fazer like ou 
                .FirstOrDefault();
                
                return sess;
            });
        }
        public Task<IEnumerable<session>> GetAllAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                return (IEnumerable<session>)context.session
                    .Include(s => s.@event)
                    .Include(s => s.@event.community)
                    .Include(s => s.@event.community.admins)
                    .Include(s => s.question)
                    .ToList();
            });
        }

        public Task<int> PostAsync(CreateSession item)
        {
            return Task.Factory.StartNew(() =>
            {
                var result = new session() {

                    description = item.description,
                    eventId = item.eventId,
                    initialDate = item.initialDate,
                    endDate = item.endDate,
                    speakerName=item.speakerName,
                    linkOfSpeaker=item.linkOfSpeaker,
                    lastName=item.lastName,
                    title=item.title
                };

                context.session.Add(result);

                try
                {
                    context.SaveChanges();
                    return result.id;
                }
                /*catch (Exception e)
                {

                    throw new ArgumentException(e.Message);
                }*/
                
                 catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        foreach (var ve in eve.ValidationErrors)
                        {
                            Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                ve.PropertyName, ve.ErrorMessage);
                        }
                    }
                    throw;
                }
                 

            });
        }

        public Task<int> PutAsync(session item)
        {
            return saveInfo(item, item.id); 
        }

        public Task<int> DeleteAsync(session id)
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    context.session.Remove(id);
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
