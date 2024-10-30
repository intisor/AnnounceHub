using AnnounceHub.AppHub;
using AnnounceHub.Data;
using AnnounceHub.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace AnnounceHub.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class HomeController : Controller
    {
        private readonly IHubContext<AnnouncementHub> _hubContext;
        private readonly AppDbContext _dbContext;

        public HomeController(IHubContext<AnnouncementHub> hubContext, AppDbContext dbContext)
        {
            _hubContext = hubContext;
            _dbContext = dbContext;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            var model = new IndexViewModel
            {
                Announcements = _dbContext.Announcements.ToList()
            };
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Privacy()
        {
            return View();
        }

        [Auth(Username = "Intitech")] 
        public IActionResult SendAnnouncement()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendAnnouncement(SendAnnouncementViewModel model)
        {
                var announcement = new Announcement
                {
                    Message = model.Message,
                    CreatedAt = DateTime.Now
                };

                _dbContext.Announcements.Add(announcement);
                await _dbContext.SaveChangesAsync();
                await _hubContext.Clients.All.SendAsync("ReceiveAnnouncement", announcement.Message);
                return RedirectToAction("Index", "Home");
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
