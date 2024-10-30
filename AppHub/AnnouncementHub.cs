using Microsoft.AspNetCore.SignalR;
namespace AnnounceHub.AppHub
{
    public class AnnouncementHub : Hub
    {
        public async Task SendAnnouncement(string message)
        {
            await Clients.All.SendAsync("ReceiveAnnouncement", message);
        }
    }
}
