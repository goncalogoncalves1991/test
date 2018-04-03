using DataAccess.Models.Create;
using DataAccess.Models.DTOs;
using DataAccess.Repositories;
using Services.Models.Roles;
using Services.Services.Other;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Services.Services
{
    public class UserService
    {
        private UserRepository userRepo;
        private static UserService userService;
        private static CommunityService communityService;
        private static EventService eventService;
        public enum Check_in { True, False };

        public class Messages
        {
            public static string USER_SUCCESS = "The user was retrieved with success";
            public static string USERS_SUCCESS = "Users retrieved with Success";
            public static string USER_NOT_EXIST = "Does not exist any user with this id";
            public static string PARAMETERS_NOT_NULL = "Some parameters must not be null";
            public static string EMAIL_EXISTS = "This email already exists";
            public static string USER_CREATED = "User created with success";
            public static string USER_UPDATED_SUCCESS = "User updated with success";
            public static string USER_DELETED_SUCCESS = "User deleted with success";
        }

        private UserService()
        {
            userRepo = new UserRepository();
        }

        public static UserService GetInstance()
        {
            if (userService == null) {
                userService = new UserService();
                communityService = CommunityService.GetInstance();
                eventService = EventService.GetInstance();
            }
            return userService;
        }
        public async Task<OperationResult<userInfo>> GetByIdAsync(string id)
        {
            userInfo userInfo = await userRepo.GetByIdAsync(id);
            if (userInfo != null)
            {
                return new OperationResult<userInfo>() { Success = true, Message = Messages.USER_SUCCESS, Result = userInfo };
            }
            return new OperationResult<userInfo>() { Success = false, Message = Messages.USER_NOT_EXIST+" ("+id+")" };
        }

        public async Task<OperationResult<IEnumerable<userInfo>>> GetAllAsync()
        {
            IEnumerable<userInfo> users = await userRepo.GetAllAsync();
            return new OperationResult<IEnumerable<userInfo>>() { Success = true, Message = Messages.USERS_SUCCESS, Result = users };
        }
        public async Task<OperationResult<IEnumerable<userInfo>>> GetUserFromCommunityByRole(int communityId, Role role)
        {
            var community = await communityService.GetByIdAsync(communityId);
            if (community.Success)
            {
                IEnumerable<userInfo> users;
                if (Role.Admin == role)
                    users = community.Result.admins;
                else
                    users = community.Result.members;

                return new OperationResult<IEnumerable<userInfo>>() { Success = true, Message = Messages.USERS_SUCCESS, Result = users };
            }
            return new OperationResult<IEnumerable<userInfo>>() { Success = false, Message = community.Message };
          
        }
        public async Task<OperationResult<IEnumerable<userInfo>>> GetUsersSubscribedOnEvent(int eventId, Check_in checkIn)
        {
            var eventRes = await eventService.GetByIdAsync(eventId);
            if (eventRes.Success)
            {
                IEnumerable<userInfo> users;
                if (Check_in.True == checkIn)
                    users = eventRes.Result.eventSubscribers.Where(sub => sub.checkIn == true).Select(sub=>sub.userInfo).ToList();
                else
                    users = eventRes.Result.eventSubscribers.Where(sub => sub.checkIn == false).Select(sub => sub.userInfo).ToList();
                return new OperationResult<IEnumerable<userInfo>>() { Success = true, Message = Messages.USERS_SUCCESS, Result = users };
            }
            return new OperationResult<IEnumerable<userInfo>>() { Success = false, Message = eventRes.Message };
        }
        public async Task<OperationResult<string>> PostUserAsync(CreateUser item)
        {
            return await Post(item);
        }

        public async Task<OperationResult<int>> DeleteUser(CreateUser item)
        {
            return await Delete(item);
        }

        public async Task<OperationResult<string>> UpdateUser(CreateUser item)
        {
            return await Put(item);
        }


        //<------------------------------------------------------------------------------------------------------>
        //<------------------------------------------------------------------------------------------------------>


        private async Task<OperationResult<string>> Post(CreateUser item)
        {
            
            if (!item.ParameterValid()) return new OperationResult<string>() { Success = false, Message = Messages.PARAMETERS_NOT_NULL };

            var users = await userRepo.GetAllAsync();
            if (users.Any(elem => elem.email == item.email)) return new OperationResult<string>() { Success = false, Message = Messages.EMAIL_EXISTS };
               
            try
            {
                string img = null;
                if (item.picture == null)
                {
                    img= (await ImageService.SendToProvider(item.id, ImageService.ImageIdentity.Users, item.profilePicture, ImageService.ImageType.Avatar)).Result;
                    item.picture = img;
                }
                

                var id = await userRepo.PostAsync(item);

                return new OperationResult<string>() { Success = true, Message = Messages.USER_CREATED, Result = (item.picture != null?item.picture:img) };
            }
            catch (Exception ex)
            {
                ImageService.DeleteFolder(item.id, ImageService.ImageIdentity.Users);
                return new OperationResult<string>() { Success = false, Message = ex.Message };

            }
            
        }

        private async Task<OperationResult<string>> Put(CreateUser item)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var user = await userRepo.GetByIdAsync(item.id);
                if (user == null) return new OperationResult<string>() { Success = false, Message = Messages.USER_NOT_EXIST };
                
                try
                {
                    user.name = item.name == null ? user.name : item.name;
                    user.lastName = item.lastName == null ? user.lastName : item.lastName;
                    user.local = item.local == null ? user.local : item.local;
                    if (item.email != null)
                    {
                        var users = await userRepo.GetAllAsync();
                        if (users.Any(elem => elem.email == item.email)) return new OperationResult<string>() { Success = false, Message = Messages.EMAIL_EXISTS };
                        user.email = item.email == null ? user.email : item.email;
                    }

                    var id = await userRepo.PutAsync(user);
                    scope.Complete();
                    return new OperationResult<string>() { Success = true, Message = Messages.USER_UPDATED_SUCCESS, Result = id };
                }
                catch (Exception ex)
                {
                    return new OperationResult<string>() { Success = false, Message = ex.InnerException.Message };
                }
            }
        }

        private async Task<OperationResult<int>> Delete(CreateUser item)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var user = await userRepo.GetByIdAsync(item.id);
                if (user == null) return new OperationResult<int>() { Success = false, Message = Messages.USER_NOT_EXIST };
                
                try
                {
                    var id = await userRepo.DeleteAsync(user);
                    scope.Complete();
                    return new OperationResult<int>() { Success = true, Message = Messages.USER_DELETED_SUCCESS, Result = id };
                }
                catch (Exception ex)
                {
                    return new OperationResult<int>() { Success = false, Message = ex.InnerException.Message };
                }
            }
        }
    }
}
