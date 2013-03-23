using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace CaseFlowManager
{
    class Case : INotifyPropertyChanged
    {
        public string id { get; set; }
        public string name { get; set; }
        private int status;
        public int Status
        {
            get { return status; }
            set 
            {
                status = value;
                NotifyPropertyChanged("Status");
            }
        }

        private DateTime startTime;
        public DateTime StartTime
        {
            get { return startTime; }
            set 
            {
                startTime = value;
                NotifyPropertyChanged("StartTime");
            }
        }

        private DateTime endTime;
        public DateTime EndTime
        {
            get { return endTime; }
            set
            {
                endTime = value;
                NotifyPropertyChanged("EndTime");
            }
        }

        public ObservableCollection<Step> workFlow = new ObservableCollection<Step>();
        public ObservableCollection<Step> WorkFlow
        {
            get { return workFlow; }
            set { workFlow = value; }
        }

        public Step CurStep
        {
            get { return workFlow[workFlow.Count - 1]; }
        }

        public Case(string id, string name, bool newcase = true)
        {
            this.id = id;
            this.name = name;

            if (newcase)
            {
                this.Status = 1; // in processing
                this.StartTime = DateTime.Today;

                Step firstStep = new Step(CaseFlowManager.Instance.firstStep);
                firstStep.Order = 1;
                firstStep.StartTime = DateTime.Today;
                workFlow.Add(firstStep);
            }
        }

        public void StepDone()
        {
            CurStep.EndTime = DateTime.Today;
            CurStep.status = 2;

            if (CurStep.HasNextStep())
            {
                Step nextStep = new Step(CurStep.NextStep());
                workFlow.Add(nextStep);
                nextStep.Order = workFlow.Count;
            }
            else
            {
                endTime = DateTime.Today;
                Status = 2; // finish
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
