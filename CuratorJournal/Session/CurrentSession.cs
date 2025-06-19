using CuratorJournal.DataBase;

namespace CuratorJournal.Session
{
    internal static class CurrentSession
    {
        public static Curator CurrentCurator { get; set; }
    }
}
