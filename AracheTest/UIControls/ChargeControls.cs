using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AracheTest.Data;
using AracheTest.Reports;
using DevExpress.XtraCharts;
using DevExpress.XtraGrid;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Preview;
using DevExpress.XtraReports.UI;

namespace AracheTest.UIControls
{
    public class ChargeControlsBase
    {
        //分别对应电费栏中的四个表格，从上到下
        protected DataTable _chargeTable_1 = new DataTable();
        protected DataTable _chargeTable_2 = new DataTable();
        protected DataTable _chargeTable_3 = new DataTable();
        protected DataTable _chargeTable_4 = new DataTable();

        protected ChartControl ChargeProportion;
        protected ChartControl Fe_CuPropotion;
        protected ChartControl ChargeChart;

        public ChargeControlsBase()
        {
            InitChargeTableCtr();
        }

        protected void InitChargeTableCtr()
        {
            _chargeTable_1.Columns.Add("MID");
            _chargeTable_1.Columns.Add("MName");
            _chargeTable_1.Columns.Add("PresentShown");
            _chargeTable_1.Columns.Add("PreviousShown");
            _chargeTable_1.Columns.Add("Rate");
            _chargeTable_1.Columns.Add("WPP");

            _chargeTable_2.Columns.Add("PCu");
            _chargeTable_2.Columns.Add("PFe");
            _chargeTable_2.Columns.Add("PTotal");
            _chargeTable_2.Columns.Add("QCu");
            _chargeTable_2.Columns.Add("QFe");
            _chargeTable_2.Columns.Add("QTotal");
            _chargeTable_2.Columns.Add("Spike");
            _chargeTable_2.Columns.Add("Valley");
            _chargeTable_2.Columns.Add("Peak");

            _chargeTable_3.Columns.Add("Capacity");
            _chargeTable_3.Columns.Add("Need");
            _chargeTable_3.Columns.Add("Shown");
            _chargeTable_3.Columns.Add("NeedInReal");
            _chargeTable_3.Columns.Add("NeedExceed");
            _chargeTable_3.Columns.Add("CapacityPause");
            _chargeTable_3.Columns.Add("DaysPause");
            _chargeTable_3.Columns.Add("Compensate");
            _chargeTable_3.Columns.Add("Advanced");


            _chargeTable_4.Columns.Add("Items");
            _chargeTable_4.Columns.Add("Quantity");
            _chargeTable_4.Columns.Add("Rate");
            _chargeTable_4.Columns.Add("Unit");
            _chargeTable_4.Columns.Add("Price");
            _chargeTable_4.Columns.Add("Total");
            _chargeTable_4.Columns.Add("Remarks");
        }

        public virtual void SetChargeUiControls(ChartControl chartControlChargeProportion,
            ChartControl feCuPropotionCtl, ChartControl chargePropotionChart, GridControl gridControl1,
            GridControl gridControl2, GridControl gridControl3, GridControl gridControl4, DocumentViewer documentView)
        {
        }

        public virtual void SetChargeData(Object _chargeObjects)
        {
            _chargeTable_1.Rows.Clear();
            _chargeTable_2.Rows.Clear();
            _chargeTable_3.Rows.Clear();
            _chargeTable_4.Rows.Clear();
            InitChargePropotionControl();
        }

        public virtual void SetPropotionData(ChargeInfo chargeInfo)
        {
        }

        protected void InitChargePropotionControl()
        {
            var diagram = ChargeChart.Diagram as XYDiagram;
            
            ChargeChart.CrosshairOptions.GroupHeaderPattern = "{A:yyyy/MM/dd}";
            var axisX = diagram.AxisX;
            diagram.EnableAxisYZooming = false;
            diagram.EnableAxisYScrolling = true;
            diagram.EnableAxisXScrolling = true;
            diagram.EnableAxisXZooming = true;
            axisX.DateTimeScaleOptions.ScaleMode = ScaleMode.Continuous;
           // axisX.Label.TextPattern = "{A:MM/dd HH:mm}";
            axisX.DateTimeScaleOptions.GridSpacing = 1;
            axisX.Tickmarks.MinorVisible = false;
            axisX.Label.Angle = -5;
            axisX.DateTimeScaleOptions.AutoGrid = true;
        }
    }

