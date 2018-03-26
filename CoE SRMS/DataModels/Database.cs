/*************************************************
 * Project:         SDSU CoE SRMS
 * File:            Database.cs
 * Author:          Drake Olson
 * Date Created:    1/26/17
 * Date Modified    2/18/17
 *************************************************/

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows;

namespace CoE_SRMS
{

    //Singleton Class
    public static class Database
    {
        private static string ConnectionString { get; set; } = $@"Server={Environment.MachineName}\SQLEXPRESS;Database=CoE SRMS;User Id={ConfigurationManager.AppSettings["DatabaseUsername"]};Password = {ConfigurationManager.AppSettings["DatabasePassword"]};";
        private static SqlConnection DatabaseConnection { get; set; } = new SqlConnection(ConnectionString);
        private const int SQL_DUPLICATE_VALUE = 2627;
        private const string emptyString = "";

        public static void ExecuteStudentImport(string firstName,string lastName, string preferredName, string email, string birthdate, string gender, string ethnicity, string race, string address, string city,
                                                string state, string postalCode, string cellPhoneNumber, string homephoneNumber, string parentName, string currentSchool, string intendedYearOfGraduation,
                                                string currentYearInSchool, string tShirtSize, string insurance, bool? potentialEngineer, string studentID, string lastContacted, string misc = emptyString, string dateOfCampusVisit = "")
        {
            try
            {
                SqlCommand command = new SqlCommand("InsertStudent", DatabaseConnection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.Add("@FirstName", SqlDbType.VarChar, 25).Value = firstName;
                command.Parameters.Add("@LastName", SqlDbType.VarChar, 25).Value = lastName;
                command.Parameters.Add("@School", SqlDbType.VarChar, -1).Value = currentSchool;
                command.Parameters.Add("@PreferredName", SqlDbType.VarChar, 25).Value = CorrectValue(preferredName);
                command.Parameters.Add("@Email", SqlDbType.VarChar, 100).Value = CorrectValue(email);
                if (birthdate == null || birthdate == String.Empty)
                {
                    command.Parameters.Add("@BirthDate", SqlDbType.Date).Value = new DateTime(1970, 1, 1).Date;
                }
                else
                {
                    command.Parameters.Add("@BirthDate", SqlDbType.Date).Value = DateTime.Parse(birthdate).Date;
                }
                command.Parameters.Add("@Gender", SqlDbType.VarChar, 10).Value = CorrectValue(gender);
                command.Parameters.Add("@Ethnicity", SqlDbType.VarChar, 35).Value = CorrectValue(ethnicity);
                command.Parameters.Add("@Race", SqlDbType.VarChar, 35).Value = CorrectValue(race);
                command.Parameters.Add("@AddressLine", SqlDbType.VarChar, 200).Value = CorrectValue(address);
                command.Parameters.Add("@City", SqlDbType.VarChar, 100).Value = CorrectValue(city);
                command.Parameters.Add("@State", SqlDbType.VarChar, 25).Value = CorrectValue(state);
                command.Parameters.Add("@Zip", SqlDbType.VarChar, 20).Value = CorrectValue(postalCode);
                command.Parameters.Add("@CellPhone", SqlDbType.VarChar, 14).Value = CorrectValue(cellPhoneNumber);
                command.Parameters.Add("@DayPhone", SqlDbType.VarChar, 14).Value = CorrectValue(homephoneNumber);
                command.Parameters.Add("@ParentName", SqlDbType.VarChar, 25).Value = CorrectValue(parentName);
                if(intendedYearOfGraduation != String.Empty)
                {
                    command.Parameters.Add("@HighSchoolGrad", SqlDbType.Int).Value = Convert.ToInt32(intendedYearOfGraduation);
                }
                else
                {
                    command.Parameters.Add("@HighSchoolGrad", SqlDbType.Int).Value = DBNull.Value;
                }
                if (intendedYearOfGraduation != String.Empty)
                {
                    command.Parameters.Add("@YrInSchool", SqlDbType.Int).Value = Convert.ToInt32(intendedYearOfGraduation);
                }
                else
                {
                    command.Parameters.Add("@YrInSchool", SqlDbType.Int).Value = DBNull.Value;
                }
                command.Parameters.Add("@TShirtSize", SqlDbType.VarChar, 10).Value = CorrectValue(tShirtSize);
                command.Parameters.Add("@Insurance", SqlDbType.VarChar, 100).Value = CorrectValue(insurance);
                command.Parameters.Add("@EngineeringStudent", SqlDbType.Bit).Value = (bool)potentialEngineer;
                if (studentID != String.Empty)
                {
                    command.Parameters.Add("@StudentID", SqlDbType.Int).Value = Convert.ToInt32(studentID);
                }
                else
                {
                    command.Parameters.Add("@StudentID", SqlDbType.Int).Value = DBNull.Value;
                }
                if (lastContacted == null || lastContacted == String.Empty)
                {
                    command.Parameters.Add("@DateOfLastContact", SqlDbType.Date).Value = DBNull.Value;
                }
                else
                {
                    command.Parameters.Add("@DateOfLastContact", SqlDbType.Date).Value = DateTime.Parse(lastContacted).Date;
                }
                command.Parameters.Add("@Misc", SqlDbType.VarChar, -1).Value = CorrectValue(misc);
                command.Parameters.Add("@DateOfCampusVisit", SqlDbType.Date).Value = CorrectValue(dateOfCampusVisit);
                SQLCommand studentInsertCommand = new SQLCommand(command, SQLCommand.CommandType.Insert);
                ExecuteStoredProcedure(studentInsertCommand);             
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        
        public static DataTable CheckForDuplicates(string firstName, string lastName, string currentSchool, string birthdate)
        {
            SqlCommand matchStudent = new SqlCommand("CheckForDuplicateStudent", DatabaseConnection)
            {
                CommandType = CommandType.StoredProcedure
            };
            matchStudent.Parameters.Add("@FirstName", SqlDbType.VarChar, 25).Value = firstName;
            matchStudent.Parameters.Add("@LastName", SqlDbType.VarChar, 25).Value = lastName;
            matchStudent.Parameters.Add("@School", SqlDbType.VarChar, -1).Value = currentSchool;
            if(birthdate != String.Empty)
            {
                matchStudent.Parameters.Add("@BirthDate", SqlDbType.Date).Value = DateTime.Parse(birthdate).Date;
            }
            else
            {
                matchStudent.Parameters.Add("@BirthDate", SqlDbType.Date).Value = new DateTime(1970, 1, 1).Date;
            }
            SQLCommand dupeCommand = new SQLCommand(matchStudent, SQLCommand.CommandType.Select);

            return (DataTable)ExecuteStoredProcedure(dupeCommand);
        }

        public static DataRow GetMailingInformation(string ID)
        {
            SqlCommand studentMailInfo = new SqlCommand("GetMailingList", DatabaseConnection)
            {
                CommandType = CommandType.StoredProcedure
            };

            studentMailInfo.Parameters.Add("@StudentID", SqlDbType.Int).Value = Convert.ToInt32(ID);
            SQLCommand command = new SQLCommand(studentMailInfo, SQLCommand.CommandType.Select);
            DataRow test = ((DataTable)ExecuteStoredProcedure(command)).Rows[0];
            return ((DataTable)ExecuteStoredProcedure(command)).Rows[0];
        }

        public static string GetUserRole(string username)
        {
            SqlCommand userRole = new SqlCommand("GetUserRole", DatabaseConnection)
            {
                CommandType = CommandType.StoredProcedure
            };
            userRole.Parameters.Add("@UserName", SqlDbType.VarChar, 25).Value = username;
            SQLCommand userRoleCommand = new SQLCommand(userRole, SQLCommand.CommandType.Select);

            return ((DataTable)ExecuteStoredProcedure(userRoleCommand)).Rows[0][0].ToString();
        }

        public static void ExecuteAddAttendance(int campID, string firstName, string lastName, string birthday, string school)
        {
            try
            {
                SqlCommand command = new SqlCommand("InsertAttendance", DatabaseConnection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.Add("@CampID", SqlDbType.Int).Value = campID;
                command.Parameters.Add("@FirstName", SqlDbType.VarChar,25).Value = firstName;
                command.Parameters.Add("@LastName", SqlDbType.VarChar,25).Value = lastName;
                command.Parameters.Add("@School", SqlDbType.VarChar,-1).Value = school;
                if (birthday == null || birthday == String.Empty)
                {
                    command.Parameters.Add("@Birthday", SqlDbType.Date).Value = new DateTime(1970, 1, 1).Date;
                }
                else
                {
                    command.Parameters.Add("@Birthday", SqlDbType.Date).Value = DateTime.Parse(birthday).Date;
                }
                SQLCommand insertAttendanceCommand = new SQLCommand(command, SQLCommand.CommandType.Insert);
                ExecuteStoredProcedure(insertAttendanceCommand);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        
        public static void ExecuteAddAttendanceManual(int campID, int studentID)
        {

            try
            {
                SqlCommand command = new SqlCommand("InsertAttendanceManual", DatabaseConnection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.Add("@CampID", SqlDbType.Int).Value = campID;
                command.Parameters.Add("@StudentID", SqlDbType.Int).Value = studentID;
                SQLCommand insertAttendanceCommand = new SQLCommand(command, SQLCommand.CommandType.Insert);
                ExecuteStoredProcedure(insertAttendanceCommand);
                //Check for duplicate camp entry for each ids
            }
            catch(SqlException e)
            {
                if(e.ErrorCode == 2627)
                {
                    MessageBox.Show("Duplicate Entry into a camp. Insertion Skipped.");
                    return;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public static List<string> ExecuteGetColumnNames()
        {
            DataTable columnNames = new DataTable();
            try
            {
                SqlCommand command = new SqlCommand("GetStudentColumnNames", DatabaseConnection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                SQLCommand getStudentColumnNames = new SQLCommand(command, SQLCommand.CommandType.Select);
                columnNames = (DataTable)ExecuteStoredProcedure(getStudentColumnNames);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            return ColumnToList(columnNames);
        }

        public static List<string> ExecuteGetNames(string procName)
        {
            DataTable columnNames = new DataTable();
            try
            {
                SqlCommand command = new SqlCommand(procName, DatabaseConnection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                SQLCommand ColumnNames = new SQLCommand(command, SQLCommand.CommandType.Select);
                columnNames = (DataTable)ExecuteStoredProcedure(ColumnNames);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            return ColumnToList(columnNames);
        }

        public static List<string> ExecuteGetTableCols(string tableName)
        {
            DataTable columnNames = new DataTable();
            try
            {
                SqlCommand command = new SqlCommand("GetTableCols", DatabaseConnection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.Add("@tableName", SqlDbType.VarChar, 30).Value = tableName;
                SQLCommand ColumnNames = new SQLCommand(command, SQLCommand.CommandType.Select);
                columnNames = (DataTable)ExecuteStoredProcedure(ColumnNames);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }


            return ColumnToList(columnNames);
        }


        public static List<string> ExecuteGetTableColsAndVals(string tableName)
        {
            DataTable columnNames = new DataTable();
            try
            {
                SqlCommand command = new SqlCommand("GetTableCols", DatabaseConnection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.Add("@tableName", SqlDbType.VarChar, 30).Value = tableName;
                SQLCommand ColumnNames = new SQLCommand(command, SQLCommand.CommandType.Select);
                columnNames = (DataTable)ExecuteStoredProcedure(ColumnNames);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            List<string> returnedList = new List<string>();
            foreach (DataRow r in columnNames.Rows)
            {
                returnedList.Add(r[0].ToString() + " " + r[1].ToString());
            }


            return returnedList;
        }


        //This NEEDS and WILL be updated! Done to quickly allow our presentation demonstration but is extrememly vulnerable to SQL Injection
        //Probably not a concern as SDSU staff will only be using it but need to have more checks or pass as parameters. 
        public static DataSet ExecuteCustomQuery(string SelCol, string table, List<string> whereClauses, ref SqlDataAdapter adapter)
        {
            DataSet dataTable = new DataSet();
            try
            {
                SqlCommand command;

                string whereCommandArg = ""; 
                foreach( string clause in whereClauses)
                {
                    whereCommandArg += clause + " and ";
                }

                if (whereCommandArg.Length > 5)
                {
                    whereCommandArg = whereCommandArg.Remove(whereCommandArg.Length - 5);
                     command = new SqlCommand("Select " + SelCol + " From " + table + " Where " + whereCommandArg, DatabaseConnection);
                }
                else
                     command = new SqlCommand("Select " + SelCol + " From " + table ,DatabaseConnection);


                adapter = new SqlDataAdapter(command);
                adapter.Fill(dataTable);

                
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            return dataTable;

        }


    public static List<string> ColumnToList(DataTable column)
        {
            List<string> returnedList = new List<string>();
            foreach(DataRow r in column.Rows)
            {
                returnedList.Add(r[0].ToString());
            }
            return returnedList;
        }

        public static DataTable GetCamps()
        {
            try
            {
                SqlCommand command = new SqlCommand("GetCamps", DatabaseConnection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                SQLCommand getCampsCommand = new SQLCommand(command, SQLCommand.CommandType.Select);
                return (DataTable)ExecuteStoredProcedure(getCampsCommand);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            return null;
        }

        public static void ExecuteCampImport(string campName, string campDate)
        {
            try
            {
                SqlCommand command = new SqlCommand("InsertCamp", DatabaseConnection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.Add("@CampName", SqlDbType.VarChar, 25).Value = CorrectValue(campName);
                command.Parameters.Add("@CampDate", SqlDbType.Date).Value = Convert.ToDateTime(campDate).Date;
                SQLCommand campInsertCommand = new SQLCommand(command, SQLCommand.CommandType.Insert);
                ExecuteStoredProcedure(campInsertCommand);

            }
            catch (Exception e)
            {
                if (e.Message.Contains("PRIMARY KEY constraint"))
                {
                    MessageBox.Show($"Cannot insert camp, {campName}, because that camp on {campDate} already exists.");
                }
                else
                {
                    MessageBox.Show(e.Message);
                }
            }
        }


        /// <summary>
        /// Executes the UserLogin Stored Procedure on database
        /// </summary>
        /// <param name="username"></param>
        /// <param name="hashedPassword"></param>
        /// <returns>If the login is successful</returns>
        public static bool ExecuteUserLoginStoredProcedure(string username, string hashedPassword)
        {
            try
            {
                SqlCommand command = new SqlCommand("AccountLogin", DatabaseConnection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.Add("@Username", SqlDbType.VarChar, 25).Value = username;
                command.Parameters.Add("@Password", SqlDbType.VarChar, 300).Value = hashedPassword;
                SQLCommand userLoginCommand = new SQLCommand(command, SQLCommand.CommandType.Select);
                DataTable returnedData = (DataTable) ExecuteStoredProcedure(userLoginCommand);
                return returnedData.Rows.Count == 1;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            return false;
        }

        /// <summary>
        /// Creates the user in the database.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="hashedPassword"></param>
        /// <returns>If the username is taken return false, otherwise return true.</returns>
        public static bool ExecuteSignUpStoredProcedure(string username, string hashedPassword)
        {
            try
            {
                SqlCommand command = new SqlCommand("AccountSignUp", DatabaseConnection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.Add("@Username", SqlDbType.VarChar, 25).Value = username;
                command.Parameters.Add("@HashedPassword", SqlDbType.VarChar, 300).Value = hashedPassword;
                command.Parameters.Add("@Role", SqlDbType.VarChar, 8).Value = "ReadOnly";
                SQLCommand userSignUpCommand = new SQLCommand(command,SQLCommand.CommandType.Insert);
                int rowsInserted = (int)ExecuteStoredProcedure(userSignUpCommand);
                return rowsInserted == 1;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            return false;
        }

        /// <summary>
        /// Executes the GetTableFields Stored Procedure on database
        /// </summary>
        /// <param name="username"></param>
        /// <returns>If table names is successful</returns>
        public static List<String> ExecuteTableFieldsProcedure()
        {
            List<string> ColumnNames = new List<String>();

            try
            {
                SqlConnection cn = new SqlConnection();
                SqlCommand cmd = new SqlCommand();
                DataTable schemaTable;
                SqlDataReader myReader;

                //Open a connection to the SQL Server Northwind database.
                cn.ConnectionString = ConnectionString;
                cn.Open();

                //Retrieve records from the Employees table into a DataReader.
                cmd.Connection = cn;
                cmd.CommandText = "SELECT * FROM CurrentStudents";
                myReader = cmd.ExecuteReader(CommandBehavior.KeyInfo);

                //Retrieve column schema into a DataTable.
                schemaTable = myReader.GetSchemaTable();

                //For each field in the table...
                foreach (DataColumn myField in schemaTable.Columns)
                {
                     
                      ColumnNames.Add(myField.ColumnName);
         
                    
                }

                //Always close the DataReader and connection.
                myReader.Close();
                cn.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            return ColumnNames;

        }

        /// <summary>
        /// Executes a stored procedure on database
        /// </summary>
        /// <param name="command"></param>
        /// <returns>Either a <see cref="DataTable"/> (Select) or a number of records effected (Insert,Update,Delete)</returns>
        private static object ExecuteStoredProcedure(SQLCommand command)
        {
            try
            {
                DatabaseConnection.Open();
                if (command.Type == SQLCommand.CommandType.Select)
                {
                    DataTable returnedData = new DataTable();
                    SqlDataReader reader = command.CommandData.ExecuteReader();
                    returnedData.Load(reader);
                    DatabaseConnection.Close();
                    return returnedData;
                }
                else if (command.Type != SQLCommand.CommandType.Select && command.Type != null)
                {
                    var recordsAffected = command.CommandData.ExecuteNonQuery();
                    DatabaseConnection.Close();
                    return recordsAffected;

                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                if (e.Message.Contains("PRIMARY KEY constraint"))
                {
                    MessageBox.Show("Cannot insert student because this student already exists in the database.");
                }
                else
                {
                    MessageBox.Show(e.Message);
                }
            }
            finally
            {
                if ((DatabaseConnection.State & ConnectionState.Open) != 0)
                {
                    DatabaseConnection.Close();
                }
            }
            return null;
        }

        internal static DataTable CheckForExactDuplicateas(string firstName, string lastName, string currentSchool, string birthdate)
        {
            SqlCommand matchStudent = new SqlCommand("CheckForExactDuplicates", DatabaseConnection)
            {
                CommandType = CommandType.StoredProcedure
            };
            matchStudent.Parameters.Add("@FirstName", SqlDbType.VarChar, 25).Value = firstName;
            matchStudent.Parameters.Add("@LastName", SqlDbType.VarChar, 25).Value = lastName;
            matchStudent.Parameters.Add("@School", SqlDbType.VarChar, -1).Value = currentSchool;
            if (birthdate != String.Empty)
            {
                matchStudent.Parameters.Add("@BirthDate", SqlDbType.Date).Value = DateTime.Parse(birthdate).Date;
            }
            else
            {
                matchStudent.Parameters.Add("@BirthDate", SqlDbType.Date).Value = new DateTime(1970, 1, 1).Date;
            }
            SQLCommand dupeCommand = new SQLCommand(matchStudent, SQLCommand.CommandType.Select);

            return (DataTable)ExecuteStoredProcedure(dupeCommand);
        }

        /// <summary>
        /// Helper function that checks if a value is empty before inserting into the database.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static object CorrectValue(string value)
        {
            if (String.IsNullOrWhiteSpace(value))
            {
                return DBNull.Value;
            }
            else
            {
                return value;
            }
        }
    }
}
