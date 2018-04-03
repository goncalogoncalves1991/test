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
    public class NoticeService
    {
        private NoticeRepository noticeRepo;
        private static CommunityService communityService;
        private static NoticeService noticeService;
        private NoticeService()
        {
            noticeRepo = new NoticeRepository();
        }

        public class Messages
        {
            public static string NOTICE_SUCCESS = "The notice was retrieved with success";
            public static string NOTICE_NOT_EXIST = "Does not exist any notice with this id";
            public static string NOTICES_SUCCESS = "All notices retrieved with success";
            public static string PARAMETERS_NOT_NULL = "Some parameters must not be null";
            public static string USER_NO_PERMISSION = "This user does not have permission to insert a notice into this community";
            public static string NOTICE_CREATED = "The notice was created with success";
            public static string NOTICE_UPDATED = "The notice was updated with success";
            public static string NOTICE_DELETED_SUCCESS = "The notice was deleted with success";
           
        }

        public static NoticeService GetInstance()
        {
            if (noticeService == null) {
                noticeService = new NoticeService();
                communityService = CommunityService.GetInstance();
            }
            return noticeService;
        }

        public async Task<OperationResult<notice>> GetByIdAsync(int id)
        {
            notice notice = await noticeRepo.GetByIdAsync(id);
            if (notice != null)
            {
                return new OperationResult<notice>() { Success = true, Message = Messages.NOTICE_SUCCESS, Result = notice };
            }
            return new OperationResult<notice>() { Success = false, Message = Messages.NOTICE_NOT_EXIST};
        }

        public async Task<OperationResult<IEnumerable<notice>>> GetAllAsync()
        {
            IEnumerable<notice> notices = await noticeRepo.GetAllAsync();
            return new OperationResult<IEnumerable<notice>>() { Success = true, Message = Messages.NOTICES_SUCCESS, Result = notices };
        }
        public async Task<OperationResult<IEnumerable<notice>>> GetNoticesFromCommunity(int communityId)
        {
            var notices = await communityService.GetByIdAsync(communityId);
            if (notices.Success)
            {
                return new OperationResult<IEnumerable<notice>>() { Success = true, Message = Messages.NOTICES_SUCCESS, Result = notices.Result.notice.ToList() };
            }
            return new OperationResult<IEnumerable<notice>>() { Success = false, Message = notices.Message };
            
        }

        public async Task<OperationResult<int>> PostNoticeAsync(CreateNotice item)
        {
            return await Post(item);
        }

        public async Task<OperationResult<int>> DeleteNotice(CreateNotice item)
        {
            return await Delete(item);
        }

        public async Task<OperationResult<int>> UpdateNotice(CreateNotice item)
        {
            return await Put(item);
        }


        //<------------------------------------------------------------------------------------------------------>
        //<------------------------------------------------------------------------------------------------------>


        private async Task<OperationResult<int>> Post(CreateNotice item)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                if (!item.ParameterValid()) return new OperationResult<int>() { Success = false, Message = Messages.PARAMETERS_NOT_NULL };

                var community = await communityService.GetByIdAsync(item.communityId);
                if (!community.Success) return new OperationResult<int>() { Success = false, Message = community.Message };
                if (!community.Result.admins.Any(elem => elem.id == item.userId)) return new OperationResult<int>() { Success = false, Message = Messages.USER_NO_PERMISSION };

                try
                {
                    var id = await noticeRepo.PostAsync(item);
                    scope.Complete();
                    return new OperationResult<int>() { Success = true, Message = Messages.NOTICE_CREATED, Result = id };
                }
                catch (Exception ex)
                {
                    return new OperationResult<int>() { Success = false, Message = ex.Message };
                }
            }
        }

        private async Task<OperationResult<int>> Put(CreateNotice item)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var notice = await noticeRepo.GetByIdAsync(item.id);
                if (notice==null) return new OperationResult<int>() { Success = false, Message = Messages.NOTICE_NOT_EXIST};
                if (!notice.community.admins.Any(elem => elem.id == item.userId)) return new OperationResult<int>() { Success = false, Message = Messages.USER_NO_PERMISSION };

                try
                {
                    notice.title = item.title == null ? notice.title : item.title;
                    notice.description = item.description == null ? notice.description : item.description;
                    var id = await noticeRepo.PutAsync(notice);
                    scope.Complete();
                    return new OperationResult<int>() { Success = true, Message = Messages.NOTICE_UPDATED, Result = id };
                }
                catch (Exception ex)
                {
                    return new OperationResult<int>() { Success = false, Message = ex.Message };
                }
            }
        }

        private async Task<OperationResult<int>> Delete(CreateNotice item)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var notice = await noticeRepo.GetByIdAsync(item.id);
                if (notice == null) return new OperationResult<int>() { Success = false, Message = Messages.NOTICE_NOT_EXIST };
                if (!notice.community.admins.Any(elem => elem.id == item.userId)) return new OperationResult<int>() { Success = false, Message = Messages.USER_NO_PERMISSION };

                try
                {
                    var id = await noticeRepo.DeleteAsync(notice);
                    scope.Complete();
                    return new OperationResult<int>() { Success = true, Message = Messages.NOTICE_DELETED_SUCCESS, Result = id };
                }
                catch (Exception ex)
                {
                    return new OperationResult<int>() { Success = false, Message = ex.InnerException.Message };
                }
            }
        }
    }
}
