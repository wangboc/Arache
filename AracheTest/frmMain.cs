using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading.Tasks;
using System.Windows.Forms;
using AracheTest.Data;
using AracheTest.Reports;
using DevExpress.Utils;
using DevExpress.Xpo;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraCharts;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Popup;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraPrinting;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Native;
using DevExpress.XtraTreeList.Nodes;


namespace AracheTest
{
    public partial class frmMain : RibbonForm
    {
        private List<ElectricityOriginalData> _electricityDataList = new List<ElectricityOriginalData>();
        private List<NodeInfo> _nodesList = new List<NodeInfo>();
        private List<ChartControl> _chartCtrs = new List<ChartControl>();
        private Timer _timer_GetRealtime = new Timer();
        private Timer _timer_GetRealtimeData = new Timer();
        private Dictionary<string, Object> chargeData = new Dictionary<string, Object>();
        private ChargeInfo _chargeDataFirst;
        private ChargeInfo _chargeDataSecond;
        private int _currentNodeMID = 1;

        //======================================================
        //分别对应电费栏中的四个表格，从上到下
        private DataTable _chargeTable0 = new DataTable();
        private DataTable _chargeTable1 = new DataTable();
        private DataTable _chargeTable2 = new DataTable();
        private DataTable _chargeTable3 = new DataTable();
        //======================================================

        private PopupControlContainer popup = new PopupControlContainer();

        public frmMain()
        {
            InitializeComponent();
            ReadUserInfo();

            InitTimer();
            InitUIControls();

            TaskPool.SetScheduler(TaskScheduler.FromCurrentSynchronizationContext());
            RefreshAllData();
        }

        private void InitTimer()
        {
            InitRealTimeTimer();
            _timer_GetRealtimeData.Interval = 5*60*1000;
            _timer_GetRealtimeData.Tick += _timer_GetRealtimeData_Tick;
            _timer_GetRealtimeData.Start();
        }

        private void _timer_GetRealtimeData_Tick(object sender, EventArgs e)
        {
            if (RealtimeCheckBtn.Down)
            {
                RefreshAllData();
            }
        }

        private void InitRealTimeTimer()
        {
            _timer_GetRealtime.Interval = 1000;
            _timer_GetRealtime.Tick += TimerGetTick;
            _timer_GetRealtime.Start();
        }

        private void TimerGetTick(object sender, EventArgs e)
        {
            StartDatetimeCtr.EditValue = DateTime.Now.AddDays(-1);
            EndDatetimeCtr.EditValue = DateTime.Now;
        }

        private void InitUIControls()
        {
            RealtimeCheckBtn.Down = true;
            var start = StartDatetimeCtr.Edit as RepositoryItemDateEdit;
            start.CalendarTimeEditing = DefaultBoolean.True;
            start.EditMask = "yyyy/MM/dd HH:mm:ss";
            start.Mask.UseMaskAsDisplayFormat = true;
            var end = EndDatetimeCtr.Edit as RepositoryItemDateEdit;
            end.EditMask = "yyyy/MM/dd HH:mm:ss";
            end.CalendarTimeEditing = DefaultBoolean.True;
            end.Mask.UseMaskAsDisplayFormat = true;

            FilterButton.Visibility = BarItemVisibility.Never;
            RealtimeCheckBtn.Down = true;

            StartDatetimeCtr.Enabled = false;
            EndDatetimeCtr.Enabled = false;
            ((GridView) gridControlDetail.Views[0]).BestFitColumns();

            InitChartControl();
            InitNodeTreeControl();
            InitChargeCustomCtr();
            InitChargeTableCtr();
        }

