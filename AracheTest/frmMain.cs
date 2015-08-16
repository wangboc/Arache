using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using AracheTest.Data;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraCharts;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Popup;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Grid;


namespace AracheTest
{
    public partial class frmMain : RibbonForm
    {
        private List<ElectricityOriginalData> _electricityDataList = new List<ElectricityOriginalData>();
        private int _currentNodeMID = 1; //由于目前只有三个电表信息，暂时不做电表和节点对应，仅查找MID为1、2、3的三组数据。
        private List<NodeInfo> _nodesList = new List<NodeInfo>();
        private List<ChartControl> _chartCtrs = new List<ChartControl>();
        private Timer _timer_GetRealtime = new Timer();
        private Timer _timer_GetRealtimeData = new Timer();
        private Dictionary<string, Object> chargeData = new Dictionary<string, Object>();
        private ChargeInfo _chargeDataFirst;
        private ChargeInfo _chargeDataSecond;

        private TaskPool taskPool = new TaskPool();

        private PopupControlContainer popup = new PopupControlContainer();

        public frmMain()
        {
            InitializeComponent();
            ReadUserInfo();
            InitUIControls();
            InitTimer();

            taskPool.SetUpdateUIDelegate(UpdateAllControlsDataSource);
            RefreshAllData();


            RepositoryItemTreeListLookUpEdit nodetree = nodeTreeCtr.Edit as RepositoryItemTreeListLookUpEdit;
            nodetree.EditValueChanged += nodetree_EditValueChanged;
            nodetree.TreeList.MoveFirst();
        }

        private void InitTimer()
        {
            InitTimerRealtime();
            _timer_GetRealtimeData.Interval = 60*1000;
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

        private void InitTimerRealtime()
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

        private void InitChargeDateSelectCtr()
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
            startDate.EditValue = DateTime.Now;
            startDate.Properties.CalendarView = CalendarView.Vista;
            startDate.Properties.VistaDisplayMode = DefaultBoolean.True;
            startDate.Properties.DisplayFormat.FormatString = "yyyy/MM/dd HH:mm:ss";
            startDate.Properties.VistaEditTime = DefaultBoolean.True;
            startDate.Properties.EditMask = "yyyy/MM/dd HH:mm:ss";
            DateEdit middleDate = new DateEdit();
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


        private void btnCancel_Click(object sender, EventArgs e)
        {
            PopupControlContainer popup = ChargeDateSelectBtn.DropDownControl as PopupControlContainer;
            popup.HidePopup();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            PopupControlContainer popup = ChargeDateSelectBtn.DropDownControl as PopupControlContainer;
            popup.HidePopup();
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
            InitChargeDateSelectCtr();
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
            var nodeTreeEdit = (RepositoryItemTreeListLookUpEdit) nodeTreeCtr.Edit;
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
            ResetUISource();
            var task = new TaskElectricityFilter("更新数据",
                new FilterCondition(DateTime.Now, DateTime.Now, _currentNodeMID), SetElectricityData, true);
            taskPool.AddTask(task);
            var taskNode = new TaskFetchNodes("更新节点", new ConditionBase(), SetNodesData);
            taskPool.AddTask(taskNode);
            taskPool.Run();
        }

        private void UpdateAllControlsDataSource()
        {
            var nodeTreeEdit = (RepositoryItemTreeListLookUpEdit) nodeTreeCtr.Edit;
            nodeTreeEdit.DataSource = _nodesList;

            UpdateElectricityUI();
        }

        private void SetNodesData(Object list)
        {
            _nodesList = list as List<NodeInfo>;
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
            taskPool.AddTask(task);

          

            taskPool.Run();
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
        }


        private void SetElectricityData(Object dataSource)
        {
            _electricityDataList = dataSource as List<ElectricityOriginalData>;
        }

        private void UpdateElectricityUI()
        {
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
            _currentNodeMID = node.NodeID;
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

            var taskCharge = new TaskChargeFilter("获取阶段计费信息",
              new ChargeFilterCondition(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0), DateTime.Now,  DateTime.Now, _currentNodeMID), SetChargeData);
            taskPool.AddTask(taskCharge);

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
    }
}