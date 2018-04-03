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
    public class CommentService
    {
        private static CommentRepository commentRepo;
        private static CommentService commentService ;
        private static CommunityService communityService;
        private static EventService eventService;

        private CommentService()
        {
            commentRepo = new CommentRepository();
            
        }

        public class Messages
        {
            public static string PARAMETERS_NOT_NULL = "Some parameters must not be null";

            public static string COMMENT_CREATED = "Comment Created With Success";
            internal static string COMMENT_NOT_EXIST = "Commnet Does no Exist";

            public static string USER_NO_COMMENT = "This is not the owner of this comment";
            internal static string COMMENT_DELETED = "Comment deleted with sucess";
        }

        public static CommentService GetInstance()
        {
            if (commentService == null) {
                commentService = new CommentService();
                communityService = CommunityService.GetInstance();
                eventService = EventService.GetInstance();
            }
            return commentService;
        }

        public async Task<OperationResult<comment>> GetCommentByIdAsync(int id)
        {
            comment comment = await commentRepo.GetByIdAsync(id);
            if (comment != null)
            {
                return new OperationResult<comment>() { Success = true, Message = "This comment was retrieved with success", Result = comment };
            }
            return new OperationResult<comment>() { Success = false, Message = "Does not exist any comment with this id" };
        }


        public async Task<OperationResult<IEnumerable<comment>>> GetAllCommentsFromCommunity(int communityId)
        {
           var community = await communityService.GetByIdAsync(communityId);
            if (community.Success)
            {
                return new OperationResult<IEnumerable<comment>>() { Success = true, Message = "All comment from community retrieved with success", Result = community.Result.comment.ToList() };
            }
            return new OperationResult<IEnumerable<comment>>() { Success = false, Message = community.Message };
            
        }

        public async Task<OperationResult<IEnumerable<comment>>> GetAllCommentsFromEvent(int eventId)
        {
            var eventRes = await eventService.GetByIdAsync(eventId);
            if (eventRes.Success)
            {
                return new OperationResult<IEnumerable<comment>>() { Success = true, Message = "All comment from community retrieved with success", Result = eventRes.Result.comment.ToList() };
            }
            return new OperationResult<IEnumerable<comment>>() { Success = false, Message = eventRes.Message };
        }

        public async Task<OperationResult<comment>> PostCommentOnEvent(CreateComment item)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                if (!item.ParameterValid()) return new OperationResult<comment>() { Success = false, Message = Messages.PARAMETERS_NOT_NULL };
                item.initialDate = DateTime.Now;
                var eventRes = await eventService.GetByIdAsync(item.eventId);
                if (!eventRes.Success) return new OperationResult<comment>() { Success = false, Message = eventRes.Message };

                try
                {
                    var newComment = await commentRepo.PostAsyncV2(item);
                    scope.Complete();
                    return new OperationResult<comment>() { Success = true, Message = Messages.COMMENT_CREATED, Result = newComment };
                }
                catch (Exception ex)
                {
                    return new OperationResult<comment>() { Success = false, Message = ex.Message };
                }

            }
        }
        public async Task<OperationResult<comment>> PostCommentOnCommunity(CreateComment item)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                if (!item.ParameterValid()) return new OperationResult<comment>() { Success = false, Message = Messages.PARAMETERS_NOT_NULL };
                item.initialDate = DateTime.Now;
                var communityRes = await communityService.GetByIdAsync(item.communityId);
                if (!communityRes.Success) return new OperationResult<comment>() { Success = false, Message = communityRes.Message };

                try
                {
                    var newComment = await commentRepo.PostAsyncV2(item);
                    scope.Complete();
                    return new OperationResult<comment>() { Success = true, Message = Messages.COMMENT_CREATED, Result = newComment };
                }
                catch (Exception ex)
                {
                    return new OperationResult<comment>() { Success = false, Message = ex.Message };
                }

            }
        }

        public async Task<OperationResult<int>> DeleteComment(CreateComment createComment)
        {
            return await Delete(createComment);
        }



        //<------------------------------------------------------------------------------------------------------>
        //<------------------------------------------------------------------------------------------------------>

        private async Task<OperationResult<int>> PostOnEvent(CreateComment item)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                if (!item.ParameterValid()) return new OperationResult<int>() { Success = false, Message = Messages.PARAMETERS_NOT_NULL };
                item.initialDate = DateTime.Now;
                var eventRes = await eventService.GetByIdAsync(item.eventId);
                if (!eventRes.Success) return new OperationResult<int>() { Success = false, Message = eventRes.Message };

                try
                {
                    var id = await commentRepo.PostAsync(item);
                    scope.Complete();
                    return new OperationResult<int>() { Success = true, Message = Messages.COMMENT_CREATED, Result = id };
                }
                catch (Exception ex)
                {
                    return new OperationResult<int>() { Success = false, Message = ex.Message };
                }

            }
        }




        private async Task<OperationResult<int>> Delete(CreateComment item)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var comment = await commentRepo.GetByIdAsync(item.id);
                if (comment == null) return new OperationResult<int>() { Success = false, Message = Messages.COMMENT_NOT_EXIST };
                if (comment.authorId != item.authorId) return new OperationResult<int>() { Success = false, Message = Messages.USER_NO_COMMENT };

                try
                {
                    var id = await commentRepo.DeleteAsync(comment);
                    scope.Complete();
                    return new OperationResult<int>() { Success = true, Message = Messages.COMMENT_DELETED, Result = id };
                }
                catch (Exception ex)
                {
                    return new OperationResult<int>() { Success = false, Message = ex.Message };
                }
            }
        }
    }
}