    public class ChargeControlFirst : ChargeControlsBase
    {
        private GridControl gridCharge_1;
        private GridControl gridCharge_2;
        private GridControl gridCharge_3;
        private GridControl gridCharge_4;
        private DocumentViewer documentViewer;

        public override void SetChargeUiControls(ChartControl chartControlChargeProportion,
            ChartControl feCuPropotionCtl, ChartControl chargePropotionChart, GridControl gridControl1,
            GridControl gridControl2, GridControl gridControl3, GridControl gridControl4, DocumentViewer documentView)
        {
            ChargeProportion = chartControlChargeProportion;
            Fe_CuPropotion = feCuPropotionCtl;
            ChargeChart = chargePropotionChart;
            gridCharge_1 = gridControl1;
            gridCharge_2 = gridControl2;
            gridCharge_3 = gridControl3;
            gridCharge_4 = gridControl4;
            documentViewer = documentView;
        }

        public override void SetPropotionData(ChargeInfo chargeInfo)
        {
            ChargeProportion.Series[0].Points.Clear();
            ChargeProportion.Series[0].Points.AddRange(new SeriesPoint[]
            {
                new SeriesPoint("尖峰", chargeInfo.spikePower.ToString("#0.00")),
                new SeriesPoint("峰", chargeInfo.peakPower.ToString("#0.00")),
                new SeriesPoint("谷", chargeInfo.valleyPower.ToString("#0.00"))
            });

            Fe_CuPropotion.Series[0].Points.Clear();
            Fe_CuPropotion.Series[0].Points.AddRange(new SeriesPoint[]
            {
                new SeriesPoint("有功铜损", chargeInfo.activeCopperLoss.ToString("#0.00")),
                new SeriesPoint("无功铜损", chargeInfo.reactiveCopperLoss.ToString("#0.00")),
                new SeriesPoint("有功铁损", chargeInfo.activeCoreLoss.ToString("#0.00")),
                new SeriesPoint("无功铁损", chargeInfo.reactiveCoreLoss.ToString("#0.00"))
            });
        }

