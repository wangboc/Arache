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
using DevExpress.XtraPrinting.Preview;

namespace AracheTest.UIControls
{
    class ChargeControls
    {
        public ChartControl chargeProportion { get; set; }
        public GridControl gridCharge_1_1 { get; set; }
        public GridControl gridCharge_1_2 { get; set; }
        public GridControl gridCharge_1_3 { get; set; }
        public GridControl gridCharge_1_4 { get; set; }
        public DocumentViewer documentViewer_1 { get; set; }


        private Dictionary<string, Object> chargeData;
        //======================================================
        //分别对应一期电费栏中的四个表格，从上到下
        private DataTable _chargeTable0 = new DataTable();
        private DataTable _chargeTable1 = new DataTable();
        private DataTable _chargeTable2 = new DataTable();
        private DataTable _chargeTable3 = new DataTable();
        //======================================================
        
        
        public ChargeControls()
        {
            chargeData = new Dictionary<string, Object>();
            InitChargeTableCtr();
        }
        
        private void InitChargeTableCtr()
        {
           _chargeTable0.Columns.Add("MID");
            _chargeTable0.Columns.Add("MName");
            _chargeTable0.Columns.Add("PresentShown");
            _chargeTable0.Columns.Add("PreviousShown");
            _chargeTable0.Columns.Add("Rate");
            _chargeTable0.Columns.Add("WPP");

            _chargeTable1.Columns.Add("PCu");
            _chargeTable1.Columns.Add("PFe");
            _chargeTable1.Columns.Add("PTotal");
            _chargeTable1.Columns.Add("QCu");
            _chargeTable1.Columns.Add("QFe");
            _chargeTable1.Columns.Add("QTotal");
            _chargeTable1.Columns.Add("Spike");
            _chargeTable1.Columns.Add("Valley");
            _chargeTable1.Columns.Add("Peak");

            _chargeTable2.Columns.Add("Capacity");
            _chargeTable2.Columns.Add("Need");
            _chargeTable2.Columns.Add("Shown");
            _chargeTable2.Columns.Add("NeedInReal");
            _chargeTable2.Columns.Add("NeedExceed");
            _chargeTable2.Columns.Add("CapacityPause");
            _chargeTable2.Columns.Add("DaysPause");
            _chargeTable2.Columns.Add("Compensate");
            _chargeTable2.Columns.Add("Advanced");


            _chargeTable3.Columns.Add("Items");
            _chargeTable3.Columns.Add("Quantity");
            _chargeTable3.Columns.Add("Rate");
            _chargeTable3.Columns.Add("Unit");
            _chargeTable3.Columns.Add("Price");
            _chargeTable3.Columns.Add("Total");
            _chargeTable3.Columns.Add("Remarks");
        }
        
        public void SetChargeUIControls(ChartControl chartControlChargeProportion, GridControl gridControl1,
            GridControl gridControl2, GridControl gridControl3, GridControl gridControl4, DocumentViewer documentViewer1)
        {
            this.chargeProportion = chartControlChargeProportion;
            this.gridCharge_1_1 = gridControl1;
            this.gridCharge_1_2 = gridControl2;
            this.gridCharge_1_3 = gridControl3;
            this.gridCharge_1_4 = gridControl4;
            this.documentViewer_1 = documentViewer1;
        }

