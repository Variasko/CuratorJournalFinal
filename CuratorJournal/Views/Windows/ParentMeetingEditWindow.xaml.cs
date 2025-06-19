using CuratorJournal.DataBase;
using CuratorJournal.Models;
using System;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace CuratorJournal.Views.Windows
{
    public partial class ParentMeetingEditWindow : Window, INotifyPropertyChanged
    {
        private readonly CuratorJournalEntities _db = new CuratorJournalEntities();
        private readonly ParentMeetingView _currentMeeting;
        private readonly int _groupId;

        private string _topic;
        private DateTime _date = DateTime.Now;
        private string _decision;
        private int _visited;
        private int _notVisitedWithReason;
        private int _notVisited;

        public bool IsSaved { get; private set; } = false;

        public string Topic
        {
            get => _topic;
            set { _topic = value; OnPropertyChanged(nameof(Topic)); }
        }

        public DateTime Date
        {
            get => _date;
            set { _date = value; OnPropertyChanged(nameof(Date)); }
        }

        public string Decision
        {
            get => _decision;
            set { _decision = value; OnPropertyChanged(nameof(Decision)); }
        }

        public int Visited
        {
            get => _visited;
            set { _visited = value; OnPropertyChanged(nameof(Visited)); }
        }

        public int NotVisitedWithReason
        {
            get => _notVisitedWithReason;
            set { _notVisitedWithReason = value; OnPropertyChanged(nameof(NotVisitedWithReason)); }
        }

        public int NotVisited
        {
            get => _notVisited;
            set { _notVisited = value; OnPropertyChanged(nameof(NotVisited)); }
        }

        public ParentMeetingEditWindow(int groupId, ParentMeetingView meeting = null)
        {
            InitializeComponent();
            DataContext = this;
            _groupId = groupId;
            _currentMeeting = meeting;

            if (_currentMeeting != null)
            {
                // Загрузка существующего протокола
                var dbMeeting = _db.ParentMeeting
                    .AsNoTracking()
                    .FirstOrDefault(pm => pm.MeetingId == _currentMeeting.MeetingId);

                if (dbMeeting != null)
                {
                    Topic = dbMeeting.Topic;
                    Decision = dbMeeting.Decision;
                    Visited = dbMeeting.Visited;
                    NotVisitedWithReason = dbMeeting.NotVisitedWithReason;
                    NotVisited = dbMeeting.NotVisited;
                }
            }
            else
            {
                // Установка даты по умолчанию для нового протокола
                Date = DateTime.Today;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Валидация данных
                if (string.IsNullOrWhiteSpace(Topic))
                {
                    MessageBox.Show("Введите тему собрания");
                    return;
                }

                if (string.IsNullOrWhiteSpace(Decision))
                {
                    MessageBox.Show("Введите решение собрания");
                    return;
                }

                if (Visited < 0 || NotVisitedWithReason < 0 || NotVisited < 0)
                {
                    MessageBox.Show("Количество участников не может быть отрицательным");
                    return;
                }

                ParentMeeting meeting;

                if (_currentMeeting == null)
                {
                    // Создание нового протокола
                    meeting = new ParentMeeting
                    {
                        StudyGroupId = _groupId,
                        Topic = Topic,
                        Decision = Decision,
                        Visited = Visited,
                        NotVisitedWithReason = NotVisitedWithReason,
                        NotVisited = NotVisited
                    };
                    _db.ParentMeeting.Add(meeting);
                }
                else
                {
                    // Редактирование существующего
                    meeting = _db.ParentMeeting.Find(_currentMeeting.MeetingId);
                    if (meeting == null)
                    {
                        MessageBox.Show("Протокол не найден");
                        return;
                    }

                    meeting.Topic = Topic;
                    meeting.Decision = Decision;
                    meeting.Visited = Visited;
                    meeting.NotVisitedWithReason = NotVisitedWithReason;
                    meeting.NotVisited = NotVisited;
                }

                _db.SaveChanges();
                IsSaved = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}