using System;
using System.Data;
using System.IO;

using OfficeOpenXml;

namespace DestinyHelper
{
    public class ExcelExport
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExcelExport"/> class.
        /// </summary>
        public ExcelExport()
        {
            this.ExportFileName = "ExportData";
        }

        /// <summary>
        /// Gets or sets the ExportFileName property.
        /// </summary>
        /// <remarks>
        /// This should not include the file extension.
        /// </remarks>
        public string ExportFileName { get; set; }

        /// <summary>
        /// Export the data from the database to an excel spreadsheet, using the default file location.
        /// </summary>
        /// <param name="table">The data table</param>
        /// <param name="worksheetName">The name of the worksheet to write to</param>
        public void Export(DataTable table, string worksheetName)
        {
            string defaultFileLocation = string.Format("{0}{1}.xlsx", AppDomain.CurrentDomain.BaseDirectory, this.ExportFileName);
            this.Export(table, worksheetName, defaultFileLocation);
        }

        /// <summary>
        /// Export the data from the database to an excel spreadsheet.
        /// </summary>
        /// <param name="table">The data table</param>
        /// <param name="worksheetName">The name of the worksheet to write to</param>
        /// <param name="filePath">The file path to write to</param>
        public void Export(DataTable table, string worksheetName, string filePath)
        {
            FileInfo file = new FileInfo(filePath);

            if (file.Exists)
            {
                this.InsertRowToExcelSpreadsheet(table, worksheetName, file);
            }
            else
            {
                this.ExportDataToNewExcelFile(table, worksheetName, file);
            }
        }

        /// <summary>
        /// Export the data to a new excel file.
        /// </summary>
        /// <param name="table">The table to use</param>
        /// <param name="worksheetName">The name of the worksheet to write to</param>
        /// <param name="file">The file to create</param>
        private void ExportDataToNewExcelFile(DataTable table, string worksheetName, FileInfo file)
        {
            using (ExcelPackage pkg = new ExcelPackage(file))
            {
                this.PopulateNewWorksheet(pkg, table, worksheetName);
                pkg.Save();
            }
        }

        /// <summary>
        /// Export the data to an existing spreadsheet
        /// </summary>
        /// <param name="table">The table to use</param>
        /// <param name="worksheetName">The name of the worksheet to write to</param>
        /// <param name="file">The file to use</param>
        private void InsertRowToExcelSpreadsheet(DataTable table, string worksheetName, FileInfo file)
        {
            using (ExcelPackage pkg = new ExcelPackage(file))
            {
                ExcelWorksheet ws = pkg.Workbook.Worksheets[worksheetName];

                if (ws == null)
                {
                    this.PopulateNewWorksheet(pkg, table, worksheetName);
                }
                else
                {
                    this.PopulateExistingWorksheet(ws, table);
                }
                
                pkg.Save();
            }
        }

        /// <summary>
        /// Populate a new worksheet.
        /// </summary>
        /// <param name="pkg">The ExcelPackage to use</param>
        /// <param name="table">The table to use</param>
        /// <param name="worksheetName">The worksheet name</param>
        private void PopulateNewWorksheet(ExcelPackage pkg, DataTable table, string worksheetName)
        {
            ExcelWorksheet ws = pkg.Workbook.Worksheets.Add(worksheetName);
            bool includeHeaders = true;
            ws.Cells["A1"].LoadFromDataTable(table, includeHeaders);
            ws.View.FreezePanes(2, 1);
            ws.Row(1).Style.Font.Bold = true;
            ws.Cells[ws.Dimension.Address].AutoFitColumns();
        }

        /// <summary>
        /// Populate a existing worksheet.
        /// </summary>
        /// <param name="pkg">The ExcelPackage to use</param>
        /// <param name="table">The table to use</param>
        private void PopulateExistingWorksheet(ExcelWorksheet ws, DataTable table)
        {
            int endRowNumber = ws.Dimension.End.Row;
            int nextRowNumber = endRowNumber + 1;
            string cellName = string.Format("A{0}", nextRowNumber);
            bool includeHeaders = false;
            ws.Cells[cellName].LoadFromDataTable(table, includeHeaders);
        }
    }
}
