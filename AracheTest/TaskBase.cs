using System;
using System.Collections.Generic;
using System.Data;
using AracheTest.Data;
using AracheTest.Tools;


namespace AracheTest
{
    /// <summary>
    /// 基础条件类，将PID作为检索的必要条件
    /// </summary>
    public class ConditionBase
    {
        public int PID { get; set; }

        public ConditionBase(int PID)
        {
            this.PID = PID;
        }
    }

    /// <summary>
    /// 电量检索类，封装开始时间、结束时间和MID，可用于封装电量的检索条件
    /// </summary>
    public class FilterCondition : ConditionBase
    {
        public DateTime StartTime { get; private set; }
        public DateTime EndTime { get; private set; }
        public int MID { get; private set; }


        public FilterCondition(DateTime startTime, DateTime endTime, int mid, int pid) : base(pid)
        {
            StartTime = startTime;
            EndTime = endTime;
            MID = mid;
            PID = pid;
        }
    }

    /// <summary>
    /// 计费检索类，封装中间二次计费时间，可用于封装计费的检索条件
    /// 第一阶段需要 StartTime 和 MiddleTime, 第二阶段需要 StartTime 和 EndTime
    /// </summary>
    public class ChargeFilterCondition : FilterCondition
    {
        public DateTime MiddleTime { get; private set; }

        public ChargeFilterCondition(DateTime startTime, DateTime middleTime, DateTime endTime, int mid, int pid)
            : base(startTime, endTime, mid, pid)
        {
            MiddleTime = middleTime;
        }
    }

    /// <summary>
    /// 任务接口，必须实现Run方法，以供TaskPool类调用
    /// </summary>
    public interface ITask
    {
        void Run();
    }

    public class TaskBase : ITask
    {
        public delegate void UpdateDataDelegate(Object dataSource, int result);

        protected ConditionBase Condition { get; set; }

        protected String Name { get; set; }

        protected DateTime Time { get; set; }

        protected Object DBData { get; set; }

        protected readonly UpdateDataDelegate UpdateDataData;

        public int result { get; set; }


        public virtual void Run()
        {
        }

        public void UpdateDataByDelegate()
        {
            UpdateDataData(DBData, result);
        }

        /// <summary>
        /// 初始化基础任务类
        /// </summary>
        /// <param name="name">任务名称</param>
        /// <param name="condition">任务条件，一般用于检索任务</param>
        /// <param name="returnFuc">任务结束后的返回函数</param>
        public TaskBase(String name, ConditionBase condition, UpdateDataDelegate returnFuc)
        {
            Name = name;
            UpdateDataData = returnFuc;
            Condition = condition;
            result = 0;
            Time = DateTime.Now;
        }
    }

    public class TaskElectricityFilter : TaskBase
    {
        private Boolean _isRealtime;
        private FilterCondition _condition;

        public TaskElectricityFilter(String name, FilterCondition condition, UpdateDataDelegate returnFuc,
            Boolean isRealtime)
            : base(name, condition, returnFuc)
        {
            this._condition = condition;
            this._isRealtime = isRealtime;
        }

        public override void Run()
        {
            if (_isRealtime)
                GetRealtimeData();
            else GetFilteredData();
        }

        private void GetRealtimeData()
        {
            DBData = DataBase.GetRealTimeData(_condition.MID, _condition.PID);
            if (DBData == null)
            {
                result = -1;
                LogHelper.WriteLog(typeof(TaskElectricityFilter), "无法读取实时电量数据");
            }
            result = (DBData as List<ElectricityOriginalData>).Count;
        }

        public void GetFilteredData()
        {
            DBData = DataBase.GetDatetimeFilteredData(_condition.StartTime, _condition.EndTime, _condition.MID,
                _condition.PID);
            if (DBData == null)
            {
                result = -1;
                LogHelper.WriteLog(typeof(TaskElectricityFilter), "无法检索相关电量数据");
            }
            result = (DBData as List<ElectricityOriginalData>).Count;
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
            DBData = DataBase.GetAmmeterInfo(Condition.PID);
            if (DBData == null)
            {
                result = -1;
                LogHelper.WriteLog(typeof(TaskFetchNodes), "无法读取电表信息");
            }
            result = (DBData as List<AmmeterInfo>).Count;
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
            returnData.Add("第一阶段", GetFirstCharge());
            returnData.Add("第二阶段", GetSecondCharge());
            DBData = returnData;
        }

        private List<ElectricityParameter> getElectricityParameter()
        {
            Object resultTemp = DataBase.GetElectricityparameter();
            if (resultTemp == null)
            {
                result = -1;
                LogHelper.WriteLog(typeof(TaskChargeFilter), "无法读取电量参数信息");
            }
            result = (resultTemp as List<ElectricityParameter>).Count;
            return (List<ElectricityParameter>) resultTemp;
        }

        private List<ElectricityPeriod> getElectricityPeriodInfo()
        {
            Object resultTemp = DataBase.GetElectricityPeriods();
            if (resultTemp == null)
            {
                result = -1;
                LogHelper.WriteLog(typeof(TaskChargeFilter), "无法读取电费时段信息");
            }
            result = (resultTemp as List<ElectricityPeriod>).Count;
            return (List<ElectricityPeriod>) resultTemp;
        }

        private ChargeInfo GetFirstCharge()
        {
            ChargeInfo charge = new ChargeInfo();
            charge.Calculating(_condition.PID, _condition.MID, _condition.StartTime, _condition.MiddleTime, false);
            return charge;
        }

        private ChargeInfo GetSecondCharge()
        {
            ChargeInfo charge = new ChargeInfo();
            charge.Calculating(_condition.PID, _condition.MID, _condition.StartTime, _condition.EndTime, true);
            return charge;
        }
    }
}