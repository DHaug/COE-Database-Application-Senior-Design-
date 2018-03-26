using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CoE_SRMS.Content
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public string WhereStatement { get { return WhereClause; } }
        private List<String> relOps = new List<string> { "<", ">", ">=", "<=", "=" };
        private string WhereClause;
        private List<String> dataTypes;
        public Window1(List<string> fields, List<string> dataTypesAndColumn)
        {
            InitializeComponent();
            foreach (string Column in fields)
            {

                WhereFieldComboPopup.Items.Add(Column);
            }

            foreach (string op in relOps)
            {

                RelationComboPopup.Items.Add(op);
            }

            dataTypes = new List<string>(dataTypesAndColumn);

            AddWhereToListButton.Click += new RoutedEventHandler(ExecuteAddWhere);
        }

        private void ExecuteAddWhere(object sender, RoutedEventArgs es)
        {

            if (WhereFieldComboPopup.SelectedIndex > -1 && RelationComboPopup.SelectedIndex > -1 && UserInputedValueWherePopup.Text != string.Empty)
            {

                var match = dataTypes.FirstOrDefault(stringToCheck => stringToCheck.Contains(WhereFieldComboPopup.SelectedItem.ToString()));

                string[] tuple = match.Split(' ');


                if (tuple[1].Contains("varchar"))
                {
                    WhereClause = tuple[0] + " " + RelationComboPopup.SelectedItem.ToString() + " '" + UserInputedValueWherePopup.Text.ToString() + "'";
                }
                else
                    WhereClause = tuple[0] + " " + RelationComboPopup.SelectedItem.ToString() + " " + UserInputedValueWherePopup.Text.ToString();

                this.Close();
            }
            else
            {
                MessageBox.Show("Please enter all of the fields!");
            }
           
        }
    }
}
