using CuratorJournal.DataBase;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CuratorJournal.Reports // Убедитесь, что такое пространство имён есть
{
    public class SocialPassportReportGenerator
    {
        public void GenerateReport(string filePath, string groupName, List<Student> students, List<SocialStatus> socialStatuses)
        {
            using (WordprocessingDocument document = WordprocessingDocument.Create(filePath, WordprocessingDocumentType.Document))
            {
                MainDocumentPart mainPart = document.AddMainDocumentPart();
                mainPart.Document = new Document();
                Body body = new Body();

                // Заголовок
                Paragraph title = CreateCenteredTitle($"Социальный паспорт группы № {groupName}");
                body.Append(title);

                // Получаем список статусов по имени
                var statusNames = socialStatuses.Select(s => s.Name).ToList();

                // Разбиваем на блоки по 6 колонок
                int chunkSize = 6;
                for (int i = 0; i < statusNames.Count; i += chunkSize)
                {
                    var currentChunk = statusNames.Skip(i).Take(chunkSize).ToList();
                    Table table = CreateTable(currentChunk, students);
                    body.Append(table);
                }

                mainPart.Document.Append(body);
                mainPart.Document.Save();
            }
        }

        private Paragraph CreateCenteredTitle(string text)
        {
            return new Paragraph(
                new Run(
                    new Text(text)
                )
                {
                    RunProperties = new RunProperties(new FontSize() { Val = "28" })
                }
            )
            {
                ParagraphProperties = new ParagraphProperties(
                    new Justification() { Val = JustificationValues.Center }
                )
            };
        }

        private Table CreateTable(List<string> statuses, List<Student> students)
        {
            Table table = new Table();

            // Стили таблицы: границы, ширина и т.п.
            TableProperties tableProps = new TableProperties(
                new TableWidth() { Width = "5000", Type = TableWidthUnitValues.Pct },
                new TableLook() { Val = "04A0" }, // стили отображения (например, полоски при печати)

                // Границы таблицы
                new TableBorders(
                    new TopBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 6 },
                    new BottomBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 6 },
                    new LeftBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 6 },
                    new RightBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 6 },
                    new InsideHorizontalBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 },
                    new InsideVerticalBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 }
                )
            );

            TableRow headerRow = new TableRow();
            TableCell indexHeader = new TableCell(
                new Paragraph(
                    new Run(
                        new Text("№")
                    )
                    {
                        RunProperties = new RunProperties(
                            new FontSize() { Val = "28" }, // чуть больше для заголовка
                            new RunFonts() { Ascii = "Times New Roman", HighAnsi = "Times New Roman" }
                        )
                    }
                )
                {
                    ParagraphProperties = new ParagraphProperties(
                        new Justification() { Val = JustificationValues.Center }
                    )
                }
            );
            headerRow.Append(indexHeader);

            foreach (var status in statuses)
            {
                TableCell cell = new TableCell(
                    new Paragraph(
                        new Run(
                            new Text(status)
                        )
                        {
                            RunProperties = new RunProperties(
                                new FontSize() { Val = "28" },
                                new RunFonts() { Ascii = "Times New Roman", HighAnsi = "Times New Roman" }
                            )
                        }
                    )
                    {
                        ParagraphProperties = new ParagraphProperties(
                            new Justification() { Val = JustificationValues.Center }
                        )
                    }
                );
                headerRow.Append(cell);
            }

            table.Append(tableProps);
            table.Append(headerRow);

            // Словарь: статус -> очередь ФИО
            Dictionary<string, Queue<string>> statusToFioQueue = new Dictionary<string, Queue<string>>();
            foreach (var status in statuses)
            {
                statusToFioQueue[status] = new Queue<string>();
            }

            foreach (var student in students)
            {
                string fullName = $"{student.Person.Surname} {student.Person.Name} {student.Person.Patronymic}";

                foreach (var socialStatus in student.SocialStatus)
                {
                    if (statusToFioQueue.ContainsKey(socialStatus.Name))
                    {
                        statusToFioQueue[socialStatus.Name].Enqueue(fullName);
                    }
                }
            }

            // Генерация строк
            bool hasData = true;
            int rowIndex = 1;

            while (hasData)
            {
                hasData = false;
                TableRow row = new TableRow();

                TableCell indexCell = new TableCell(
                    new Paragraph(
                        new Run(
                            new Text(rowIndex.ToString())
                        )
                        {
                            RunProperties = new RunProperties(
                                new FontSize() { Val = "28" },
                                new RunFonts() { Ascii = "Times New Roman", HighAnsi = "Times New Roman" }
                            )
                        }
                    )
                    {
                        ParagraphProperties = new ParagraphProperties(
                            new Justification() { Val = JustificationValues.Center }
                        )
                    }
                );
                row.Append(indexCell);

                foreach (var status in statuses)
                {
                    string nameToShow = "";
                    if (statusToFioQueue[status].Count > 0)
                    {
                        nameToShow = statusToFioQueue[status].Dequeue();
                        hasData = true;
                    }

                    TableCell cell = new TableCell(
                        new Paragraph(
                            new Run(
                                new Text(nameToShow)
                            )
                            {
                                RunProperties = new RunProperties(
                                    new FontSize() { Val = "28" },
                                    new RunFonts() { Ascii = "Times New Roman", HighAnsi = "Times New Roman" }
                                )
                            }
                        )
                        {
                            ParagraphProperties = new ParagraphProperties(
                                new Justification() { Val = JustificationValues.Center }
                            )
                        }
                    );
                    row.Append(cell);
                }

                if (hasData)
                {
                    table.Append(row);
                    rowIndex++;
                }
            }

            return table;
        }
    }
}