/*************************************************
 * Project:         SDSU CoE SRMS
 * File:            LoginPage.xaml.cs
 * Author:          Drake Olson
 * Date Created:    3/15/18
 * Date Modified    10/26/17
 *************************************************/

using System;
using System.Configuration;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CoE_SRMS.Content
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        private bool UsernameBorderIsRedFlag { get; set; } = false;
        private bool PasswordBorderIsRedFlag { get; set; } = false;

        public LoginPage()
        {
            InitializeComponent();
        }
        /// <summary>
        /// When login button is clicked, and either the username textbox or password textbox is empty set the border to red. If not, attempt login process.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLoginButtonClick(object sender, RoutedEventArgs e)
        {
            if (UsernameTextBox.Text == String.Empty || PasswordTextBox.Password == String.Empty)
            {
                if (UsernameTextBox.Text == String.Empty)
                {
                    UsernameBorder.BorderBrush = Brushes.Red;
                    UsernameBorder.BorderThickness = new Thickness(2.0);
                    UsernameBorderIsRedFlag = true;
                }

                if(PasswordTextBox.Password == String.Empty)
                {
                    PasswordBorder.BorderBrush = Brushes.Red;
                    PasswordBorder.BorderThickness = new Thickness(2.0);
                    PasswordBorderIsRedFlag = true;
                }
            }
            else
            {
                bool succesfullLogin = AttemptLogin(UsernameTextBox.Text, PasswordTextBox.Password);
                if (succesfullLogin)
                {
                    if (NavigationService != null)
                    {
                        string userRole = Database.GetUserRole(UsernameTextBox.Text);
                        var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                        config.AppSettings.Settings["UserRole"].Value = userRole;
                        config.Save(ConfigurationSaveMode.Modified);
                        ConfigurationManager.RefreshSection("appSettings");
                        NavigationService.Navigate(new Uri("Content/Dashboard.xaml", UriKind.Relative));
                    }
                    else
                    {
                        MessageBox.Show("Unexpected Error:", "Navigation Service is NULL. Cannot navigat to dashboard.",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    IncorrectLoginLabel.Visibility = Visibility.Visible;
                }
            }
        }
        
        /// <summary>
        /// Attempt to login into the system.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns>If the attemp was successful</returns>
        private bool AttemptLogin(string username, string password)
        {
            string hashedPassword = HashPassword(password);
            return Database.ExecuteUserLoginStoredProcedure(username, hashedPassword);
        }

        /// <summary>
        /// This will hash the password for login or creation of an account.
        /// </summary>
        /// <param name="password"></param>
        /// <returns>A hashed password.</returns>
        public static string HashPassword(string password)
        {
            byte[] bytes = new UTF8Encoding().GetBytes(password);
            byte[] hashbytes;
            using (SHA512Managed algorithm = new SHA512Managed())
            {
                hashbytes = algorithm.ComputeHash(bytes);
            }
            return Convert.ToBase64String(hashbytes);
        }

        /// <summary>
        /// If user textbox's border is red (empty) and the user types in a space, an alphanumeric, or special symbol take username textbox's red  outer border. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Username_OnKeyPress(object sender, KeyEventArgs e)
        {
            //All Keys that are Alphanumeric, Symbols, and Space
            if (e.Key == Key.Space || (e.Key >= Key.D0 && e.Key <= Key.Z) || (e.Key >= Key.NumPad0 && e.Key <= Key.Divide) && UsernameBorderIsRedFlag)
            {
                UsernameBorder.BorderThickness = new Thickness(0.0);
            }
        }
        /// <summary>
        /// If passwordtext box's border is red (empty) and the user types in a space, an alphanumeric, or special symbol take password textbox's red outer border. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Password_OnKeyPress(object sender, KeyEventArgs e)
        {
            //All Keys that are Alphanumeric, Symbols, and Space
            if (e.Key == Key.Space || (e.Key >= Key.D0 && e.Key <= Key.Z) || (e.Key >= Key.NumPad0 && e.Key <= Key.Divide) && PasswordBorderIsRedFlag)
            {
                PasswordBorder.BorderThickness = new Thickness(0.0);
            }
        }

        private void OnSignUpButtonClick(object sender, RoutedEventArgs e)
        {
            if (UsernameTextBox.Text != String.Empty)
            {
                NavigationService?.Navigate(new SignUp(UsernameTextBox.Text));
            }
            else
            {
                NavigationService?.Navigate(new SignUp());
            }
        }

        private void OnMouseEnterLoginButton(object sender, MouseEventArgs e)
        {
            LoginButtonBorder.BorderThickness = new Thickness(2.0);
            LoginButtonBorder.BorderBrush = BackgroundBorder.Background;
        }

        private void OnMouseLeaveLoginButton(object sender, MouseEventArgs e)
        {
            LoginButtonBorder.BorderThickness = new Thickness(0.0);
            LoginButtonBorder.Background = Brushes.White;
        }
    }
}
