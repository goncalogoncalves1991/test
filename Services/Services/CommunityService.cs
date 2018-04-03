using DataAccess.Models.Create;
using DataAccess.Models.DTOs;
using DataAccess.Repositories;
using Services.Models.Roles;
using Services.Services.Other;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using System.IO;
using Services.Models;
using Services.Utils;

namespace Services.Services
{
    public class CommunityService
    {
        private CommunityRepository communityRepo;
        private CommunitySocialRepository communitySocialRepo;
        private static CommunityService communityService;
        private static UserService userService; 

        public class Messages
        {
            public static string COMMUNITY_NAME = "Community must have at list a Name";
            public static string COMMUNITY_SUCCESS = "The community was retrieved with success";
            public static string COMMUNITIES_SUCCESS = "Communities retrieved with success";
            public static string COMMUNITY_NOT_EXIST = "Does not exist any community with this id";
            public static string COMMUNITY_NOT_EXIST_NAME = "Does not exist any community with this name";
            public static string OPERATION_SUCCESS = "Operation completed with sucess";
            public static string COMMUNITY_CREATED_SUCCESS = "The community was created with success";
            public static string COMMUNITY_UPDATED_SUCCESS = "The community was updated with success";
            public static string COMMUNITY_DELETED_SUCCESS = "The community was deleted with success";
            public static string USER_NOT_PERMISSION = "This user does not have permission to alter this community";
            public static string USER_NOT_ADMIN = "This user is not an administrator of this community";
            public static string COMMUNITY_HAS_MEMBER = "This member already belong to this community";
            public static string COMMUNITY_HAS_NO_MEMBER = "This member does not belong to this community";
            public static string COMMUNITY_MEMBER_DELETED = "The member of this community was deleted with success";
            public static string COMMUNITY_MEMBER_INSERTED = "The member of this community was inserted with success";
            public static string COMMUNITY_HAS_ADMIN = "This user already is an admin of this community";
            public static string COMMUNITY_ADMIN_INSERTED = "The admin of this community was inserted with success";
            public static string COMMUNITY_ADMIN_DELETED = "The admin of this community was deleted with success";
            public static string COMMUNITY_HAS_NO_ADMIN = "This admin does not belong to this community";
            public static string COMMUNITY_TAG_EXIST = "Tag already exists in this community";
            public static string TAG_INSERTED = "Tag(s) inserted with success in this community";
            public static string COMMUNITY_TAG_NOT_EXIST = "Tag(s) does not exist on this community";
            public static string COMMUNITY_TAG_DELETED = "Tag(s) deleted with success in this community";
            public static string PARAMS_INVALID = "The params are invalid";
            public static string COMMUNITY_SOCIAL_CREATED_SUCCESS= "The community social network was associated with success";
            public static string MISSING_LOCATION = "Location object must have latitude, longitude and radius";
        }

        private CommunityService()
        {
            communityRepo = new CommunityRepository();
            communitySocialRepo = new CommunitySocialRepository();
        }
        public static CommunityService GetInstance()
        {
            if (communityService == null)
            {
                communityService = new CommunityService();
                userService = UserService.GetInstance();
            }
            return communityService;
        }

        public async Task<OperationResult<community>> GetByIdAsync(int id)
        {
            community community = await communityRepo.GetByIdAsync(id);
            if (community != null)
            {

                return new OperationResult<community>() { Success = true, Message = Messages.COMMUNITY_SUCCESS, Result = community };
            }
            return new OperationResult<community>() { Success = false, Message = Messages.COMMUNITY_NOT_EXIST };
        }

        
        public async Task<OperationResult<community>> GetByNameAsync(string name)
        {
            community community = await communityRepo.GetByNameAsync(name);
            if (community != null)
            {
                community.comment = community.comment.OrderByDescending(item => item.initialDate).ToList();
                return new OperationResult<community>() { Success = true, Message = Messages.COMMUNITY_SUCCESS, Result = community };
            }
            return new OperationResult<community>() { Success = false, Message = Messages.COMMUNITY_NOT_EXIST_NAME };
        }

        public async Task<OperationResult<IEnumerable<community>>> Search(LocationCoordinates location, string name, string[] tags)
        {
            if(location!=null && location.isEmpty()) new OperationResult<IEnumerable<community>>() { Success = false, Message = Messages.MISSING_LOCATION };

            var box = CoordinatesBoundingBox.GetBoundingBox(new CoordinatesBoundingBox.MapPoint { Longitude = location.longitude, Latitude = location.latitude }, location.radius);

            IEnumerable<community> communities = await communityRepo.GetByParamsAsync(new CoordinatesRange { MaxLatitude=box.MaxPoint.Latitude,MaxLongitude=box.MaxPoint.Longitude,MinLatitude=box.MinPoint.Latitude,MinLongitude=box.MinPoint.Longitude}, name,tags);
            if (communities != null)
            {
                return new OperationResult<IEnumerable<community>>() { Success = true, Message = Messages.COMMUNITY_SUCCESS, Result = communities };
            }
            return new OperationResult<IEnumerable<community>>() { Success = false, Message = Messages.COMMUNITY_NOT_EXIST_NAME };
        }

