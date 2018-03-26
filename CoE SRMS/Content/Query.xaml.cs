/*************************************************
 * Project:         SDSU CoE SRMS
 * File:            Dashboard.xaml.cs
 * Author:          Derek Haugen
 * Date Created:    1/10/18
 * Date Modified    1/10/18
 *************************************************/

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Xceed.Wpf.Toolkit.Primitives;



namespace CoE_SRMS.Content
{
    /// <summary>
    /// Interaction logic for Query.xaml
    /// </summary>
    public partial class Query : Page
    {

        private List<String> relOps = new List<string> { "<", ">", ">=", "<=", "=" };
        DataSet RetTbl = new DataSet();
        List<string> TableFields;
        List<string> currentFields;
        List<string> whereClauses = new List<string>();
        SqlDataAdapter adapter;



        public Query()
        {
            InitializeComponent();
            PopulateComboBoxes();

            FromTableCombo.SelectionChanged += new SelectionChangedEventHandler(TableComboChanged);
            ExecuteQueryButton.Click += new RoutedEventHandler(ExecuteSelect);
            CurrentTableCheckbox.Click += new RoutedEventHandler(CurrentCheckboxSelected);
            ArchiveTableCheckbox.Click += new RoutedEventHandler(ArchiveCheckboxSelected);
            AddAndButton.Click += new RoutedEventHandler(StartWhereDialog);
            SaveChangesButton.Click += new RoutedEventHandler(SaveData);
            ExportQueryButton.Click += new RoutedEventHandler(ExecuteQueryButton_Click);
            WhereFieldCombo.SelectionChanged += new SelectionChangedEventHandler(WhereFieldChanged);

        }

        private void WhereFieldChanged(object sender, SelectionChangedEventArgs e)
        {
           
            if(WhereFieldCombo.SelectedItem != null && WhereFieldCombo.SelectedItem.ToString() == "NONE")
            {
                UserInputedValueTextBox.IsEnabled = false;
                RelationCombo.IsEnabled = false;
                ExecuteQueryButton.IsEnabled = true;
                ExportQueryButton.IsEnabled = true;
            }
            else
            {
                UserInputedValueTextBox.IsEnabled = true;
                RelationCombo.IsEnabled = true;
                ExecuteQueryButton.IsEnabled = true;
                ExportQueryButton.IsEnabled = true;
            }

        }

