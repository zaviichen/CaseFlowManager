using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace CaseFlowManager
{
    class Step : INotifyPropertyChanged
    {
        public string id { get; set; }
        public string name { get; set; }

        private string document = null;
        public string Document
        {
            get { return document; }
            set
            {
                document = value;
                NotifyPropertyChanged("Document");
            }
        }

        private int order;
        public int Order
        {
            get { return order; }
            set
            {
                order = value;
                NotifyPropertyChanged("Order");
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

        public int status;

        public List<Step> nextSteps = new List<Step>();
        public List<Step> NextSteps
        {
            get { return nextSteps; }
            set { nextSteps = value; }
        }

        public Step(string id, string name)
        {
            this.id = id;
            this.name = name;
            this.startTime = DateTime.Today;
        }

        public Step(Step step)
        {
            this.id = step.id;
            this.name = step.name;
            this.startTime = DateTime.Today;
            this.Document = step.Document;
        }

        public Step NextStep()
        {
            Step nextStep = null;
            if (nextSteps.Count == 1)
            {
                nextStep = nextSteps[0];
            }
            return nextStep;
        }

        public bool HasNextStep()
        {
            return NextStep() != null;
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
