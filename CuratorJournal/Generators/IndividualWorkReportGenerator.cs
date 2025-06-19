using CuratorJournal.DataBase;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CuratorJournal.Reports
{
    public class IndividualWorkReportGenerator
    {
        private readonly CuratorJournalEntities _db;

        public IndividualWorkReportGenerator(CuratorJournalEntities db)
        {
            _db = db;
        }

        /// <summary>
        /// Генерирует отчёт по студентам (IndividualWork с IsStudent = true)
        /// </summary>
        public void GenerateStudentReport(string filePath, StudyGroup studyGroup)
        {
            var studentIdsInGroup = _db.Student
                .Where(s => s.StudyGroup.Any(g => g.StudyGroupId == studyGroup.StudyGroupId))
                .Select(s => s.StudentId)
                .ToList();

            var studentWorks = _db.IndividualWork
                .Where(iw => iw.StudentId != null && studentIdsInGroup.Contains((int)iw.StudentId))
                .ToList();

            if (!studentWorks.Any())
                throw new InvalidOperationException("Нет данных для формирования отчёта.");

            GenerateReport(filePath, studentWorks, isStudent: true);
        }

        /// <summary>
        /// Генерирует отчёт по родителям (IndividualWork с IsStudent = false)
        /// </summary>
        public void GenerateParentReport(string filePath, StudyGroup studyGroup)
        {
            var studentIdsInGroup = _db.Student
                .Where(s => s.StudyGroup.Any(g => g.StudyGroupId == studyGroup.StudyGroupId))
                .Select(s => s.StudentId)
                .ToList();

            var parentIds = _db.Parent
                .Where(p => p.Student.Any(s => studentIdsInGroup.Contains(s.StudentId)))
                .Select(p => p.ParentId)
                .ToList();

            var parentWorks = _db.IndividualWork
                .Where(iw => iw.ParentId != null && parentIds.Contains((int)iw.ParentId))
                .ToList();

            if (!parentWorks.Any())
                throw new InvalidOperationException("Нет данных для формирования отчёта.");

            GenerateReport(filePath, parentWorks, isStudent: false);
        }

        private void GenerateReport(string filePath, IEnumerable<IndividualWork> works, bool isStudent)
        {
            using (WordprocessingDocument document = WordprocessingDocument.Create(filePath, WordprocessingDocumentType.Document))
            {
                MainDocumentPart mainPart = document.AddMainDocumentPart();
                mainPart.Document = new Document();
                Body body = new Body();

                // Заголовок
                Paragraph title = CreateCenteredParagraph(isStudent ? "Отчёт по индивидуальной работе со студентами" : "Отчёт по индивидуальной работе с родителями");
                body.Append(title);

                // Таблица
                Table table = CreateTable(works, isStudent);
                body.Append(table);

                mainPart.Document.Append(body);
                mainPart.Document.Save();
            }
        }

        private Table CreateTable(IEnumerable<IndividualWork> works, bool isStudent)
        {
            Table table = new Table();

            // Стиль таблицы
            table.Append(
                new TableProperties(
                    new TableStyle() { Val = "TableGrid" },
                    new TableWidth() { Width = "5000", Type = TableWidthUnitValues.Dxa }
                )
            );

            // Шапка таблицы
            TableRow headerRow = new TableRow();
            headerRow.Append(
                CreateTableCell("Дата"),
                CreateTableCell(isStudent ? "Студент" : "Родитель"),
                CreateTableCell("Тема работы"),
                CreateTableCell("Решение")
            );
            ApplyHeaderStyle(headerRow);
            table.Append(headerRow);

            // Данные
            foreach (var work in works)
            {
                TableRow row = new TableRow();

                row.Append(
                    CreateTableCell(work.Date.ToString("dd.MM.yyyy")),
                    CreateTableCell(isStudent
                        ? $"{work.Student?.Person?.Surname} {work.Student?.Person?.Name} {work.Student?.Person?.Patronymic}"
                        : $"{work.Parent?.Surname} {work.Parent?.Name} {work.Parent?.Patronymic}"),
                    CreateTableCell(work.Topic),
                    CreateTableCell(work.Decision)
                );

                ApplyRowStyle(row);
                table.Append(row);
            }

            return table;
        }

        private TableCell CreateTableCell(string text)
        {
            TableCell cell = new TableCell(new Paragraph(new Run(new Text(text))));

            // Центрирование текста внутри ячейки
            var paragraph = cell.Elements<Paragraph>().First();
            paragraph.ParagraphProperties = new ParagraphProperties(
                new Justification() { Val = JustificationValues.Center }
            );

            // Границы ячейки
            cell.TableCellProperties = new TableCellProperties(
                new TableCellBorders(
                    new TopBorder() { Val = BorderValues.Single, Size = 4 },
                    new BottomBorder() { Val = BorderValues.Single, Size = 4 },
                    new LeftBorder() { Val = BorderValues.Single, Size = 4 },
                    new RightBorder() { Val = BorderValues.Single, Size = 4 }
                )
            );

            return cell;
        }

        private void ApplyHeaderStyle(TableRow row)
        {
            foreach (var cell in row.Elements<TableCell>())
            {
                var paragraph = cell.Elements<Paragraph>().First();
                paragraph.ParagraphProperties = new ParagraphProperties(
                    new Justification() { Val = JustificationValues.Center }
                );

                cell.TableCellProperties = new TableCellProperties(
                    new Shading()
                    {
                        Fill = "D9D9D9",
                        Val = ShadingPatternValues.Clear
                    },
                    new TableCellBorders(
                        new TopBorder() { Val = BorderValues.Single, Size = 4 },
                        new BottomBorder() { Val = BorderValues.Single, Size = 4 },
                        new LeftBorder() { Val = BorderValues.Single, Size = 4 },
                        new RightBorder() { Val = BorderValues.Single, Size = 4 }
                    )
                );
            }
        }

        private void ApplyRowStyle(TableRow row)
        {
            foreach (var cell in row.Elements<TableCell>())
            {
                var paragraph = cell.Elements<Paragraph>().First();
                paragraph.ParagraphProperties = new ParagraphProperties(
                    new Justification() { Val = JustificationValues.Center }
                );
            }
        }

        private Paragraph CreateCenteredParagraph(string text)
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
    }
}