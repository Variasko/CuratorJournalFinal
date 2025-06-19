namespace CuratorJournal.Models
{
    public class StudentSocialView
    {
        public int StudentId { get; set; }
        public int SocialStatusId { get; set; }
        public string SocialStatusName { get; set; }
        public string StudentFullName { get; set; }

        // Добавим идентификатор связи для точного удаления
        public int StudentSocialStatusId { get; set; }
    }
}