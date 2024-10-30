using AnnounceHub.Data;

namespace AnnounceHub.Models
{
    public class SendAnnouncementViewModel
    {
        public string Message { get; set; }
        public List<Announcement> Announcements { get; set; }
    }
}
