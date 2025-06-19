using CuratorJournal.DataBase;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Collections.Generic;
using System.Linq;

namespace CuratorJournal.Reports
{
    public class ActivistReportGenerator
    {
        public void GenerateReport(string filePath, string groupName, List<Student> students, int currentCourse)
        {
            using (WordprocessingDocument document = WordprocessingDocument.Create(filePath, WordprocessingDocumentType.Document))
            {
                MainDocumentPart mainPart = document.AddMainDocumentPart();
                mainPart.Document = new Document();
                Body body = new Body();

                // Заголовок
                Paragraph title = CreateCenteredTitle($"Актив группы №{groupName}");
                body.Append(title);

                // Таблица
                Table table = CreateTable(students, currentCourse);
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
                    {
                        Space = SpaceProcessingModeValues.Preserve
                    }
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

        private Table CreateTable(List<Student> students, int currentCourse)
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
                new GridAfter() { Val = 4 }
            );

            TableRow headerRow1 = new TableRow();
            TableRow headerRow2 = new TableRow();

            // Ячейка "Социальная нагрузка"
            TableCell fullNameHeader = new TableCell(
                new Paragraph(
                    new Run(
                        new Text("Социальная\nнагрузка")
                        {
                            Space = SpaceProcessingModeValues.Preserve
                        }
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
            fullNameHeader.Append(new VerticalMerge() { Val = MergedCellValues.Restart });
            headerRow1.Append(fullNameHeader);
            headerRow2.Append(CreateEmptyCell());

            // Добавляем все 4 курса в заголовок
            for (int course = 1; course <= 4; course++)
            {
                string courseText = $"{course} курс";

                TableCell courseCell = new TableCell(
                    new Paragraph(
                        new Run(
                            new Text(courseText)
                            {
                                Space = SpaceProcessingModeValues.Preserve
                            }
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
                headerRow1.Append(courseCell);

                TableCell numberCell = new TableCell(
                    new Paragraph(
                        new Run(
                            new Text("ФИО")
                            {
                                Space = SpaceProcessingModeValues.Preserve
                            }
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
                headerRow2.Append(numberCell);
            }

            // Повторение заголовков при разрыве страницы
            headerRow1.InsertAt(new TableRowProperties(new CantSplit()), 0);
            headerRow2.InsertAt(new TableRowProperties(new CantSplit()), 0);

            table.Append(tableProps);
            table.Append(headerRow1);
            table.Append(headerRow2);

            // Группируем должности
            var groupPosts = students
                .SelectMany(s => s.GroupPost)
                .Select(gp => gp.Name)
                .Distinct()
                .ToList();

            foreach (var post in groupPosts)
            {
                TableRow dataRow = new TableRow();

                // Тип активизма
                TableCell postCell = new TableCell(
                    new Paragraph(
                        new Run(
                            new Text(post)
                            {
                                Space = SpaceProcessingModeValues.Preserve
                            }
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
                dataRow.Append(postCell);

                // ФИО студентов для каждого курса
                for (int course = 1; course <= 4; course++)
                {
                    if (course > currentCourse)
                    {
                        dataRow.Append(CreateEmptyCell());
                        continue;
                    }

                    string fioList = string.Join(", ", students
                        .Where(s => s.GroupPost.Any(gp => gp.Name == post))
                        .Select(s => $"{s.Person.Surname} {s.Person.Name} {s.Person.Patronymic}")
                    );

                    TableCell fioCell = new TableCell(
                        new Paragraph(
                            new Run(
                                new Text(fioList)
                                {
                                    Space = SpaceProcessingModeValues.Preserve
                                }
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
                    dataRow.Append(fioCell);
                }

                table.Append(dataRow);
            }

            return table;
        }

        private TableCell CreateEmptyCell()
        {
            return new TableCell(
                new Paragraph(
                    new Run(new Text(""))
                )
            );
        }
    }
}