        public void SetChargeData(Object dataSource)
        {
            chargeData = dataSource as Dictionary<string, Object>;
          
            
            _chargeTable0.Rows.Clear();
            _chargeTable1.Rows.Clear();
            _chargeTable2.Rows.Clear();
            _chargeTable3.Rows.Clear();

            if (chargeData["第一阶段"] != null)
            {
                ChargeInfo chargeInfo = chargeData["第一阶段"] as ChargeInfo;

                DataRow row = _chargeTable0.NewRow();
                row[0] = chargeInfo.MID;
                row[1] = "有功（总）";
                row[2] = chargeInfo.PowerTotal.ToString("#0.00");
                //  row[3] = chargeInfo.WPPOld.ToString();
                row[4] = "30";
                row[5] = chargeInfo.EnergyTotal.ToString("#0.00");
                _chargeTable0.Rows.Add(row);

                row = _chargeTable0.NewRow();
                row[0] = chargeInfo.MID;
                row[1] = "有功（尖峰）";
                row[2] = chargeInfo.PowerSpike.ToString("#0.00");
                //  row[3] = chargeFirstPeak.WPPOld.ToString();
                row[4] = "30";
                row[5] = chargeInfo.EnergySpike.ToString("#0.00");
                _chargeTable0.Rows.Add(row);

                row = _chargeTable0.NewRow();
                row[0] = chargeInfo.MID;
                row[1] = "有功（峰）";
                row[2] = chargeInfo.PowerPeak.ToString("#0.00");
                //  row[3] = chargeFirstPeak.WPPOld.ToString();
                row[4] = "30";
                row[5] = chargeInfo.EnergyPeak.ToString("#0.00");
                _chargeTable0.Rows.Add(row);

                row = _chargeTable0.NewRow();
                row[0] = chargeInfo.MID;
                row[1] = "有功（谷）";
                row[2] = chargeInfo.PowerValley.ToString("#0.00");
                //  row[3] = chargeFirstPeak.WPPOld.ToString();
                row[4] = "30";
                row[5] = chargeInfo.EnergyValley.ToString("#0.00");
                _chargeTable0.Rows.Add(row);
                gridCharge_1_1.DataSource = _chargeTable0;


                DataRow row1 = _chargeTable1.NewRow();
                row1[0] = chargeInfo.ActiveCopperLoss.ToString("#0.00");
                row1[1] = chargeInfo.ActiveCoreLoss.ToString("#0.00");
                row1[2] = chargeInfo.EnergyTotal.ToString("#0.00");
                row1[3] = chargeInfo.ReactiveCopperLoss.ToString("#0.00");
                row1[4] = chargeInfo.ReactiveCoreLoss.ToString("#0.00");
                row1[5] = 0;
                row1[6] = chargeInfo.EnergySpike.ToString("#0.00");
                row1[7] = chargeInfo.EnergyValley.ToString("#0.00");
                row1[8] = chargeInfo.EnergyPeak.ToString("#0.00");
                _chargeTable1.Rows.Add(row1);
                gridCharge_1_2.DataSource = _chargeTable1;

                DataRow row2 = _chargeTable2.NewRow();
                row2[0] = 0;
                row2[1] = 0;
                row2[2] = 0;
                row2[3] = 0;
                row2[4] = 0;
                row2[5] = 0;
                row2[6] = 0;
                row2[7] = 0;
                row2[8] = 0;
                _chargeTable2.Rows.Add(row2);
                gridCharge_1_3.DataSource = _chargeTable2;

                DataRow row3 = _chargeTable3.NewRow();
                Double total = 0;
                row3[0] = "尖 一般工商";
                row3[1] = chargeInfo.EnergySpike.ToString("#0.00");
                row3[2] = 0;
                row3[3] = "kW.h";
                row3[4] = 1.39760;
                row3[5] = (Convert.ToDouble(row3[4].ToString()) * Convert.ToDouble(row3[1].ToString())).ToString("#0.00");
                total += Convert.ToDouble(row3[5]);
                _chargeTable3.Rows.Add(row3);
                row3 = _chargeTable3.NewRow();
                row3[0] = "峰 一般工商";
                row3[1] = chargeInfo.EnergyPeak.ToString("#0.00");
                row3[2] = 0;
                row3[3] = "kW.h";
                row3[4] = 1.09960;
                row3[5] = (Convert.ToDouble(row3[4].ToString()) * Convert.ToDouble(row3[1].ToString())).ToString("#0.00");
                total += Convert.ToDouble(row3[5]);
                _chargeTable3.Rows.Add(row3);
                row3 = _chargeTable3.NewRow();
                row3[0] = "谷 一般工商";
                row3[1] = chargeInfo.EnergyValley.ToString("#0.00");
                row3[2] = 0;
                row3[3] = "kW.h";
                row3[4] = 0.58760;
                row3[5] = (Convert.ToDouble(row3[4].ToString()) * Convert.ToDouble(row3[1].ToString())).ToString("#0.00");
                total += Convert.ToDouble(row3[5]);
                _chargeTable3.Rows.Add(row3);
                row3 = _chargeTable3.NewRow();
                row3[0] = "合计";
                row3[1] = "";
                row3[2] = "";
                row3[3] = "";
                row3[4] = "";
                row3[5] = total.ToString("#0.00");
                _chargeTable3.Rows.Add(row3);
                gridCharge_1_4.DataSource = _chargeTable3;

                XtraReport1 report = new  XtraReport1();
                documentViewer_1.DocumentSource = report;
                report.SetReportDataSource(_chargeTable0, _chargeTable1, _chargeTable2, _chargeTable3);
                report.CreateDocument();

                chargeProportion.Series[0].Points.Clear();
                chargeProportion.Series[0].Points.AddRange(new SeriesPoint[]
                {
                    new SeriesPoint("尖峰", chargeInfo.EnergySpike.ToString("#0.00")),
                    new SeriesPoint("峰", chargeInfo.EnergyPeak.ToString("#0.00")),
                    new SeriesPoint("谷", chargeInfo.EnergyTotal.ToString("#0.00")),
                });
            }
        }

        
    }
}
