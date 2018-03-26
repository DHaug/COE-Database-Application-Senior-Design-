/*************************************************
 * Project:         SDSU CoE SRMS
 * File:            Dashboard.xaml.cs
 * Author:          Drake Olson
 * Date Created:    1/10/18
 * Date Modified    1/10/18
 *************************************************/

using System;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CoE_SRMS.Content
{
    /// <summary>
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Dashboard : Page
    {
        public Dashboard()
        {
            InitializeComponent();
            if (ConfigurationManager.AppSettings["UserRole"] == "ReadOnly")
            {
                ImportButton.IsEnabled = false;
                SettingsButton.IsEnabled = false;
                ToolsButton.IsEnabled = false;
                ImportButton.Background = Brushes.DimGray;
                ImportButtonBorder.Background = Brushes.DimGray;
                ToolsButton.Background = Brushes.DimGray;
                ToolsButtonBorder.Background = Brushes.DimGray;
                SettingsButton.Background = Brushes.DimGray;
                SettingsButtonBorder.Background = Brushes.DimGray;
            }
        }
        /// <summary>
        /// When a mouse enters a button that is displayed the border will change.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            Button buttonEntered = sender as Button;
            string nameOfButton = buttonEntered.Name;

            if (nameOfButton == "ImportButton")
            {
                ImportButtonBorder.BorderThickness = new Thickness(2.0);
                ImportButtonBorder.BorderBrush = BackgroundBorder.Background;
            }
            else if(nameOfButton == "QueryButton")
            {
                QueryButtonBorder.BorderThickness = new Thickness(2.0);
                QueryButtonBorder.BorderBrush = BackgroundBorder.Background;
            }
            else if(nameOfButton == "ToolsButton")
            {
                ToolsButtonBorder.BorderThickness = new Thickness(2.0);
                ToolsButtonBorder.BorderBrush = BackgroundBorder.Background;
            }
            else if (nameOfButton == "ExportButton")
            {
                ExportButtonBorder.BorderThickness = new Thickness(2.0);
                ExportButtonBorder.BorderBrush = BackgroundBorder.Background;
            }
            else if (nameOfButton == "HelpButton")
            {
                HelpButtonBorder.BorderThickness = new Thickness(2.0);
                HelpButtonBorder.BorderBrush = BackgroundBorder.Background;
            }
            else if (nameOfButton == "SettingsButton")
            {
                SettingsButtonBorder.BorderThickness = new Thickness(2.0);
                SettingsButtonBorder.BorderBrush = BackgroundBorder.Background;
            }
        }
        /// <summary>
        /// When the curser moves out of the button's area the border will go back to white.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            Button buttonEntered = sender as Button;
            string nameOfButton = buttonEntered.Name;

            if (nameOfButton == "ImportButton")
            {
                ImportButtonBorder.BorderThickness = new Thickness(2.0);
                ImportButtonBorder.BorderBrush = Brushes.White;
            }
            else if (nameOfButton == "QueryButton")
            {
                QueryButtonBorder.BorderThickness = new Thickness(2.0);
                QueryButtonBorder.BorderBrush = Brushes.White;
            }
            else if (nameOfButton == "ToolsButton")
            {
                ToolsButtonBorder.BorderThickness = new Thickness(2.0);
                ToolsButtonBorder.BorderBrush = Brushes.White;
            }
            else if (nameOfButton == "ExportButton")
            {
                ExportButtonBorder.BorderThickness = new Thickness(2.0);
                ExportButtonBorder.BorderBrush = Brushes.White;
            }
            else if (nameOfButton == "HelpButton")
            {
                HelpButtonBorder.BorderThickness = new Thickness(2.0);
                HelpButtonBorder.BorderBrush = Brushes.White;
            }
            else if (nameOfButton == "SettingsButton")
            {
                SettingsButtonBorder.BorderThickness = new Thickness(2.0);
                SettingsButtonBorder.BorderBrush = Brushes.White;
            }
        }
        /// <summary>
        /// Navigation to the Query UI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnQueryButtonClick(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new Uri("Content/Query.xaml", UriKind.Relative));
        }
        /// <summary>
        /// Navigation to the Import UI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnImportButtonClick(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new Uri("Content/Input.xaml", UriKind.Relative));
        }
        /// <summary>
        /// Navigation to the Tools UI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnToolsButtonClick(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new Uri("Content/Tools.xaml", UriKind.Relative));
        }
        /// <summary>
        /// Navigation to the Export UI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnExportButtonClick(object sender, RoutedEventArgs e)
        {

        }
        /// <summary>
        /// Navigation to the Help UI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnHelpButtonClick(object sender, RoutedEventArgs e)
        {

        }
        /// <summary>
        /// Navigation to the Settings UI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSettingsButtonClick(object sender, RoutedEventArgs e)
        {

        }
    }
}