        public override void SetChargeData(Object _chargeObjects)
        {
            ChargeInfo chargeInfo = (_chargeObjects as Dictionary<string, object>)["第一阶段"] as ChargeInfo;
            base.SetChargeData(chargeInfo);
            if (chargeInfo != null)
            {
                DataRow row = _chargeTable_1.NewRow();
                row[0] = chargeInfo.MID;
                row[1] = "有功（总）";
                row[2] = chargeInfo.endEnergy.ToString("#0.00");
                row[3] = chargeInfo.startEnergy.ToString("#0.00");
                row[4] = "30";
                row[5] = chargeInfo.totalPower.ToString("#0.00");
                _chargeTable_1.Rows.Add(row);

                row = _chargeTable_1.NewRow();
                row[0] = chargeInfo.MID;
                row[1] = "有功（尖峰）";
                row[2] = "";
                row[4] = "30";
                row[5] = chargeInfo.spikePower.ToString("#0.00");
                _chargeTable_1.Rows.Add(row);

                row = _chargeTable_1.NewRow();
                row[0] = chargeInfo.MID;
                row[1] = "有功（峰）";row[2] = "";
                row[4] = "30";
                row[5] = chargeInfo.peakPower.ToString("#0.00");
                _chargeTable_1.Rows.Add(row);

                row = _chargeTable_1.NewRow();
                row[0] = chargeInfo.MID;
                row[1] = "有功（谷）";
                row[2] = "";
                row[4] = "30";
                row[5] = chargeInfo.valleyPower.ToString("#0.00");
                _chargeTable_1.Rows.Add(row);

                row = _chargeTable_1.NewRow();
                row[0] = chargeInfo.MID;
                row[1] = "无功（QI象限）";
                row[2] = "";
                row[4] = "30";
                row[5] = chargeInfo.ReactiveQI.ToString("#0.00");
                _chargeTable_1.Rows.Add(row);

                DataRow row1 = _chargeTable_2.NewRow();
                row1[0] = chargeInfo.activeCopperLoss.ToString("#0.00");
                row1[1] = chargeInfo.activeCoreLoss.ToString("#0.00");
                row1[2] = chargeInfo.activeAll.ToString("#0.00");
                row1[3] = chargeInfo.reactiveCopperLoss.ToString("#0.00");
                row1[4] = chargeInfo.reactiveCoreLoss.ToString("#0.00");
                row1[5] = chargeInfo.reactiveAll.ToString("#0.00");
                row1[6] = chargeInfo.peakPower.ToString("#0.00");
                row1[7] = chargeInfo.valleyPower.ToString("#0.00");
                row1[8] = chargeInfo.spikePower.ToString("#0.00");
                _chargeTable_2.Rows.Add(row1);

                DataRow row2 = _chargeTable_3.NewRow();
                row2[0] = 0;
                row2[1] = 0;
                row2[2] = 0;
                row2[3] = 0;
                row2[4] = 0;
                row2[5] = 0;
                row2[6] = 0;
                row2[7] = 0;
                row2[8] = 0;
                _chargeTable_3.Rows.Add(row2);

                DataRow row3 = _chargeTable_4.NewRow();
                Double total = 0;
                row3[0] = "尖 一般工商";
                row3[1] = chargeInfo.spikePower.ToString("#0.00");
                row3[2] = 0;
                row3[3] = "kW.h";
                row3[4] =
                    ((_chargeObjects as Dictionary<string, object>)["电量参数"] as List<ElectricityParameter>)[0].PriceSpike;
                row3[5] = (Convert.ToDouble(row3[4].ToString())*Convert.ToDouble(row3[1].ToString())).ToString("#0.00");
                total += Convert.ToDouble(row3[5]);
                _chargeTable_4.Rows.Add(row3);
                row3 = _chargeTable_4.NewRow();
                row3[0] = "峰 一般工商";
                row3[1] = chargeInfo.peakPower.ToString("#0.00");
                row3[2] = 0;
                row3[3] = "kW.h";
                row3[4] =
                    ((_chargeObjects as Dictionary<string, object>)["电量参数"] as List<ElectricityParameter>)[0].PricePeak;
                row3[5] = (Convert.ToDouble(row3[4].ToString())*Convert.ToDouble(row3[1].ToString())).ToString("#0.00");
                total += Convert.ToDouble(row3[5]);
                _chargeTable_4.Rows.Add(row3);
                row3 = _chargeTable_4.NewRow();
                row3[0] = "谷 一般工商";
                row3[1] = chargeInfo.valleyPower.ToString("#0.00");
                row3[2] = 0;
                row3[3] = "kW.h";
                row3[4] =
                    ((_chargeObjects as Dictionary<string, object>)["电量参数"] as List<ElectricityParameter>)[0]
                        .PriceValley;
                row3[5] = (Convert.ToDouble(row3[4].ToString())*Convert.ToDouble(row3[1].ToString())).ToString("#0.00");
                total += Convert.ToDouble(row3[5]);
                _chargeTable_4.Rows.Add(row3);
                row3 = _chargeTable_4.NewRow();
                row3[0] = "合计";
                row3[1] = "";
                row3[2] = "";
                row3[3] = "";
                row3[4] = "";
                row3[5] = total.ToString("#0.00");
                _chargeTable_4.Rows.Add(row3);


                gridCharge_1.DataSource = _chargeTable_1;
                gridCharge_2.DataSource = _chargeTable_2;
                gridCharge_3.DataSource = _chargeTable_3;
                gridCharge_4.DataSource = _chargeTable_4;


                XtraReport1 report = new XtraReport1();
                report.ExportOptions.PrintPreview.DefaultFileName = "国网浙江慈溪市供电公司非居民用户电费复核单据 （一次抄表）" +
                                                                    DateTime.Now.ToString("D");
 

                documentViewer.DocumentSource = report;
                report.SetReportDataSource(_chargeTable_1, _chargeTable_2, _chargeTable_3, _chargeTable_4);
            }
        }
    }

