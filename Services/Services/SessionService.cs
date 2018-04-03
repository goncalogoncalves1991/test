using DataAccess.Models.Create;
using DataAccess.Models.DTOs;
using DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Services.Services
{
    public class SessionService
    {
        private SessionRepository sessionRepo;
        private static EventService eventService;
        private static SessionService sessionService;
        private SessionService()
        {
            sessionRepo = new SessionRepository();
        }
        public class Messages
        {
            public static string SESSION_SUCCESS = "The session was retrieved with success";
            public static string SESSION_NOT_EXIST = "Does not exist any session with this id";
            public static string SESSIONS_SUCCESS = "All sessions retrieved with sucess";
            public static string PARAMETERS_NOT_NULL = "Some parameters must not be null";
            public static string USER_NO_PERMISSION = "This user does not have permission to insert a session into this event";
            public static string SESSION_CREATED = "The session was created with success";
            public static string SESSION_UPDATED = "The session was updated with success";
            public static string SESSION_DELETED_SUCCESS = "The session was deleted with success";
            internal static string END_DATE_NOT_VALID = "The end Date must be after the init date";
        }


        public static SessionService GetInstance()
        {
            if (sessionService == null) {
                sessionService = new SessionService();
                eventService = EventService.GetInstance();
            }
            return sessionService;
        }
        public async Task<OperationResult<session>> GetByIdAsync(int id)
        {
            session session = await sessionRepo.GetByIdAsync(id);
            if (session != null)
            {
                return new OperationResult<session>() { Success = true, Message = Messages.SESSION_SUCCESS, Result = session };
            }
            return new OperationResult<session>() { Success = false, Message = Messages.SESSION_NOT_EXIST };
        }

        public async Task<OperationResult<IEnumerable<session>>> GetAllAsync()
        {
            IEnumerable<session> sessions = await sessionRepo.GetAllAsync();
            return new OperationResult<IEnumerable<session>>() { Success = true, Message = Messages.SESSIONS_SUCCESS, Result = sessions };
        }
        public async Task<OperationResult<IEnumerable<session>>> GetSessionsFromEvent(int eventId)
        {
            var eventRes = await eventService.GetByIdAsync(eventId);
            if (eventRes.Success)
            {
                return new OperationResult<IEnumerable<session>>() { Success = true, Message = Messages.SESSIONS_SUCCESS, Result = eventRes.Result.session.ToList() };
            }
            return new OperationResult<IEnumerable<session>>() { Success = false, Message = eventRes.Message };   
        }

        public async Task<OperationResult<int>> PostSessionAsync(CreateSession item)
        {
            return await Post(item);
        }

        public async Task<OperationResult<int>> DeleteSession(CreateSession item)
        {
            return await Delete(item);
        }

        public async Task<OperationResult<int>> UpdateSession(CreateSession item)
        {
            return await Put(item);
        }


        //<------------------------------------------------------------------------------------------------------>
        //<------------------------------------------------------------------------------------------------------>


        private async Task<OperationResult<int>> Post(CreateSession item)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                if (!item.ParameterValid()) return new OperationResult<int>() { Success = false, Message = Messages.PARAMETERS_NOT_NULL };

                var EventRes = await eventService.GetByIdAsync(item.eventId);
                if (!EventRes.Success) return new OperationResult<int>() { Success = false, Message = EventRes.Message };
                if (!EventRes.Result.community.admins.Any(elem => elem.id == item.userId)) return new OperationResult<int>() { Success = false, Message = Messages.USER_NO_PERMISSION };
                
                if(item.endDate.CompareTo(item.initialDate)<=0) return new OperationResult<int>() { Success = false, Message = Messages.END_DATE_NOT_VALID };

                try
                {
                    var id = await sessionRepo.PostAsync(item);
                    scope.Complete();
                    return new OperationResult<int>() { Success = true, Message = Messages.SESSION_CREATED, Result = id };
                }
                catch (Exception ex)
                {
                    return new OperationResult<int>() { Success = false, Message = ex.Message };
                }
            }
        }

        private async Task<OperationResult<int>> Put(CreateSession item)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var session = await sessionRepo.GetByIdAsync(item.id);
                if (session == null) return new OperationResult<int>() { Success = false, Message = Messages.SESSION_NOT_EXIST };
                if (!session.@event.community.admins.Any(elem => elem.id == item.userId)) return new OperationResult<int>() { Success = false, Message = Messages.USER_NO_PERMISSION };

                try
                {
                    session.title = item.title == null ? session.title : item.title;
                    session.description = item.description == null ? session.description : item.description;
                    session.speakerName = item.speakerName == null ? session.speakerName : item.speakerName;
                    session.lastName = item.lastName == null ? session.lastName : item.lastName;
                    session.linkOfSpeaker = item.linkOfSpeaker == null ? session.linkOfSpeaker : item.linkOfSpeaker;
                    session.initialDate = item.initialDate == default(DateTime) ? session.initialDate : item.initialDate;
                    session.endDate = item.endDate == default(DateTime) ? session.endDate : item.endDate;

                    var id = await sessionRepo.PutAsync(session);
                    scope.Complete();
                    return new OperationResult<int>() { Success = true, Message = Messages.SESSION_UPDATED, Result = id };
                }
                catch (Exception ex)
                {
                    return new OperationResult<int>() { Success = false, Message = ex.Message };
                }
            }
        }

        private async Task<OperationResult<int>> Delete(CreateSession item)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var session = await sessionRepo.GetByIdAsync(item.id);
                if (session == null) return new OperationResult<int>() { Success = false, Message = Messages.SESSION_NOT_EXIST };
                if (!session.@event.community.admins.Any(elem => elem.id == item.userId)) return new OperationResult<int>() { Success = false, Message = Messages.USER_NO_PERMISSION };

                try
                {
                    var id = await sessionRepo.DeleteAsync(session);
                    scope.Complete();
                    return new OperationResult<int>() { Success = true, Message = Messages.SESSION_DELETED_SUCCESS, Result = id };
                }
                catch (Exception ex)
                {
                    return new OperationResult<int>() { Success = false, Message = ex.InnerException.Message };
                }
            }
        }



    }
}
