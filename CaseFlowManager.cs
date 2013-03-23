using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows;
using System.IO;

namespace CaseFlowManager
{
    class CaseFlowManager : INotifyPropertyChanged
    {
        private static CaseFlowManager instance = null;
        public static CaseFlowManager Instance
        {
            get 
            {
                if (instance == null)
                {
                    instance = new CaseFlowManager();
                }
                return instance;
            }
        }

        private CaseFlowManager() 
        {
            parser = new XmlParser();
        }

        public XmlParser parser;
        public int idCount = 0;

        // for generic work flow
        public List<Step> workFlow = new List<Step>();
        public Step firstStep = null;

        // store all the case data
        private ObservableCollection<Case> cases = new ObservableCollection<Case>();
        public ObservableCollection<Case> Cases
        {
            get 
            { 
                return cases; 
            }
            set
            {
                cases = value;
                NotifyPropertyChanged("Cases");
            }
        }

        public Case curCase = null;
        public int remainderDay = 1;

        public void Initial()
        {
            int maxCaseId = 0;
            char delim = '_';
            try
            {
                parser.ParseSysConfig(@"Resource\Configs\SysConfig.xml");

                DirectoryInfo DataDir = new DirectoryInfo(@"Data");
                foreach (DirectoryInfo caseDir in DataDir.GetDirectories())
                {
                    FileInfo xmlFile = null;
                    int id = Int32.Parse(caseDir.Name.Split(delim)[0]);
                    maxCaseId = (id > maxCaseId) ? id : maxCaseId;

                    foreach (FileInfo file in caseDir.GetFiles())
                    {
                        if (file.Name.EndsWith(@".xml"))
                        {
                            xmlFile = file;
                            break;
                        }
                    }
                    Case acase = LoadCase(xmlFile.FullName);
                    cases.Add(acase);
                }

                idCount = maxCaseId;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public int FillNextSteps()
        {
            Step curStep = curCase.CurStep;
            curStep.NextSteps.Clear();

            for (int i = 0; i < workFlow.Count; i++)
            {
                if (workFlow[i].id.Equals(curStep.id))
                {
                    foreach (Step s in workFlow[i].NextSteps)
                    {
                        curStep.NextSteps.Add(new Step(s));
                    }
                }
            }

            return curStep.NextSteps.Count;
        }

        public void RemoveCase(Case acase)
        {
            Cases.Remove(acase);
            string name = String.Format("{0}_{1}", acase.id, acase.name);
            string path = String.Format("Data\\{0}", name);
            DeleteFolder(path);
        }

        public void DeleteFolder(string dir)
        {
            if (Directory.Exists(dir))
            {
                foreach (string d in Directory.GetFileSystemEntries(dir))
                {
                    if (File.Exists(d))
                        File.Delete(d);
                    else
                        DeleteFolder(d);
                }
                Directory.Delete(dir);
            }
        }

        public void CopyFiles(string srcDir, string tgtDir)
        {
            char delim = '\\';
            if (Directory.Exists(srcDir) && Directory.Exists(tgtDir))
            {
                foreach (string srcFile in Directory.GetFileSystemEntries(srcDir))
                {
                    string [] arr = srcFile.Split(delim);
                    string tgtFile = String.Format("{0}\\{1}", tgtDir, arr[arr.Length-1]);
                    File.Copy(srcFile, tgtFile);
                }
            }
        }

        public void DumpCase(Case acase)
        {
            string name = String.Format("{0}_{1}", acase.id, acase.name);
            string path = String.Format("Data\\{0}", name);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                CopyFiles(@"Resource\Doc", path);
            }
            parser.DumpCase(acase, String.Format("{0}\\{1}.xml", path, name));
        }

        public Case LoadCase(string caseFile)
        {
            return parser.LoadCase(caseFile);
        }

        public void RemaindCases()
        {
            Dictionary<Case, int> remainderList = new Dictionary<Case, int>();

            foreach (Case acase in Cases)
            {
                if (acase.Status == 1)
                { 
                    TimeSpan ts = DateTime.Today - acase.StartTime;
                    if (ts.Days >= this.remainderDay)
                        remainderList[acase] = ts.Days;
                }
            }

            if (remainderList.Count > 0)
            {
                string msg = "注意！以下案件已经过期：\n";
                foreach (Case acase in remainderList.Keys)
                {
                    msg += String.Format("{0} : {1} 天\n", acase.name, remainderList[acase]);
                }
                MessageBox.Show(msg);
            }
        }

        # region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
        # endregion
    }
}