        //Starts dialog to build another where statement and grabs the value from it as well
        private void StartWhereDialog(object sender, RoutedEventArgs e)
        {
            if (CheckWhereClauseHasBeenFilled() == true)
            {
                Window1 dialog = new Window1(TableFields, currentFields);
                dialog.ShowDialog();
                string statement = dialog.WhereStatement;
                if (!string.IsNullOrWhiteSpace(statement))
                {
                    whereClauses.Add(statement);
                    AndWhereCombo.Items.Add(statement);
                }
            }
            else
            {
                System.Windows.MessageBox.Show("You must finish a default Where Clause before adding more!");
            }
        }


//Executes Select that the user entered
        private void ExecuteSelect(object sender, RoutedEventArgs e)
        {

            if (CheckValidFields())
            {
                int SelectedItemsTotal = SelectFieldCombo.SelectedItems.Count;
                string selectedItems = "";
                string tableName = "";

                for (int i = 0; i < SelectedItemsTotal; i++)
                {
                    if (SelectFieldCombo.SelectedItems[i] == "All")
                        SelectFieldCombo.SelectedItems[i] = "*";

                    selectedItems += SelectFieldCombo.SelectedItems[i].ToString() + ",";
                }

                MessageBox.Show("Query in progress....");

                selectedItems = selectedItems.Remove(selectedItems.Length - 1);

                switch (FromTableCombo.Text.ToString())
                {
                    case "Students":
                        ReturnedTable.IsReadOnly = false;
                        if (ArchiveTableCheckbox.IsChecked == true)
                            tableName = "ArchiveStudents";
                        else
                            tableName = "CurrentStudents";
                        break;
                    case "Attendance":
                        ReturnedTable.IsReadOnly = true;
                        if (ArchiveTableCheckbox.IsChecked == true)
                            tableName = "ArchiveAttendanceView";
                        else
                            tableName = "CurrentAttendanceView";
                        break;
                    case "Camps":
                        ReturnedTable.IsReadOnly = false;
                        if(ArchiveTableCheckbox.IsChecked == true)
                            tableName = "ArchiveCamps";
                        else
                            tableName = "CurrentCamps";
                        break;
                    case "NoAttendance":
                        ReturnedTable.IsReadOnly = true;
                        if (ArchiveTableCheckbox.IsChecked == true)
                            tableName = "ArchiveNotAttendanceView";
                        else
                            tableName = "CurrentNotAttendanceView";
                        break;
                }

                if (WhereFieldCombo.SelectedItem.ToString() != "NONE")
                {
                    var match = currentFields.FirstOrDefault(stringToCheck => stringToCheck.Contains(WhereFieldCombo.SelectedItem.ToString()));
                    string[] tuple = match.Split(' ');

                    if (tuple[1].Contains("varchar"))
                    {
                        whereClauses.Add(tuple[0] + " " + RelationCombo.SelectedItem.ToString() + " '" + UserInputedValueTextBox.Text.ToString() + "'");
                    }
                    else
                        whereClauses.Add(tuple[0] + " " + RelationCombo.SelectedItem.ToString() + " " + UserInputedValueTextBox.Text.ToString());
                }
                try
                {
                    RetTbl = Database.ExecuteCustomQuery(selectedItems, tableName, whereClauses, ref adapter);
                   
                }
                catch (Exception ex)
                {
                    MessageBox.Show("There was an errorexecuting the query. Make sure your input is legal!");
                }
                           
                try
                {
                    ReturnedTable.ItemsSource = RetTbl.Tables[0].DefaultView; //Make sure to catch exception here
                    ExportQueryButton.IsEnabled = true;
                }
                catch(Exception ex)
                {
                    MessageBox.Show("There was an error loading the table. Make sure your input is legal!");
                }

                //Reset all the fields
                SelectFieldCombo.SelectedItems.Clear();
                WhereFieldCombo.SelectedIndex = -1;
                UserInputedValueTextBox.Text = string.Empty;
                RelationCombo.SelectedIndex = -1;
                whereClauses = new List<string>();
                AndWhereCombo.Items.Clear();
            }
            else
            {
                System.Windows.MessageBox.Show("Please make sure all fields have been filled!");
            }

            if (RetTbl != null)
            {
                ExportQueryButton.IsEnabled = true;
                SaveChangesButton.IsEnabled = true;
            }
        }

        private void TableComboChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectFieldCombo.IsEnabled = true;
            WhereFieldCombo.IsEnabled = true;
            RelationCombo.IsEnabled = true;
            UserInputedValueTextBox.IsEnabled = true;
            String TableSelected = FromTableCombo.SelectedItem.ToString();
            TableFields = new List<string>();


            if (CurrentTableCheckbox.IsChecked == true)
            {
                if (TableSelected == "Students")
                    TableFields = Database.ExecuteGetTableColsAndVals("CurrentStudents");
                else if (TableSelected == "Camps")
                    TableFields = Database.ExecuteGetTableColsAndVals("CurrentCamps");
                else if (TableSelected == "Attendance")
                    TableFields = Database.ExecuteGetTableColsAndVals("CurrentAttendanceView");
                else if (TableSelected == "NoAttendance")
                    TableFields = Database.ExecuteGetTableColsAndVals("CurrentNotAttendanceView");
            }
            else if (ArchiveTableCheckbox.IsChecked == true)
            {
                if (TableSelected == "Students")
                    TableFields = Database.ExecuteGetTableColsAndVals("ArchiveStudents");
                else if (TableSelected == "Camps")
                    TableFields = Database.ExecuteGetTableColsAndVals("ArchiveCamps");
                else if (TableSelected == "Attendance")
                    TableFields = Database.ExecuteGetTableColsAndVals("ArchiveAttendanceView");
                else if (TableSelected == "NoAttendance")
                    TableFields = Database.ExecuteGetTableColsAndVals("ArchiveNotAttendanceView");
            }