        private void InitChargeCustomCtr()
        {
            TableLayoutPanel panel = new TableLayoutPanel();
            panel.Dock = DockStyle.Fill;
            panel.BorderStyle = BorderStyle.FixedSingle;
            panel.RowCount = 4;
            panel.ColumnCount = 2;
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 25));
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 25));
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 25));
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 25));
            Label starttimeLabel = new Label();
            starttimeLabel.Text = "计费开始时间:";
            starttimeLabel.TextAlign = ContentAlignment.MiddleRight;
            Label middletimeLabel = new Label();
            middletimeLabel.Text = "第一次计费时间:";
            middletimeLabel.TextAlign = ContentAlignment.MiddleRight;
            Label lasttimeLabel = new Label();
            lasttimeLabel.Text = "第二次计费时间:";
            lasttimeLabel.TextAlign = ContentAlignment.MiddleRight;
            DateEdit startDate = new DateEdit();
            startDate.Dock = DockStyle.Fill;
            startDate.Name = "startDate";
            startDate.EditValue = DateTime.Now;
            startDate.Properties.CalendarView = CalendarView.Vista;
            startDate.Properties.VistaDisplayMode = DefaultBoolean.True;
            startDate.Properties.DisplayFormat.FormatString = "yyyy/MM/dd HH:mm:ss";
            startDate.Properties.VistaEditTime = DefaultBoolean.True;
            startDate.Properties.EditMask = "yyyy/MM/dd HH:mm:ss";
            DateEdit middleDate = new DateEdit();
            middleDate.Name = "middleDate";
            middleDate.Dock = DockStyle.Fill;
            middleDate.EditValue = DateTime.Now;
            middleDate.Properties.CalendarView = CalendarView.Vista;
            middleDate.Properties.VistaDisplayMode = DefaultBoolean.True;
            middleDate.Properties.DisplayFormat.FormatString = "yyyy/MM/dd HH:mm:ss";
            middleDate.Properties.EditMask = "yyyy/MM/dd HH:mm:ss";
            middleDate.Properties.VistaEditTime = DefaultBoolean.True;
            panel.Controls.Add(starttimeLabel, 0, 0);
            DateEdit lastDate = new DateEdit();
            lastDate.Dock = DockStyle.Fill;
            lastDate.Name = "lastDate";
            lastDate.EditValue = DateTime.Now;
            lastDate.Properties.CalendarView = CalendarView.Vista;
            lastDate.Properties.VistaDisplayMode = DefaultBoolean.True;
            lastDate.Properties.DisplayFormat.FormatString = "yyyy/MM/dd HH:mm:ss";
            lastDate.Properties.EditMask = "yyyy/MM/dd HH:mm:ss";
            lastDate.Properties.VistaEditTime = DefaultBoolean.True;
            panel.Controls.Add(starttimeLabel, 0, 0);
            panel.Controls.Add(middletimeLabel, 0, 1);
            panel.Controls.Add(lasttimeLabel, 0, 2);
            panel.Controls.Add(startDate, 1, 0);
            panel.Controls.Add(middleDate, 0, 1);
            panel.Controls.Add(lastDate, 1, 2);
            TableLayoutPanel panelInside = new TableLayoutPanel();
            panelInside.RowCount = 1;
            panelInside.ColumnCount = 4;
            SimpleButton btnOK = new SimpleButton();
            btnOK.Text = "确定";
            btnOK.Click += btnOK_Click;
            btnOK.Dock = DockStyle.Right;
            SimpleButton btnCancel = new SimpleButton();
            btnCancel.Text = "取消";
            btnCancel.Dock = DockStyle.Right;
            panelInside.Controls.Add(btnOK, 2, 0);
            panelInside.Controls.Add(btnCancel, 3, 0);
            btnCancel.Click += btnCancel_Click;
            panel.Controls.Add(panelInside, 1, 3);


            popup.Width = 300;
            popup.Height = 110;
            popup.Controls.Add(panel);

            ChargeDateSelectBtn.DropDownControl = popup;
        }


        private void InitChargeTableCtr()
        {
            XtraReport2 report = new XtraReport2();
            documentViewer1.DocumentSource = report;

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

        private void InitChartControl()
        {
            _chartCtrs.Add(PCChartControl);
            _chartCtrs.Add(SChartControl);
            _chartCtrs.Add(PUChartControl);
            _chartCtrs.Add(PFChartControl);
            _chartCtrs.Add(PChartControl);
            _chartCtrs.Add(QChartControl);
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

        private void InitNodeTreeControl()
        {
            RepositoryItemTreeListLookUpEdit nodeTreeEdit = nodeTreeCtr.Edit as RepositoryItemTreeListLookUpEdit;
            nodeTreeEdit.EditValueChanged += nodetree_EditValueChanged;
            nodeTreeEdit.TreeList.CustomDrawNodeCell += TreeList_CustomDrawNodeCell;
            nodeTreeEdit.TreeList.StateImageList = sharedImageCollection;
            nodeTreeEdit.TreeList.GetStateImage += TreeList_GetStateImage;
            nodeTreeEdit.TreeList.NodeCellStyle += TreeList_NodeCellStyle;
            nodeTreeEdit.QueryCloseUp += nodeTreeEdit_QueryCloseUp;
            nodeTreeEdit.TreeList.MoveFirst();


            nodeTreeEdit.ValueMember = "NodeID";
            nodeTreeEdit.DisplayMember = "Name";
            var nodeTree = nodeTreeEdit.TreeList;
            var columnName = nodeTree.Columns.Add();
            columnName.Caption = "节点名称";
            columnName.Name = "NodeName";
            columnName.Visible = true;
            columnName.FieldName = "Name";

            var columnNodeID = nodeTree.Columns.Add();
            columnNodeID.Caption = "NodeID";
            columnNodeID.FieldName = "NodeID";
            columnNodeID.Name = "NodeID";
            columnNodeID.Visible = true;

            var columnParentID = nodeTree.Columns.Add();
            columnParentID.Caption = "ParentID";
            columnParentID.Name = "ParentID";
            columnParentID.FieldName = "ParentID";
            columnParentID.Visible = false;

            var columnPID = nodeTree.Columns.Add();
            columnPID.Caption = "PID";
            columnPID.Name = "PID";
            columnPID.FieldName = "PID";
            columnPID.Visible = false;

            var columnMID = nodeTree.Columns.Add();
            columnMID.Caption = "MID";
            columnMID.Name = "MID";
            columnMID.FieldName = "MID";
            columnMID.Visible = false;

            nodeTree.KeyFieldName = "NodeID";
            nodeTree.ParentFieldName = "ParentID";


            nodeTreeEdit.AutoExpandAllNodes = false;
        }
        void nodeTreeEdit_QueryCloseUp(object sender, CancelEventArgs e)
        {
            TreeListLookUpEdit nodeTreeEdit = sender as TreeListLookUpEdit;
            var node = nodeTreeEdit.Properties.TreeList.FocusedNode;
            bool isNode = Convert.ToBoolean(node.GetValue("IsNode"));
            e.Cancel = isNode;
        }

 

        void TreeList_GetStateImage(object sender, GetStateImageEventArgs e)
        {
            bool isNode = Convert.ToBoolean(e.Node.GetValue("IsNode"));

            if (isNode)
                e.NodeImageIndex = 2;
            else
                e.NodeImageIndex = 5;
        }

        private void TreeList_NodeCellStyle(object sender, GetCustomNodeCellStyleEventArgs e)
        {
            bool isNode = Convert.ToBoolean(e.Node.GetValue("IsNode"));
            if (isNode == false)
            {
                e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
            }
        }

        private void TreeList_CustomDrawNodeCell(object sender, DevExpress.XtraTreeList.CustomDrawNodeCellEventArgs e)
        {
            Brush backBrush, foreBrush;
            if (e.Node != (sender as TreeList).FocusedNode)
            {
                backBrush = new LinearGradientBrush(e.Bounds, Color.PapayaWhip, Color.PeachPuff,
                    LinearGradientMode.ForwardDiagonal);
                foreBrush = Brushes.Black;
            }
            else
            {
                backBrush = Brushes.PeachPuff;
                foreBrush = new SolidBrush(Color.Black);
            }
            // Fill the background.
            e.Graphics.FillRectangle(backBrush, e.Bounds);
            // Paint the node value.
            e.Graphics.DrawString(e.CellText, e.Appearance.Font, foreBrush, e.Bounds,
                e.Appearance.GetStringFormat());
            // Prohibit default painting.
            e.Handled = true;
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

        private void RefreshAllData()
        {
            var task = new TaskElectricityFilter("更新数据",
                new FilterCondition(DateTime.Now, DateTime.Now, _currentNodeMID), SetElectricityData, true);
            TaskPool.AddTask(task);
            var taskNode = new TaskFetchNodes("更新节点", new ConditionBase(), SetNodesData);
            TaskPool.AddTask(taskNode);
        }

        private void UpdateNodesData()
        {
            var nodeTreeEdit = (RepositoryItemTreeListLookUpEdit) nodeTreeCtr.Edit;
            nodeTreeEdit.DataSource = _nodesList;

            nodeTreeEdit.TreeList.CollapseAll();
        }

        private void SetNodesData(Object list)
        {
            _nodesList = list as List<NodeInfo>;
            UpdateNodesData();
        }

        private void RealtimeCheckBtn_DownChanged(object sender, ItemClickEventArgs e)
        {
            if (RealtimeCheckBtn.Down)
            {
                StartDatetimeCtr.Enabled = false;
                EndDatetimeCtr.Enabled = false;

                FilterButton.Visibility = BarItemVisibility.Never;

                _timer_GetRealtime.Start();
                _timer_GetRealtimeData.Start();

                ResetUISource();
            }
            else
            {
                FilterButton.Visibility = BarItemVisibility.Always;
                StartDatetimeCtr.Enabled = true;
                EndDatetimeCtr.Enabled = true;

                _timer_GetRealtime.Stop();
                _timer_GetRealtimeData.Stop();

                ResetUISource();
            }
        }

        private void GetFilteredData()
        {
            ResetUISource();
            var task = new TaskElectricityFilter("检索数据",
                new FilterCondition((DateTime) StartDatetimeCtr.EditValue, (DateTime) EndDatetimeCtr.EditValue,
                    _currentNodeMID), SetElectricityData, false);
            TaskPool.AddTask(task);
        }


        private void FilterButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (!RealtimeCheckBtn.Down)
            {
                var bs = new BackgroundWorker();
                if ((StartDatetimeCtr.EditValue != null && EndDatetimeCtr.EditValue != null) &&
                    (StartDatetimeCtr.EditValue.ToString() != "" && EndDatetimeCtr.EditValue.ToString() != "") &&
                    ((DateTime) EndDatetimeCtr.EditValue > (DateTime) StartDatetimeCtr.EditValue))
                {
                    GetFilteredData();
                }
                else MessageBox.Show("时间设定错误");
            }
        }

        private void ResetUISource()
        {
            ResetChartDataSource();
            ResetGridDataDetailDataSource();
            ResetRealTimeGridDataSource();
        }

        private void SetChargeData(Object dataSource)
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
                gridControl1.DataSource = _chargeTable0;


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
                gridControl2.DataSource = _chargeTable1;

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
                gridControl3.DataSource = _chargeTable2;

                DataRow row3 = _chargeTable3.NewRow();
                Double total = 0;
                row3[0] = "尖 一般工商";
                row3[1] = chargeInfo.EnergySpike.ToString("#0.00");
                row3[2] = 0;
                row3[3] = "kW.h";
                row3[4] = 1.39760;
                row3[5] = (Convert.ToDouble(row3[4].ToString())*Convert.ToDouble(row3[1].ToString())).ToString("#0.00");
                total += Convert.ToDouble(row3[5]);
                _chargeTable3.Rows.Add(row3);
                row3 = _chargeTable3.NewRow();
                row3[0] = "峰 一般工商";
                row3[1] = chargeInfo.EnergyPeak.ToString("#0.00");
                row3[2] = 0;
                row3[3] = "kW.h";
                row3[4] = 1.09960;
                row3[5] = (Convert.ToDouble(row3[4].ToString())*Convert.ToDouble(row3[1].ToString())).ToString("#0.00");
                total += Convert.ToDouble(row3[5]);
                _chargeTable3.Rows.Add(row3);
                row3 = _chargeTable3.NewRow();
                row3[0] = "谷 一般工商";
                row3[1] = chargeInfo.EnergyValley.ToString("#0.00");
                row3[2] = 0;
                row3[3] = "kW.h";
                row3[4] = 0.58760;
                row3[5] = (Convert.ToDouble(row3[4].ToString())*Convert.ToDouble(row3[1].ToString())).ToString("#0.00");
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
                gridControl4.DataSource = _chargeTable3;


                XtraReport2 report = documentViewer1.DocumentSource as XtraReport2;
                report.SetReportDataSource(_chargeTable0, _chargeTable1, _chargeTable2, _chargeTable3);
                report.CreateDocument();


                chartControlChargeProportion.Series[0].Points.Clear();
                chartControlChargeProportion.Series[0].Points.AddRange(new SeriesPoint[]
                {
                    new SeriesPoint("尖峰", chargeInfo.EnergySpike),
                    new SeriesPoint("峰", chargeInfo.EnergyPeak),
                    new SeriesPoint("谷", chargeInfo.EnergyTotal),
                });
            }
        }

        private void SetElectricityData(Object dataSource)
        {
            _electricityDataList = dataSource as List<ElectricityOriginalData>;
            UpdateRealtimeDataGrid();
            UpdateGridControlDetail();
            UpdateChart();
        }


        private void ResetRealTimeGridDataSource()
        {
            propertyGridRealtime.SelectedObject = null;
        }

        private void UpdateRealtimeDataGrid()
        {
            if (_electricityDataList.Count > 0)
            {
                ElectricityOriginalData data = _electricityDataList[_electricityDataList.Count - 1];
                propertyGridRealtime.SelectedObject = data;
                propertyGridRealtime.Visible = true;
                labelControl1.Text = "实时数据";
            }
            else
            {
                propertyGridRealtime.Visible = false;
                labelControl1.Text = "暂无实时数据";
            }
        }


        private void ResetGridDataDetailDataSource()
        {
            gridControlDetail.DataSource = null;
        }

        private void UpdateGridControlDetail()
        {
            gridControlDetail.DataSource = _electricityDataList;
        }

        private void ResetChartDataSource()
        {
            foreach (var chart in _chartCtrs)
            {
                chart.DataSource = null;
            }
        }

        private void UpdateChart()
        {
            ResetChartDataSource();
            foreach (var chart in _chartCtrs)
            {
                chart.DataSource = _electricityDataList;
            }
        }

        private void RefreshBtn_ItemClick(object sender, ItemClickEventArgs e)
        {
            RefreshAllData();
        }

        /// <summary>
        /// 改变线路
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void nodetree_EditValueChanged(object sender, EventArgs e)
        {
            TreeListLookUpEdit nodetree = sender as TreeListLookUpEdit;
            NodeInfo node = nodetree.GetSelectedDataRow() as NodeInfo;

            if (node.IsNode)
                return;

            _currentNodeMID = node.MID[0];
            if (RealtimeCheckBtn.Down)
            {
                RefreshAllData();
            }
            else
            {
                GetFilteredData();
            }
        }

        private void barCheckItemCurrentDay_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (ChargeDateSelectBtn.Down)
            {
                ChargeDateSelectBtn.Down = false;
            }
            barCheckItemCurrentDay.ItemAppearance.Normal.ForeColor = Color.Black;
            barCheckItemCurrentMonth.ItemAppearance.Normal.ForeColor = Color.Black;
            barCheckItemCurrentYear.ItemAppearance.Normal.ForeColor = Color.Black;
        }

        private void barCheckItemCurrentDay_CheckedChanged(object sender, ItemClickEventArgs e)
        {
            var taskCharge = new TaskChargeFilter("获取当天计费信息",
                new ChargeFilterCondition(
                    new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0), DateTime.Now,
                    DateTime.Now, _currentNodeMID), SetChargeData);
            TaskPool.AddTask(taskCharge);
        }

        private void barCheckItemCurrentMonth_CheckedChanged(object sender, ItemClickEventArgs e)
        {
            var taskCharge = new TaskChargeFilter("获取本月计费信息",
                new ChargeFilterCondition(new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0), DateTime.Now,
                    DateTime.Now, _currentNodeMID), SetChargeData);
            TaskPool.AddTask(taskCharge);
        }

        private void barCheckItemCurrentYear_CheckedChanged(object sender, ItemClickEventArgs e)
        {
            var taskCharge = new TaskChargeFilter("获取本年计费信息",
                new ChargeFilterCondition(new DateTime(DateTime.Now.Year, 1, 1, 0, 0, 0), DateTime.Now, DateTime.Now,
                    _currentNodeMID), SetChargeData);
            TaskPool.AddTask(taskCharge);
        }

        private void barCheckItemCurrentMonth_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (ChargeDateSelectBtn.Down)
            {
                ChargeDateSelectBtn.Down = false;
            }
            barCheckItemCurrentDay.ItemAppearance.Normal.ForeColor = Color.Black;
            barCheckItemCurrentMonth.ItemAppearance.Normal.ForeColor = Color.Black;
            barCheckItemCurrentYear.ItemAppearance.Normal.ForeColor = Color.Black;
        }

        private void barCheckItemCurrentYear_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (ChargeDateSelectBtn.Down)
            {
                ChargeDateSelectBtn.Down = false;
            }


            barCheckItemCurrentDay.ItemAppearance.Normal.ForeColor = Color.Black;
            barCheckItemCurrentMonth.ItemAppearance.Normal.ForeColor = Color.Black;
            barCheckItemCurrentYear.ItemAppearance.Normal.ForeColor = Color.Black;
        }

        private void ChargeDateSelectBtn_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (!ChargeDateSelectBtn.Down)
            {
                ChargeDateSelectBtn.Down = true;
            }
            barCheckItemCurrentDay.ItemAppearance.Normal.ForeColor = Color.DarkGray;
            barCheckItemCurrentMonth.ItemAppearance.Normal.ForeColor = Color.DarkGray;
            barCheckItemCurrentYear.ItemAppearance.Normal.ForeColor = Color.DarkGray;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            PopupControlContainer popup = ChargeDateSelectBtn.DropDownControl as PopupControlContainer;
            popup.HidePopup();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            PopupControlContainer popup = ChargeDateSelectBtn.DropDownControl as PopupControlContainer;
            popup.HidePopup();

            DateEdit startDate = popup.Controls.Find("startDate", true)[0] as DateEdit;
            DateEdit middleDate = popup.Controls.Find("middleDate", true)[0] as DateEdit;
            DateEdit lastDate = popup.Controls.Find("lastDate", true)[0] as DateEdit;

            if (startDate.EditValue != null && startDate.EditValue != "" && middleDate.EditValue != null &&
                middleDate.EditValue != "" && lastDate.EditValue != null && lastDate.EditValue != "" &&
                startDate.DateTime <= middleDate.DateTime && middleDate.DateTime <= lastDate.DateTime)
            {
                var taskCharge = new TaskChargeFilter("获取自定义时段计费信息",
                    new ChargeFilterCondition(startDate.DateTime, middleDate.DateTime, lastDate.DateTime,
                        _currentNodeMID), SetChargeData);
                TaskPool.AddTask(taskCharge);
            }
            else MessageBox.Show("计费时间设定错误");
        }

        #region 凌老师代码

        private void ReadUserInfo()
        {
            treeListUser.BeginUnboundLoad();
            treeListUser.AppendNode(new object[] {"XXX-XXX-XX-1"}, -1);
            treeListUser.Nodes[0].ImageIndex = treeListUser.Nodes[0].SelectImageIndex = 0;
            treeListUser.Nodes[0].Nodes.Add("包钢展昊", null);
            treeListUser.Nodes[0].Nodes[0].ImageIndex = treeListUser.Nodes[0].Nodes[0].SelectImageIndex = 1;

            treeListUser.Nodes[0].Nodes[0].Nodes.Add("1#变压器", null);
            treeListUser.Nodes[0].Nodes[0].Nodes[0].SelectImageIndex =
                treeListUser.Nodes[0].Nodes[0].Nodes[0].ImageIndex = 2;

            treeListUser.Nodes[0].Nodes[0].Nodes[0].Nodes.Add("1#出线", null);
            treeListUser.Nodes[0].Nodes[0].Nodes[0].Nodes[0].ImageIndex =
                treeListUser.Nodes[0].Nodes[0].Nodes[0].Nodes[0].SelectImageIndex = 3;

            treeListUser.Nodes[0].Nodes[0].Nodes[0].Nodes.Add("2#出线", null);
            treeListUser.Nodes[0].Nodes[0].Nodes[0].Nodes[1].ImageIndex =
                treeListUser.Nodes[0].Nodes[0].Nodes[0].Nodes[1].SelectImageIndex = 3;

            treeListUser.Nodes[0].Nodes[0].Nodes[0].Nodes.Add("3#出线", null);
            treeListUser.Nodes[0].Nodes[0].Nodes[0].Nodes[2].ImageIndex =
                treeListUser.Nodes[0].Nodes[0].Nodes[0].Nodes[2].SelectImageIndex = 3;

            treeListUser.Nodes[0].Nodes[0].Nodes[0].Nodes.Add("4#出线", null);
            treeListUser.Nodes[0].Nodes[0].Nodes[0].Nodes[3].ImageIndex =
                treeListUser.Nodes[0].Nodes[0].Nodes[0].Nodes[3].SelectImageIndex = 3;

            treeListUser.Nodes[0].Nodes[0].Nodes[0].Nodes.Add("5#出线", null);
            treeListUser.Nodes[0].Nodes[0].Nodes[0].Nodes[4].ImageIndex =
                treeListUser.Nodes[0].Nodes[0].Nodes[0].Nodes[4].SelectImageIndex = 3;

            treeListUser.Nodes[0].Nodes[0].Nodes[0].Nodes.Add("6#出线", null);
            treeListUser.Nodes[0].Nodes[0].Nodes[0].Nodes[5].ImageIndex =
                treeListUser.Nodes[0].Nodes[0].Nodes[0].Nodes[5].SelectImageIndex = 3;

            treeListUser.Nodes[0].Nodes[0].Nodes[0].Nodes.Add("7#出线", null);
            treeListUser.Nodes[0].Nodes[0].Nodes[0].Nodes[6].ImageIndex =
                treeListUser.Nodes[0].Nodes[0].Nodes[0].Nodes[6].SelectImageIndex = 3;

            treeListUser.Nodes[0].Nodes[0].Nodes[0].Nodes.Add("8#出线", null);
            treeListUser.Nodes[0].Nodes[0].Nodes[0].Nodes[7].ImageIndex =
                treeListUser.Nodes[0].Nodes[0].Nodes[0].Nodes[7].SelectImageIndex = 3;

            treeListUser.Nodes[0].Nodes[0].Nodes[0].Nodes.Add("9#出线", null);
            treeListUser.Nodes[0].Nodes[0].Nodes[0].Nodes[8].ImageIndex =
                treeListUser.Nodes[0].Nodes[0].Nodes[0].Nodes[8].SelectImageIndex = 3;

            treeListUser.Nodes[0].Nodes[0].Nodes[0].Nodes.Add("10#出线", null);
            treeListUser.Nodes[0].Nodes[0].Nodes[0].Nodes[9].ImageIndex =
                treeListUser.Nodes[0].Nodes[0].Nodes[0].Nodes[9].SelectImageIndex = 3;

            treeListUser.Nodes[0].Nodes[0].Nodes[0].Nodes.Add("11#出线", null);
            treeListUser.Nodes[0].Nodes[0].Nodes[0].Nodes[10].ImageIndex =
                treeListUser.Nodes[0].Nodes[0].Nodes[0].Nodes[10].SelectImageIndex = 3;

            treeListUser.Nodes[0].Nodes[0].Nodes[0].Nodes.Add("12#出线", null);
            treeListUser.Nodes[0].Nodes[0].Nodes[0].Nodes[11].ImageIndex =
                treeListUser.Nodes[0].Nodes[0].Nodes[0].Nodes[11].SelectImageIndex = 3;

            treeListUser.Nodes[0].Nodes[0].Nodes.Add("2#变压器", null);
            treeListUser.Nodes[0].Nodes[0].Nodes[1].SelectImageIndex =
                treeListUser.Nodes[0].Nodes[0].Nodes[1].ImageIndex = 2;

            treeListUser.Nodes[0].Nodes[0].Nodes.Add("3#变压器", null);
            treeListUser.Nodes[0].Nodes[0].Nodes[2].SelectImageIndex =
                treeListUser.Nodes[0].Nodes[0].Nodes[2].ImageIndex = 2;

            treeListUser.Nodes[0].Nodes[0].Nodes.Add("4#变压器", null);
            treeListUser.Nodes[0].Nodes[0].Nodes[3].SelectImageIndex =
                treeListUser.Nodes[0].Nodes[0].Nodes[3].ImageIndex = 2;

            treeListUser.Nodes[0].Nodes[0].Nodes.Add("5#变压器", null);
            treeListUser.Nodes[0].Nodes[0].Nodes[4].SelectImageIndex =
                treeListUser.Nodes[0].Nodes[0].Nodes[4].ImageIndex = 2;


            treeListUser.ExpandAll();
            treeListUser.Nodes.Add("XXX-XXX-XX-2", null);
            treeListUser.EndUnboundLoad();
        }


        private void frmMain_Load(object sender, EventArgs e)
        {
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            labelControlTime.Text = DateTime.Now.TimeOfDay.ToString().Substring(0, 8);
            labelControlDate.Text = DateTime.Now.Date.ToLongDateString();
        }

        #endregion

        private void documentViewer1_Load(object sender, EventArgs e)
        {
        }
    }
}