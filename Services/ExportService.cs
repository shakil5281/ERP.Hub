using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ERPHub.Models;

namespace ERPHub.Services
{
    public class ExportService : IExportService
    {
        static ExportService()
        {
            // Set QuestPDF license (MIT license - free for commercial use)
            QuestPDF.Settings.License = LicenseType.Community;

            // Set EPPlus license context
            ExcelPackage.License.SetNonCommercialOrganization("ERPHub");
        }

        public async Task<byte[]> ExportAttendanceToExcelAsync(
            IEnumerable<AttendanceRecord> data,
            string reportTitle,
            DateTime reportDate,
            Company company,
            AttendanceSummary summary,
            Dictionary<string, Func<AttendanceRecord, object>> columns)
        {
            return await Task.Run(() =>
            {
                using var package = new ExcelPackage();
                var worksheet = package.Workbook.Worksheets.Add("Daily Attendance");
                worksheet.View.ShowGridLines = false; // Hide gridlines
                int colCount = columns.Count;

                // Page setup - A4 Portrait
                worksheet.PrinterSettings.PaperSize = ePaperSize.A4;
                worksheet.PrinterSettings.Orientation = eOrientation.Portrait;
                worksheet.PrinterSettings.FitToPage = true;
                worksheet.PrinterSettings.FitToWidth = 1;
                worksheet.PrinterSettings.FitToHeight = 0;
                worksheet.PrinterSettings.TopMargin = 0.5;
                worksheet.PrinterSettings.BottomMargin = 0.5;
                worksheet.PrinterSettings.LeftMargin = 0.4;
                worksheet.PrinterSettings.RightMargin = 0.4;

                int currentRow = 1;

                // ---- HEADER SECTION ----

                // Row 1: Unified Centered Header (Company + Address + Title)
                int headerRowIndex = currentRow;
                worksheet.Cells[headerRowIndex, 1, headerRowIndex, colCount].Merge = true;
                var companyCell = worksheet.Cells[headerRowIndex, 1];
                companyCell.IsRichText = true;
                
                var companyName = company?.CompanyNameEn ?? "ERPHub";
                var address = company?.AddressEn ?? "";
                
                var r1 = companyCell.RichText.Add(companyName + "\n");
                r1.Bold = true;
                r1.Size = 14;
                r1.Color = System.Drawing.Color.Black;

                if (!string.IsNullOrEmpty(address))
                {
                    var r2 = companyCell.RichText.Add(address + "\n");
                    r2.Size = 9;
                    r2.Color = System.Drawing.Color.Black;
                }

                var r3 = companyCell.RichText.Add(reportTitle);
                r3.Bold = true;
                r3.Size = 11;
                r3.Color = System.Drawing.Color.Black;
                
                companyCell.Style.WrapText = true;
                companyCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                companyCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Row(headerRowIndex).Height = 55;
                currentRow++;

                // Row 2: Date Row (Right-aligned, with bottom thin border separator)
                int dateRowIndex = currentRow;
                worksheet.Cells[dateRowIndex, 1, dateRowIndex, colCount].Merge = true;
                var dateCell = worksheet.Cells[dateRowIndex, 1];
                dateCell.Value = $"Date: {reportDate:yyyy-MM-dd}";
                dateCell.Style.Font.Bold = true;
                dateCell.Style.Font.Size = 10;
                dateCell.Style.Font.Color.SetColor(System.Drawing.Color.Black);
                dateCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                dateCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                
                worksheet.Cells[dateRowIndex, 1, dateRowIndex, colCount].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[dateRowIndex, 1, dateRowIndex, colCount].Style.Border.Bottom.Color.SetColor(System.Drawing.Color.FromArgb(0xD1, 0xD5, 0xDB));
                worksheet.Row(dateRowIndex).Height = 18;
                currentRow++;

                // Row 3: Empty spacer
                worksheet.Row(currentRow).Height = 4;
                currentRow++;

                // ---- COLUMN HEADER ROW ----
                int headerRow = currentRow;
                int colIndex = 1;
                foreach (var col in columns.Keys)
                {
                    var cell = worksheet.Cells[headerRow, colIndex];
                    cell.Value = col;
                    cell.Style.Font.Bold = true;
                    cell.Style.Font.Size = 10;
                    cell.Style.Font.Color.SetColor(System.Drawing.Color.Black);
                    cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    cell.Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.FromArgb(0xD1, 0xD5, 0xDB));
                    colIndex++;
                }
                worksheet.Row(headerRow).Height = 22;
                currentRow++;

                // ---- DATA ROWS ----
                int dataStartRow = currentRow;
                var dataList = data.ToList();
                var columnNames = columns.Keys.ToList();
                var columnAlignments = new Dictionary<string, ExcelHorizontalAlignment>(StringComparer.OrdinalIgnoreCase)
                {
                    ["Date"] = ExcelHorizontalAlignment.Center,
                    ["Employee ID"] = ExcelHorizontalAlignment.Center,
                    ["Employee Name"] = ExcelHorizontalAlignment.Left,
                    ["In Time"] = ExcelHorizontalAlignment.Center,
                    ["Out Time"] = ExcelHorizontalAlignment.Center,
                    ["Late (min)"] = ExcelHorizontalAlignment.Center,
                    ["Worked (hrs)"] = ExcelHorizontalAlignment.Center,
                    ["OT (min)"] = ExcelHorizontalAlignment.Center,
                    ["Status"] = ExcelHorizontalAlignment.Center,
                };

                foreach (var item in dataList)
                {
                    colIndex = 1;
                    foreach (var columnFunc in columns.Values)
                    {
                        var cell = worksheet.Cells[currentRow, colIndex];
                        var value = columnFunc(item);
                        cell.Value = value ?? "";
                        cell.Style.Font.Size = 9;
                        cell.Style.Font.Color.SetColor(System.Drawing.Color.Black);
                        cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                        // Align based on column header
                        var colName = columnNames[colIndex - 1];
                        if (columnAlignments.TryGetValue(colName, out var alignment))
                        {
                            cell.Style.HorizontalAlignment = alignment;
                        }
                        else
                        {
                            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        }

                        if (value is DateTime dt)
                            cell.Style.Numberformat.Format = "yyyy-MM-dd";
                        else if (value is TimeSpan ts)
                            cell.Style.Numberformat.Format = @"hh\:mm\:ss";
                        else if (value is double d)
                            cell.Style.Numberformat.Format = "0.00";
                        else if (value is decimal dec)
                            cell.Style.Numberformat.Format = "0.00";

                        cell.Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.FromArgb(0xD1, 0xD5, 0xDB));
                        colIndex++;
                    }
                    worksheet.Row(currentRow).Height = 20;
                    currentRow++;
                }

                // ---- SUMMARY / FOOTER SECTION ----
                currentRow++; // Empty spacer row

                // Draw a top border line for footer
                worksheet.Cells[currentRow, 1, currentRow, colCount].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[currentRow, 1, currentRow, colCount].Style.Border.Top.Color.SetColor(System.Drawing.Color.FromArgb(0xD1, 0xD5, 0xDB));
                worksheet.Row(currentRow).Height = 4;
                currentRow++;

                var summaryItems = new[]
                {
                    ("Total Employee", summary.TotalEmployees.ToString()),
                    ("Total Present", summary.TotalPresent.ToString()),
                    ("Total Absent", summary.TotalAbsent.ToString()),
                    ("Total Late", summary.TotalLate.ToString()),
                    ("Total Leave", summary.TotalLeave.ToString()),
                };

                foreach (var (label, value) in summaryItems)
                {
                    int endCol = Math.Min(3, colCount);
                    var range = worksheet.Cells[currentRow, 1, currentRow, endCol];
                    range.Merge = true;
                    range.Value = $"{label}: {value}";
                    range.Style.Font.Bold = true;
                    range.Style.Font.Size = 10;
                    range.Style.Font.Color.SetColor(System.Drawing.Color.Black);
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    
                    worksheet.Row(currentRow).Height = 20;
                    currentRow++;
                }

                // Column widths
                var columnWidths = new Dictionary<string, double>
                {
                    ["Sl"] = 6,
                    ["Date"] = 12,
                    ["Employee ID"] = 11,
                    ["Employee Name"] = 28,
                    ["In Time"] = 10,
                    ["Out Time"] = 10,
                    ["Late (min)"] = 10,
                    ["Worked (hrs)"] = 10,
                    ["OT (min)"] = 10,
                    ["Status"] = 10,
                };
                colIndex = 1;
                foreach (var col in columns.Keys)
                {
                    double width = columnWidths.TryGetValue(col, out var w) ? w : 12;
                    worksheet.Column(colIndex).Width = width;
                    colIndex++;
                }

                // Freeze header
                worksheet.View.FreezePanes(headerRow + 1, 1);

                return package.GetAsByteArray();
            });
        }

        public async Task<byte[]> ExportToExcelAsync<T>(IEnumerable<T> data, string sheetName, string title, Dictionary<string, Func<T, object>> columns)
        {
            return await Task.Run(() =>
            {
                using var package = new ExcelPackage();
                var worksheet = package.Workbook.Worksheets.Add(sheetName);
                worksheet.View.ShowGridLines = false; // Hide gridlines

                // Title row
                worksheet.Cells[1, 1].Value = title;
                worksheet.Cells[1, 1, 1, columns.Count].Merge = true;
                worksheet.Cells[1, 1].Style.Font.Bold = true;
                worksheet.Cells[1, 1].Style.Font.Size = 14;
                worksheet.Cells[1, 1].Style.Font.Color.SetColor(System.Drawing.Color.Black);
                worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[1, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Row(1).Height = 35;

                // Subtitle row with date
                worksheet.Cells[2, 1].Value = $"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
                worksheet.Cells[2, 1, 2, columns.Count].Merge = true;
                worksheet.Cells[2, 1].Style.Font.Bold = true;
                worksheet.Cells[2, 1].Style.Font.Size = 10;
                worksheet.Cells[2, 1].Style.Font.Color.SetColor(System.Drawing.Color.Black);
                worksheet.Cells[2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                worksheet.Cells[2, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                
                worksheet.Cells[2, 1, 2, columns.Count].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[2, 1, 2, columns.Count].Style.Border.Bottom.Color.SetColor(System.Drawing.Color.FromArgb(0xD1, 0xD5, 0xDB));
                worksheet.Row(2).Height = 18;

                // Row 3: Empty spacer
                worksheet.Row(3).Height = 4;

                // Header row
                int headerRow = 4;
                int colIndex = 1;
                foreach (var col in columns.Keys)
                {
                    var cell = worksheet.Cells[headerRow, colIndex];
                    cell.Value = col;
                    cell.Style.Font.Bold = true;
                    cell.Style.Font.Size = 10;
                    cell.Style.Font.Color.SetColor(System.Drawing.Color.Black);
                    cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    cell.Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.FromArgb(0xD1, 0xD5, 0xDB));
                    colIndex++;
                }
                worksheet.Row(headerRow).Height = 22;

                // Data rows
                int rowIndex = headerRow + 1;
                var dataList = data.ToList();
                foreach (var item in dataList)
                {
                    colIndex = 1;
                    foreach (var col in columns.Values)
                    {
                        var cell = worksheet.Cells[rowIndex, colIndex];
                        var value = col(item);
                        cell.Value = value ?? "";
                        cell.Style.Font.Size = 9;
                        cell.Style.Font.Color.SetColor(System.Drawing.Color.Black);
                        cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        
                        // Apply formatting based on value type
                        if (value is DateTime dt)
                        {
                            cell.Style.Numberformat.Format = "yyyy-MM-dd";
                        }
                        else if (value is TimeSpan ts)
                        {
                            cell.Style.Numberformat.Format = @"hh\:mm\:ss";
                        }
                        else if (value is double d)
                        {
                            cell.Style.Numberformat.Format = "0.00";
                        }
                        else if (value is decimal dec)
                        {
                            cell.Style.Numberformat.Format = "0.00";
                        }

                        cell.Style.Border.BorderAround(ExcelBorderStyle.Thin, System.Drawing.Color.FromArgb(0xD1, 0xD5, 0xDB));
                        colIndex++;
                    }
                    worksheet.Row(rowIndex).Height = 20;
                    rowIndex++;
                }

                // Auto-fit columns
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                
                // Freeze header row
                worksheet.View.FreezePanes(headerRow + 1, 1);

                // Add summary row
                int spacerRow = rowIndex + 1;
                worksheet.Cells[spacerRow, 1, spacerRow, columns.Count].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[spacerRow, 1, spacerRow, columns.Count].Style.Border.Top.Color.SetColor(System.Drawing.Color.FromArgb(0xD1, 0xD5, 0xDB));
                worksheet.Row(spacerRow).Height = 4;

                var summaryRow = spacerRow + 1;
                int endCol = Math.Min(3, columns.Count);
                var summaryRange = worksheet.Cells[summaryRow, 1, summaryRow, endCol];
                summaryRange.Merge = true;
                summaryRange.Value = $"Total Records: {dataList.Count}";
                summaryRange.Style.Font.Bold = true;
                summaryRange.Style.Font.Size = 10;
                summaryRange.Style.Font.Color.SetColor(System.Drawing.Color.Black);
                summaryRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                summaryRange.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Row(summaryRow).Height = 20;

                return package.GetAsByteArray();
            });
        }

        public async Task<byte[]> ExportAttendanceToPdfAsync(
            IEnumerable<AttendanceRecord> data,
            string reportTitle,
            DateTime reportDate,
            Company company,
            AttendanceSummary summary,
            Dictionary<string, Func<AttendanceRecord, object>> columns)
        {
            return await Task.Run(() =>
            {
                var dataList = data.ToList();
                var columnNames = columns.Keys.ToList();
                var columnFuncs = columns.Values.ToList();
                var companyName = company?.CompanyNameEn ?? "ERPHub";
                var companyAddress = company?.AddressEn ?? "";

                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Margin(30);
                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(x => x.FontSize(9).FontFamily(Fonts.Calibri));
                        page.Size(PageSizes.A4.Portrait()); // Portrait layout

                        // Header
                        page.Header().Column(col =>
                        {
                            col.Item().AlignCenter().Text(companyName).FontSize(14).Bold().FontColor(Colors.Black);
                            if (!string.IsNullOrEmpty(companyAddress))
                                col.Item().AlignCenter().Text(companyAddress).FontSize(9).FontColor(Colors.Black);
                            col.Item().AlignCenter().Text(reportTitle).FontSize(11).Bold().FontColor(Colors.Black);
                            col.Item().AlignRight().Text($"Date: {reportDate:yyyy-MM-dd}").FontSize(10).Bold().FontColor(Colors.Black);
                            col.Item().PaddingTop(4).LineHorizontal(1f).LineColor("#D1D5DB");
                            col.Item().PaddingBottom(8);
                        });

                        // Content
                        page.Content().Table(table =>
                        {
                            table.ColumnsDefinition(columnsDef =>
                            {
                                foreach (var colName in columnNames)
                                {
                                    if (colName.Equals("Date", StringComparison.OrdinalIgnoreCase))
                                        columnsDef.RelativeColumn(12);
                                    else if (colName.Equals("Employee ID", StringComparison.OrdinalIgnoreCase))
                                        columnsDef.RelativeColumn(11);
                                    else if (colName.Equals("Employee Name", StringComparison.OrdinalIgnoreCase))
                                        columnsDef.RelativeColumn(28);
                                    else
                                        columnsDef.RelativeColumn(10); // In Time, Out Time, Late, Worked, OT, Status
                                }
                            });

                            table.Header(header =>
                            {
                                foreach (var colName in columnNames)
                                {
                                    header.Cell().Element(CellStyle).AlignCenter().Text(colName).Bold().FontColor(Colors.Black).FontSize(9);
                                }
                            });

                            for (int i = 0; i < dataList.Count; i++)
                            {
                                var item = dataList[i];
                                for (int colIdx = 0; colIdx < columnNames.Count; colIdx++)
                                {
                                    var colName = columnNames[colIdx];
                                    var func = columnFuncs[colIdx];
                                    var value = func(item);

                                    var cellElement = table.Cell().Element(CellStyle);
                                    if (colName.Equals("Employee Name", StringComparison.OrdinalIgnoreCase))
                                        cellElement.AlignLeft().Text(GetCellText(value)).FontSize(8).FontColor(Colors.Black);
                                    else
                                        cellElement.AlignCenter().Text(GetCellText(value)).FontSize(8).FontColor(Colors.Black);
                                }
                            }
                        });

                        // Footer
                        page.Footer().Column(col =>
                        {
                            col.Item().PaddingTop(8).LineHorizontal(1f).LineColor("#D1D5DB");
                            col.Item().PaddingTop(4);
                            
                            col.Item().AlignLeft().Width(200).Column(summaryCol =>
                            {
                                summaryCol.Item().Text($"Total Employee: {summary.TotalEmployees}").Bold().FontSize(9).FontColor(Colors.Black);
                                summaryCol.Item().Text($"Total Present: {summary.TotalPresent}").Bold().FontSize(9).FontColor(Colors.Black);
                                summaryCol.Item().Text($"Total Absent: {summary.TotalAbsent}").Bold().FontSize(9).FontColor(Colors.Black);
                                summaryCol.Item().Text($"Total Late: {summary.TotalLate}").Bold().FontSize(9).FontColor(Colors.Black);
                                summaryCol.Item().Text($"Total Leave: {summary.TotalLeave}").Bold().FontSize(9).FontColor(Colors.Black);
                            });

                            col.Item().PaddingTop(8).AlignCenter().Text(text =>
                            {
                                text.Span("Page ").FontSize(8).FontColor(Colors.Grey.Medium);
                                text.CurrentPageNumber().FontSize(8).FontColor(Colors.Grey.Medium);
                                text.Span(" of ").FontSize(8).FontColor(Colors.Grey.Medium);
                                text.TotalPages().FontSize(8).FontColor(Colors.Grey.Medium);
                            });
                        });
                    });
                });

                return document.GeneratePdf();
            });
        }

        public async Task<byte[]> ExportToPdfAsync<T>(IEnumerable<T> data, string title, Dictionary<string, Func<T, object>> columns)
        {
            return await Task.Run(() =>
            {
                var dataList = data.ToList();
                var columnNames = columns.Keys.ToList();
                var columnFuncs = columns.Values.ToList();

                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Margin(30);
                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(x => x.FontSize(9).FontFamily(Fonts.Calibri));
                        page.Size(PageSizes.A4.Portrait()); // Portrait layout

                        // Header
                        page.Header().Column(col =>
                        {
                            col.Item().AlignCenter().Text(title).FontSize(14).Bold().FontColor(Colors.Black);
                            col.Item().AlignRight().Text($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}").FontSize(10).Bold().FontColor(Colors.Black);
                            col.Item().PaddingTop(4).LineHorizontal(1f).LineColor("#D1D5DB");
                            col.Item().PaddingBottom(8);
                        });

                        // Content
                        page.Content().Table(table =>
                        {
                            table.ColumnsDefinition(columnsDef =>
                            {
                                foreach (var _ in columnNames)
                                    columnsDef.RelativeColumn();
                            });

                            // Header row
                            table.Header(header =>
                            {
                                foreach (var colName in columnNames)
                                {
                                    header.Cell().Element(CellStyle).AlignCenter().Text(colName).Bold().FontColor(Colors.Black).FontSize(9);
                                }
                            });

                            // Data rows
                            for (int i = 0; i < dataList.Count; i++)
                            {
                                var item = dataList[i];
                                foreach (var func in columnFuncs)
                                {
                                    var value = func(item);
                                    table.Cell()
                                        .Element(CellStyle)
                                        .AlignCenter()
                                        .Text(GetCellText(value)).FontSize(8).FontColor(Colors.Black);
                                }
                            }
                        });

                        // Footer with page numbers
                        page.Footer().Column(col =>
                        {
                            col.Item().PaddingTop(8).LineHorizontal(1f).LineColor("#D1D5DB");
                            col.Item().PaddingTop(4);
                            
                            col.Item().AlignLeft().Text($"Total Records: {dataList.Count}").Bold().FontSize(9).FontColor(Colors.Black);
                            
                            col.Item().PaddingTop(4).AlignCenter().Text(text =>
                            {
                                text.Span("Page ").FontSize(8).FontColor(Colors.Grey.Medium);
                                text.CurrentPageNumber().FontSize(8).FontColor(Colors.Grey.Medium);
                                text.Span(" of ").FontSize(8).FontColor(Colors.Grey.Medium);
                                text.TotalPages().FontSize(8).FontColor(Colors.Grey.Medium);
                            });
                        });
                    });
                });

                return document.GeneratePdf();
            });
        }

        public async Task<byte[]> ExportJobCardToExcelAsync(JobCardReportDto report)
        {
            return await Task.Run(() =>
            {
                using var package = new ExcelPackage();
                var worksheet = package.Workbook.Worksheets.Add("Job Card");
                worksheet.View.ShowGridLines = false;
                int colCount = 9;

                // Page setup - A4 Portrait
                worksheet.PrinterSettings.PaperSize = ePaperSize.A4;
                worksheet.PrinterSettings.Orientation = eOrientation.Portrait;
                worksheet.PrinterSettings.FitToPage = true;
                worksheet.PrinterSettings.FitToWidth = 1;
                worksheet.PrinterSettings.FitToHeight = 0;
                worksheet.PrinterSettings.TopMargin = 0.5;
                worksheet.PrinterSettings.BottomMargin = 0.5;
                worksheet.PrinterSettings.LeftMargin = 0.4;
                worksheet.PrinterSettings.RightMargin = 0.4;

                int currentRow = 1;

                // ---- HEADER SECTION ----
                worksheet.Cells[currentRow, 1, currentRow, colCount].Merge = true;
                var companyCell = worksheet.Cells[currentRow, 1];
                companyCell.IsRichText = true;
                
                var r1 = companyCell.RichText.Add(report.CompanyName + "\n");
                r1.Bold = true;
                r1.Size = 14;
                r1.Color = System.Drawing.Color.Black;

                if (!string.IsNullOrEmpty(report.CompanyAddress))
                {
                    var r2 = companyCell.RichText.Add(report.CompanyAddress + "\n");
                    r2.Size = 9;
                    r2.Color = System.Drawing.Color.Black;
                }

                var r3 = companyCell.RichText.Add("Job Card");
                r3.Bold = true;
                r3.Size = 11;
                r3.Color = System.Drawing.Color.Black;
                
                companyCell.Style.WrapText = true;
                companyCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                companyCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Row(currentRow).Height = 55;
                currentRow++;

                // Row 2: Date range block
                worksheet.Cells[currentRow, 1, currentRow, colCount].Merge = true;
                var dateCell = worksheet.Cells[currentRow, 1];
                dateCell.Value = $"{report.FromDate} To {report.ToDate}";
                dateCell.Style.Font.Bold = true;
                dateCell.Style.Font.Size = 10;
                dateCell.Style.Font.Color.SetColor(System.Drawing.Color.Black);
                dateCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                dateCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                
                worksheet.Cells[currentRow, 1, currentRow, colCount].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[currentRow, 1, currentRow, colCount].Style.Border.Bottom.Color.SetColor(System.Drawing.Color.FromArgb(0xD1, 0xD5, 0xDB));
                worksheet.Row(currentRow).Height = 18;
                currentRow++;

                // Row 3: Empty spacer
                worksheet.Row(currentRow).Height = 4;
                currentRow++;

                // ---- EMPLOYEE INFO GRID (Rows 4-7) ----
                var infoRows = new[]
                {
                    new[] { ("ID", report.EmployeeId), ("Name", report.EmployeeName) },
                    new[] { ("Designation", report.Designation), ("Grade", report.Grade) },
                    new[] { ("Department", report.Department), ("Line", report.Line) },
                    new[] { ("Joining Date", report.JoiningDate), ("Section", report.Section) }
                };

                var borderCol = System.Drawing.Color.FromArgb(0xD1, 0xD5, 0xDB);

                foreach (var rowInfo in infoRows)
                {
                    int r = currentRow;
                    
                    // Col 1-2: Label 1 (Merged), Col 3-4: Value 1 (Merged)
                    var cellL1 = worksheet.Cells[r, 1, r, 2];
                    cellL1.Merge = true;
                    cellL1.Value = rowInfo[0].Item1;
                    cellL1.Style.Font.Bold = true;
                    cellL1.Style.Font.Size = 9;
                    cellL1.Style.Font.Color.SetColor(System.Drawing.Color.Black);
                    cellL1.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    cellL1.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    var cellV1 = worksheet.Cells[r, 3, r, 4];
                    cellV1.Merge = true;
                    cellV1.Value = rowInfo[0].Item2;
                    cellV1.Style.Font.Size = 9;
                    cellV1.Style.Font.Color.SetColor(System.Drawing.Color.Black);
                    cellV1.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    cellV1.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    // Col 5-6: Label 2 (Merged), Col 7-9: Value 2 (Merged)
                    var cellL2 = worksheet.Cells[r, 5, r, 6];
                    cellL2.Merge = true;
                    cellL2.Value = rowInfo[1].Item1;
                    cellL2.Style.Font.Bold = true;
                    cellL2.Style.Font.Size = 9;
                    cellL2.Style.Font.Color.SetColor(System.Drawing.Color.Black);
                    cellL2.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    cellL2.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    var cellV2 = worksheet.Cells[r, 7, r, 9];
                    cellV2.Merge = true;
                    cellV2.Value = rowInfo[1].Item2;
                    cellV2.Style.Font.Size = 9;
                    cellV2.Style.Font.Color.SetColor(System.Drawing.Color.Black);
                    cellV2.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    cellV2.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    for (int c = 1; c <= colCount; c++)
                    {
                        worksheet.Cells[r, c].Style.Border.BorderAround(ExcelBorderStyle.Thin, borderCol);
                    }

                    worksheet.Row(r).Height = 20;
                    currentRow++;
                }

                // Row 8: Empty spacer
                worksheet.Row(currentRow).Height = 4;
                currentRow++;

                // ---- DAILY LOG TABLE ----
                int tableHeaderRow = currentRow;
                var tableHeaders = new[] { "Date", "Shift", "In Time", "Out Time", "Late", "Working Hour", "EO", "OT", "Status" };
                for (int c = 1; c <= colCount; c++)
                {
                    var cell = worksheet.Cells[tableHeaderRow, c];
                    cell.Value = tableHeaders[c - 1];
                    cell.Style.Font.Bold = true;
                    cell.Style.Font.Size = 10;
                    cell.Style.Font.Color.SetColor(System.Drawing.Color.Black);
                    cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    cell.Style.Border.BorderAround(ExcelBorderStyle.Thin, borderCol);
                }
                worksheet.Row(tableHeaderRow).Height = 22;
                currentRow++;

                // Data Rows
                foreach (var day in report.Days)
                {
                    int r = currentRow;
                    var values = new[] { day.Date, day.Shift, day.InTime, day.OutTime, day.Late, day.WorkingHour, day.EarlyOut, day.Overtime, day.Status };
                    for (int c = 1; c <= colCount; c++)
                    {
                        var cell = worksheet.Cells[r, c];
                        cell.Value = values[c - 1];
                        cell.Style.Font.Size = 9;
                        cell.Style.Font.Color.SetColor(System.Drawing.Color.Black);
                        cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        cell.Style.Border.BorderAround(ExcelBorderStyle.Thin, borderCol);
                    }
                    worksheet.Row(r).Height = 20;
                    currentRow++;
                }

                // Totals Row
                int totalsRow = currentRow;
                for (int c = 1; c <= colCount; c++)
                {
                    var cell = worksheet.Cells[totalsRow, c];
                    cell.Style.Border.BorderAround(ExcelBorderStyle.Thin, borderCol);
                    if (c == 5) // Late
                    {
                        cell.Value = report.Totals.Late;
                        cell.Style.Font.Bold = true;
                        cell.Style.Font.Size = 9;
                        cell.Style.Font.Color.SetColor(System.Drawing.Color.Black);
                        cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    }
                    else if (c == 7) // EO
                    {
                        cell.Value = report.Totals.EarlyOut;
                        cell.Style.Font.Bold = true;
                        cell.Style.Font.Size = 9;
                        cell.Style.Font.Color.SetColor(System.Drawing.Color.Black);
                        cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    }
                    else if (c == 8) // OT
                    {
                        cell.Value = report.Totals.Overtime;
                        cell.Style.Font.Bold = true;
                        cell.Style.Font.Size = 9;
                        cell.Style.Font.Color.SetColor(System.Drawing.Color.Black);
                        cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    }
                }
                worksheet.Row(totalsRow).Height = 22;
                currentRow++;

                // ---- SUMMARY / FOOTER SECTION ----
                currentRow++; // Empty spacer row

                // Draw a top border line for footer
                worksheet.Cells[currentRow, 1, currentRow, colCount].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[currentRow, 1, currentRow, colCount].Style.Border.Top.Color.SetColor(borderCol);
                worksheet.Row(currentRow).Height = 4;
                currentRow++;

                var summaryItems = new[]
                {
                    ("Present", report.Summary.Present.ToString()),
                    ("Absent", report.Summary.Absent.ToString()),
                    ("Late", report.Summary.Late.ToString())
                };

                foreach (var (label, value) in summaryItems)
                {
                    int endCol = 3;
                    var range = worksheet.Cells[currentRow, 1, currentRow, endCol];
                    range.Merge = true;
                    range.Value = $"{label}: {value}";
                    range.Style.Font.Bold = true;
                    range.Style.Font.Size = 10;
                    range.Style.Font.Color.SetColor(System.Drawing.Color.Black);
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    
                    worksheet.Row(currentRow).Height = 20;
                    currentRow++;
                }

                // Column widths: Date = 12, other columns = 10
                worksheet.Column(1).Width = 12;
                for (int c = 2; c <= colCount; c++)
                {
                    worksheet.Column(c).Width = 10;
                }

                // Freeze header
                worksheet.View.FreezePanes(tableHeaderRow + 1, 1);

                return package.GetAsByteArray();
            });
        }

        public async Task<byte[]> ExportJobCardToPdfAsync(JobCardReportDto report)
        {
            return await Task.Run(() =>
            {
                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Margin(30);
                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(x => x.FontSize(9).FontFamily(Fonts.Calibri));
                        page.Size(PageSizes.A4.Portrait()); // Portrait layout

                        // Header
                        page.Header().Column(col =>
                        {
                            col.Item().AlignCenter().Text(report.CompanyName).FontSize(14).Bold().FontColor(Colors.Black);
                            if (!string.IsNullOrEmpty(report.CompanyAddress))
                                col.Item().AlignCenter().Text(report.CompanyAddress).FontSize(9).FontColor(Colors.Black);
                            col.Item().AlignCenter().Text("Job Card").FontSize(11).Bold().FontColor(Colors.Black);
                            col.Item().AlignRight().Text($"{report.FromDate} To {report.ToDate}").FontSize(10).Bold().FontColor(Colors.Black);
                            col.Item().PaddingTop(4).LineHorizontal(1f).LineColor("#D1D5DB");
                            col.Item().PaddingBottom(8);
                        });

                        // Content
                        page.Content().Column(col =>
                        {
                            // Employee Info Grid
                            col.Item().Table(infoTable =>
                            {
                                infoTable.ColumnsDefinition(c =>
                                {
                                    c.RelativeColumn(1.5f);
                                    c.RelativeColumn(3f);
                                    c.RelativeColumn(1.5f);
                                    c.RelativeColumn(3f);
                                });

                                infoTable.Cell().Element(CellStyle).Text("ID").Bold().FontColor(Colors.Black);
                                infoTable.Cell().Element(CellStyle).Text(report.EmployeeId).FontColor(Colors.Black);
                                infoTable.Cell().Element(CellStyle).Text("Name").Bold().FontColor(Colors.Black);
                                infoTable.Cell().Element(CellStyle).Text(report.EmployeeName).FontColor(Colors.Black);

                                infoTable.Cell().Element(CellStyle).Text("Designation").Bold().FontColor(Colors.Black);
                                infoTable.Cell().Element(CellStyle).Text(report.Designation).FontColor(Colors.Black);
                                infoTable.Cell().Element(CellStyle).Text("Grade").Bold().FontColor(Colors.Black);
                                infoTable.Cell().Element(CellStyle).Text(report.Grade).FontColor(Colors.Black);

                                infoTable.Cell().Element(CellStyle).Text("Department").Bold().FontColor(Colors.Black);
                                infoTable.Cell().Element(CellStyle).Text(report.Department).FontColor(Colors.Black);
                                infoTable.Cell().Element(CellStyle).Text("Line").Bold().FontColor(Colors.Black);
                                infoTable.Cell().Element(CellStyle).Text(report.Line).FontColor(Colors.Black);

                                infoTable.Cell().Element(CellStyle).Text("Joining Date").Bold().FontColor(Colors.Black);
                                infoTable.Cell().Element(CellStyle).Text(report.JoiningDate).FontColor(Colors.Black);
                                infoTable.Cell().Element(CellStyle).Text("Section").Bold().FontColor(Colors.Black);
                                infoTable.Cell().Element(CellStyle).Text(report.Section).FontColor(Colors.Black);
                            });

                            col.Item().PaddingTop(12);

                            // Daily Log Table
                            col.Item().Table(logTable =>
                            {
                                logTable.ColumnsDefinition(c =>
                                {
                                    c.RelativeColumn(12);
                                    c.RelativeColumn(10);
                                    c.RelativeColumn(10);
                                    c.RelativeColumn(10);
                                    c.RelativeColumn(10);
                                    c.RelativeColumn(10);
                                    c.RelativeColumn(10);
                                    c.RelativeColumn(10);
                                    c.RelativeColumn(10);
                                });

                                var headers = new[] { "Date", "Shift", "In Time", "Out Time", "Late", "Working Hour", "EO", "OT", "Status" };
                                foreach (var h in headers)
                                {
                                    logTable.Cell().Element(CellStyle).AlignCenter().Text(h).Bold().FontSize(9).FontColor(Colors.Black);
                                }

                                foreach (var d in report.Days)
                                {
                                    logTable.Cell().Element(CellStyle).AlignCenter().Text(d.Date).FontSize(8).FontColor(Colors.Black);
                                    logTable.Cell().Element(CellStyle).AlignCenter().Text(d.Shift).FontSize(8).FontColor(Colors.Black);
                                    logTable.Cell().Element(CellStyle).AlignCenter().Text(d.InTime).FontSize(8).FontColor(Colors.Black);
                                    logTable.Cell().Element(CellStyle).AlignCenter().Text(d.OutTime).FontSize(8).FontColor(Colors.Black);
                                    logTable.Cell().Element(CellStyle).AlignCenter().Text(d.Late).FontSize(8).FontColor(Colors.Black);
                                    logTable.Cell().Element(CellStyle).AlignCenter().Text(d.WorkingHour).FontSize(8).FontColor(Colors.Black);
                                    logTable.Cell().Element(CellStyle).AlignCenter().Text(d.EarlyOut).FontSize(8).FontColor(Colors.Black);
                                    logTable.Cell().Element(CellStyle).AlignCenter().Text(d.Overtime).FontSize(8).FontColor(Colors.Black);
                                    logTable.Cell().Element(CellStyle).AlignCenter().Text(d.Status).FontSize(8).FontColor(Colors.Black);
                                }

                                for (int i = 0; i < 9; i++)
                                {
                                    var cell = logTable.Cell().Element(CellStyle);
                                    if (i == 4)
                                        cell.AlignCenter().Text(report.Totals.Late).Bold().FontSize(8).FontColor(Colors.Black);
                                    else if (i == 6)
                                        cell.AlignCenter().Text(report.Totals.EarlyOut).Bold().FontSize(8).FontColor(Colors.Black);
                                    else if (i == 7)
                                        cell.AlignCenter().Text(report.Totals.Overtime).Bold().FontSize(8).FontColor(Colors.Black);
                                    else
                                        cell.Text("");
                                }
                            });
                        });

                        // Footer
                        page.Footer().Column(col =>
                        {
                            col.Item().PaddingTop(8).LineHorizontal(1f).LineColor("#D1D5DB");
                            col.Item().PaddingTop(4);

                            col.Item().AlignLeft().Width(120).Column(summaryCol =>
                            {
                                summaryCol.Item().Text($"Present: {report.Summary.Present}").Bold().FontSize(9).FontColor(Colors.Black);
                                summaryCol.Item().Text($"Absent: {report.Summary.Absent}").Bold().FontSize(9).FontColor(Colors.Black);
                                summaryCol.Item().Text($"Late: {report.Summary.Late}").Bold().FontSize(9).FontColor(Colors.Black);
                            });

                            col.Item().PaddingTop(8).AlignCenter().Text(text =>
                            {
                                text.Span("Page ").FontSize(8).FontColor(Colors.Grey.Medium);
                                text.CurrentPageNumber().FontSize(8).FontColor(Colors.Grey.Medium);
                                text.Span(" of ").FontSize(8).FontColor(Colors.Grey.Medium);
                                text.TotalPages().FontSize(8).FontColor(Colors.Grey.Medium);
                            });
                        });
                    });
                });

                return document.GeneratePdf();
            });
        }

        private static IContainer CellStyle(IContainer container)
        {
            return container.Border(0.5f).BorderColor("#D1D5DB").PaddingVertical(4).PaddingHorizontal(4);
        }

        private static IContainer CellStyle(IContainer container, QuestPDF.Infrastructure.Color backgroundColor)
        {
            return container.Border(0.5f).BorderColor("#D1D5DB").PaddingVertical(4).PaddingHorizontal(4).Background(backgroundColor);
        }

        private static string GetCellText(object? value)
        {
            if (value == null) return "";
            
            return value switch
            {
                DateTime dt => dt.ToString("yyyy-MM-dd"),
                TimeSpan ts => ts.ToString(@"hh\:mm\:ss"),
                double d => d.ToString("F2"),
                decimal dec => dec.ToString("F2"),
                _ => value.ToString() ?? ""
            };
        }

        private static void ApplyStatusStyle(ExcelRange cell, string status)
        {
            if (string.IsNullOrEmpty(status)) return;

            System.Drawing.Color bgColor;
            System.Drawing.Color textColor;

            if (status.Contains("Present", StringComparison.OrdinalIgnoreCase) || 
                status.Contains("Worked", StringComparison.OrdinalIgnoreCase))
            {
                bgColor = System.Drawing.Color.FromArgb(0xE6, 0xF4, 0xEA); // Light green
                textColor = System.Drawing.Color.FromArgb(0x13, 0x73, 0x33); // Dark green
            }
            else if (status.Contains("Late", StringComparison.OrdinalIgnoreCase) || 
                     status.Contains("Early Exit", StringComparison.OrdinalIgnoreCase) || 
                     status.Contains("Half Day", StringComparison.OrdinalIgnoreCase))
            {
                bgColor = System.Drawing.Color.FromArgb(0xFE, 0xF7, 0xED); // Light yellow
                textColor = System.Drawing.Color.FromArgb(0xB0, 0x60, 0x00); // Dark orange/yellow
            }
            else if (status.Contains("Absent", StringComparison.OrdinalIgnoreCase) || 
                     status.Contains("Missing", StringComparison.OrdinalIgnoreCase))
            {
                bgColor = System.Drawing.Color.FromArgb(0xFC, 0xE8, 0xE6); // Light red
                textColor = System.Drawing.Color.FromArgb(0xC5, 0x22, 0x1F); // Dark red
            }
            else if (status.Contains("Leave", StringComparison.OrdinalIgnoreCase) || 
                     status.Contains("Holiday", StringComparison.OrdinalIgnoreCase) || 
                     status.Contains("Off", StringComparison.OrdinalIgnoreCase))
            {
                bgColor = System.Drawing.Color.FromArgb(0xE8, 0xEA, 0xF6); // Light indigo/purple
                textColor = System.Drawing.Color.FromArgb(0x1A, 0x23, 0x7E); // Dark indigo/purple
            }
            else
            {
                return;
            }

            cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
            cell.Style.Fill.BackgroundColor.SetColor(bgColor);
            cell.Style.Font.Color.SetColor(textColor);
            cell.Style.Font.Bold = true;
        }
    }
}