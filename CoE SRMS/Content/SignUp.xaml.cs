/*************************************************
 * Project:         SDSU CoE SRMS
 * File:            LoginPage.xaml.cs
 * Author:          Drake Olson
 * Date Created:    10/28/17
 * Date Modified    10/29/17
 *************************************************/

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CoE_SRMS.Content
{
    /// <summary>
    /// Interaction logic for SignUp.xaml
    /// </summary>
    public partial class SignUp : Page
    {
        private bool UsernameBorderIsRedFlag { get; set; } = false;
        private bool PasswordBorderIsRedFlag { get; set; } = false;
        private bool ConformPasswordBorderIsRedFlag { get; set; } = false;
        
        /// <summary>
        /// Default Constructer. This is called if there is no text in the login username textbox.
        /// </summary>
        public SignUp()
        {
            InitializeComponent();
        }

        /// <summary>
        /// This is called if there is text in the login username textbox.
        /// </summary>
        /// <param name="desiredUserName">String in the username textbox from the login page.</param>
        public SignUp(string desiredUserName)
        {
            InitializeComponent();
            UsernameTextBox.Text = desiredUserName;
        }

        /// <summary>
        /// When the signup button is clicked. The program will create the account via the stored procedure. If any errors, text will appear.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSignupButtonClick(object sender, RoutedEventArgs e)
        {
            if (UsernameTextBox.Text == String.Empty || PasswordTextBox.Password == String.Empty ||
                ConfirmPasswordTextBox.Password == String.Empty)
            {
                if (UsernameTextBox.Text == String.Empty)
                {
                    UsernameBorder.BorderBrush = Brushes.Red;
                    UsernameBorder.BorderThickness = new Thickness(2.0);
                    UsernameBorderIsRedFlag = true;
                }

                if (PasswordTextBox.Password == String.Empty)
                {
                    PasswordBorder.BorderBrush = Brushes.Red;
                    PasswordBorder.BorderThickness = new Thickness(2.0);
                    PasswordBorderIsRedFlag = true;

                }
                if (ConfirmPasswordTextBox.Password == String.Empty)
                {
                    ConfirmPasswordBorder.BorderBrush = Brushes.Red;
                    ConfirmPasswordBorder.BorderThickness = new Thickness(2.0);
                    ConformPasswordBorderIsRedFlag = true;
                }
            }
            else
            {
                string hashedPassword = LoginPage.HashPassword(PasswordTextBox.Password);
                string hashedConfirmedPassword = LoginPage.HashPassword(ConfirmPasswordTextBox.Password);
                if (hashedPassword == hashedConfirmedPassword)
                {
                    bool successful = Database.ExecuteSignUpStoredProcedure(UsernameTextBox.Text, hashedPassword);
                    if (successful)
                    {
                        NavigationService?.Navigate(new Uri("Content/LoginPage.xaml", UriKind.Relative));
                    }
                    else
                    {
                        UsernameTakenLabel.Visibility = Visibility.Visible;
                    }
                }
                else
                {
                    PasswordsDoNotMatchLabel.Visibility = Visibility.Visible;
                }
            }
        }

        /// <summary>
        /// Refreshes the username textbox if there is a red border on it.
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
        /// Refreshed the username textbox if there is a red border on it.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Password_OnKeyPress(object sender, KeyEventArgs e)
        {
            //All Keys that are Alphanumeric, Symbols, and Space
            if (e.Key == Key.Space || (e.Key >= Key.D0 && e.Key <= Key.Z) || (e.Key >= Key.NumPad0 && e.Key <= Key.Divide) && (PasswordBorderIsRedFlag || ConformPasswordBorderIsRedFlag))
            {
                PasswordBorder.BorderThickness = new Thickness(0.0);
            }
        }

        /// <summary>
        /// Gives the effect that the button is 3D when on focus.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMouseEneterSignupButton(object sender, MouseEventArgs e)
        {
            SignUpButtonBorder.BorderThickness = new Thickness(2.0);
            SignUpButtonBorder.BorderBrush = BackgroundBorder.Background;
        }

        /// <summary>
        /// Gives the effect that the button is not focused after it has been focused.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMouseLeaveSignupButton(object sender, MouseEventArgs e)
        {
            SignUpButtonBorder.BorderThickness = new Thickness(0.0);
            SignUpButtonBorder.Background = Brushes.White;
        }
    }
}
