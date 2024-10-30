using AnnounceHub.AppHub;
using AnnounceHub.Data;
using AnnounceHub.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace AnnounceHub.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IHubContext<AnnouncementHub> _hubContext;
        private readonly AppDbContext _dbContext;

        public AdminController(IHubContext<AnnouncementHub> hubContext, AppDbContext dbContext )
        {
            _hubContext = hubContext;
            _dbContext = dbContext;
        }

        
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
    }
}
