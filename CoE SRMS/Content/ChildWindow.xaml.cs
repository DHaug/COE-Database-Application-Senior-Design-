using System;
using System.Windows.Navigation;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Collections.Generic;
using System.Windows;
using System.Data;
using System.Windows.Controls;

namespace CoE_SRMS.Content
{
    /// <summary>
    /// Interaction logic for ChildWindow.xaml
    /// </summary>
    public partial class ChildWindow : Window
    {
        private DataTable duplicates;
        private DataTable camps;
        private List<string> importAttributes;
        private List<string> importAttributesCopy;
        private ComboBox CampsListComboBox;
        private bool isAttendance = true;
        public ChildWindow(List<string> importAttributes,DataTable duplicates,DataTable camps,ComboBox CampsListComboBox,bool isAttendance = true)
        { 
            InitializeComponent();
            this.HorizontalAlignment = HorizontalAlignment.Center;
            this.VerticalAlignment = VerticalAlignment.Center;
            this.duplicates = duplicates;
            this.camps = camps;
            this.importAttributes = importAttributes;
            this.CampsListComboBox = CampsListComboBox;
            this.isAttendance = isAttendance;
            firstNameLabelLeft.Content = "First Name: " + importAttributes[0];
            lastNameLabelLeft.Content = "Last Name: " + importAttributes[1];
            if (importAttributes[11].Length > 10)
            {
                birthdateLabelLeft.Content = "Birth Date: " + importAttributes[11].Substring(0, importAttributes[11].Length - 12);
            }
            else
            {
                birthdateLabelLeft.Content = "Birth Date: " + importAttributes[11];
            }
            parentNameLabelLeft.Content = "Parent Name: " + importAttributes[10];
            currentSchoolLabelLeft.Content = "Current School: " + importAttributes[15];

            int studentCounter = 1;
            foreach (DataRow row in duplicates.Rows)
            {
                DuplicateStudentsDropDown.Items.Add($"Student {studentCounter}");
                studentCounter++;
            }
            importAttributesCopy = importAttributes;
        }
        private void DuplicateStudentsDropDown_Selected(object sender, RoutedEventArgs e)
        {
            int indexOfStudent = DuplicateStudentsDropDown.SelectedIndex;

            firstNameLabelRight.Content = $"First Name: {this.duplicates.Rows[indexOfStudent][1].ToString()}";
            lastNameLabelRight.Content = $"Last Name: {this.duplicates.Rows[indexOfStudent][2].ToString()}";
            if (this.duplicates.Rows[indexOfStudent][12].ToString().Contains("1970"))
            {
                birthdateLabelRight.Content = $"Birth Date: ";
            }
            else
            {
                birthdateLabelRight.Content = $"Birth Date: {this.duplicates.Rows[indexOfStudent][12].ToString().ToString().Substring(0, this.duplicates.Rows[indexOfStudent][12].ToString().Length - 12)}";
            }
            parentNameLabelRight.Content = $"Parent Name: {this.duplicates.Rows[indexOfStudent][11].ToString()}";
            currentSchoolLabelLeft.Content = $"Current School: {this.duplicates.Rows[indexOfStudent][16].ToString()}";

        }

        private void NotDuplicateButton_Click(object sender, RoutedEventArgs e)
        {
            if (isAttendance)
            {
                Database.ExecuteStudentImport(importAttributesCopy[0], importAttributesCopy[1], importAttributesCopy[3], importAttributesCopy[16], importAttributesCopy[11], importAttributesCopy[12], importAttributesCopy[13],
                                                  importAttributesCopy[14], importAttributesCopy[4], importAttributesCopy[5], importAttributesCopy[6], importAttributesCopy[7], importAttributesCopy[9], importAttributesCopy[8], importAttributesCopy[10],
                                                  importAttributesCopy[2], importAttributesCopy[15], importAttributesCopy[22], importAttributesCopy[23], importAttributesCopy[17], Convert.ToBoolean(importAttributesCopy[18]),
                                                  importAttributesCopy[19], importAttributesCopy[20], importAttributesCopy[24], importAttributesCopy[21]);
                foreach (DataRow c in camps.Rows)
                {
                    string rowCurrentCamp = $"{c[2].ToString()} {Convert.ToDateTime(c[1].ToString()).Date}".Substring(0, c[2].ToString().Length + c[1].ToString().Length - 11);
                    if (rowCurrentCamp == CampsListComboBox.SelectedValue.ToString())
                    {
                        int campID = Convert.ToInt32(c[0].ToString());
                        Database.ExecuteAddAttendance(campID, importAttributesCopy[0], importAttributesCopy[1], importAttributesCopy[11], importAttributesCopy[2]);
                    }
                }
            }
            else
            {
                Database.ExecuteStudentImport(importAttributesCopy[0], importAttributesCopy[1], importAttributesCopy[3], importAttributesCopy[16], importAttributesCopy[11], importAttributesCopy[12], importAttributesCopy[13],
                                                  importAttributesCopy[14], importAttributesCopy[4], importAttributesCopy[5], importAttributesCopy[6], importAttributesCopy[7], importAttributesCopy[9], importAttributesCopy[8], importAttributesCopy[10],
                                                  importAttributesCopy[2], importAttributesCopy[15], importAttributesCopy[22], importAttributesCopy[23], importAttributesCopy[17], Convert.ToBoolean(importAttributesCopy[18]),
                                                  importAttributesCopy[19], importAttributesCopy[20], importAttributesCopy[24], importAttributesCopy[21]);
            }
            this.Close();
        }
        private void SubmitDuplicateButton_Click(object sender, RoutedEventArgs e)
        {
            if(DuplicateStudentsDropDown.SelectedItem == null)
            {
                MessageBox.Show("You must pick a student.");
            }
            else
            {
                if (isAttendance)
                {
                    int indexOfStudent = DuplicateStudentsDropDown.SelectedIndex;

                    for (int i = 0; i <= indexOfStudent; i++)
                    {
                        if (i == indexOfStudent)
                        {
                            int studentID = Convert.ToInt32(duplicates.Rows[i][0].ToString());
                            int campID = Convert.ToInt32(camps.Rows[CampsListComboBox.SelectedIndex][0].ToString());
                            Database.ExecuteAddAttendanceManual(campID, studentID);
                        }
                    }
                }
                this.Close();
            }
        }
    }
}