        public async Task<OperationResult<IEnumerable<community>>> GetAllAsync()
        {
            IEnumerable<community> communities = await communityRepo.GetAllAsync();
            return new OperationResult<IEnumerable<community>>() { Success = true, Message = Messages.OPERATION_SUCCESS, Result = communities };
        }

        public async Task<OperationResult<IEnumerable<community>>> GetCommunitiesFromUserByRole(string userId, Role role)
        {
            var user = await userService.GetByIdAsync(userId);
            if (user.Success)
            {
                IEnumerable<community> communities;
                if (Role.Admin == role)
                    communities = user.Result.admin;
                else
                    communities = user.Result.member;
                return new OperationResult<IEnumerable<community>>() { Success = true, Message = Messages.COMMUNITIES_SUCCESS, Result = communities };
            }
            return new OperationResult<IEnumerable<community>>() { Success = false, Message = user.Message };
        }


        public async Task<OperationResult<int>> CreateCommunity(CreateCommunity item)
        {
             return await Post(item);//extraido para outro método, para caso, este método queira interagir com outras componentes, o código ficar legível
        }

        public async Task<OperationResult<int>> DeleteCommunity(CreateCommunity item)
        {
            return await Delete(item);
        }

        public async Task<OperationResult<int>> UpdateCommunity(CreateCommunity item)
        {
           return await Put(item);
        }

        public async Task<OperationResult<string>> InsertMember(int communityId, string userId)
        {
            return await PostMember(communityId, userId);
        }

        public async Task<OperationResult<string>> RemoveMember(int communityId, string userId)
        {
            return await DeleteMember(communityId, userId);
        }

        public async Task<OperationResult<string>> InsertAdmin(int communityId, string adminId, string userId)
        {
            return await PostAdmin(communityId,adminId, userId);
        }

        public async Task<OperationResult<string>> RemoveAdmin(int communityId, string adminId, string userId)
        {
            return await DeleteAdmin(communityId,adminId, userId);
        }

        public async Task<OperationResult<bool>> InsertTag(int communityId, string adminId, int[] tags)
        {
            return await PostTag(communityId, tags, adminId);
        }

        public async Task<OperationResult<bool>> RemoveTag(int communityId, string adminId, int[] tags)
        {
            return await DeleteTag(communityId, tags, adminId);
        }

        public async Task<OperationResult<bool>> InsertSocialNetwork(CreateCommunitySocial social)
        {
            if(!social.VerifyProperties()) return new OperationResult<bool>() { Success = false, Message = Messages.PARAMS_INVALID };

            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {

                var community = await GetByIdAsync(social.communityId);
                if(!community.Success) return new OperationResult<bool>() { Success = false, Message = community.Message };
                if (!community.Result.admins.Any(item => item.id == social.adminId)) return new OperationResult<bool>() { Success = false, Message = Messages.COMMUNITY_HAS_NO_ADMIN };

                try
                {
                    var res = await communitySocialRepo.PostAsync(social);
                    scope.Complete();
                    return new OperationResult<bool>() { Success = true, Message = Messages.COMMUNITY_SOCIAL_CREATED_SUCCESS, Result = true };
                }
                catch (Exception ex)
                {
                    return new OperationResult<bool>() { Success = false, Message = ex.Message };
                }             
               
            }
        }


        //<------------------------------------------------------------------------------------------------------>
        //<------------------------------------------------------------------------------------------------------>


        private async Task<OperationResult<int>> Put(CreateCommunity item)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var community = await communityRepo.GetByIdAsync(item.Id);
                if (community == null) return new OperationResult<int>() { Success = false, Message = Messages.COMMUNITY_NOT_EXIST };
                if (community.admins.FirstOrDefault(user => user.id == item.UserId) == null) return new OperationResult<int>() { Success = false, Message = Messages.USER_NOT_PERMISSION };

