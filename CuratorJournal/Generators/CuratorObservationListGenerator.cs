using CuratorJournal.DataBase;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Collections.Generic;
using System.Linq;

namespace CuratorJournal.Reports
{
    public class CuratorObservationListGenerator
    {
        public void GenerateReport(string filePath, string groupName, List<Student> students)
        {
            using (WordprocessingDocument document = WordprocessingDocument.Create(filePath, WordprocessingDocumentType.Document))
            {
                MainDocumentPart mainPart = document.AddMainDocumentPart();
                mainPart.Document = new Document();
                Body body = new Body();

                // Заголовок
                Paragraph title = CreateCenteredTitle($"Лист наблюдений куратора");
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
                    RunProperties = new RunProperties(new FontSize() { Val = "28" })
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
                new GridAfter() { Val = 2 } // 2 столбца
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
                });
            headerRow.Append(fioHeader);

            // Ячейка "Характеристика"
            TableCell characteristicHeader = new TableCell(
                new Paragraph(
                    new Run(
                        new Text("Характеристика")
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
                });
            headerRow.Append(characteristicHeader);

            table.Append(tableProps);
            table.Append(headerRow);

            // Добавляем строки с данными
            foreach (var student in students)
            {
                if (student.CuratorCharacteristic == null) continue;

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

                // Характеристика
                TableCell characteristicCell = new TableCell(
                    new Paragraph(
                        new Run(
                            new Text(student.CuratorCharacteristic.FirstOrDefault()?.Characteristic ?? "")
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
                dataRow.Append(characteristicCell);

                table.Append(dataRow);
            }

            return table;
        }
    }
}