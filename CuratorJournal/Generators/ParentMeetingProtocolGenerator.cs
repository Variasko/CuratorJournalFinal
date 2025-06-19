using CuratorJournal.DataBase;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CuratorJournal.Reports
{
    public class ParentMeetingProtocolGenerator
    {
        private readonly CuratorJournalEntities _db;

        public ParentMeetingProtocolGenerator(CuratorJournalEntities db)
        {
            _db = db;
        }

        public void GenerateReport(string filePath, StudyGroup studyGroup)
        {
            using (WordprocessingDocument document = WordprocessingDocument.Create(filePath, WordprocessingDocumentType.Document))
            {
                MainDocumentPart mainPart = document.AddMainDocumentPart();
                mainPart.Document = new Document();
                Body body = new Body();

                // Получаем все собрания для данной группы
                var meetings = _db.ParentMeeting.Where(m => m.StudyGroupId == studyGroup.StudyGroupId).ToList();

                if (meetings.Count == 0)
                {
                    throw new InvalidOperationException("Нет данных о родительских собраниях для выбранной группы.");
                }

                foreach (var meeting in meetings)
                {
                    // Разрыв страницы перед каждым протоколом
                    if (body.ChildElements.Count > 0)
                    {
                        body.Append(new Paragraph(new Run(new Break() { Type = BreakValues.Page })));
                    }

                    // Заголовок: Протокол родительского собрания №____ от «__»________20__г.
                    Paragraph title1 = CreateCenteredParagraph("Протокол родительского собрания №____");
                    body.Append(title1);

                    Paragraph dateLine = CreateCenteredParagraph($"от «__»________20__г.");
                    body.Append(dateLine);

                    // Приглашённые / Присутствовали / Отсутствовали
                    int invitedCount = GetInvitedParentsCount(studyGroup.StudyGroupId);
                    int visitedCount = meeting.Visited;
                    int notVisitedWithReasonCount = meeting.NotVisitedWithReason;
                    int notVisitedCount = meeting.NotVisited;

                    body.Append(CreateMeetingInfo(invitedCount, visitedCount, notVisitedWithReasonCount, notVisitedCount));

                    // Тема собрания
                    body.Append(CreateParagraph($"Тема собрания: {meeting.Topic}"));

                    // Выступили
                    body.Append(CreateParagraph("По теме собрания выступили:"));

                    // 5–8 строк подчёркиваний
                    for (int i = 0; i < 8; i++)
                    {
                        body.Append(CreateUnderlinedLine());
                    }

                    // Решение собрания
                    body.Append(CreateParagraph($"В ходе собрания решено: {meeting.Decision}"));

                    // Куратор
                    string curatorName = GetCuratorFullName(studyGroup);
                    body.Append(CreateParagraph($"Куратор: ___________/{curatorName}"));

                    // Председатель родительского комитета
                    body.Append(CreateParagraph("Председатель родительского комитета (при наличии): ______________/_________"));
                }

                // Добавляем всё в документ
                mainPart.Document.Append(body);
                mainPart.Document.Save();
            }
        }

        private static Paragraph CreateCenteredParagraph(string text)
        {
            return new Paragraph(
                new Run(new Text(text))
                {
                    RunProperties = new RunProperties(new FontSize() { Val = "24" })
                }
            )
            {
                ParagraphProperties = new ParagraphProperties(
                    new Justification() { Val = JustificationValues.Center }
                )
            };
        }

        private static Paragraph CreateParagraph(string text)
        {
            return new Paragraph(
                new Run(new Text(text))
                {
                    RunProperties = new RunProperties(new FontSize() { Val = "22" })
                }
            );
        }

        private static Paragraph CreateUnderlinedLine()
        {
            return new Paragraph(
                new Run(
                    new Text("")
                )
                {
                    RunProperties = new RunProperties(
                        new Underline() { Val = UnderlineValues.Single }
                    )
                }
            );
        }

        private static Paragraph CreateMeetingInfo(int invitedCount, int visitedCount, int notVisitedWithReasonCount, int notVisitedCount)
        {
            return new Paragraph(
                new Run(new Text($"Приглашённые: {invitedCount}\n\n")),
                new Run(new Text($"Присутствовали: {visitedCount}\n\n")),
                new Run(new Text($"Отсутствовали по уважительной причине: {notVisitedWithReasonCount}\n\n")),
                new Run(new Text($"Отсутствовали: {notVisitedCount}"))
            );
        }

        private string GetCuratorFullName(StudyGroup studyGroup)
        {
            if (studyGroup?.Curator?.Person == null)
                return "___________________";

            Person person = studyGroup.Curator.Person;
            return $"{person.Surname} {person.Name} {person.Patronymic}";
        }

        private int GetInvitedParentsCount(int studyGroupId)
        {
            // Здесь должен быть запрос к БД для получения количества родителей
            // Пример:
            // using (var context = new CuratorJournalEntities())
            // {
            //     return context.Student.Count(s => s.StudyGroup.Any(g => g.StudyGroupId == studyGroupId));
            // }
            return 30; // Тестовое значение
        }
    }
}