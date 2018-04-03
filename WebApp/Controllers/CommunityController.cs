using DataAccess.Models.Create;
using Services.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebApp.Models;
using Microsoft.AspNet.Identity;
using DataAccess.Models.DTOs;
using WebApp.Extensions;

namespace WebApp.Controllers
{

    [RoutePrefix("community")]
    public class CommunityController : Controller
    {

        // GET: Community/Details/5
        [Route("{id}")]
        public async Task<ActionResult> Get(string id)
        {
            CommunityService service = CommunityService.GetInstance();
            var res = await service.GetByNameAsync(id);
            if (res.Success)
            {
                var principal = await Request.GetOwinContext().Authentication.AuthenticateAsync("OAuth");
                if (principal == null) ViewBag.token = "none";
                else{
                    var token = principal.Identity.FindFirst("access_token").Value;
                    ViewBag.token = token;
                }

                ViewBag.hasEvents = res.Result.@event.Count != 0 ? true : false;
                ViewBag.hasNotices = res.Result.notice.Count != 0 ? true : false;
                ViewBag.Owner = res.Result.admins.Any(u => u.id == User.Identity.GetUserId());
                ViewBag.Twitter = getCommunityTwitterFromDB(res.Result); 
                res.Result.@event = res.Result.@event.OrderBy(x => x.initDate).Reverse().ToList();
                res.Result.notice = res.Result.notice.OrderBy(x => x.initialDate).Reverse().ToList();

                return View(res.Result);
            }
            //mandar no viewBag a mensagem de erro para depois ser apresentada na view do Index

            ViewBag.Error = true;
            ViewBag.ErrorMessage = res.Message;
            return RedirectToAction("index");
        }

        // GET: Community/Create
        [Authorize]
        [Route("create")]
        public ActionResult Create()
        {
            ViewBag.Error = false;

            return View();
        }

        // POST: Community/Create

        [Authorize]
        [HttpPost]
        [Route("create")]
        public async Task<ActionResult> Create(Community community)
        {

            if (ModelState.IsValid)
            {
                CommunityService service = CommunityService.GetInstance();
                //ImageCommunityService imgService = new ImageCommunityService(service);

                CreateCommunity newCommunity = new CreateCommunity()
                {
                    Name = community.Name,
                    Description = community.Description,
                    Local = community.Local,
                    FoundationDate = community.date,
                    Avatar = community.Avatar == null?null:community.Avatar.InputStream,                  
                    UserId = User.Identity.GetUserId(),
                    Latitude = community.Latitude,
                    Longitude = community.Longitude
                };
                var res = await service.CreateCommunity(newCommunity);

                if (res.Success) {
                 
                    return RedirectToAction("Get", new { id = community.Name }); }
                else
                {
                    ViewBag.Error = true;
                    ViewBag.Message = res.Message;
                }
                return View(community);

            }
            else
            {
                return View(community);
            }
        }

        [Authorize]
        [HttpPost]
        [Route("UploadPhoto")]
        public async Task<ActionResult> UploadPhoto(HttpPostedFileBase[] files)
        {
            return null;
        }


        [Authorize]
        [HttpPost]
        [Route("CreateEvent")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateEvent(Event eve)
        {

            if (ModelState.IsValid)
            {
                EventService service = EventService.GetInstance();

                CreateEvent newEvent = new CreateEvent()
                {
                    title = eve.Title,
                    communityId = eve.communityId,
                    description = eve.Description,
                    endDate = DateTime.Parse(eve.EndDate),
                    initDate = DateTime.Parse(eve.InitDate),
                    local = eve.Local,
                    nrOfTickets = eve.NrOfTickets,
                    latitude = eve.Latitude,
                    longitude = eve.Longitude,

                    UserId = User.Identity.GetUserId()
                };
                var res = await service.PostEventAsync(newEvent);// retorna o id do evento que foi criado se quiser fazer redirect falta o nome da comunidade

                if (res.Success){

                    return Json(new { url = Constants.WebAPPAddress + "/community/" + eve.communityName + "/event/" + res.Result.id });
                }

                else
                {
                    //tratar do erro que ainda falta pode dar um erro na inserção
                    ViewBag.Error = true;
                    ViewBag.Message = res.Message;
                }
            }
            
            return PartialView("_CreateEvent",eve);

        }

        [Authorize]
        [HttpPost]
        [Route("CreateNews")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateNews(News news)
        {
            if (ModelState.IsValid)
            {
                NoticeService service = NoticeService.GetInstance();

                CreateNotice newNews = new CreateNotice()
                {
                    title = news.Title,
                    communityId = news.CommunityId,
                    description = news.Description,
                    initialDate = DateTime.Now,
                    userId = User.Identity.GetUserId()
                };
                var res = await service.PostNoticeAsync(newNews);// retorna o id do evento que foi criado se quiser fazer redirect falta o nome da comunidade

                if (res.Success)
                {
                    return Json(new { url = Constants.WebAPPAddress+"/community/"+ news.CommunityName  });
                }

                else
                {
                    //tratar do erro que ainda falta pode dar um erro na inserção
                    ViewBag.Error = true;
                    ViewBag.Message = res.Message;
                }
            }

            return PartialView("_CreateEvent", news);
        
        }

        [Authorize]
        [HttpPost]
        [Route("PutCommunity")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> PutCommunity(Community community)
        {

            CommunityService service = CommunityService.GetInstance();


            CreateCommunity newCommunity = new CreateCommunity()
            {
                Id = community.Id,
                Name = community.Name,
                Description = community.Description,
                Local = community.Local,
                FoundationDate = community.date,
                Site = community.Site,
                GitHub = community.GitHub,
                Mail = community.Mail,
                Avatar = community.Avatar == null ?null: community.Avatar.InputStream,
                UserId = User.Identity.GetUserId()
            };
           

            var updated = await service.UpdateCommunity(newCommunity);
            if (updated.Success)
            {

                return Redirect(Request.UrlReferrer.PathAndQuery);
            }
            return PartialView("_UpdateCommunity", community);
           

        }


        [Authorize]
        [HttpPost]
        [Route("DeleteCommunity")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteCommunity(DeleteCommunity community)
        {
            CommunityService service = CommunityService.GetInstance();
            var res = await service.GetByIdAsync(community.Id);
            if (res.Success)
            {
                if(res.Result.name == community.Name)
                {
                    var deleted =await service.DeleteCommunity(new CreateCommunity { Id = community.Id, UserId = User.Identity.GetUserId() });
                    if(deleted.Success)
                        return RedirectToAction("Index", "Home");

                }
                else
                {
                    return PartialView("_DeleteCommunity", community);
                } 

            }

            return View();
        }

        
        

        private object getCommunityTwitterFromDB(community res)
        {
            var r = res.CommunitySocialNetwork.FirstOrDefault(item => item.provider == "Twitter");
            if (r!=null)
            {
                return new Twitter (){ HasToken = true, Token = r.Acess_Token, NotInDB = false };
            }
            if(Session["AccessTokenTwitterSecret"] !=null && Session["AccessTokenTwitterValue"] != null)
                return new Twitter() { HasToken = true, Token = Session["AccessTokenTwitterSecret"].ToString()+":"+ Session["AccessTokenTwitterValue"].ToString(), NotInDB = true };

            return new Twitter(){ HasToken = false, NotInDB = true };
        }

        public class Twitter
        {
            public bool HasToken { get; internal set; }
            public bool NotInDB { get; internal set; }
            public string Token { get; internal set; }            
        }

       
    }
}