            SelectFieldCombo.Items.Clear();
            WhereFieldCombo.Items.Clear();
            SelectFieldCombo.Items.Add("All");
            WhereFieldCombo.Items.Add("NONE");
            currentFields = new List<string>();

            foreach (string Column in TableFields)
            {
                currentFields.Add(Column);
                string[] temp = Column.Split(' ');
                SelectFieldCombo.Items.Add(temp[0]);
                WhereFieldCombo.Items.Add(temp[0]);
            }


        }

        public void PopulateComboBoxes()
        {
            List<string> TableFields = new List<string> { "Students", "Camps", "Attendance", "NoAttendance" };

            foreach (string Column in TableFields)
            {
                FromTableCombo.Items.Add(Column);
            }

            foreach (string Entry in relOps)
            {
                RelationCombo.Items.Add(Entry);
            }

            SelectFieldCombo.IsEnabled = false;
            WhereFieldCombo.IsEnabled = false;
            RelationCombo.IsEnabled = false;
            UserInputedValueTextBox.IsEnabled = false;
            ExecuteQueryButton.IsEnabled = false;
            ExportQueryButton.IsEnabled = false;
            CurrentTableCheckbox.IsChecked = true;
            ArchiveTableCheckbox.IsChecked = false;
            OrderCombo.IsEnabled = false;
            SaveChangesButton.IsEnabled = false;
        }

        public void OnExecuteClick()
        {

        }

        public void OnExportClick()
        {

        }

        private void ArchiveCheckboxSelected(object sender, RoutedEventArgs e)
        {
            CurrentTableCheckbox.IsChecked = false;
        }

        private void CurrentCheckboxSelected(object sender, RoutedEventArgs e)
        {
            ArchiveTableCheckbox.IsChecked = false;
        }

        private void ExecuteQueryButton_Click(object sender, RoutedEventArgs e)
        {

            if (ConfigurationManager.AppSettings["UserRole"] == "ReadOnly")
            {
                MessageBox.Show("You cannot Export data with only Read-Only Access!");
                return;
            }

            bool isCurrent = false;

            if (CurrentTableCheckbox.IsChecked == true)
                isCurrent = true;
            NavigationService?.Navigate(new Export(RetTbl.Tables[0] , isCurrent));
     

        }
        private bool CheckValidFields()
        {

            if (!(SelectFieldCombo.SelectedItems.Count > -1))
                return false;
            if (!(FromTableCombo.SelectedIndex > -1))
                return false;
            if(WhereFieldCombo.SelectedItem.ToString() == "NONE")
            {
                return true;
            }
            if (!(UserInputedValueTextBox.Text.Length > 0))
                return false;
            if (!(RelationCombo.SelectedIndex > -1))
                return false;

            return true;
        }

        private bool CheckWhereClauseHasBeenFilled()
        {

            if (!(UserInputedValueTextBox.Text.Length > 0))
                return false;
            if (!(RelationCombo.SelectedIndex > -1))
                return false;
            if (!(WhereFieldCombo.SelectedIndex > -1))
                return false;

            return true;

        }


        void SaveData(object sender, RoutedEventArgs e)
        {
            if(ConfigurationManager.AppSettings["UserRole"] == "ReadOnly")
            {
                MessageBox.Show("You cannot edit data with only Read-Only Access!");
                return;
            }

            try
            {
                SqlCommandBuilder com = new SqlCommandBuilder(adapter);
                adapter.Update(RetTbl);
            }
            catch(Exception ex)
            {
                MessageBox.Show("Unable to update table.");
                return;
            }

            MessageBox.Show("Successfully Updated Table!");
           
        }


































    } 
}
