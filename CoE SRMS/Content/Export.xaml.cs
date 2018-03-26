/*************************************************
 * Project:         SDSU CoE SRMS
 * File:            Export.xaml.cs
 * Author:          Chandler Bauer
 * Date Created:    1/10/18
 * Date Modified    1/10/18
 *************************************************/

using ClosedXML.Excel;
using System.Data;
using System.Windows.Controls;
using System.IO;
using Microsoft.Win32;
using Microsoft.Office.Interop.Outlook;
using Outlook = Microsoft.Office.Interop.Outlook;
using OutlookApp = Microsoft.Office.Interop.Outlook.Application;
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using WordApp = Microsoft.Office.Interop.Word.Application;
using Word = Microsoft.Office.Interop.Word;
using System.Windows;

namespace CoE_SRMS.Content
{
    /// <summary>
    /// Interaction logic for Query.xaml
    /// </summary>
    public partial class Export : Page
    {
        private bool isCurrent;
        public Export(DataTable dataTable, bool isCurrent)
        {
            InitializeComponent();
            ExportTable.ItemsSource = dataTable.DefaultView;
            this.isCurrent = isCurrent;
        }

        
        public void OnPostalExcelClick()
        {
            DataTable outputtable = ((DataView)ExportTable.ItemsSource).ToTable();
            int indexOfID = FindAttribute("ID");
            DataTable excel = new DataTable();
            excel.Columns.Add("First Name");
            excel.Columns.Add("Last Name");
            excel.Columns.Add("Address");
            excel.Columns.Add("City");
            excel.Columns.Add("State");
            excel.Columns.Add("Zip");
            foreach (DataRow r in outputtable.Rows)
            {
                DataRow test = Database.GetMailingInformation(r[indexOfID].ToString());
                excel.Rows.Add(test.ItemArray);
            }
            for(int i = 0; i < excel.Rows.Count; i++)
            {
                if(String.IsNullOrWhiteSpace(excel.Rows[2].ToString()) || String.IsNullOrWhiteSpace(excel.Rows[3].ToString()) || String.IsNullOrWhiteSpace(excel.Rows[4].ToString()) || String.IsNullOrWhiteSpace(excel.Rows[5].ToString()))
                {
                    excel.Rows.RemoveAt(i);
                }
            }
            SaveFileDialog file = new SaveFileDialog();
            file.DefaultExt = ".xlsx";
            file.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm;*.csv;";
            file.ShowDialog();
            if (file.FileName != "")
            {
                XLWorkbook workbook = new XLWorkbook();
                workbook.Worksheets.Add(excel, "Mailing List");
                workbook.SaveAs(file.FileName);
            }
            if (File.Exists(file.FileName))
            {
                MessageBox.Show("   Export Successful   ");
            }
        }

        public void OnPostalWordClick()
        {
            DataTable outputtable = ((DataView)ExportTable.ItemsSource).ToTable();
            int indexOfID = FindAttribute("ID");
            DataTable excel = new DataTable();
            excel.Columns.Add("First Name");
            excel.Columns.Add("Last Name");
            excel.Columns.Add("Address");
            excel.Columns.Add("City");
            excel.Columns.Add("State");
            excel.Columns.Add("Zip");
            foreach (DataRow r in outputtable.Rows)
            {
                DataRow test = Database.GetMailingInformation(r[indexOfID].ToString());
                excel.Rows.Add(test.ItemArray);
            }
            for (int i = 0; i < excel.Rows.Count; i++)
            {
                if (String.IsNullOrWhiteSpace(excel.Rows[2].ToString()) || String.IsNullOrWhiteSpace(excel.Rows[3].ToString()) || String.IsNullOrWhiteSpace(excel.Rows[4].ToString()) || String.IsNullOrWhiteSpace(excel.Rows[5].ToString()))
                {
                    excel.Rows.RemoveAt(i);
                }
            }
            SaveFileDialog file = new SaveFileDialog();
            file.FileName = "untitled";
            file.Filter = "Word Doc|*.doc";
            file.ShowDialog();
            if (file.FileName != "")
            {
                WordApp application = null;
                try
                {
                    application = new WordApp();
                    var document = application.Documents.Add();
                    for (int i = 0; i < excel.Rows.Count; i++)
                    {
                        var paragraph = document.Paragraphs.Add();
                        paragraph.Range.Text += $"{excel.Rows[i][0].ToString()},{excel.Rows[i][1].ToString()},{excel.Rows[i][2].ToString()},{excel.Rows[i][3].ToString()},{excel.Rows[i][4].ToString()},{excel.Rows[i][5].ToString()}\n";
                    }
                    application.ActiveDocument.SaveAs2(file.FileName, Word.WdSaveFormat.wdFormatDocument);
                    document.Close();
                }
                finally
                {
                    if (application != null)
                    {
                        application.Quit();
                        Marshal.FinalReleaseComObject(application);
                    }
                }
            }
            if (File.Exists(file.FileName))
            {
                MessageBox.Show("   Export Successful   ");
            }
        }

