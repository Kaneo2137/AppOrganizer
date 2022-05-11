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

namespace AppOrganizer
{
    /// <summary>
    /// Interaction logic for NewProfileWindow.xaml
    /// </summary>
    public partial class NewProfileWindow : Window
    {
        public string ProfileName { get; set; }
        public NewProfileWindow()
        {
            InitializeComponent();
        }

        void ConfirmWithEnter(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Confirm();
        }

        void Button_Ok(object sender, RoutedEventArgs e) => Confirm();

        void Confirm()
        {
            var name = TextBoxName.Text;
            if (name != string.Empty)
            {
                DialogResult = true;
                ProfileName = TextBoxName.Text;
            }
        }

        void Button_Cancel(object sender, RoutedEventArgs e) => DialogResult = false;
    }
}