    public class ChargeControlSecond : ChargeControlsBase
    {
        private GridControl gridCharge_1;
        private GridControl gridCharge_2;
        private GridControl gridCharge_3;
        private GridControl gridCharge_4;
        private DocumentViewer documentViewer;

        public override void SetChargeUiControls(ChartControl chartControlChargeProportion,
            ChartControl Fe_CuPropotionCtl, ChartControl chargePropotionChart, GridControl gridControl1,
            GridControl gridControl2, GridControl gridControl3, GridControl gridControl4, DocumentViewer documentView)
        {
            ChargeProportion = chartControlChargeProportion;
            Fe_CuPropotion = Fe_CuPropotionCtl;
            ChargeChart = chargePropotionChart;
            gridCharge_1 = gridControl1;
            gridCharge_2 = gridControl2;
            gridCharge_3 = gridControl3;
            gridCharge_4 = gridControl4;
            documentViewer = documentView;
        }

        public override void SetPropotionData(ChargeInfo chargeInfo)
        {
            ChargeProportion.Series[0].Points.Clear();
            ChargeProportion.Series[0].Points.AddRange(new SeriesPoint[]
            {
                new SeriesPoint("尖峰", chargeInfo.spikePower.ToString("#0.00")),
                new SeriesPoint("峰", chargeInfo.peakPower.ToString("#0.00")),
                new SeriesPoint("谷", chargeInfo.valleyPower.ToString("#0.00")),
            });

            Fe_CuPropotion.Series[0].Points.Clear();
            Fe_CuPropotion.Series[0].Points.AddRange(new SeriesPoint[]
            {
                new SeriesPoint("有功铜损", chargeInfo.activeCopperLoss.ToString("#0.00")),
                new SeriesPoint("无功铜损", chargeInfo.reactiveCopperLoss.ToString("#0.00")),
                new SeriesPoint("有功铁损", chargeInfo.activeCoreLoss.ToString("#0.00")),
                new SeriesPoint("无功铁损", chargeInfo.reactiveCoreLoss.ToString("#0.00"))
            });
        }

