using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows;

namespace CaseFlowManager
{
    class XmlParser
    {
        private XmlDocument doc;

        public void ParseSysConfig(string path)
        {
            try
            {
                doc = new XmlDocument();
                doc.Load(path);

                // parse the remainder day
                XmlNode remainderNode = doc.GetElementsByTagName("reminder")[0];
                CaseFlowManager.Instance.remainderDay = Int32.Parse(remainderNode.InnerText);

                // 1. create all step instances
                Dictionary<string, Step> stepMaps = new Dictionary<string, Step>();
                XmlNodeList stepList = doc.GetElementsByTagName("step");

                for (int i = 0; i < stepList.Count; ++i)
                {
                    XmlNode node = stepList[i];
                    string id = node.Attributes["id"].Value;
                    string name = node.Attributes["name"].Value;
                    Step step = new Step(id, name);

                    // add document information
                    for (int j = 0; j < node.ChildNodes.Count; ++j)
                    {
                        XmlNode child = node.ChildNodes[j];
                        if (child.Name.Equals("document"))
                        {
                            step.Document = child.InnerText;
                        }
                    }
                    stepMaps.Add(id, step);
                }

                // 2. chain the steps
                for (int i = 0; i < stepList.Count; ++i)
                {
                    XmlNode node = stepList[i];
                    string id = node.Attributes["id"].Value;
                    Step step = stepMaps[id];

                    for (int j = 0; j < node.ChildNodes.Count; ++j)
                    {
                        XmlNode child = node.ChildNodes[j];
                        if (child.Name.Equals("next") && !child.InnerText.Equals("null"))
                        {
                            step.nextSteps.Add(stepMaps[child.InnerText]);
                        }
                    }

                    CaseFlowManager.Instance.workFlow.Add(step);
                }

                // 3. mark the first step
                XmlNode workFlow = doc.GetElementsByTagName("WorkFlow")[0];
                string firstId = workFlow.FirstChild.Attributes["id"].Value;
                CaseFlowManager.Instance.firstStep = stepMaps[firstId];
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public void SaveRemainderDay(int day, string path)
        {
            try
            {
                doc = new XmlDocument();
                doc.Load(path);
                XmlNode remainderNode = doc.GetElementsByTagName("reminder")[0];
                remainderNode.InnerText = CaseFlowManager.Instance.remainderDay.ToString();
                doc.Save(path);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public Case LoadCase(string file)
        {
            Case acase = null;
            try
            {
                doc = new XmlDocument();
                doc.Load(file);

                // retrive <CaseInfo>
                XmlNode caseId = doc.GetElementsByTagName("CaseId")[0];
                string id = caseId.InnerText;
                XmlNode caseName = doc.GetElementsByTagName("CaseName")[0];
                string name = caseName.InnerText;

                acase = new Case(id, name, false);

                XmlNode caseStatus = doc.GetElementsByTagName("CaseStatus")[0];
                acase.Status = Int32.Parse(caseStatus.InnerText);
                XmlNode caseStart = doc.GetElementsByTagName("CaseBegin")[0];
                acase.StartTime = StringtoDateTime(caseStart.InnerText);
                if (acase.Status == 2)
                {
                    XmlNode caseEnd = doc.GetElementsByTagName("CaseEnd")[0];
                    acase.EndTime = StringtoDateTime(caseEnd.InnerText);
                }

                // retrive <WorkFlow>
                XmlNodeList stepList = doc.GetElementsByTagName("step");
                for (int i = 0; i < stepList.Count; ++i)
                {
                    XmlNode stepNode = stepList[i];
                    string _id = stepNode.Attributes["id"].Value;
                    string _name = stepNode.Attributes["name"].Value;
                    Step step = new Step(_id, _name);
                    for (int j = 0; j < stepNode.ChildNodes.Count; ++j)
                    {
                        string tag = stepNode.ChildNodes[j].Name;
                        string text = stepNode.ChildNodes[j].InnerText;
                        if (tag.Equals("status"))
                            step.status = Int32.Parse(text);
                        if (tag.Equals("begin"))
                            step.StartTime = StringtoDateTime(text);
                        if (tag.Equals("end"))
                            step.EndTime = StringtoDateTime(text);
                        if (tag.Equals("order"))
                            step.Order = Int32.Parse(text);
                        if (tag.Equals("document"))
                            step.Document = text;
                    }
                    acase.workFlow.Add(step);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            return acase;
        }

        public void DumpCase(Case acase, string file)
        {
            doc = new XmlDocument();
            doc.LoadXml("<Case></Case>");
            XmlElement root = doc.DocumentElement;

            // assemble <CaseInfo>
            XmlElement caseInfo = doc.CreateElement("CaseInfo");

            XmlElement id = doc.CreateElement("CaseId");
            id.InnerText = acase.id;
            caseInfo.AppendChild(id);

            XmlElement name = doc.CreateElement("CaseName");
            name.InnerText = acase.name;
            caseInfo.AppendChild(name);

            XmlElement status = doc.CreateElement("CaseStatus");
            status.InnerText = acase.Status.ToString();
            caseInfo.AppendChild(status);

            XmlElement begin = doc.CreateElement("CaseBegin");
            begin.InnerText = DateTimeToString(acase.StartTime);
            caseInfo.AppendChild(begin);

            if (acase.Status == 2)
            {
                XmlElement end = doc.CreateElement("CaseEnd");
                end.InnerText = DateTimeToString(acase.EndTime);
                caseInfo.AppendChild(end);  
            }

            root.AppendChild(caseInfo);

            // assemble <WorkFlow>
            XmlElement workFlow = doc.CreateElement("WorkFlow");

            for (int i = 0; i < acase.workFlow.Count; ++i)
            {
                Step step = acase.workFlow[i];
                XmlElement _step = doc.CreateElement("step");
                _step.SetAttribute("id", step.id);
                _step.SetAttribute("name", step.name);

                XmlElement _order = doc.CreateElement("order");
                _order.InnerText = step.Order.ToString();
                _step.AppendChild(_order);

                XmlElement _status = doc.CreateElement("status");
                _status.InnerText = step.status.ToString();
                _step.AppendChild(_status);

                XmlElement _begin = doc.CreateElement("begin");
                _begin.InnerText = DateTimeToString(step.StartTime);
                _step.AppendChild(_begin);

                if (step.status == 2)
                {
                    XmlElement _end = doc.CreateElement("end");
                    _end.InnerText = DateTimeToString(step.EndTime);
                    _step.AppendChild(_end);
                }

                if (step.Document != null)
                {
                    XmlElement _doc = doc.CreateElement("document");
                    _doc.InnerText = step.Document;
                    _step.AppendChild(_doc);
                }

                workFlow.AppendChild(_step);
            }

            root.AppendChild(workFlow);
            doc.Save(file);
        }

        private DateTime StringtoDateTime(string str)
        {
            string[] arr = str.Split('/');
            return new DateTime(Int32.Parse(arr[0]), Int32.Parse(arr[1]), Int32.Parse(arr[2]));
        }

        private string DateTimeToString(DateTime date)
        {
            return date.ToShortDateString();
        }
    }
}
