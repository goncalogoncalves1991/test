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
using static DataAccess.Models.Create.CreateSurvey;
using static DataAccess.Models.Create.CreateSurvey.SurveyQuestion;

namespace DataAccess.Repositories
{
    public class SurveyRepository : Repository<surveyQuestion>, IRepository<CreateSurvey, surveyQuestion, int>
    {
        public Task<int> DeleteAsync(IEnumerable<surveyQuestion> item)
        {
            return Task.Factory.StartNew(() =>
            {
                using (var dbcxtransaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        int res=0;
                        foreach (surveyQuestion question in item)
                        {
                            context.surveyQuestion.Remove(question);
                            res = context.SaveChanges();
                        }
                        dbcxtransaction.Commit();
                        return res;
                    }
                    catch (SqlException ex)
                    {
                        throw new ArgumentException(ex.Message);
                    }
                }

            });
        }

        public Task<IEnumerable<surveyQuestion>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<surveyQuestion> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<surveyQuestion>> GetFromEventAsync(int eventId)
        {
            return Task.Factory.StartNew(() =>
            {
                using (var dbcxtransaction = context.Database.BeginTransaction())
                {
                    var res=  (IEnumerable<surveyQuestion>)context.surveyQuestion
                        .Include(s => s.surveyChoice)
                        .Where(s => s.eventId == eventId)
                        .ToList();

                    dbcxtransaction.Commit();
                    return res;
                }
            });
        }

        public Task<surveyQuestion> GetByIdAsync(int eventId, string userId)
        {
            throw new NotImplementedException();
        }

        public Task<int> PostAsync(CreateSurvey item)
        {
            return Task.Factory.StartNew(() =>
            {
                using (var dbcxtransaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        int i = 0;
                        foreach (SurveyQuestion question in item.questions)
                        {
                            var result = new surveyQuestion()
                            {
                                eventId = item.eventId,
                                questionId = ++i,
                                question = question.question,
                                type = question.type
                            };

                            context.surveyQuestion.Add(result);
                            context.SaveChanges();

                            if (question.choices_messages != null)
                            {
                                int ii = 0;
                                foreach (string c in question.choices_messages)
                                {
                                    var result2 = new surveyChoice()
                                    {
                                        choiceId = ++ii,
                                        eventId = item.eventId,
                                        message = c,
                                        questionId = i
                                    };
                                    context.surveyChoice.Add(result2);
                                    context.SaveChanges();
                                }
                            }
                        }
                        dbcxtransaction.Commit();
                    }
                    catch (Exception e)
                    {
                        throw new ArgumentException(e.Message);
                    }

                    return 0;
                }
            });
        }

        public Task<int> PutAsync(surveyQuestion item)
        {
            throw new NotImplementedException();
        }

        public Task<int> DeleteAsync(surveyQuestion item)
        {
            throw new NotImplementedException();
        }
    }
}
