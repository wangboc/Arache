using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using AracheTest.Data;
using AracheTest.Reports;
using AracheTest.UIControls;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraPrinting.Preview;
using log4net.Repository;

namespace AracheTest
{
    public partial class frmMain : RibbonForm
    {
        private int CurrentPID;
        private List<ElectricityOriginalData> _electricityDataList = new List<ElectricityOriginalData>();
        private Dictionary<string, Object> _chargeObjects = new Dictionary<string, object>();

        private Timer _timer_GetRealtime = new Timer();
        private Timer _timer_GetRealtimeData = new Timer();

        private ChargeControlFirst _chargeControlsFirst;
        private ChargeControlSecond _chargeControlsSecond;
        private ElectricityCharts _electricityCharts;
        private NodeTreeControl _nodeTreeControl;
        private PopupControlContainer popup = new PopupControlContainer();

        public frmMain()
        {
            InitializeComponent();
            ReadUserInfo();
            InitTimer();
            InitUIControls();
            CurrentPID = int.Parse(ConfigurationManager.AppSettings["PID"]);
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
                RefreshAllData();
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
            InitChargeDateCustomCtr();
        }

        private void SetChargeGridandChartControls()
        {
            _chargeControlsFirst = new ChargeControlFirst();
            _chargeControlsSecond = new ChargeControlSecond();

            _chargeControlsFirst.SetChargeUiControls(chartControlChargeProportion, Fe_CuPropotion, gridControl_1_1,
                gridControl_1_2,
                gridControl_1_3, gridControl_1_4, documentViewer1);
            _chargeControlsFirst.SetChargeData(_chargeObjects);

            _chargeControlsSecond.SetChargeUiControls(chartControlChargeProportion, Fe_CuPropotion, gridControl_2_1,
                gridControl_2_2,
                gridControl_2_3, gridControl_2_4, documentViewer2);
            _chargeControlsSecond.SetChargeData(_chargeObjects);

            _chargeControlsFirst.SetPropotionData(_chargeObjects["第一阶段"] as ChargeInfo);
            xtraTabControl2.SelectedTabPageIndex = 0;
            xtraTabControl4.SelectedTabPageIndex = 0;
        }

        private void InitChargeDateCustomCtr()
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

        private void InitChartControl()
        {
            _electricityCharts = new ElectricityCharts();
            _electricityCharts.PCChartControl = PCChartControl;
            _electricityCharts.SChartControl = SChartControl;
            _electricityCharts.PUChartControl = PUChartControl;
            _electricityCharts.PFChartControl = PFChartControl;
            _electricityCharts.PChartControl = PChartControl;
            _electricityCharts.QChartControl = QChartControl;
            _electricityCharts.InitControls();
        }

        private void InitNodeTreeControl()
        {
            _nodeTreeControl = new NodeTreeControl();
            _nodeTreeControl.NodeTreeEdit = nodeTreeCtr.Edit as RepositoryItemTreeListLookUpEdit;
            _nodeTreeControl.NodeTreeEdit.EditValueChanged += nodetree_EditValueChanged;
            _nodeTreeControl.NodeTreeEdit.TreeList.StateImageList = sharedImageCollection;
            _nodeTreeControl.Init();
        }

        private void RefreshAllData()
        {
            var task = new TaskElectricityFilter("更新数据",
                new FilterCondition(DateTime.Now, DateTime.Now, _nodeTreeControl.CurrentNodeMid, CurrentPID),
                SetElectricityData,
                true);
            TaskPool.AddTask(task, TaskScheduler.FromCurrentSynchronizationContext());

            var taskNode = new TaskFetchNodes("更新节点", new ConditionBase(CurrentPID), SetNodesData);
            TaskPool.AddTask(taskNode, TaskScheduler.FromCurrentSynchronizationContext());

            var taskCharge = new TaskChargeFilter("获取当天计费信息", new ChargeFilterCondition(
                new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0), DateTime.Now,
                DateTime.Now, _nodeTreeControl.CurrentNodeMid, CurrentPID), SetChargeData);
            TaskPool.AddTask(taskCharge, TaskScheduler.FromCurrentSynchronizationContext());
        }

        /// <summary>
        /// 更新电表树控件
        /// </summary>
        /// <param name="list">数据源</param>
        /// <param name="result">数据库访问结果，-1代表数据库连接错误</param>
        private void SetNodesData(Object list, int result)
        {
            if (result == -1) return;
            List<AmmeterInfo> nodeList = list as List<AmmeterInfo>;
            _nodeTreeControl.UpdateNodesData(nodeList);

            if (nodeTreeCtr.EditValue == null || nodeTreeCtr.EditValue == "")
                nodeTreeCtr.EditValue = nodeList[0].MID;
        }

        /// <summary>
        /// 用于控制 "实时" 或 "筛选" 电量信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// 筛选电量信息，发起一个新的任务，后台刷新数据
        /// </summary>
        private void GetFilteredData()
        {
            ResetUISource();
            var task = new TaskElectricityFilter("检索数据",
                new FilterCondition((DateTime) StartDatetimeCtr.EditValue, (DateTime) EndDatetimeCtr.EditValue,
                    _nodeTreeControl.CurrentNodeMid, CurrentPID), SetElectricityData, false);
            TaskPool.AddTask(task, TaskScheduler.FromCurrentSynchronizationContext());
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
            _electricityCharts.ResetChartDataSource();
            ResetGridDataDetailDataSource();
            ResetRealTimeGridDataSource();
        }

        private void SetChargeData(Object dataSource, int result)
        {
            if (result == -1) return;
            _chargeObjects = dataSource as Dictionary<string, object>;
            SetChargeGridandChartControls();
        }

