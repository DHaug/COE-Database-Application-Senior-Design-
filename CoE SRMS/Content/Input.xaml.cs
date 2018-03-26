using ClosedXML.Excel;
using CoE_SRMS.DataModels;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace CoE_SRMS.Content
{
    /// <summary>
    /// Interaction logic for Input.xaml
    /// </summary>
    public partial class Input : Page
    {
        private DataTable camps;
        private readonly List<string> genders = new List<string>() { "Male", "M", "Female", "F","Other","O" };
        private readonly List<string> races = new List<string>();
        private readonly List<string> ethnicities = new List<string>();

        public Input()
        {
            InitializeComponent();
        }

        private void ClearFields()
        {
            FirstNameInput.Text = String.Empty;
            LastNameInput.Text = String.Empty;
            PreferredNameInput.Text = String.Empty;
            EmailInput.Text = String.Empty;
            BirthDateInput.Text = String.Empty;
            GenderInput.Text = String.Empty;
            EthnicityInput.Text = String.Empty;
            RaceInput.Text = String.Empty;
            AddressInput.Text = String.Empty;
            CityInput.Text = String.Empty;
            StateInput.Text = String.Empty;
            PostalCodeInput.Text = String.Empty;
            CellPhoneInput.Text = String.Empty;
            HomePhoneInput.Text = String.Empty;
            ParentNameInput.Text = String.Empty;
            CurrentSchoolInput.Text = String.Empty;
            IntendedDateOfGraduationInput.Text = String.Empty;
            CurrentSchoolInput.Text = String.Empty;
            HomePhoneInput.Text = String.Empty;
            TShirtSizeInput.Text = String.Empty;
            InsuranceInput.Text = String.Empty;
            DateLastContacted.Text = String.Empty;
            CampNameInput.Text = String.Empty;
            CampDateInput.Text = String.Empty;
        }

        private void SubmitStudentButton_Click(object sender, RoutedEventArgs e)
        {
            if(String.IsNullOrWhiteSpace(FirstNameInput.Text) || String.IsNullOrWhiteSpace(LastNameInput.Text) || String.IsNullOrWhiteSpace(CurrentSchoolInput.Text) || String.IsNullOrWhiteSpace(BirthDateInput.Text))
            {
                MessageBox.Show("Error: Couldn't submit student. Required Feilds are left empty.");
            }
            else
            {
                if (IsValidInputUI())
                {
                    DataTable duplicates = Database.CheckForDuplicates(FirstNameInput.Text, LastNameInput.Text, CurrentSchoolInput.Text, BirthDateInput.Text);
                    if (CampsListBorder.Visibility == Visibility.Hidden || CampsListComboBox.SelectedValue == null)
                    {
                        if(duplicates.Rows.Count > 0)
                        {
                            foreach (DataRow r in duplicates.Rows)
                            {
                                MessageBoxResult result = MessageBox.Show($"Is this a duplicate? Inputted:{FirstNameInput.Text} {LastNameInput.Text}, {CurrentSchoolInput.Text}, {CurrentYearInSchoolInput} Found: {r[1].ToString()} {r[2].ToString()}, {r[3].ToString()},{r[12].ToString()}", "Confirmation", MessageBoxButton.YesNo);

                                if (result == MessageBoxResult.No)
                                {
                                    Database.ExecuteStudentImport(FirstNameInput.Text, LastNameInput.Text, PreferredNameInput.Text, EmailInput.Text, BirthDateInput.Text, GenderInput.Text, EthnicityInput.Text,
                                                      RaceInput.Text, AddressInput.Text, CityInput.Text, StateInput.Text, PostalCodeInput.Text, CellPhoneInput.Text, HomePhoneInput.Text, ParentNameInput.Text,
                                                      CurrentSchoolInput.Text, IntendedDateOfGraduationInput.Text, CurrentYearInSchoolInput.Text, TShirtSizeInput.Text, InsuranceInput.Text, PotentialEngineerInput.IsChecked,
                                                      StudentIDInput.Text, DateLastContacted.Text);
                                }
                            }
                        }
                        else
                        {
                            Database.ExecuteStudentImport(FirstNameInput.Text, LastNameInput.Text, PreferredNameInput.Text, EmailInput.Text, BirthDateInput.Text, GenderInput.Text, EthnicityInput.Text,
                                                      RaceInput.Text, AddressInput.Text, CityInput.Text, StateInput.Text, PostalCodeInput.Text, CellPhoneInput.Text, HomePhoneInput.Text, ParentNameInput.Text,
                                                      CurrentSchoolInput.Text, IntendedDateOfGraduationInput.Text, CurrentYearInSchoolInput.Text, TShirtSizeInput.Text, InsuranceInput.Text, PotentialEngineerInput.IsChecked,
                                                      StudentIDInput.Text, DateLastContacted.Text);
                        }
                        
                    }
                    else
                    {
                        int campID;
                        if(duplicates.Rows.Count <= 0)
                        {
                            Database.ExecuteStudentImport(FirstNameInput.Text, LastNameInput.Text, PreferredNameInput.Text, EmailInput.Text, BirthDateInput.Text, GenderInput.Text, EthnicityInput.Text,
                                                  RaceInput.Text, AddressInput.Text, CityInput.Text, StateInput.Text, PostalCodeInput.Text, CellPhoneInput.Text, HomePhoneInput.Text, ParentNameInput.Text,
                                                  CurrentSchoolInput.Text, IntendedDateOfGraduationInput.Text, CurrentYearInSchoolInput.Text, TShirtSizeInput.Text, InsuranceInput.Text, PotentialEngineerInput.IsChecked,
                                                  StudentIDInput.Text, DateLastContacted.Text);
                        }
                        else
                        {
                            if (duplicates.Rows.Count > 0)
                            {
                                foreach (DataRow row in duplicates.Rows)
                                {
                                    MessageBoxResult result = MessageBox.Show($"Is this a duplicate? Inputted:{FirstNameInput.Text} {LastNameInput.Text}, {CurrentSchoolInput.Text}, {CurrentYearInSchoolInput} Found: {row[1].ToString()} {row[2].ToString()}, {row[3].ToString()},{row[12].ToString()}", "Confirmation", MessageBoxButton.YesNo);

                                    if (result == MessageBoxResult.No)
                                    {
                                        Database.ExecuteStudentImport(FirstNameInput.Text, LastNameInput.Text, PreferredNameInput.Text, EmailInput.Text, BirthDateInput.Text, GenderInput.Text, EthnicityInput.Text,
                                                          RaceInput.Text, AddressInput.Text, CityInput.Text, StateInput.Text, PostalCodeInput.Text, CellPhoneInput.Text, HomePhoneInput.Text, ParentNameInput.Text,
                                                          CurrentSchoolInput.Text, IntendedDateOfGraduationInput.Text, CurrentYearInSchoolInput.Text, TShirtSizeInput.Text, InsuranceInput.Text, PotentialEngineerInput.IsChecked,
                                                          StudentIDInput.Text, DateLastContacted.Text);
                                        foreach (DataRow r in camps.Rows)
                                        {
                                            string rowCurrentCamp = $"{r[2].ToString()} {Convert.ToDateTime(r[1].ToString()).Date}".Substring(0, r[2].ToString().Length + r[1].ToString().Length - 11);
                                            if (rowCurrentCamp == CampsListComboBox.SelectedValue.ToString())
                                            {
                                                campID = Convert.ToInt32(r[0].ToString());
                                                Database.ExecuteAddAttendance(campID, FirstNameInput.Text, LastNameInput.Text, BirthDateInput.Text, CurrentSchoolInput.Text);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    MessageBox.Show("Successful inserted");
                }
            }
        }


        private bool IsValidInputUI()
        {
            bool validInput = true;
            DateTime tempDateTime;
            int tempNum;
            List<TextBox> inputs = new List<TextBox>();
            inputs.Add(PreferredNameInput);
            inputs.Add(EmailInput);
            inputs.Add(BirthDateInput);
            inputs.Add(CityInput);
            inputs.Add(StateInput);
            inputs.Add(PostalCodeInput);
            inputs.Add(CellPhoneInput);
            inputs.Add(HomePhoneInput);
            inputs.Add(ParentNameInput);
            inputs.Add(CurrentSchoolInput);
            inputs.Add(IntendedDateOfGraduationInput);
            inputs.Add(CurrentYearInSchoolInput);
            inputs.Add(InsuranceInput);
            inputs.Add(DateLastContacted);
            for(int i = 0; i < inputs.Count; i++)
            {
                if(inputs[i].Text != String.Empty)
                {
                    if(i == 0 && Regex.Match(ParentNameInput.Text, @"^[A-z]*[-A-z]*").Success == false)
                    {
                        PreferredNameBorder.Background = Brushes.Red;
                        validInput = false;
                    }
                    else if(i == 1 && !inputs[i].Text.Contains("@"))
                    {
                        EmailBorder.Background = Brushes.Red;
                        validInput = false;
                    }
                    else if (i == 2 && !DateTime.TryParse(inputs[i].Text,out tempDateTime))
                    {
                        BirthDateInput.BorderBrush = Brushes.Red;
                        validInput = false;
                    }
                    else if (i == 3 && Regex.Match(inputs[i].Text, @"^[0-9A-Za-z ]*").Success == false)
                    {
                        CityBorder.Background = Brushes.Red;
                        validInput = false;
                    }
                    else if (i == 4 && Regex.Match(inputs[i].Text, @"^[0-9A-Za-z]*").Success == false)
                    {
                        StateBorder.Background = Brushes.Red;
                        validInput = false;
                    }
                    else if (i == 5 && !Int32.TryParse(inputs[i].Text.ToString(), out tempNum))
                    {
                        PostalCodeBorder.Background = Brushes.Red;
                        validInput = false;
                    }
                    else if ((i == 6 || i == 7) && Regex.Match(inputs[i].Text, @"[0-9]*[.0-9]*").Success == false)
                    {
                        if(i == 6)
                        {
                            CellPhoneBorder.Background = Brushes.Red;
                        }
                        else
                        {
                            HomePhoneBorder.Background = Brushes.Red;
                        }
                        validInput = false;
                    }
                    else if (i == 8 && Regex.Match(inputs[i].Text, @"^[A-z]*[-A-z]*[&A-z]*[& A-z]*").Success == false)
                    {
                        ParentNameBorder.Background = Brushes.Red;
                        validInput = false;
                    }
                    else if (i == 9 && Regex.Match(inputs[i].Text, @"^[A-z]*[-A-z]*").Success == false)
                    {
                        CurrentSchoolBorder.Background = Brushes.Red;
                        validInput = false;
                    }
                    else if (i == 10 && !Int32.TryParse(inputs[i].Text.ToString(), out tempNum))
                    {
                        IntededDateBorder.Background = Brushes.Red;
                        validInput = false;
                    }
                    else if (i == 11 && !Int32.TryParse(inputs[i].Text.ToString(),out tempNum))
                    {
                        CurrentYearInSchoolBorder.Background = Brushes.Red;
                        validInput = false;
                    }
                    else if (i == 12 && !inputs[i].Text.Any(Char.IsLetter))
                    {
                        InsuraceBorder.Background = Brushes.Red;
                        validInput = false;
                    }
                    else if (i == 13 && !DateTime.TryParse(inputs[i].Text, out tempDateTime))
                    {
                        DateLastContactedBorder.Background = Brushes.Red;
                        validInput = false;
                    }
                }
            }
            return validInput;
        }

        private void SubmitCampButton_Click(object sender, RoutedEventArgs e)
        {
            if(String.IsNullOrWhiteSpace(CampNameInput.Text) || String.IsNullOrWhiteSpace(CampDateInput.Text))
            {
                MessageBox.Show("Error: Couldn't submit camp. Required Feilds are left empty.");
            }
            else
            {
                Database.ExecuteCampImport(CampNameInput.Text, CampDateInput.Text);
                MessageBox.Show("Successfully inserted Camp!");
            }
        }

        private void AddCampButton_Click(object sender, RoutedEventArgs e)
        {
            if (ManualCampEntryUI.Visibility != Visibility.Visible)
            {
                ManualCampEntryUI.Visibility = Visibility.Visible;
                ManualStudentEntryUI.Visibility = Visibility.Collapsed;
                ContentTitle.Text = "Camp Import";
            }
        }

        private void AddStudentButton_Click(object sender, RoutedEventArgs e)
        {
            ManualCampEntryUI.Visibility = Visibility.Collapsed;
            ManualStudentEntryUI.Visibility = Visibility.Visible;
            CampsListBorder.Visibility = Visibility.Hidden;
            ContentTitle.Text = "Student Import";
        }

        private void AddAttendanceButton_Click(object sender, RoutedEventArgs e)
        {
            CampsListBorder.Visibility = Visibility.Visible;
            ManualCampEntryUI.Visibility = Visibility.Collapsed;
            ManualStudentEntryUI.Visibility = Visibility.Visible;
            camps = Database.GetCamps();
            if(camps != null)
            {
                CampsListComboBox.Items.Clear();
                foreach (DataRow r in camps.Rows)
                {
                    CampsListComboBox.Items.Add($"{r[2].ToString()} {Convert.ToDateTime(r[1].ToString()).Date}".Substring(0,r[2].ToString().Length + r[1].ToString().Length - 11));
                }
            }
        }

        private void ImportByFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.DefaultExt = ".xlsx";
            dialog.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm;*.csv;";
            bool? result = dialog.ShowDialog();

            if(result == true)
            {
                ImportStudentsByFile(dialog.FileName);
            }

        }

        private void ImportStudentsByFile(string filepath)
        {
            int rowCount = 0;
            List<string> attributes = Database.ExecuteGetColumnNames();
            List<string> importAttributes = new List<string>();
            attributes.Remove("ID");
            Excel excel;

            try
            {
                excel = new Excel(filepath);
            }
            catch (IOException e)
            {
                MessageBox.Show(e.Message.ToString());
                return;
            }
            IXLRangeRows rows = excel.GetContentRows();

            foreach (IXLRangeRow r in rows)
            {
                rowCount++;
                importAttributes = excel.GetAttributesFromRow(r.WorksheetRow(), attributes);
                importAttributes[24] = (excel.GetMiscAttributes(r.WorksheetRow(), attributes));
                if (IsValidInput(importAttributes))
                {
                    DataTable exactDuplicates = Database.CheckForExactDuplicateas(importAttributes[0], importAttributes[1], importAttributes[2], importAttributes[11]);
                    DataTable duplicates = Database.CheckForDuplicates(importAttributes[0], importAttributes[1], importAttributes[2], importAttributes[11]);
                    if (importAttributes[18] == String.Empty)
                    {
                        importAttributes[18] = null;
                    }
                    //If In Camps UI and no value or in student ui
                    if (CampsListBorder.Visibility == Visibility.Hidden || CampsListComboBox.SelectedValue == null)
                    {
                        if(exactDuplicates.Rows.Count == 0)
                        {
                            if (duplicates.Rows.Count > 0)
                            {
                                ChildWindow childWindow = new ChildWindow(importAttributes, duplicates, camps, CampsListComboBox, false);
                                childWindow.ShowInTaskbar = false;
                                childWindow.Owner = Application.Current.MainWindow;
                                childWindow.ShowDialog();
                            }
                            else
                            {
                                Database.ExecuteStudentImport(importAttributes[0], importAttributes[1], importAttributes[3], importAttributes[16], importAttributes[11], importAttributes[12], importAttributes[13],
                                                     importAttributes[14], importAttributes[4], importAttributes[5], importAttributes[6], importAttributes[7], importAttributes[9], importAttributes[8], importAttributes[10],
                                                     importAttributes[2], importAttributes[15], importAttributes[22], importAttributes[23], importAttributes[17], Convert.ToBoolean(importAttributes[18]),
                                                     importAttributes[19], importAttributes[20], importAttributes[24], importAttributes[21]);
                            }
                        }
                    }
                    //If in camps and something is selected
                    else
                    {
                        int campID;
                        if(exactDuplicates.Rows.Count == 0)
                        {
                            if (duplicates.Rows.Count <= 0)
                            {
                                Database.ExecuteStudentImport(importAttributes[0], importAttributes[1], importAttributes[3], importAttributes[16], importAttributes[11], importAttributes[12], importAttributes[13],
                                                     importAttributes[14], importAttributes[4], importAttributes[5], importAttributes[6], importAttributes[7], importAttributes[9], importAttributes[8], importAttributes[10],
                                                     importAttributes[2], importAttributes[15], importAttributes[22], importAttributes[23], importAttributes[17], Convert.ToBoolean(importAttributes[18]),
                                                     importAttributes[19], importAttributes[20], importAttributes[24], importAttributes[21]);
                                foreach (DataRow c in camps.Rows)
                                {
                                    string rowCurrentCamp = $"{c[2].ToString()} {Convert.ToDateTime(c[1].ToString()).Date}".Substring(0, c[2].ToString().Length + c[1].ToString().Length - 11);
                                    if (rowCurrentCamp == CampsListComboBox.SelectedValue.ToString())
                                    {
                                        campID = Convert.ToInt32(c[0].ToString());
                                        Database.ExecuteAddAttendance(campID, importAttributes[0], importAttributes[1], importAttributes[11], importAttributes[2]);
                                    }
                                }
                            }
                            //Handels Duplicate Students when insertting into a camp by using the child window
                            else
                            {
                                ChildWindow childWindow = new ChildWindow(importAttributes, duplicates, camps, CampsListComboBox);
                                childWindow.ShowInTaskbar = false;
                                childWindow.Owner = Application.Current.MainWindow;
                                childWindow.ShowDialog();
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show($"Invalid input on row {rowCount}.");
                }
            }
            MessageBox.Show("Insert by file Complete");
        }
        private bool IsValidInput(List<string> importAttributes)
        {
            DateTime tempDateTime;
            int tempInt;

            for (int i = 0; i < importAttributes.Count; i++)
            {
                if(importAttributes[i] != String.Empty)
                {
                    if (i <= 3)
                    {
                        if (Regex.IsMatch(importAttributes[i], @"^[A-z]*[-A-z]*") == false)
                        {
                            return false;
                        }
                    }
                    else if (i == 5 || i == 6 || i == 18)
                    {
                        if (Regex.IsMatch(importAttributes[i], @"^[A-z]*[-A-z]*[ A-z]*") == false)
                        {
                            return false;
                        }
                    }
                    else if (i == 7)
                    {
                        if (Regex.IsMatch(importAttributes[i], @"^[0-9]*") == false)
                        {
                            return false;
                        }
                    }
                    else if (i == 8 || i == 9)
                    {
                        if (Regex.IsMatch(importAttributes[i], @"\s*(?:\+?(\d{1,3}))?([-. (]*(\d{3})[-. )]*)?((\d{3})[-. ]*(\d{2,4})(?:[-.x ]*(\d+))?)\s*") == false)
                        {
                            return false;
                        }
                    }
                    else if (i == 10)
                    {
                        if (Regex.IsMatch(importAttributes[i], @"^[A-z]*[-A-z]*[&A-z]*[& A-z]*") == false)
                        {
                            return false;
                        }
                    }
                    else if (i == 11 || i == 20 || i == 21)
                    {
                        if (!DateTime.TryParse(importAttributes[i], out tempDateTime))
                        {
                            try
                            {
                                importAttributes[i] = DateTime.FromOADate(Convert.ToUInt32(importAttributes[i])).Date.ToString();
                            }
                            catch
                            {
                                return false;
                            }
                        }
                    }
                    else if (i == 12)
                    {
                        if (!genders.Contains(importAttributes[i]))
                        {
                            return false;
                        }
                        else
                        {
                            if (importAttributes[i] == "M")
                            {
                                importAttributes[i] = "Male";
                            }
                            else if (importAttributes[i] == "F")
                            {
                                importAttributes[i] = "Female";
                            }
                            else if (importAttributes[i] == "O")
                            {
                                importAttributes[i] = "Other";
                            }
                        }
                    }
                    else if (i == 15 || i==19 || i == 22)
                    {
                        if(i == 22 && importAttributes[22].Contains("th") || importAttributes[22].Contains("TH"))
                        {
                            importAttributes[i] = importAttributes[i].Substring(0, importAttributes[i].Length - 2);
                        }
                        if (Int32.TryParse(importAttributes[i], out tempInt) == false)
                        {
                            return false;
                        }
                        if (i == 22)
                        {
                            if (tempInt < 3 && tempInt > 12)
                            {
                                return false;
                            }
                        }
                    }
                    else if (i == 16)
                    {
                        if (!importAttributes[i].Contains("@"))
                        {
                            return false;
                        }
                    }
                    else if(i == 18)
                    {
                        if(importAttributes[i] != "Y" && importAttributes[i] != "N")
                        {
                            return false;
                        }
                        else
                        {
                            if(importAttributes[i] == "Y")
                            {
                                importAttributes[i] = "True";
                            }
                            else if (importAttributes[i] == "N")
                            {
                                importAttributes[i] = "False";
                            }
                        }
                    }
                }
                else if(i == 18 || i == 19)
                {
                    importAttributes[i] = null;
                }
            }
            return true;
        }
    }
}