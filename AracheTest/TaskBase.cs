using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AracheTest.Data;
using DevExpress.DashboardCommon;
using DevExpress.XtraBars;
using DevExpress.XtraPrinting.Native;

namespace AracheTest
{
    public class ConditionBase
    {}

    public class FilterCondition : ConditionBase
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int MID { get; set; }
        public int NodeID { get; set; }
        public FilterCondition(DateTime startTime, DateTime endTime, int MID) : base()
        {
            StartTime = startTime;
            EndTime = endTime;
            NodeID = MID;
            this.MID = NodeID;
        }
    }

    public class ChargeFilterCondition : FilterCondition
    {
        /// <summary>
        /// 只用于计算计费信息，第一阶段需要 StartTime 和 SecondTime, 第二阶段需要 SecondTime 和 EndTime
        /// </summary>
        public DateTime SecondTime { get; set; }

        public ChargeFilterCondition(DateTime StartTime, DateTime SecondTime, DateTime EndTime, int NodeID)
            : base(StartTime, EndTime, NodeID)
        {
            this.SecondTime = SecondTime;
        }
    }

    public interface ITask
    {
        void Run();
    }

    public class TaskBase:ITask
    {
        public delegate void updateDataDelegate(Object dataSource);

        protected ConditionBase Condition { get; set; }

        protected String Name { get; set; }
        
        protected DateTime Time { get; set; }
        
        protected Object DBData = null;

        protected updateDataDelegate _updateDataData;

        public bool IsFinished { get; set; }

        
        public virtual void Run()
        {
             
        }

        public void UpdateDataByDelegate()
        {
            _updateDataData(DBData);
            IsFinished = true;
        }

        public TaskBase(String name, ConditionBase condition, updateDataDelegate returnFuc)
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
        public TaskElectricityFilter(String name, FilterCondition condition, updateDataDelegate returnFuc, Boolean IsRealtime)
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

        private void GetRealtimeData(){
            DBData = DataBase.GetRealTimeData(_condition.MID);
        }

        public void GetFilteredData()
        {
            DBData = DataBase.GetDatetimeFilteredData(_condition.StartTime, _condition.EndTime, _condition.MID);
        }
    }

    public class TaskFetchNodes : TaskBase
    {
        public TaskFetchNodes(String name, ConditionBase condition, updateDataDelegate returnFuc)
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
        }
    }

    public class TaskChargeFilter : TaskBase
    {
        private ChargeFilterCondition _condition;
        public TaskChargeFilter(String name, ChargeFilterCondition condition, updateDataDelegate returnFuc)
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
            return charge.FirstMeasureData(_condition.StartTime, _condition.SecondTime, _condition.EndTime, _condition.MID);
        }

        private ChargeInfo getSecondCharge()
        {
            CalculateChargeClass charge = new CalculateChargeClass();
            return charge.SecondMeasureData(_condition.StartTime, _condition.SecondTime, _condition.EndTime,
                _condition.MID);
        }
    }
}