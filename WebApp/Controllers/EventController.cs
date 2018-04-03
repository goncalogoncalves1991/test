using DataAccess.Models.Create;
using Services.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebApp.Models;
using Microsoft.AspNet.Identity;
using WebApp.Models.Delete;

namespace WebApp.Controllers
{
    
    public class EventController : Controller
    {

        // GET: Community/{communityId}/Event/{eventId}
        [Route("community/{communityId}/Event/{eventId}")]
        public async Task<ActionResult> Get(string communityId, int eventId)
        {
            EventService service = EventService.GetInstance();

            var res = await service.GetByIdAsync(eventId);
           
            if (res.Success)
            {
                var principal = await Request.GetOwinContext().Authentication.AuthenticateAsync("OAuth");
                ViewBag.Owner = res.Result.community.admins.Any(u => u.id == User.Identity.GetUserId());
                if (principal == null) ViewBag.token = "none";
                else
                {
                    var token = principal.Identity.FindFirst("access_token").Value;
                    ViewBag.token = token;
                }
                ViewBag.days = (int)(res.Result.endDate - res.Result.initDate).TotalDays +1;
                //res.Result.session= res.Result.session.OrderBy(x => x.initialDate).ToList();
                ViewBag.sessions = res.Result.session
                                .OrderBy(x => x.initialDate)
                                .GroupBy(p => p.initialDate.Date)
                                .Select(g => g.ToList())
                                .ToArray();


                return View(res.Result);
            }
            else
            {
                return RedirectToAction("Index","Home");
            }
            
        }

        [Authorize]
        [HttpPost]
        [Route("CreateSession")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateSession(Session session)
        {

            if (ModelState.IsValid)
            {
                SessionService service = SessionService.GetInstance();

                CreateSession newSession = new CreateSession()
                {
                    title = session.Title,
                    description = session.Description,
                    eventId = session.EventId,
                    initialDate = DateTime.Parse(session.StartDate),
                    endDate = DateTime.Parse(session.EndDate),
                    speakerName = session.SpeakerName,
                    lastName = session.LastName,
                    linkOfSpeaker= session.LinkOfSpeaker,
                    userId = User.Identity.GetUserId()
                };
                var res = await service.PostSessionAsync(newSession);// retorna o id da sessão que foi criado se quiser fazer redirect falta o nome da comunidade

                if (res.Success)
                {
                    //Falta meter o redirect como deve ser
                    //return Json(new { url = Constants.WebAPPAddress + "/Home/"  });
                    return Redirect(Request.UrlReferrer.PathAndQuery );
                }

                else
                {
                    //tratar do erro que ainda falta pode dar um erro na inserção
                    
                    ViewBag.Message = res.Message;
                }
            }

            return PartialView("_CreateSession", session);

        }

        [Authorize]
        [HttpPost]
        [Route("PutEvent")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> PutEvent(Event eve)
        {
            if (ModelState.IsValid)
            {
                EventService service = EventService.GetInstance();


                CreateEvent newEvent = new CreateEvent()
                {
                    Id = eve.Id,
                    title = eve.Title,
                    description = eve.Description,
                    local = eve.Local,
                    initDate = DateTime.Parse(eve.InitDate),
                    endDate = DateTime.Parse(eve.EndDate),
                    nrOfTickets = eve.NrOfTickets,
                    latitude = eve.Latitude,
                    longitude = eve.Longitude,
                    UserId = User.Identity.GetUserId()
                };


                var updated = await service.UpdateEvent(newEvent);
                if (updated.Success)
                {

                    return Redirect(Request.UrlReferrer.PathAndQuery);
                }
            }
            
            return PartialView("_UpdateEvent", eve);

        }

        [Authorize]
        [HttpPost]
        [Route("DeleteEvent")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteEvent(DeleteEvent eve)
        {
            EventService service = EventService.GetInstance();
            var res = await service.GetByIdAsync(eve.Id);
            if (res.Success)
            {
                if (res.Result.title == eve.Name)
                {
                    var deleted = await service.DeleteEvent(new CreateEvent { Id = eve.Id, UserId = User.Identity.GetUserId() });
                    if (deleted.Success)
                        return Redirect(Request.UrlReferrer.PathAndQuery);

                }
                else
                {
                    return PartialView("_DeleteCommunity", eve);
                }

            }

            return View();
        }

        
    }
}
