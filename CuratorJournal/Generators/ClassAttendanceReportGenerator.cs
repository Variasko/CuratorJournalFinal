using CuratorJournal.DataBase;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CuratorJournal.Reports
{
    public class ClassAttendanceReportGenerator
    {
        public void GenerateReport(string filePath, string groupName, List<Student> students, List<ClassHour> classHours)
{
    using (WordprocessingDocument document = WordprocessingDocument.Create(filePath, WordprocessingDocumentType.Document))
    {
        MainDocumentPart mainPart = document.AddMainDocumentPart();
        mainPart.Document = new Document();

        // Тело документа
        Body body = new Body();

        // Установка альбомной ориентации через SectionProperties
        SectionProperties sectionProperties = new SectionProperties();
        PageSize pageSize = new PageSize()
        {
            Width = 16838,   // A4 в альбоме: 297 мм × 210 мм → ширина = 16838 twentieths of a point
            Height = 11906,   // высота = 11906 twentieths of a point
            Orient = PageOrientationValues.Landscape
        };
        PageMargin pageMargin = new PageMargin()
        {
            Top = 1440,
            Right = 1440,
            Bottom = 1440,
            Left = 1440,
            Header = 851,
            Footer = 851,
            Gutter = 0
        };

        sectionProperties.Append(pageSize);
        sectionProperties.Append(pageMargin);

        // Заголовок
        Paragraph title = CreateCenteredTitle($"Учет посещений классных часов студентами");
        body.Append(title);

        // Создаем таблицы для учёта посещений
        var tables = CreateTables(students, classHours);
        foreach (var table in tables)
        {
            body.Append(table);
        }

        // Добавляем свойства раздела после последнего элемента
        body.Append(sectionProperties);

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

        private List<Table> CreateTables(List<Student> students, List<ClassHour> classHours)
        {
            List<Table> tables = new List<Table>();

            // Максимальное количество столбцов в одной таблице
            const int MaxColumnsPerTable = 12;

            // Разбиваем классные часы на блоки по 12
            for (int i = 0; i < classHours.Count; i += MaxColumnsPerTable)
            {
                var chunk = classHours.Skip(i).Take(MaxColumnsPerTable).ToList();

                Table table = CreateTable(students, chunk);
                tables.Add(table);
            }

            return tables;
        }

        private Table CreateTable(List<Student> students, List<ClassHour> classHours)
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
                new GridAfter() { Val = classHours.Count + 1 } // Добавляем ФИО
            );

            TableRow headerRow = new TableRow();

            // Ячейка "ФИО"
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

            // Добавляем даты классных часов
            foreach (var classHour in classHours)
            {
                TableCell dateCell = new TableCell(
                    new Paragraph(
                        new Run(
                            new Text(classHour.Date.ToString("dd.MM.yyyy"))
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
                headerRow.Append(dateCell);
            }

            table.Append(tableProps);
            table.Append(headerRow);

            // Данные о посещении
            foreach (var student in students)
            {
                TableRow dataRow = new TableRow();

                // ФИО студента
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

                // Посещаемость
                foreach (var classHour in classHours)
                {
                    bool attended = student.StudentClassHour.Any(sch => sch.ClassHourId == classHour.ClassHourId && sch.IsVisited);

                    TableCell attendanceCell = new TableCell(
                        new Paragraph(
                            new Run(
                                new Text(attended ? "я" : "н")
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
                    dataRow.Append(attendanceCell);
                }

                table.Append(dataRow);
            }

            return table;
        }
    }
}