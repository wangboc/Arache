using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AracheTest.Data;
using DevExpress.XtraCharts;

namespace AracheTest.UIControls
{
    internal class ElectricityCharts
    {
        public List<ChartControl> _chartCtrs { get; set; }

        private ChartControl pcChartControl;
        public ChartControl PCChartControl
        {
            get { return pcChartControl; }
            set
            {
                pcChartControl = value;
                _chartCtrs.Add(pcChartControl);
            }
        }

        private ChartControl sChartControl;
        public ChartControl SChartControl
        {
            get { return sChartControl; }
            set
            {
                sChartControl = value;
                _chartCtrs.Add(sChartControl);
            }
        }

        private ChartControl puChartControl;
        public ChartControl PUChartControl
        {
            get { return puChartControl; }
            set
            {
                puChartControl = value;
                _chartCtrs.Add(puChartControl);
            }
        }

        private ChartControl qChartControl;
        public ChartControl QChartControl
        {
            get { return qChartControl; }
            set
            {
                qChartControl = value;
                _chartCtrs.Add(qChartControl);
            }
        }

        private ChartControl pChartControl;
        public ChartControl PChartControl
        {
            get { return pChartControl; }
            set
            {
                pChartControl = value;
                _chartCtrs.Add(pChartControl);
            }
        }

        private ChartControl pfChartControl;
        public ChartControl PFChartControl
        {
            get { return pfChartControl; }
            set
            {
                pfChartControl = value;
                _chartCtrs.Add(pfChartControl);
            }
        }

        public ElectricityCharts()
        {
            _chartCtrs = new List<ChartControl>();
        }

        private void ResetAxisOptions(AxisX xAxis)
        {
            xAxis.DateTimeScaleOptions.ScaleMode = ScaleMode.Continuous;
            xAxis.Label.TextPattern = "{A:MM/dd HH:mm}";
            xAxis.DateTimeScaleOptions.GridSpacing = 1;
            xAxis.Tickmarks.MinorVisible = false;
            xAxis.Label.Angle = -5;
            xAxis.DateTimeScaleOptions.AutoGrid = true;
        }

        public void ResetChartDataSource()
        {
            foreach (var chart in _chartCtrs)
            {
                chart.DataSource = null;
            }
        }

        public void UpdateChart(List<ElectricityOriginalData> electricityDataList)
        {
            ResetChartDataSource();
            foreach (var chart in _chartCtrs)
            {
                chart.DataSource = electricityDataList;
            }
        }

        public void InitControls()
        {
            foreach (var chartControl in _chartCtrs)
            {
                foreach (Series s in chartControl.Series)
                {
                    s.ArgumentDataMember = "Eventtime";
                    s.ArgumentScaleType = ScaleType.DateTime;
                }
                var diagram = chartControl.Diagram as XYDiagram;
                chartControl.CrosshairOptions.GroupHeaderPattern = "{A:yyyy/M/d HH:mm:ss}";
                var axisX = diagram.AxisX;
                diagram.EnableAxisYZooming = false;
                diagram.EnableAxisYScrolling = true;
                diagram.EnableAxisXScrolling = true;
                diagram.EnableAxisXZooming = true;
                ResetAxisOptions(axisX);
            }

            //设定相电流曲线图
            PCChartControl.Series[0].ValueDataMembers.AddRange("IA");
            PCChartControl.Series[1].ValueDataMembers.AddRange("IB");
            PCChartControl.Series[2].ValueDataMembers.AddRange("IC");

            //设定电能曲线图
            SChartControl.Series[0].ValueDataMembers.AddRange("SA");
            SChartControl.Series[1].ValueDataMembers.AddRange("SB");
            SChartControl.Series[2].ValueDataMembers.AddRange("SC");
            SChartControl.Series[3].ValueDataMembers.AddRange("SS");

            //设定相电压曲线图
            PUChartControl.Series[0].ValueDataMembers.AddRange("UA");
            PUChartControl.Series[1].ValueDataMembers.AddRange("UB");
            PUChartControl.Series[2].ValueDataMembers.AddRange("UC");

            //设定无功率曲线图
            QChartControl.Series[0].ValueDataMembers.AddRange("QA");
            QChartControl.Series[1].ValueDataMembers.AddRange("QB");
            QChartControl.Series[2].ValueDataMembers.AddRange("QC");
            QChartControl.Series[3].ValueDataMembers.AddRange("QS");

            //设定有功功率曲线图
            PChartControl.Series[0].ValueDataMembers.AddRange("PA");
            PChartControl.Series[1].ValueDataMembers.AddRange("PB");
            PChartControl.Series[2].ValueDataMembers.AddRange("PC");
            PChartControl.Series[3].ValueDataMembers.AddRange("PS");

            //设定功率因数曲线图
            PFChartControl.Series[0].ValueDataMembers.AddRange("PFA");
            PFChartControl.Series[1].ValueDataMembers.AddRange("PFB");
            PFChartControl.Series[2].ValueDataMembers.AddRange("PFC");
            PFChartControl.Series[3].ValueDataMembers.AddRange("PFS");
        }
    }
}