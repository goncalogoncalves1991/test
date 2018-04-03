
using DataAccess.Models.Create;
using DataAccess.Models.DTOs;
using DataAccess.Repositories;
using Services.Models;
using Services.Other;
using Services.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Services.Services
{
    public class EventService 
    {
        private const int HOUR_INTERVAL_FOR_NEW_EVENTS = -10;
        private EventRepository eventRepo;
        private static EventService eventService ;
        private static UserService userService;
        private static CommunityService communityService;
        private static EventSubscribersRepository eventSubscribers;

        public enum Time { Past, Future,All}

        public class Messages
        {
            public static string EVENT_SUCCESS = "The event was retrieved with success";
            public static string EVENT_NOT_EXIST = "Does not exist any event with this id";
            public static string OPERATION_SUCCESS = "Operation completed with sucess";
            public static string EVENTS_SUCCESS = "Retrieved all events of community";
            public static string USER_EVENTS_SUCCESS = "Retrieved all events of user";
            public static string PARAMETERS_NOT_NULL = "Some Parameters must not be null";
            public static string USER_NO_PERMISSION = "This Id does not have permission to operate with an event on this community";
            public static string EVENT_CREATED="Event created with Success";
            public static string EVENT_UPDATED_SUCCESS = "Event was updated with Success";
            public static string EVENT_DELETED_SUCCESS = "Event deleted with Success";
            public static string EVENT_HAS_SUBSCRIBER="This user is already subscribed on this event";
            public static string EVENT_SUBSCRIBER_INSERTED ="This user was subscribed to this event with success";
            public static string EVENT_HAS_NO_SUBSCRIBER = "This user is not subscribed on this event";
            public static string EVENT_SUBSCRIBER_DELETED = "This user was deleted from this event with success";
            public static string EVENT_TAG_EXIST = "this tag already belongs to this event";
            public static string TAG_INSERTED = "Tag(s) inserted with success in this event";
            public static string TAG_DELETED ="Tag deleted with success from this event";
            public static string EVENT_TAG_NOT_EXIST = "tag doesn't exist on this event";
            public static string USER_NOT_SUBSCRIBED = "This user is not subscribed on this event";
            public static string USER_CHECKEDIN = "This user is already checkeIn ";
            public static string CHECKEDIN_SUCESSE = "The user was checked in with sucess";
            public static string CODE_NOT_MATCH = "This code does not match the event code";
            public static string USER_NOT_CHECKIN = "This user is not check in on this event";
            public static string MISSING_LOCATION = "Location object must have latitude, longitude and radius";
        }


        private EventService()
        {
            eventRepo = new EventRepository();
            eventSubscribers = new EventSubscribersRepository();

        }
        
        public static EventService GetInstance()
        {
            if (eventService == null) {
                eventService = new EventService();
                userService = UserService.GetInstance();
                communityService = CommunityService.GetInstance();
                 
            }
            return eventService;
        }


        public async Task<OperationResult<@event>> GetByIdAsync(int id)
        {
            @event eventResult = await eventRepo.GetByIdAsync(id);
            if (eventResult != null)
            {
                return new OperationResult<@event>() { Success = true, Message = Messages.EVENT_SUCCESS, Result = eventResult };
            }
            return new OperationResult<@event>() { Success = false, Message = Messages.EVENT_NOT_EXIST };
        }

        public async Task<OperationResult<IEnumerable<@event>>> GetAllAsync()
        {
            IEnumerable<@event> events = await eventRepo.GetAllAsync();
            return new OperationResult<IEnumerable<@event>>() { Success = true, Message =Messages.OPERATION_SUCCESS, Result = events };
        }



        public async Task<OperationResult<IEnumerable<@event>>> GetAllEventsFromCommunityInTime(int communityId, Time time)
        {
            var community = await communityService.GetByIdAsync(communityId);
            if (community.Success)
            {
                IEnumerable<@event> events;
                if (Time.Past == time)
                    events = community.Result.@event.Where(eve => eve.endDate < DateTime.Now).ToList();
                else  
                    events = community.Result.@event.Where(eve => eve.initDate > DateTime.Now).ToList();

                return new OperationResult<IEnumerable<@event>>() { Success = true, Message = Messages.EVENT_SUCCESS, Result = events };
            }
            return new OperationResult<IEnumerable<@event>>() { Success = false, Message = community.Message };  
            
        }

        public async Task<OperationResult<IEnumerable<@event>>> GetEventsByCommunityId(int communityId)
        {
            var community = await communityService.GetByIdAsync(communityId);
            if (community.Success)
            {
                return new OperationResult<IEnumerable<@event>>()
                {
                    Success = true,
                    Result = community.Result.@event,
                    Message = Messages.EVENTS_SUCCESS
                };
            }
            return new OperationResult<IEnumerable<@event>>() { Success = false, Message = community.Message };
            
        }


        public async Task<OperationResult<IEnumerable<@event>>> GetAllEventsFromUserAsync(string userId,Time time)
        {
            var userRes = await userService.GetByIdAsync(userId);
            if (userRes.Success)
            {
                if (Time.Past == time)
                {
                    return new OperationResult<IEnumerable<@event>>()
                    {
                        Success = true,
                        Result = userRes.Result.eventSubscribers.Where(i => i.@event.endDate < DateTime.Now).Select(i => i.@event),
                        Message = Messages.USER_EVENTS_SUCCESS
                    };
                }
                else if (Time.Future == time)
                {
                    return new OperationResult<IEnumerable<@event>>()
                    {
                        Success = true,
                        Result = userRes.Result.eventSubscribers.Where(i=> i.@event.initDate > DateTime.Now.Date).Select(i => i.@event),
                        Message = Messages.USER_EVENTS_SUCCESS
                    };
                }
                else
                {
                    return new OperationResult<IEnumerable<@event>>()
                    {
                        Success = true,
                        Result = userRes.Result.eventSubscribers.Select(i => i.@event),
                        Message = Messages.USER_EVENTS_SUCCESS
                    };
                }
            }
            return new OperationResult<IEnumerable<@event>>() { Success = false, Message = userRes.Message };
        }

        /** This methods gets all future events from the communities that the user belongs, but is not subscribed.
         * Used for future notifications */
        public async Task<OperationResult<IEnumerable<@event>>> Get_Events_To_Come_From_Communities_Which_User_Belongs(string id)
        {
            var communities = await communityService.GetCommunitiesFromUserByRole(id, Models.Roles.Role.Member);
            if (communities.Success)
            {
                List<@event> list = new List<@event>();
                foreach (var community in communities.Result)
                {
                    var events = await GetAllEventsFromCommunityInTime(community.id, Time.Future);
                    foreach (var eventLowInfo in events.Result)
                    {
                        var eventAllInfo = await eventRepo.GetByIdAsync(eventLowInfo.id);//gets the information attached to the events e.g. the event subscribers needed for below operation
                        if (!eventAllInfo.eventSubscribers.Any(e => e.userId == id))
                            list.Add(eventAllInfo);
                    }
                }

                return new OperationResult<IEnumerable<@event>>() { Success = true, Result = list };
            }
            return new OperationResult<IEnumerable<@event>>() { Success = false, Message = communities.Message};
        }

        public async Task<OperationResult<IEnumerable<@event>>> Get_New_Events_From_Community(string userId, string dateTime)
        {
            var communities = await communityService.GetCommunitiesFromUserByRole(userId, Models.Roles.Role.Member);
            if (communities.Success)
            {
                DateTime dt = Convert.ToDateTime(dateTime);
                List<@event> list = new List<@event>();
                foreach (var community in communities.Result)
                {
                    var events = await GetEventsByCommunityId(community.id);
                    if (events.Success)
                    {
                        foreach(var eventRes in events.Result)
                        {
                            if (eventRes.creationDate.Value.CompareTo(dt.AddHours(HOUR_INTERVAL_FOR_NEW_EVENTS))>0)
                            {
                                list.Add(eventRes);
                            }
                        }
                    }
                }
                return new OperationResult<IEnumerable<@event>> { Success = true, Result = list };
            }
            return new OperationResult<IEnumerable<@event>>() { Success = false, Message = communities.Message };
        }

        public async Task<OperationResult<IEnumerable<@event>>> Search(LocationCoordinates location, string[] tags)
        {
            if (location != null && location.isEmpty()) new OperationResult<IEnumerable<@event>>() { Success = false, Message = Messages.MISSING_LOCATION };

            var box = CoordinatesBoundingBox.GetBoundingBox(new CoordinatesBoundingBox.MapPoint { Longitude = location.longitude, Latitude = location.latitude }, location.radius);

            IEnumerable<@event> events = await eventRepo.GetByParamsAsync(new CoordinatesRange { MaxLatitude = box.MaxPoint.Latitude, MaxLongitude = box.MaxPoint.Longitude, MinLatitude = box.MinPoint.Latitude, MinLongitude = box.MinPoint.Longitude }, tags);
            if (events != null)
            {
                return new OperationResult<IEnumerable<@event>>() { Success = true, Message = Messages.EVENTS_SUCCESS, Result = events };
            }
            return new OperationResult<IEnumerable<@event>>() { Success = false, Message = Messages.EVENT_NOT_EXIST };
        }

        public async Task<OperationResult<@event>> PostEventAsync(CreateEvent eve)   
        {
            var res = await Post(eve);
            if (!res.Success) return res;
            
            EmailSenderService.SendQrCodeToAdmins(res.Result);

            return res;
        }

        public async Task<OperationResult<bool>> SelfCheckIn(string id, int eventId, string code)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {

                var user = await userService.GetByIdAsync(id);
                if (!user.Success) return new OperationResult<bool>() { Success = false, Message = user.Message };
                var eve = await GetByIdAsync(eventId);
                if (!eve.Success) return new OperationResult<bool>() { Success = false, Message = eve.Message };
                if (!eve.Result.eventSubscribers.Any(item => item.userId == id)) return new OperationResult<bool>() { Success = false, Message = Messages.USER_NOT_SUBSCRIBED };
                if (eve.Result.eventSubscribers.Any(item => item.userId == id && item.checkIn== true)) return new OperationResult<bool>() { Success = false, Message = Messages.USER_CHECKEDIN };
                if (eve.Result.qrcode!=code) return new OperationResult<bool>() { Success = false, Message = Messages.CODE_NOT_MATCH };

                try
                {
                    var subscribedEventModel = eve.Result.eventSubscribers.Where(item => item.userId == id).First();
                    var subscribed = await eventSubscribers.GetByIdAsync(eventId, id);
                    subscribed.checkIn = true;
                    var result = await eventSubscribers.PutAsync(subscribed);
                    subscribedEventModel.checkIn = true;//para fazer update do modelo do evento do entity framework porque este n faz sempre chamadas a BD e neste caso precisa-se de actualizar
                    scope.Complete();
                    return new OperationResult<bool>() { Success = true, Message = Messages.CHECKEDIN_SUCESSE, Result = true };
                }
                catch (Exception ex)
                {
                    return new OperationResult<bool>() { Success = false, Message = ex.InnerException.Message };
                }

            }
                
        }
        public async Task<OperationResult<bool>> isSubscribed(int id, string userId)
        {
            var eve = await GetByIdAsync(id);
            if (!eve.Success) return new OperationResult<bool> { Success = false, Result = false, Message = EventService.Messages.EVENT_NOT_EXIST };
            var subscriber = eve.Result.eventSubscribers.Where(sub => sub.userId == userId).FirstOrDefault();

            if (subscriber == null) return new OperationResult<bool> { Success = false, Result = false, Message = EventService.Messages.USER_NOT_SUBSCRIBED };
            else
            {
                if (subscriber.checkIn == true) return new OperationResult<bool> { Success = true, Result = true, Message = EventService.Messages.USER_CHECKEDIN };
                else return new OperationResult<bool> { Success = true, Result = false, Message = Messages.USER_NOT_CHECKIN };
            }
        }

        public async Task<OperationResult<int>> DeleteEvent(CreateEvent item)
        {
            return await Delete(item);
        }

        public async Task<OperationResult<int>> UpdateEvent(CreateEvent item)
        {
            return await Put(item);
        }

        public async Task<OperationResult<userInfo>> InsertSubscriber(int eventId, string userId)
        {
            return await PostSubscriber(eventId, userId);
        }

        public async Task<OperationResult<string>> RemoveSubscriber(int eventId, string userId)
        {
            return await DeleteSubscriber(eventId, userId);
        }
        public async Task<OperationResult<bool>> InsertTag(int eventId, string adminId, int[] tags)
        {
            return await PostTag(eventId, tags, adminId);
        }

        public async Task<OperationResult<bool>> RemoveTag(int eventId, string adminId, int[] tags)
        {
            return await DeleteTag(eventId, tags, adminId);
        }



        //<------------------------------------------------------------------------------------------------------>
        //<------------------------------------------------------------------------------------------------------>



        private async Task<OperationResult<@event>> Post(CreateEvent item)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                if (!item.ParameterValid()) return new OperationResult<@event>() { Success = false, Message = Messages.PARAMETERS_NOT_NULL };
                
                var community = await communityService.GetByIdAsync(item.communityId);
                if (!community.Success) return new OperationResult<@event>() { Success = false, Message = community.Message };
                if (!community.Result.admins.Any(elem => elem.id == item.UserId)) return new OperationResult<@event>() { Success = false, Message = Messages.USER_NO_PERMISSION };

                try
                {
                    if (item.Tags != null) item.Tags=item.Tags.Distinct().ToArray();
                    item.qrcode = Guid.NewGuid().ToString();
                    var evId = await eventRepo.PostAsync(item);
                    var eve = await eventRepo.GetByIdAsync(evId);
                    scope.Complete();
                    return new OperationResult<@event>() { Success = true, Message = Messages.EVENT_CREATED, Result = eve };
                }
                catch (Exception ex)
                {
                    return new OperationResult<@event>() { Success = false, Message = ex.Message };
                }
            }
        }

        private async Task<OperationResult<int>> Put(CreateEvent item)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var EventRes = await eventRepo.GetByIdAsync(item.Id);
                if (EventRes == null) return new OperationResult<int>() { Success = false, Message = Messages.EVENT_NOT_EXIST };
                if (!EventRes.community.admins.Any(user => user.id == item.UserId)) return new OperationResult<int>() { Success = false, Message = Messages.USER_NO_PERMISSION };

                try
                {
                    EventRes.title = item.title == null ? EventRes.title : item.title;
                    EventRes.description = item.description == null ? EventRes.description : item.description;
                    EventRes.local = item.local == null ? EventRes.local : item.local;
                    EventRes.nrOfTickets = item.nrOfTickets == 0 ? EventRes.nrOfTickets : item.nrOfTickets;
                    EventRes.initDate = item.initDate == DateTime.MinValue ? EventRes.initDate : item.initDate;
                    EventRes.endDate = item.endDate == DateTime.MinValue ? EventRes.endDate : item.endDate;

                    var id = await eventRepo.PutAsync(EventRes);
                    scope.Complete();
                    return new OperationResult<int>() { Success = true, Message = Messages.EVENT_UPDATED_SUCCESS, Result = id };
                }
                catch (Exception ex)
                {
                    return new OperationResult<int>() { Success = false, Message = ex.InnerException.Message };
                }
            }
        }

        private async Task<OperationResult<int>> Delete(CreateEvent item)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var EventRes = await eventRepo.GetByIdAsync(item.Id);
                if (EventRes == null) return new OperationResult<int>() { Success = false, Message = Messages.EVENT_NOT_EXIST };
                if (!EventRes.community.admins.Any(user => user.id == item.UserId)) return new OperationResult<int>() { Success = false, Message = Messages.USER_NO_PERMISSION };

                try
                {
                    var id = await eventRepo.DeleteAsync(EventRes);
                    scope.Complete();
                    return new OperationResult<int>() { Success = true, Message = Messages.EVENT_DELETED_SUCCESS, Result = id };
                }
                catch (Exception ex)
                {
                    return new OperationResult<int>() { Success = false, Message = ex.InnerException.Message };
                }
            }
        }

        private async Task<OperationResult<userInfo>> PostSubscriber(int eventId, string userId)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var EventRes = await GetByIdAsync(eventId);
                var user = await userService.GetByIdAsync(userId);

                if (!user.Success) return new OperationResult<userInfo>() { Success = false, Message = user.Message };
                if (!EventRes.Success) return new OperationResult<userInfo>() { Success = false, Message = Messages.EVENT_NOT_EXIST };
                if (EventRes.Result.eventSubscribers.Any(elem => elem.userId == userId)) return new OperationResult<userInfo>() { Success = false, Message = Messages.EVENT_HAS_SUBSCRIBER };

                try
                {
                    var id = await eventRepo.InsertSubscriber(eventId, userId);
                    scope.Complete();
                    return new OperationResult<userInfo>() { Success = true, Message = Messages.EVENT_SUBSCRIBER_INSERTED, Result = user.Result };
                }
                catch (Exception ex)
                {
                    return new OperationResult<userInfo>() { Success = false, Message = ex.InnerException.Message };
                }
            }
        }

        private async Task<OperationResult<string>> DeleteSubscriber(int eventId, string userId)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var EventRes = await GetByIdAsync(eventId);
                var user = await userService.GetByIdAsync(userId);

                if (!user.Success) return new OperationResult<string>() { Success = false, Message = user.Message };
                if (!EventRes.Success) return new OperationResult<string>() { Success = false, Message = Messages.EVENT_NOT_EXIST };
                if (!EventRes.Result.eventSubscribers.Any(elem => elem.userId == userId)) return new OperationResult<string>() { Success = false, Message = Messages.EVENT_HAS_NO_SUBSCRIBER };

                try
                {
                    var id = await eventRepo.DeleteMember(eventId, userId);
                    scope.Complete();
                    return new OperationResult<string>() { Success = true, Message = Messages.EVENT_SUBSCRIBER_DELETED, Result = id };
                }
                catch (Exception ex)
                {
                    return new OperationResult<string>() { Success = false, Message = ex.InnerException.Message };
                }
            }

        }

        private async Task<OperationResult<bool>> PostTag(int eventId, int[] tags, string adminId)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var EventRes = await GetByIdAsync(eventId);

                if (!EventRes.Success) return new OperationResult<bool>() { Success = false, Message = Messages.EVENT_NOT_EXIST };
                if (!EventRes.Result.community.admins.Any(elem => elem.id==adminId)) return new OperationResult<bool>() { Success = false, Message = Messages.USER_NO_PERMISSION };

                var newTags = tags.Distinct().ToArray();
                //check if tags exist when have the tags service....
                if (EventRes.Result.tag.Any(elem => newTags.Contains(elem.id))) return new OperationResult<bool>() { Success = false, Message = Messages.EVENT_TAG_EXIST };

                try
                {
                    var success = await eventRepo.InsertTag(eventId, newTags);
                    scope.Complete();
                    return new OperationResult<bool>() { Success = true, Message = Messages.TAG_INSERTED, Result = true };
                }
                catch (Exception ex)
                {
                    return new OperationResult<bool>() { Success = false, Message = ex.InnerException.Message, Result = false };
                }
            }
        }

        private async Task<OperationResult<bool>> DeleteTag(int eventId, int[] tags, string adminId)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var EventRes = await GetByIdAsync(eventId);

                if (!EventRes.Success) return new OperationResult<bool>() { Success = false, Message = Messages.EVENT_NOT_EXIST };
                if (!EventRes.Result.community.admins.Any(elem => elem.id == adminId)) return new OperationResult<bool>() { Success = false, Message = Messages.USER_NO_PERMISSION };

                var newTags = tags.Distinct().ToArray();
                var currentTags = EventRes.Result.tag.Select(x => x.id);
                //check if tags exist when have the tags service....
                if (!newTags.All(elem => currentTags.Contains(elem))) return new OperationResult<bool>() { Success = false, Message = Messages.EVENT_TAG_NOT_EXIST };

                try
                {
                    var success = await eventRepo.DeleteTag(eventId, newTags);
                    scope.Complete();
                    return new OperationResult<bool>() { Success = true, Message = Messages.TAG_DELETED, Result = true };
                }
                catch (Exception ex)
                {
                    return new OperationResult<bool>() { Success = false, Message = ex.InnerException.Message, Result = false };
                }
            }
        }

        /**
         * This method is just for test purposes.
         * It should not be used for UI Operations, because lacks data verifications
         */
        public async Task<object> UpdateCheckInSubscriber(eventSubscribers sub)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try{
                    var subscribed = await eventSubscribers.GetByIdAsync(sub.eventId, sub.userId);
                    subscribed.checkIn = false;
                    var result = await eventSubscribers.PutAsync(subscribed);
                    sub.checkIn = false;
                    scope.Complete();
                    return new OperationResult<bool>() { Success = true, Message = Messages.CHECKEDIN_SUCESSE, Result = true };
                }
                catch (Exception ex){
                    return new OperationResult<bool>() { Success = false, Message = ex.InnerException.Message };
                }
            }
        }

    }
}
