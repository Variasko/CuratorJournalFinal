using System;

namespace CuratorJournal.Models
{
    public class ParentMeetingView
    {
        public int MeetingId { get; set; }
        public string Topic { get; set; }
        public DateTime Date { get; set; }
        public string Decision { get; set; }
        public int Visited { get; set; }
        public int NotVisitedWithReason { get; set; }
        public int NotVisited { get; set; }
    }
}