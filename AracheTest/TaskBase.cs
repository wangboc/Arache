using System;
using System.Collections.Generic;
using System.Data;
using AracheTest.Data;


namespace AracheTest
{
    public class ConditionBase
    {
    }

    public class FilterCondition : ConditionBase
    {
        public DateTime StartTime { get; private set; }
        public DateTime EndTime { get; private set; }
        public int Mid { get; private set; }
        public int NodeId { get; set; }

        public FilterCondition(DateTime startTime, DateTime endTime, int mid) 
        {
            StartTime = startTime;
            EndTime = endTime;
            NodeId = mid;
            Mid = NodeId;
        }
    }

    public class ChargeFilterCondition : FilterCondition
    {
        /// <summary>
        /// 只用于计算计费信息，第一阶段需要 StartTime 和 SecondTime, 第二阶段需要 SecondTime 和 EndTime
        /// </summary>
        public DateTime SecondTime { get; private set; }

        public ChargeFilterCondition(DateTime startTime, DateTime secondTime, DateTime endTime, int nodeId)
            : base(startTime, endTime, nodeId)
        {
            SecondTime = secondTime;
        }
    }

    public interface ITask
    {
        void Run();
    }

    public class TaskBase : ITask
    {
        public delegate void UpdateDataDelegate(Object dataSource);

        protected ConditionBase Condition { get; set; }

        protected String Name { get; set; }

        protected DateTime Time { get; set; }

        protected Object DBData = null;

        protected UpdateDataDelegate _updateDataData;

        public bool IsFinished { get; set; }


        public virtual void Run()
        {
        }

        public void UpdateDataByDelegate()
        {
            _updateDataData(DBData);
            IsFinished = true;
        }

        public TaskBase(String name, ConditionBase condition, UpdateDataDelegate returnFuc)
        {
            this.Name = name;
            this._updateDataData = returnFuc;
            this.Condition = condition;
            this.IsFinished = false;
            this.Time = DateTime.Now;
        }
    }

    public class TaskElectricityFilter : TaskBase
    {
        private Boolean _isRealtime;
        private FilterCondition _condition;

        public TaskElectricityFilter(String name, FilterCondition condition, UpdateDataDelegate returnFuc,
            Boolean IsRealtime)
            : base(name, condition, returnFuc)
        {
            this._condition = condition;
            this._isRealtime = IsRealtime;
        }

        public override void Run()
        {
            if (_isRealtime)
                GetRealtimeData();
            else GetFilteredData();
        }

        private void GetRealtimeData()
        {
            DBData = DataBase.GetRealTimeData(_condition.Mid);
        }

        public void GetFilteredData()
        {
            DBData = DataBase.GetDatetimeFilteredData(_condition.StartTime, _condition.EndTime, _condition.Mid);
        }
    }

    public class TaskFetchNodes : TaskBase
    {
        public TaskFetchNodes(String name, ConditionBase condition, UpdateDataDelegate returnFuc)
            : base(name, condition, returnFuc)
        {
        }

        public override void Run()
        {
            FetchNodesInfo();
        }

        private void FetchNodesInfo()
        {
            DBData = DataBase.GetAllNodeInfo();
            List<NodeInfo> data = DBData as List<NodeInfo>;
            int nodeTotal = data.Count;
            for (int i = 0; i < nodeTotal; i++)
            {
                List<Correspondnode> MIDs = DataBase.GetCorrespondMid(data[i].NodeID);

                if (MIDs != null && MIDs.Count > 0)
                {
                    DataTable dt = new DataTable();
                    dt.Columns.Add("NodeID");
                    dt.Columns.Add("PID");
                    dt.Columns.Add("ParentID");
                    dt.Columns.Add("Name");
                    dt.Columns.Add("MID");

                    foreach (Correspondnode correspondnode in MIDs)
                    {
                        DataRow row = dt.NewRow();
                        long tick = DateTime.Now.Ticks;
                        //Random r = new Random((int)(tick & 0xffffffffL) | (int)(tick >> 32));

                        //产生唯一NodeID值，在电表节点中，该NodeID没用，只是用来生成树状结构。
                        row["NodeID"] = 10000.ToString() + correspondnode.NodeID.ToString() +
                                        correspondnode.MID.ToString();
                        row["PID"] = 1;
                        row["ParentID"] = correspondnode.NodeID;
                        row["Name"] = "电表: " + correspondnode.MID;
                        row["MID"] = correspondnode.MID;
                        dt.Rows.Add(row);
                        NodeInfo node = new NodeInfo(row);
                        node.MID.Add(correspondnode.MID);
                        node.IsNode = false;
                        data.Add(node);
                    }
                }
            }
            data.Reverse();
        }
    }

    public class TaskChargeFilter : TaskBase
    {
        private ChargeFilterCondition _condition;

        public TaskChargeFilter(String name, ChargeFilterCondition condition, UpdateDataDelegate returnFuc)
            : base(name, condition, returnFuc)
        {
            _condition = condition;
        }

        public override void Run()
        {
            Dictionary<string, Object> returnData = new Dictionary<string, Object>();
            returnData.Add("电量参数", getElectricityParameter());
            returnData.Add("时段信息", getElectricityPeriodInfo());
            returnData.Add("第一阶段", getFirstCharge());
            returnData.Add("第二阶段", getSecondCharge());

            DBData = returnData;
        }

        private List<ElectricityParameter> getElectricityParameter()
        {
            return DataBase.GetElectricityparameter();
        }

        private List<ElectricityPeriod> getElectricityPeriodInfo()
        {
            return DataBase.GetElectricityPeriods();
        }

        private ChargeInfo getFirstCharge()
        {
            CalculateChargeClass charge = new CalculateChargeClass();
            return charge.FirstMeasureData(_condition.StartTime, _condition.SecondTime, _condition.EndTime,
                _condition.Mid);
        }

        private ChargeInfo getSecondCharge()
        {
            CalculateChargeClass charge = new CalculateChargeClass();
            return charge.SecondMeasureData(_condition.StartTime, _condition.SecondTime, _condition.EndTime,
                _condition.Mid);
        }
    }
}