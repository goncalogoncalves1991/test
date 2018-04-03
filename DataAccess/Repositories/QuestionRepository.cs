using DataAccess.Models.Create;
using DataAccess.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Data;
using System.Data.Entity.Core.Objects;

namespace DataAccess.Repositories
{
    public class QuestionRepository : Repository<question>, IRepository<CreateQuestion, question,int>
    {
        public Task<question> GetByIdAsync(int id)
        {
            return Task.Factory.StartNew(() =>
            {
                return context.question.Where(x => x.id == id)
                    .Include(q => q.userInfo)
                    .Include(q => q.session)
                    .Include(q => q.session.@event)
                    .Include(q => q.user_like)
                    .Include(q => q.session.@event.eventSubscribers)
                    .FirstOrDefault();
            });
        }
        public Task<IEnumerable<question>> GetAllAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                return (IEnumerable<question>)context.question
                    .Include(q => q.userInfo)
                    .Include(q => q.session)
                    .Include(q => q.session.@event)
                    .Include(q => q.session.@event.community.admins)
                    .ToList();
            });
        }

        public Task<int> PostAsync(CreateQuestion item)
        {
            return Task.Factory.StartNew(() =>
            {
                var result = new question() { authorId = item.authorId, message=item.message, sessionId=item.sessionId, likes=0 };

                context.question.Add(result);

                try
                {
                    context.SaveChanges();
                    return result.id;
                }
                catch (Exception e)
                {
                    throw new ArgumentException(e.InnerException.InnerException.Message);
                }

            });
        }

        public Task<int> PutAsync(question item)
        {
            return saveInfo(item, item.id); 
        }

        public Task<int> DeleteAsync(question id)
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    context.question.Remove(id);
                    return context.SaveChanges();
                }
                catch (SqlException ex)
                {
                    throw new ArgumentException(ex.Message);
                }

            }); 
        }

        public Task insertLiker(userInfo user, question q)
        {
            return Task.Factory.StartNew(() =>
            {             
                try
                {
                    context.insert_question_Liker(user.id,q.id);
                    return;
                }
                catch (SqlException ex)
                {
                    throw new ArgumentException(ex.Message);
                }

            });
        }

        public Task deleteLiker(userInfo user, question question)
        {
            return Task.Factory.StartNew(() =>
            {
                var userId = new SqlParameter("userId", SqlDbType.VarChar);
                userId.Value = user.id;
                var questionId = new SqlParameter("questionId", SqlDbType.Int);
                questionId.Value = question.id;

                try
                {
                    context.Database.ExecuteSqlCommand("EXEC delete_question_Liker @userId,@questionId", userId, questionId);

                    return;
                }
                catch (SqlException ex)
                {
                    throw new ArgumentException(ex.Message);
                }

            });
        }
    }
}