        private void SetElectricityData(Object dataSource, int result)
        {
            if (result == -1) return;
            _electricityDataList = dataSource as List<ElectricityOriginalData>;
            UpdateRealtimeDataGrid();
            UpdateGridControlDetail();
            _electricityCharts.UpdateChart(_electricityDataList);
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


        private void RefreshBtn_ItemClick(object sender, ItemClickEventArgs e)
        {
            RefreshAllData();
        }

        private void nodetree_EditValueChanged(object sender, EventArgs e)
        {
            TreeListLookUpEdit nodetree = sender as TreeListLookUpEdit;
            AmmeterInfo ammeter = nodetree.GetSelectedDataRow() as AmmeterInfo;


            _nodeTreeControl.CurrentNodeMid = ammeter.MID;
            if (RealtimeCheckBtn.Down)
                RefreshAllData();
            else
                GetFilteredData();
        }

        private void barCheckItemCurrentDay_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (ChargeDateSelectBtn.Down)
                ChargeDateSelectBtn.Down = false;
            xtraTabControlBasicInfo.SelectedTabPageIndex = 1;
            barCheckItemCurrentDay.ItemAppearance.Normal.ForeColor = Color.Black;
            barCheckItemCurrentMonth.ItemAppearance.Normal.ForeColor = Color.Black;
            barCheckItemCurrentYear.ItemAppearance.Normal.ForeColor = Color.Black;
        }

        private void barCheckItemCurrentDay_CheckedChanged(object sender, ItemClickEventArgs e)
        {
            var taskCharge = new TaskChargeFilter("获取当天计费信息",
                new ChargeFilterCondition(
                    new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0), DateTime.Now,
                    DateTime.Now, _nodeTreeControl.CurrentNodeMid, CurrentPID), SetChargeData);
            TaskPool.AddTask(taskCharge, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void barCheckItemCurrentMonth_CheckedChanged(object sender, ItemClickEventArgs e)
        {
            var taskCharge = new TaskChargeFilter("获取本月计费信息",
                new ChargeFilterCondition(new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0), DateTime.Now,
                    DateTime.Now, _nodeTreeControl.CurrentNodeMid, CurrentPID), SetChargeData);
            TaskPool.AddTask(taskCharge, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void barCheckItemCurrentYear_CheckedChanged(object sender, ItemClickEventArgs e)
        {
            var taskCharge = new TaskChargeFilter("获取本年计费信息",
                new ChargeFilterCondition(new DateTime(DateTime.Now.Year, 1, 1, 0, 0, 0), DateTime.Now, DateTime.Now,
                    _nodeTreeControl.CurrentNodeMid, CurrentPID), SetChargeData);
            TaskPool.AddTask(taskCharge, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void barCheckItemCurrentMonth_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (ChargeDateSelectBtn.Down)
            {
                ChargeDateSelectBtn.Down = false;
            }
            xtraTabControlBasicInfo.SelectedTabPageIndex = 1;
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

            xtraTabControlBasicInfo.SelectedTabPageIndex = 1;
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
            xtraTabControlBasicInfo.SelectedTabPageIndex = 1;
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
                        _nodeTreeControl.CurrentNodeMid, CurrentPID), SetChargeData);
                TaskPool.AddTask(taskCharge, TaskScheduler.FromCurrentSynchronizationContext());
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

        /// <summary>
        /// 第一阶段计费并不包含铜铁损，所以在点击"第一阶段"标签时，为用户跳转至基本电费比例
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barButtonItemVoltage_ItemClick(object sender, ItemClickEventArgs e)
        {
            xtraTabControlBasicInfo.SelectedTabPageIndex = 0;
        }

        private void xtraTabControl4_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            if (xtraTabControl4.SelectedTabPageIndex == 0)
            {
                xtraTabControl2.SelectedTabPageIndex = 0;
                this.previewBar1.StandaloneBarDockControl = this.standaloneBarDockControl1;
                documentViewerBarManager1.DocumentViewer = documentViewer1;
                if (!_chargeObjects.ContainsKey("第一阶段")) return;
                _chargeControlsFirst.SetPropotionData(_chargeObjects["第一阶段"] as ChargeInfo);
            }
            else
            {
                xtraTabControl2.SelectedTabPageIndex = 1;
                this.previewBar1.StandaloneBarDockControl = this.standaloneBarDockControl2;
                documentViewerBarManager1.DocumentViewer = documentViewer2;
                if (!_chargeObjects.ContainsKey("第二阶段")) return;
                _chargeControlsSecond.SetPropotionData(_chargeObjects["第二阶段"] as ChargeInfo);
            }
        }

        /// <summary>
        /// 设定报表标题方法
        /// 在开启设定对话框时，首先读取当前标题
        /// 在点击确认按钮后，将修改后的标题设定给报表，并刷新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetupReportBtn_ItemClick(object sender, ItemClickEventArgs e)
        {
            ReportSetupForm form = new ReportSetupForm();
            DocumentViewer currentViewer = documentViewerBarManager1.DocumentViewer;
            int reportType = 0;
            if (currentViewer == documentViewer1)
            {
                form.SetReportContent((currentViewer.DocumentSource as XtraReport1).GetReportContent());
            }
            else
            {
                reportType = 1;
                form.SetReportContent((currentViewer.DocumentSource as XtraReport2).GetReportContent());
            }


            if (form.ShowDialog() == DialogResult.OK)
            {
                if (reportType == 0)
                    (currentViewer.DocumentSource as XtraReport1).SetReportContent(form.GetReportContent());


                else
                    (currentViewer.DocumentSource as XtraReport2).SetReportContent(form.GetReportContent());
            }
        }
    }
}