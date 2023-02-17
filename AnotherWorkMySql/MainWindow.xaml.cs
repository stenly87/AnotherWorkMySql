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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AnotherWorkMySql
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
           /* if (DB.GetInstance().RemoveRow(new Position { Id = 9 }))
                MessageBox.Show("F");*/
            if (DB.GetInstance().RemoveRow(new Table { Id = 11 }))
                MessageBox.Show("F");
        }
    }
}
