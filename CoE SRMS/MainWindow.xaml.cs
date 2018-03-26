using System;
using System.Windows.Navigation;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace CoE_SRMS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            MainContent.Source = new Uri("Content/LoginPage.xaml", UriKind.Relative);
            
        }
    }
}
