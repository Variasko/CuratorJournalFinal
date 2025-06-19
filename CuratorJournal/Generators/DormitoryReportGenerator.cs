using CuratorJournal.DataBase;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CuratorJournal.Reports
{
    public class DormitoryReportGenerator
    {
        public void GenerateReport(string filePath, string groupName, List<Student> students, int currentCourse)
        {
            using (WordprocessingDocument document = WordprocessingDocument.Create(filePath, WordprocessingDocumentType.Document))
            {
                MainDocumentPart mainPart = document.AddMainDocumentPart();
                mainPart.Document = new Document();
                Body body = new Body();

                // Заголовок
                Paragraph title = CreateCenteredTitle("Список студентов проживающих в общежитии");
                body.Append(title);

                // Создаем таблицу
                Table table = CreateTable(students, currentCourse, groupName);
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

        private Table CreateTable(List<Student> students, int currentCourse, string groupName)
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

            // Ячейка "ФИО"
            TableCell fullNameHeader = new TableCell(
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
            headerRow1.Append(fullNameHeader);

            // Заголовки: 1-4 курсы
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
            }

            // Вторая строка заголовков: все №
            TableCell emptyCell = new TableCell(new Paragraph());
            headerRow2.Append(emptyCell);

            for (int course = 1; course <= 4; course++)
            {
                TableCell numberCell = new TableCell(
                    new Paragraph(
                        new Run(
                            new Text("№")
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

            // Делаем первые две строки повторяющимися при разрыве страницы
            headerRow1.InsertAt(new TableRowProperties(new CantSplit()), 0);
            headerRow2.InsertAt(new TableRowProperties(new CantSplit()), 0);

            table.Append(tableProps);
            table.Append(headerRow1);
            table.Append(headerRow2);

            // Основные данные
            foreach (var student in students)
            {
                if (student.StudentInDormitory == null) continue;

                TableRow dataRow = new TableRow();

                string fullName = $"{student.Person.Surname} {student.Person.Name} {student.Person.Patronymic}";

                // ФИО
                TableCell nameCell = new TableCell(
                    new Paragraph(
                        new Run(
                            new Text(fullName)
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
                dataRow.Append(nameCell);

                // Курс 1-4
                for (int course = 1; course <= 4; course++)
                {
                    string roomNumber = "";
                    if (course <= currentCourse)
                    {
                        roomNumber = student.StudentInDormitory.Room.ToString();
                    }

                    TableCell roomCell = new TableCell(
                        new Paragraph(
                            new Run(
                                new Text(roomNumber)
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
                    dataRow.Append(roomCell);
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