        private int FindAttribute(string attribute)
        {
            for(int i = 0; i < ExportTable.Columns.Count; i++)
            {
                if(((string)ExportTable.Columns[i].Header).ToUpper() == attribute)
                {
                    return i;
                }
            }
            return -1;
        }

        public void OnExcelClick()
        {
            XLWorkbook workbook = new XLWorkbook();
            workbook.Worksheets.Add(((DataView)ExportTable.ItemsSource).ToTable(), "Exported Query");
            SaveFileDialog file = new SaveFileDialog();
            file.DefaultExt = ".xlsx";
            file.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm;*.csv;";
            file.ShowDialog();
            if(file.FileName != "")
            {
                workbook.SaveAs(file.FileName);
            }
            if (File.Exists(file.FileName))
            {
                MessageBox.Show("   Export Successful   ");
            }

        }

        public void OnWordClick()
        {
            SaveFileDialog file = new SaveFileDialog();
            file.FileName = "untitled";
            file.Filter = "Word Doc (*.doc)|*.doc";
            file.ShowDialog();
            if(file.FileName != "")
            {
                WordApp application = null;
                try
                {
                    DataTable excel = ((DataView)ExportTable.ItemsSource).ToTable();
                    
                    application = new WordApp();
                    var document = application.Documents.Add();
                    var paragraph = document.Paragraphs.Add();
                    for (int i = 0; i < excel.Rows.Count; i++)
                    {
                        string tempText = "";
                        for(int j = 0; j < excel.Columns.Count; j++)
                        {
                            if (excel.Rows[i][j].ToString() != "")
                            {
                                tempText += $"{excel.Rows[i][j].ToString()}, ";
                            }
                        }
                        paragraph.Range.Text += tempText.TrimEnd(',') + "\n";
                    }
                    application.ActiveDocument.SaveAs2(file.FileName, Word.WdSaveFormat.wdFormatDocument);
                    document.Close();
                }
                finally
                {
                    if (application != null)
                    {
                        application.Quit();
                        Marshal.FinalReleaseComObject(application);
                    }
                }
                if (File.Exists(file.FileName))
                {
                    MessageBox.Show("   Export Successful   ");
                }
            }
        }

        public void OnEMailClick()
        {
            OutlookApp application = new OutlookApp();
            MailItem email = application.CreateItem(OlItemType.olMailItem);
            email.Subject = "";
            int index = FindAttribute("EMAIL");
            if(index > 0)
            {
                foreach (DataRow r in ((DataView)ExportTable.ItemsSource).ToTable().Rows)
                {
                    if (r[index].ToString() != String.Empty)
                    {
                        Outlook.Recipient rec = email.Recipients.Add(r[index].ToString());
                        rec.Type = (int)Outlook.OlMailRecipientType.olBCC;
                        email.Recipients.ResolveAll();
                    }
                }
            }
            email.Display(false);
        }

        private void PostalExcel_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            OnPostalExcelClick();
        }

        private void PostalWord_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            OnPostalWordClick();
        }

        private void RegularExcel_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            OnExcelClick();
        }

        private void RegularWord_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            OnWordClick();
        }

        private void Email_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            OnEMailClick();
        }
    }
    
}
