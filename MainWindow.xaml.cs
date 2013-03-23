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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using Excel = Microsoft.Office.Interop.Excel;

namespace CaseFlowManager
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            manager.Initial();

            this.remainderText.Text = manager.remainderDay.ToString();
            this.caseSummaryView.ItemsSource = manager.Cases;

            this.Loaded += new RoutedEventHandler(MainWindow_Loaded);
        }

        private CaseFlowManager manager = CaseFlowManager.Instance;

        private void newBtn_Click(object sender, RoutedEventArgs e)
        {
            NewCaseView newCaseWin = new NewCaseView();
            newCaseWin.Show();
        }

        private void MainWindow_Loaded(object sender, EventArgs e)
        {
            manager.RemaindCases();
        }

        private void caseSummaryView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            manager.curCase = this.caseSummaryView.SelectedItem as Case;
            this.caseDetailView.ItemsSource = (manager.curCase != null) ? manager.curCase.WorkFlow : null;
            this.caseDetailView.SelectedIndex = 0;
        }

        private void caseDetailView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int curId = this.caseDetailView.SelectedIndex;
            int totalStep = this.caseDetailView.Items.Count;

            if (curId == totalStep - 1 && manager.curCase != null && manager.curCase.Status != 2)
            {
                this.finishBtn.IsEnabled = true;
            }
            else
            {
                this.finishBtn.IsEnabled = false;
            }
        }

        private void finishBtn_Click(object sender, RoutedEventArgs e)
        {
            Case curCase = manager.curCase;
            int curId = this.caseDetailView.SelectedIndex;

            int nextCnt = manager.FillNextSteps();
            if (nextCnt > 1)
            {
                StepSelectView selectView = new StepSelectView();
                selectView.Show();
            }
            else
            {
                curCase.StepDone();
            }

            this.finishBtn.IsEnabled = false;
            this.caseDetailView.SelectedIndex = curId + 1;
            manager.DumpCase(curCase);
        }

        private void deleteBtn_Click(object sender, RoutedEventArgs e)
        {
            int selectId = this.caseSummaryView.SelectedIndex;
            int lastId = this.caseSummaryView.Items.Count - 1;
            if (selectId != -1)
            { 
                this.caseSummaryView.SelectedIndex = (selectId == lastId) ? -1 : selectId + 1;
                manager.RemoveCase(manager.Cases[selectId]);
            }
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DependencyObject img = (DependencyObject)sender;
            DependencyObject obj = VisualTreeHelper.GetParent(img);
            DependencyObject obj1 = VisualTreeHelper.GetParent(obj);
            Step docStep = ((GridViewRowPresenter)obj1).Content as Step;
            
            string doc = docStep.Document;
            string path = String.Format("Data\\{0}_{1}\\{2}", manager.curCase.id, manager.curCase.name, doc);
            try
            {
                Process.Start(path);
            }
            catch
            {
                string msg = String.Format("在文件夹( {0}_{1} )中找不到文件: {2}", manager.curCase.id, manager.curCase.name, doc);
                MessageBox.Show(msg);
            }
        }

        private void exportBtn_Click(object sender, RoutedEventArgs e)
        {
            Excel.Application excel = new Excel.Application();
            excel.DisplayAlerts = false;
            excel.AlertBeforeOverwriting = false;
            object misValue = System.Reflection.Missing.Value;

            try
            {
                Excel.Workbook workbook = excel.Workbooks.Add(misValue);

                for (int i = 1; i <= manager.Cases.Count; i++)
                {
                    Case acase = manager.Cases[i - 1];
                    Excel.Worksheet worksheet = (Excel.Worksheet)workbook.Worksheets.get_Item(i);
                    worksheet.Name = acase.name;

                    for (int j = 1; j <= acase.WorkFlow.Count; j++)
                    {
                        Step astep = acase.WorkFlow[j - 1];
                        worksheet.Cells[j, 1] = astep.Order.ToString();
                        worksheet.Cells[j, 2] = astep.name;
                        worksheet.Cells[j, 3] = astep.StartTime.ToShortDateString();
                        if (astep.status == 2)
                            worksheet.Cells[j, 4] = astep.EndTime.ToShortDateString();
                    }
                }

                workbook.SaveAs(@"案件汇总.xls", Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                workbook.Close(true, misValue, misValue);
                MessageBox.Show("导出Excel成功，存放在“我的文档”文件夹下");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                excel.Quit();
            }
        }

        private void setdayBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                manager.remainderDay = Int32.Parse(this.remainderText.Text);
                manager.RemaindCases();
                manager.parser.SaveRemainderDay(manager.remainderDay, @"Resource\Configs\SysConfig.xml");
            }
            catch
            {
                MessageBox.Show("请输入正确的日期");
            }
        }
    }
}
