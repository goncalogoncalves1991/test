using DataAccess.Models.Create;
using Services.Services.Other;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public class ImageCommunityService
    {
        /*private CommunityService service;

        public ImageCommunityService(CommunityService service)
        {
            this.service = service;
        }

        public async Task<OperationResult<string>> InsertProfileImage(int communityId, Stream image, string userId)
        {
            var res = await ImageService.SendToProvider(communityId, ImageService.ImageIdentity.Communities, image, ImageService.ImageType.Avatar);

            if (res.Success)
            {
                var r = await service.UpdateCommunity(new CreateCommunity { Id = communityId, UserId = userId, Avatar = res.Result });
                if (r.Success)
                {
                    return res;
                }
                await ImageService.DeleteFromProvider(res.Message);
                return new OperationResult<string> { Success = false, Message = r.Message };
            }

            return res;
        }


        public async Task<OperationResult<string>> PutProfileImage(int communityId, Stream image, string userId)
        {
            
            return await ImageService.SendToProvider(communityId, ImageService.ImageIdentity.Communities, image, ImageService.ImageType.Avatar);
        }*/
    }
}