                string img = null;
                if(item.Avatar != null)
                {
                    img = (await ImageService.SendToProvider(item.Name, ImageService.ImageIdentity.Communities, item.Avatar, ImageService.ImageType.Avatar)).Result;   
                }
                try
                {
                    community.avatar = img == null ? community.avatar : img;
                    community.description = item.Description == null ? community.description : item.Description;
                    community.foundationDate = item.FoundationDate == DateTime.MinValue ? community.foundationDate : item.FoundationDate;
                    community.local = item.Local == null ? community.local : item.Local;
                    community.site = item.Site == null ? community.site : item.Site;
                    community.mail = item.Mail == null ? community.mail : item.Mail;
                    community.gitHub = item.GitHub == null ? community.gitHub : item.GitHub;

                    var id = await communityRepo.PutAsync(community);
                    scope.Complete();
                    return new OperationResult<int>() { Success = true, Message = Messages.COMMUNITY_UPDATED_SUCCESS, Result = id };
                }
                catch (Exception ex)
                {
                    return new OperationResult<int>() { Success = false, Message = ex.InnerException.Message };
                }
            }
        }

        private async Task<OperationResult<int>> Post(CreateCommunity item)
        {
            
            if (item.Name == null) return new OperationResult<int>() { Success = false, Message = Messages.COMMUNITY_NAME };
            var user = await userService.GetByIdAsync(item.UserId);
            if (!user.Success) return new OperationResult<int>() { Success = false, Message = user.Message };

                
            try
            {
                if (item.Tags != null) item.Tags = item.Tags.Distinct().ToArray();
                if (item.Sponsors != null) item.Sponsors = item.Sponsors.Distinct().ToArray();


                string img = (await ImageService.SendToProvider(item.Name, ImageService.ImageIdentity.Communities, item.Avatar, ImageService.ImageType.Avatar)).Result;
                item.AvatarLink = img;
                var id = await communityRepo.PostAsync(item);
                    
                return new OperationResult<int>() { Success = true, Message = Messages.COMMUNITY_CREATED_SUCCESS, Result = id };
            }
            catch (Exception ex)
            {
                ImageService.DeleteFolder(item.Name, ImageService.ImageIdentity.Communities);
                return new OperationResult<int>() { Success = false, Message = ex.Message };
            }
            
        }

        private async Task<OperationResult<int>> Delete(CreateCommunity item)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var community = await GetByIdAsync(item.Id);
                if (!community.Success) return new OperationResult<int>() { Success = false, Message = community.Message };
                if (!community.Result.admins.Select(elem => elem.id).Contains(item.UserId)) return new OperationResult<int>() { Success = false, Message = "the user with id(" + item.UserId + ") does not have permission to delete this community" };

                try
                {
                    var id = await communityRepo.DeleteAsync(community.Result);
                    scope.Complete();
                    ImageService.DeleteFolder(item.Name, ImageService.ImageIdentity.Communities);
                    return new OperationResult<int>() { Success = true, Message = Messages.COMMUNITY_DELETED_SUCCESS, Result = id };
                }
                catch (Exception ex)
                {
                    return new OperationResult<int>() { Success = false, Message = ex.InnerException.Message };
                }
            }
        }

        private async Task<OperationResult<string>> PostMember(int communityId, string userId)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var community = await GetByIdAsync(communityId);
                var user = await userService.GetByIdAsync(userId);

                if (!community.Success) return new OperationResult<string>() { Success = false, Message = community.Message };
                if (!user.Success) return new OperationResult<string>() { Success = false, Message = user.Message };
                if (community.Result.members.Select(elem => elem.id).Contains(userId)) return new OperationResult<string>() { Success = false, Message = Messages.COMMUNITY_HAS_MEMBER };

                try
                {
                    var id = await communityRepo.InsertMember(communityId, userId);
                    scope.Complete();
                    return new OperationResult<string>() { Success = true, Message = Messages.COMMUNITY_MEMBER_INSERTED, Result = id };
                }
                catch (Exception ex)
                {
                    return new OperationResult<string>() { Success = false, Message = ex.InnerException.Message };
                }
            }
        }

        private async Task<OperationResult<string>> DeleteMember(int communityId, string userId)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var community = await GetByIdAsync(communityId);
                var user = await userService.GetByIdAsync(userId);

                if (!community.Success) return new OperationResult<string>() { Success = false, Message = community.Message };
                if (!user.Success) return new OperationResult<string>() { Success = false, Message = user.Message };
                if (!community.Result.members.Select(elem => elem.id).Contains(userId)) return new OperationResult<string>() { Success = false, Message = Messages.COMMUNITY_HAS_NO_MEMBER };

                try
                {
                    var id = await communityRepo.DeleteMember(communityId, userId);
                    scope.Complete();
                    return new OperationResult<string>() { Success = true, Message = Messages.COMMUNITY_MEMBER_DELETED, Result = id };
                }
                catch (Exception ex)
                {
                    return new OperationResult<string>() { Success = false, Message = ex.InnerException.Message };
                }
            }

        }

        private async Task<OperationResult<string>> PostAdmin(int communityId, string adminId, string userId)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var community = await GetByIdAsync(communityId);
                var user = await userService.GetByIdAsync(userId);
                //var admin = await userService.GetByIdAsync(adminId);

                if (!community.Success) return new OperationResult<string>() { Success = false, Message = community.Message };
                //if (!admin.Success) return new OperationResult<int>() { Success = false, Message = user.Message };
                if (!community.Result.admins.Select(elem => elem.id).Contains(adminId)) return new OperationResult<string>() { Success = false, Message = Messages.USER_NOT_ADMIN };
                if (!user.Success) return new OperationResult<string>() { Success = false, Message = user.Message };
                if (community.Result.admins.Select(elem => elem.id).Contains(userId)) return new OperationResult<string>() { Success = false, Message = Messages.COMMUNITY_HAS_ADMIN };

                try
                {
                    var id = await communityRepo.InsertAdmin(communityId, userId);
                    scope.Complete();
                    return new OperationResult<string>() { Success = true, Message = Messages.COMMUNITY_ADMIN_INSERTED, Result = id };
                }
                catch (Exception ex)
                {
                    return new OperationResult<string>() { Success = false, Message = ex.InnerException.Message };
                }
            }
        }

        private async Task<OperationResult<string>> DeleteAdmin(int communityId, string adminId, string userId)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var community = await GetByIdAsync(communityId);

                if (!community.Success) return new OperationResult<string>() { Success = false, Message = community.Message };
                if (!community.Result.admins.Select(elem => elem.id).Contains(adminId)) return new OperationResult<string>() { Success = false, Message = Messages.USER_NOT_ADMIN };
                if (!community.Result.admins.Select(elem => elem.id).Contains(userId)) return new OperationResult<string>() { Success = false, Message = Messages.COMMUNITY_HAS_NO_ADMIN };

                try
                {
                    var id = await communityRepo.DeleteAdmin(communityId, userId);
                    scope.Complete();
                    return new OperationResult<string>() { Success = true, Message = Messages.COMMUNITY_ADMIN_DELETED, Result = id };
                }
                catch (Exception ex)
                {
                    return new OperationResult<string>() { Success = false, Message = ex.InnerException.Message };
                }
            }
        }

        private async Task<OperationResult<bool>> PostTag(int communityId, int[] tags, string adminId)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var community = await GetByIdAsync(communityId);

                if (!community.Success) return new OperationResult<bool>() { Success = false, Message = community.Message };
                if (!community.Result.admins.Select(elem => elem.id).Contains(adminId)) return new OperationResult<bool>() { Success = false, Message = Messages.USER_NOT_ADMIN };

                var newTags = tags.Distinct().ToArray();
                //check if tags exist when have the tags service....
                if (community.Result.tag.Any(elem => newTags.Contains(elem.id))) return new OperationResult<bool>() { Success = false, Message = Messages.COMMUNITY_TAG_EXIST };

                try
                {
                    var success = await communityRepo.InsertTag(communityId, newTags);
                    scope.Complete();
                    return new OperationResult<bool>() { Success = true, Message = Messages.TAG_INSERTED, Result=true };
                }
                catch (Exception ex)
                {
                    return new OperationResult<bool>() { Success = false, Message = ex.InnerException.Message, Result = false };
                }
            }
        }

        private async Task<OperationResult<bool>> DeleteTag(int communityId, int[] tags, string adminId)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var community = await GetByIdAsync(communityId);

                if (!community.Success) return new OperationResult<bool>() { Success = false, Message = community.Message };
                if (!community.Result.admins.Select(elem => elem.id).Contains(adminId)) return new OperationResult<bool>() { Success = false, Message = Messages.USER_NOT_ADMIN };
                var newTags = tags.Distinct().ToArray();
                var currentTags = community.Result.tag.Select(x => x.id);

                if (!newTags.All(elem => currentTags.Contains(elem))) return new OperationResult<bool>() { Success = false, Message = Messages.COMMUNITY_TAG_NOT_EXIST };

                try
                {
                    var id = await communityRepo.DeleteTag(communityId, newTags);
                    scope.Complete();
                    return new OperationResult<bool>() { Success = true, Message = Messages.COMMUNITY_TAG_DELETED, Result = true };
                }
                catch (Exception ex)
                {
                    return new OperationResult<bool>() { Success = false, Message = ex.InnerException.Message, Result = false };
                }
            }
        }

    }
}
