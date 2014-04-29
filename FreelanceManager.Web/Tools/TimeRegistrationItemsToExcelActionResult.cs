using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using FreelanceManager.ReadModel;
using Nancy;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace FreelanceManager.Web.Tools
{
    public static class TimeRegistrationsToExcelExtension
    {
        public static Response AsExcel(this IResponseFormatter response, IEnumerable<TimeRegistration> items)
        {
            using (var package = new ExcelPackage())
            {
                WriteSheet(package, items);
                var stream = new MemoryStream(package.GetAsByteArray());

                return response.FromStream(() => stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                              .WithHeader("content-disposition", string.Format("attachment;filename=Timeregistrations_all.xlsx"));
            }

        }

        private static void WriteSheet(ExcelPackage package, IEnumerable<TimeRegistration> items)
        {
            var worksheet = package.Workbook.Worksheets.Add("Report");

            var columnCount = 1;
            var totalColumnCount = 1;
            var rowCount = 2;

            foreach (var column in new[] { "Project", "Client", "Task", "Description", "Income", "Date", "From", "To" })
            {
                worksheet.Cells[1, columnCount].Value = column;
                StyleHeader(worksheet.Cells[1, columnCount].Style);
                worksheet.Column(columnCount).Width = 20;
                columnCount++;
                totalColumnCount++;
            }

            foreach (var item in items.OrderBy(i => i.Date.Numeric).ThenBy(i => i.From.Numeric))
            {
                worksheet.Cells[rowCount, 1].Value = item.ProjectName;
                worksheet.Cells[rowCount, 2].Value = item.ClientName;
                worksheet.Cells[rowCount, 3].Value = item.Task;
                worksheet.Cells[rowCount, 4].Value = item.Description;
                worksheet.Cells[rowCount, 5].Value = item.Income.ToString();
                worksheet.Cells[rowCount, 6].Value = item.Date.ToString();
                worksheet.Cells[rowCount, 7].Value = item.From.ToString();
                worksheet.Cells[rowCount, 8].Value = item.To.ToString();

                rowCount++;
            }

            worksheet.Cells[1, 1, 1, totalColumnCount - 1].AutoFilter = true;
            StyleCells(worksheet.Cells[1, 1, rowCount - 1, totalColumnCount - 1].Style);
        }

        private static void StyleHeader(ExcelStyle style)
        {
            style.Font.Bold = true;
            style.Fill.PatternType = ExcelFillStyle.Solid;
            style.Fill.BackgroundColor.SetColor(Color.FromArgb(50, 118, 177));
            style.Font.Color.SetColor(Color.FromArgb(255, 255, 255));
        }

        private static void StyleCells(ExcelStyle style)
        {
            style.Border.Top.Style = ExcelBorderStyle.Thin;
            style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            style.Border.Left.Style = ExcelBorderStyle.Thin;
            style.Border.Right.Style = ExcelBorderStyle.Thin;

            style.Border.Top.Color.SetColor(Color.FromArgb(85, 85, 85));
            style.Border.Bottom.Color.SetColor(Color.FromArgb(85, 85, 85));
            style.Border.Left.Color.SetColor(Color.FromArgb(85, 85, 85));
            style.Border.Right.Color.SetColor(Color.FromArgb(85, 85, 85));
        }
    }
}