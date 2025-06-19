using CuratorJournal.DataBase;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CuratorJournal.Reports
{
    public class ExtracurricularActivityReportGenerator
    {
        public void GenerateReport(string filePath, string groupName, List<Student> students, int currentCourse)
        {
            using (WordprocessingDocument document = WordprocessingDocument.Create(filePath, WordprocessingDocumentType.Document))
            {
                MainDocumentPart mainPart = document.AddMainDocumentPart();
                mainPart.Document = new Document();
                Body body = new Body();

                // Заголовок
                Paragraph title = CreateCenteredTitle($"Внеучебная занятость студентов ({currentCourse} курс)");
                body.Append(title);

                // Создаем таблицу
                Table table = CreateTable(students);
                body.Append(table);

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
                    RunProperties = new RunProperties(
                        new RunFonts() { Ascii = "Times New Roman", HighAnsi = "Times New Roman" },
                        new FontSize() { Val = "28" }
                    )
                }
            )
            {
                ParagraphProperties = new ParagraphProperties(
                    new Justification() { Val = JustificationValues.Center }
                )
            };
        }

        private Table CreateTable(List<Student> students)
        {
            Table table = new Table();

            // Стили таблицы
            TableProperties tableProps = new TableProperties(
                new TableWidth() { Width = "5000", Type = TableWidthUnitValues.Pct },
                new TableLook() { Val = "04A0" },
                new TableBorders(
                    new TopBorder() { Val = BorderValues.Single, Size = 6 },
                    new BottomBorder() { Val = BorderValues.Single, Size = 6 },
                    new LeftBorder() { Val = BorderValues.Single, Size = 6 },
                    new RightBorder() { Val = BorderValues.Single, Size = 6 },
                    new InsideHorizontalBorder() { Val = BorderValues.Single, Size = 4 },
                    new InsideVerticalBorder() { Val = BorderValues.Single, Size = 4 }
                ),
                new GridAfter() { Val = 2 }
            );

            TableRow headerRow = new TableRow();

            // Заголовки столбцов
            TableCell fioHeader = new TableCell(
                new Paragraph(
                    new Run(
                        new Text("ФИО")
                    )
                    {
                        RunProperties = new RunProperties(
                            new RunFonts() { Ascii = "Times New Roman", HighAnsi = "Times New Roman" },
                            new FontSize() { Val = "28" }
                        )
                    }
                )
                {
                    ParagraphProperties = new ParagraphProperties(
                        new Justification() { Val = JustificationValues.Center }
                    )
                }
            );
            headerRow.Append(fioHeader);

            TableCell onCampusHeader = new TableCell(
                new Paragraph(
                    new Run(
                        new Text("В коллеже")
                    )
                    {
                        RunProperties = new RunProperties(
                            new RunFonts() { Ascii = "Times New Roman", HighAnsi = "Times New Roman" },
                            new FontSize() { Val = "28" }
                        )
                    }
                )
                {
                    ParagraphProperties = new ParagraphProperties(
                        new Justification() { Val = JustificationValues.Center }
                    )
                }
            );
            headerRow.Append(onCampusHeader);

            TableCell offCampusHeader = new TableCell(
                new Paragraph(
                    new Run(
                        new Text("Вне колледжа")
                    )
                    {
                        RunProperties = new RunProperties(
                            new RunFonts() { Ascii = "Times New Roman", HighAnsi = "Times New Roman" },
                            new FontSize() { Val = "28" }
                        )
                    }
                )
                {
                    ParagraphProperties = new ParagraphProperties(
                        new Justification() { Val = JustificationValues.Center }
                    )
                }
            );
            headerRow.Append(offCampusHeader);

            table.Append(tableProps);
            table.Append(headerRow);

            // Основные данные
            foreach (var student in students)
            {
                if (student.Hobby == null) continue;

                TableRow dataRow = new TableRow();

                // ФИО
                TableCell nameCell = new TableCell(
                    new Paragraph(
                        new Run(
                            new Text($"{student.Person.Surname} {student.Person.Name} {student.Person.Patronymic}")
                        )
                        {
                            RunProperties = new RunProperties(
                                new RunFonts() { Ascii = "Times New Roman", HighAnsi = "Times New Roman" },
                                new FontSize() { Val = "28" }
                            )
                        }
                    )
                    {
                        ParagraphProperties = new ParagraphProperties(
                            new Justification() { Val = JustificationValues.Left }
                        )
                    }
                );
                dataRow.Append(nameCell);

                // В коллеже (пусто)
                TableCell onCampusCell = new TableCell(
                    new Paragraph(
                        new Run(
                            new Text("")
                        )
                        {
                            RunProperties = new RunProperties(
                                new RunFonts() { Ascii = "Times New Roman", HighAnsi = "Times New Roman" },
                                new FontSize() { Val = "28" }
                            )
                        }
                    )
                    {
                        ParagraphProperties = new ParagraphProperties(
                            new Justification() { Val = JustificationValues.Center }
                        )
                    }
                );
                dataRow.Append(onCampusCell);

                // Вне колледжа (список хобби)
                TableCell offCampusCell = new TableCell(
                    new Paragraph(
                        new Run(
                            new Text(string.Join(", ", student.Hobby.Select(h => h.Name)))
                        )
                        {
                            RunProperties = new RunProperties(
                                new RunFonts() { Ascii = "Times New Roman", HighAnsi = "Times New Roman" },
                                new FontSize() { Val = "28" }
                            )
                        }
                    )
                    {
                        ParagraphProperties = new ParagraphProperties(
                            new Justification() { Val = JustificationValues.Left }
                        )
                    }
                );
                dataRow.Append(offCampusCell);

                table.Append(dataRow);
            }

            return table;
        }
    }
}