using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CaseFlowManager
{
    /// <summary>
    /// NewCase.xaml 的交互逻辑
    /// </summary>
    public partial class NewCaseView : Window
    {
        public NewCaseView()
        {
            InitializeComponent();
        }

        private CaseFlowManager manager = CaseFlowManager.Instance;

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            string id = (++manager.idCount).ToString();
            Case acase = new Case(id, this.textBox1.Text);
            manager.Cases.Add(acase);
            manager.curCase = acase;
            manager.DumpCase(acase);
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