        public override void SetChargeData(Object _chargeObjects)
        {
            ChargeInfo chargeInfo = (_chargeObjects as Dictionary<string, object>)["第二阶段"] as ChargeInfo;
            base.SetChargeData(chargeInfo);
            if (chargeInfo != null)
            {
                DataRow row = _chargeTable_1.NewRow();
                row[0] = chargeInfo.MID;
                row[1] = "有功（总）";
                row[2] = chargeInfo.endEnergy.ToString("#0.00");
                row[4] = "30";
                row[5] = chargeInfo.totalPower.ToString("#0.00");
                _chargeTable_1.Rows.Add(row);

                row = _chargeTable_1.NewRow();
                row[0] = chargeInfo.MID;
                row[1] = "有功（尖峰）";
                row[2] = "";
                row[4] = "30";
                row[5] = chargeInfo.spikePower.ToString("#0.00");
                _chargeTable_1.Rows.Add(row);

                row = _chargeTable_1.NewRow();
                row[0] = chargeInfo.MID;
                row[1] = "有功（峰）";
                row[2] = "";
                row[4] = "30";
                row[5] = chargeInfo.peakPower.ToString("#0.00");
                _chargeTable_1.Rows.Add(row);

                row = _chargeTable_1.NewRow();
                row[0] = chargeInfo.MID;
                row[1] = "有功（谷）";
                row[2] = "";
                row[4] = "30";
                row[5] = chargeInfo.valleyPower.ToString("#0.00");
                _chargeTable_1.Rows.Add(row);

                row = _chargeTable_1.NewRow();
                row[0] = chargeInfo.MID;
                row[1] = "无功（QI象限）";
                row[2] = "";
                row[4] = "30";
                row[5] = chargeInfo.ReactiveQI.ToString("#0.00");
                _chargeTable_1.Rows.Add(row);

                DataRow row1 = _chargeTable_2.NewRow();
                row1[0] = chargeInfo.activeCopperLoss.ToString("#0.00");
                row1[1] = chargeInfo.activeCoreLoss.ToString("#0.00");
                row1[2] = chargeInfo.activeAll.ToString("#0.00");
                row1[3] = chargeInfo.reactiveCopperLoss.ToString("#0.00");
                row1[4] = chargeInfo.reactiveCoreLoss.ToString("#0.00");
                row1[5] = chargeInfo.reactiveAll.ToString("#0.00");
                row1[6] = chargeInfo.peakPower.ToString("#0.00");
                row1[7] = chargeInfo.valleyPower.ToString("#0.00");
                row1[8] = chargeInfo.spikePower.ToString("#0.00");
                _chargeTable_2.Rows.Add(row1);

                DataRow row2 = _chargeTable_3.NewRow();
                row2[0] = 0;
                row2[1] = 0;
                row2[2] = 0;
                row2[3] = 0;
                row2[4] = 0;
                row2[5] = 0;
                row2[6] = 0;
                row2[7] = 0;
                row2[8] = 0;
                _chargeTable_3.Rows.Add(row2);
                DataRow row3 = _chargeTable_4.NewRow();
                Double total = 0;
                row3[0] = "尖 一般工商";
                row3[1] = chargeInfo.spikePower.ToString("#0.00");
                row3[2] = 0;
                row3[3] = "kW.h";
                row3[4] =
                    ((_chargeObjects as Dictionary<string, object>)["电量参数"] as List<ElectricityParameter>)[0].PriceSpike;
                row3[5] = (Convert.ToDouble(row3[4].ToString())*Convert.ToDouble(row3[1].ToString())).ToString("#0.00");
                total += Convert.ToDouble(row3[5]);
                _chargeTable_4.Rows.Add(row3);
                row3 = _chargeTable_4.NewRow();
                row3[0] = "峰 一般工商";
                row3[1] = chargeInfo.peakPower.ToString("#0.00");
                row3[2] = 0;
                row3[3] = "kW.h";
                row3[4] =
                    ((_chargeObjects as Dictionary<string, object>)["电量参数"] as List<ElectricityParameter>)[0].PricePeak;
                row3[5] = (Convert.ToDouble(row3[4].ToString())*Convert.ToDouble(row3[1].ToString())).ToString("#0.00");
                total += Convert.ToDouble(row3[5]);
                _chargeTable_4.Rows.Add(row3);
                row3 = _chargeTable_4.NewRow();
                row3[0] = "谷 一般工商";
                row3[1] = chargeInfo.valleyPower.ToString("#0.00");
                row3[2] = 0;
                row3[3] = "kW.h";
                row3[4] =
                    ((_chargeObjects as Dictionary<string, object>)["电量参数"] as List<ElectricityParameter>)[0]
                        .PriceValley;
                row3[5] = (Convert.ToDouble(row3[4].ToString())*Convert.ToDouble(row3[1].ToString())).ToString("#0.00");
                total += Convert.ToDouble(row3[5]);
                _chargeTable_4.Rows.Add(row3);
                row3 = _chargeTable_4.NewRow();
                row3[0] = "合计";
                row3[1] = "";
                row3[2] = "";
                row3[3] = "";
                row3[4] = "";
                row3[5] = total.ToString("#0.00");
                _chargeTable_4.Rows.Add(row3);


                gridCharge_1.DataSource = _chargeTable_1;
                gridCharge_2.DataSource = _chargeTable_2;
                gridCharge_3.DataSource = _chargeTable_3;
                gridCharge_4.DataSource = _chargeTable_4;


                XtraReport2 report = new XtraReport2();
                report.ExportOptions.PrintPreview.DefaultFileName = "国网浙江慈溪市供电公司非居民用户电费复核单据 （二次抄表）" +
                                                                    DateTime.Now.ToString("D");

                documentViewer.DocumentSource = report;
                report.SetReportDataSource(_chargeTable_1, _chargeTable_2, _chargeTable_3, _chargeTable_4);
            }
        }
    }
}