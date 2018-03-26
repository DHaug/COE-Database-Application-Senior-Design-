/*************************************************
 * Project:         SDSU CoE SRMS
 * File:            Excel.cs
 * Author:          Drake Olson
 * Date Created:    2/16/17
 * Date Modified    2/16/17
 *************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using ClosedXML.Excel;
using OfficeOpenXml;

namespace CoE_SRMS.DataModels
{
    class Excel
    {
        private XLWorkbook workbook;
        public IXLWorksheet worksheet;
        private string convertedWorkbook = "";
        private FileInfo workbookDestination;


        public Excel() { }

        public Excel(string pathToWorkBook)
        {
            try
            {
                if (pathToWorkBook.Contains(".csv"))
                {
                    convertedWorkbook = $"{pathToWorkBook.Substring(0,pathToWorkBook.Length-4)} Converted.xlsx";
                    string worksheetName = "Converted CSV";
                    ExcelTextFormat format = new ExcelTextFormat();
                    format.Delimiter = ',';
                    using (ExcelPackage package = new ExcelPackage(new FileInfo(convertedWorkbook)))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(worksheetName);
                        worksheet.Cells["A1"].LoadFromText(new FileInfo(pathToWorkBook), format).Style.Numberformat.Format = "@";
                        package.Save();
                        workbookDestination = package.File;
                    }
                    workbook = new XLWorkbook(convertedWorkbook);
                    worksheet = workbook.Worksheet(1);
                }
                else
                {
                    workbook = new XLWorkbook(pathToWorkBook);
                    worksheet = workbook.Worksheet(1);
                }
                
            }
            catch (IOException)
            {
                MessageBox.Show("Excel Sheet is in use. Please close it and try again.");
                throw new IOException();
            }
        }
        ~Excel()
        {
            if(workbookDestination != null)
            {
                workbookDestination.Delete();
            }
        }
        /// <summary>
        /// Gets the range of the contents in an excel worksheet.
        /// </summary>
        /// <returns>Range of the contents in an excel worksheet.</returns>
        private IXLRange GetContentsRange()
        {
            return worksheet.Range($"A2:{worksheet.RangeUsed().RangeAddress.LastAddress}");
        }
        /// <summary>
        /// Gets all the rows that have content in them.
        /// </summary>
        /// <returns>Collection of rows that have content in them.</returns>
        public IXLRangeRows GetContentRows()
        {
            return GetContentsRange().Rows();
        }
        /// <summary>
        /// Converts a IXLRow to a List<string>.
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public List<string> RowToList(IXLRow row)
        {
            List<string> list = new List<string>();

            foreach(IXLCell c in row.Cells())
            {
                list.Add(c.Value.ToString());
            }
            return list;
        }

        /// <summary>
        /// Gets column headers assuming the headers are in row 1 of the worksheet.
        /// </summary>
        /// <returns></returns>
        public IXLRow GetColumnHeaders()
        {
            return worksheet.Row(1);
        }

        /// <summary>
        /// Find the ColumnLetter of a column header.
        /// </summary>
        /// <param name="headerName"></param>
        /// <returns>ColumnLetter</returns>
        public string FindColumnHeader(string headerName)
        {
            IXLRow headers = GetColumnHeaders();
            foreach (IXLCell c in headers.Cells())
            {
                if (c.Value.ToString().Replace(" ",String.Empty).Equals(headerName.Replace(" ",String.Empty), StringComparison.InvariantCultureIgnoreCase))
                {
                    return c.Address.ColumnLetter;
                }
            }

            return String.Empty;
        }
        /// <summary>
        /// Gets all the attributes of a row
        /// </summary>
        /// <param name="currentRow"></param>
        /// <param name="attributes"></param>
        /// <returns></returns>
        public List<string> GetAttributesFromRow(IXLRow currentRow, List<string> attributes)
        {
            List<string> attributeValues = new List<string>();
            string valueReturned = String.Empty;
            string columnLetter = String.Empty;
            string miscAttributes = String.Empty;

            foreach(string s in attributes)
            {
                columnLetter = FindColumnHeader(s);
                if(columnLetter != String.Empty)
                {
                    attributeValues.Add(currentRow.Cell(columnLetter).Value.ToString());
                }
                else
                {
                    attributeValues.Add(String.Empty);
                }
            }
            return attributeValues;
        }
        
        /// <summary>
        /// Gets all the misc attributes that are not in the database.
        /// </summary>
        /// <param name="currentRow"></param>
        /// <param name="neededAttributes"></param>
        /// <returns>A CSV formatted string of the misc attributes not tracked.</returns>
        public string GetMiscAttributes(IXLRow currentRow, List<string> neededAttributes)
        {
            string miscAttributes = String.Empty;
            List<string> headers = RowToList(GetColumnHeaders());
            string columnLetter = String.Empty;
            string tempHeader;
            string tempattribute;
           
            foreach(string header in headers.ToList())
            {
                tempHeader = header.Replace(" ", String.Empty);
                foreach(string attribute in neededAttributes)
                {
                    tempattribute = attribute.Replace(" ", String.Empty);
                    if (tempHeader.Equals(tempattribute,StringComparison.InvariantCultureIgnoreCase))
                    {
                        headers.Remove(header);
                    }
                }
            }

            foreach(string header in headers)
            {
                columnLetter = FindColumnHeader(header);
                miscAttributes += $"{header},{currentRow.Cell(columnLetter).Value.ToString()},";
            }

            return miscAttributes;
        }
    }
}
