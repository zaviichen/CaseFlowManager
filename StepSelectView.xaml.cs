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
    /// StepSelectView.xaml 的交互逻辑
    /// </summary>
    public partial class StepSelectView : Window
    {
        public StepSelectView()
        {
            InitializeComponent();

            curCase = manager.curCase;
            curStep = curCase.CurStep;
            this.listBox1.ItemsSource = curStep.NextSteps;
        }

        private CaseFlowManager manager = CaseFlowManager.Instance;

        private Case curCase = null;
        private Step curStep = null;

        public void OkBtn_Click(object sender, RoutedEventArgs e)
        {
            if (this.listBox1.SelectedIndex < 0)
            {
                MessageBox.Show(@"请选择下一步");
                return;
            }

            Step nextStep = curStep.NextSteps[this.listBox1.SelectedIndex];
            curStep.NextSteps.Clear();
            curStep.NextSteps.Add(nextStep);
            curCase.StepDone();
            manager.DumpCase(curCase);

            this.Close();
        }
    }
}
