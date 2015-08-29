using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.ComponentModel;
using AracheTest.Tools;

namespace AracheTest.Data
{
    public class ChargeInfo
    {
        public ElectricityParameter Electricityparameter; //计算用数据参数
        public List<ElectricityPeriod> Electricitytimeprice; //计算用尖峰谷时段表
        //输入数据-------------------------------------------------------------------------------------------------------------
        public int PID; //项目ID
        public int MID; //表ID
        private DateTime StartTime; //测量开始时间
        private DateTime EndTime; //测量结束时间
        private bool CouplerLoss; //是否计算损耗
        //参数-----------------------------------------------------------------------------------------------------------------
        private DataTable DataSource; //数据表
        private DataTable electricityparameterTable ; //参数表
        private DataTable electricitytimeTable;//时段表
        //---------------------------------------------------------------------------------------------------------------------

        [CategoryAttribute("Outputs"), DescriptionAttribute("电表示数：总有功")]
        public double totalEnergy
        {
            get { return TotalEnergy; }
        }

        [CategoryAttribute("Outputs"), DescriptionAttribute("电表示数：有功尖峰")]
        public double spikeEnergy
        {
            get { return SpikeEnergy; }
        }

        [CategoryAttribute("Outputs"), DescriptionAttribute("电表示数：有功高峰")]
        public double peakEnergy
        {
            get { return PeakEnergy; }
        }

        [CategoryAttribute("Outputs"), DescriptionAttribute("电表示数：有功低谷")]
        public double valleyEnergy
        {
            get { return ValleyEnergy; }
        }

        [CategoryAttribute("Outputs"), DescriptionAttribute("电表示数：初始电量")]
        public double startEnergy
        {
            get { return StartEnergy; }
        }

        [CategoryAttribute("Outputs"), DescriptionAttribute("电表示数：初始时间")]
        public DateTime firstDataTime
        {
            get { return FirstDataTime; }
        }

        [CategoryAttribute("Outputs"), DescriptionAttribute("电表示数：结束电量")]
        public double endEnergy
        {
            get { return EndEnergy; }
        }

        [CategoryAttribute("Outputs"), DescriptionAttribute("电表示数：结束时间")]
        public DateTime lastDataTime
        {
            get { return LastDataTime; }
        }

        [CategoryAttribute("Outputs"), DescriptionAttribute("倍率")]
        public double powerRate
        {
            get { return PowerRate; }
        }

        [CategoryAttribute("Outputs"), DescriptionAttribute("电量总（KW.h）")]
        public double totalPower
        {
            get { return TotalPower; }
        }

        [CategoryAttribute("Outputs"), DescriptionAttribute("电量尖峰（KW.h）")]
        public double spikePower
        {
            get { return SpikePower; }
        }

        [CategoryAttribute("Outputs"), DescriptionAttribute("电量高峰（KW.h）")]
        public double peakPower
        {
            get { return PeakPower; }
        }

        [CategoryAttribute("Outputs"), DescriptionAttribute("电量低谷（KW.h）")]
        public double valleyPower
        {
            get { return ValleyPower; }
        }

        [CategoryAttribute("Outputs"), DescriptionAttribute("有功铜损")]
        public double activeCopperLoss
        {
            get { return ActiveCopperLoss; }
        }

        [CategoryAttribute("Outputs"), DescriptionAttribute("有功铁损")]
        public double activeCoreLoss
        {
            get { return ActiveCoreLoss; }
        }

        [CategoryAttribute("Outputs"), DescriptionAttribute("有功合计")]
        public double activeAll
        {
            get { return ActiveAll; }
        }

        [CategoryAttribute("Outputs"), DescriptionAttribute("无功铜损")]
        public double reactiveCopperLoss
        {
            get { return ReactiveCopperLoss; }
        }

        [CategoryAttribute("Outputs"), DescriptionAttribute("无功铁损")]
        public double reactiveCoreLoss
        {
            get { return ReactiveCoreLoss; }
        }

        [CategoryAttribute("Outputs"), DescriptionAttribute("无功合计")]
        public double reactiveAll
        {
            get { return ReactiveAll; }
        }

        [CategoryAttribute("Outputs"), DescriptionAttribute("尖峰电量 = 计费数量（尖 一般工商）")]
        public double activeSpike
        {
            get { return ActiveSpike; }
        }

        [CategoryAttribute("Outputs"), DescriptionAttribute("高峰电量 = 计费数量（峰 一般工商）")]
        public double activePeak
        {
            get { return ActivePeak; }
        }

        [CategoryAttribute("Outputs"), DescriptionAttribute("低谷电量 = 计费数量（谷 一般工商）")]
        public double activeValley
        {
            get { return ActiveValley; }
        }

        [CategoryAttribute("Outputs"), DescriptionAttribute("尖峰电价")]
        public double priceSpike
        {
            get { return PriceSpike; }
        }

        [CategoryAttribute("Outputs"), DescriptionAttribute("高峰电价")]
        public double pricePeak
        {
            get { return PricePeak; }
        }

        [CategoryAttribute("Outputs"), DescriptionAttribute("低谷电价")]
        public double priceValley
        {
            get { return PriceValley; }
        }

        [CategoryAttribute("Outputs"), DescriptionAttribute("尖峰金额")]
        public double costSpike
        {
            get { return CostSpike; }
        }

        [CategoryAttribute("Outputs"), DescriptionAttribute("高峰金额")]
        public double costPeak
        {
            get { return CostPeak; }
        }

        [CategoryAttribute("Outputs"), DescriptionAttribute("低谷金额")]
        public double costValley
        {
            get { return CostValley; }
        }

        [CategoryAttribute("Outputs"), DescriptionAttribute("总金额")]
        public double costAll
        {
            get { return CostAll; }
        }


        private double TotalHour; //总时间
        private double SpikeHour; //尖峰时间
        private double PeakHour; //高峰时间
        private double ValleyHour; //低谷时间

        private double StartEnergy; //初始电量
        private DateTime FirstDataTime; //初始时间

        private double EndEnergy; //结束电量
        private DateTime LastDataTime; //结束时间

        private double TotalEnergy; //有功总
        private double SpikeEnergy; //有功尖峰
        private double PeakEnergy; //有功高峰
        private double ValleyEnergy; //有功低谷

        public double ReactiveQI { get; set; }

        private double PowerRate; //倍率

        private double TotalPower; //电量总（KW.h）
        private double SpikePower; //电量尖峰（KW.h）
        private double PeakPower; //电量高峰（KW.h）
        private double ValleyPower; //电量低谷（KW.h）

        private double ActiveCopperLoss; //有功铜损
        private double ActiveCoreLoss; //有功铁损
        private double ActiveAll; //有功合计

        private double ReactiveCopperLoss; //无功铜损
        private double ReactiveCoreLoss; //无功铁损
        private double ReactiveAll; //无功合计 

        private double ActiveSpike; //尖峰电量（计费数量）
        private double ActivePeak; //高峰电量（计费数量）
        private double ActiveValley; //低谷电量（计费数量）

        private double PriceSpike; //尖峰电价
        private double PricePeak; //高峰电价
        private double PriceValley; //低谷电价

        private double CostSpike; //尖峰金额
        private double CostPeak; //高峰金额
        private double CostValley; //低谷金额
        private double CostAll; //总金额

        //方法-----------------------------------------------------------------------------------------------------------------


        private DataTable MeasureData(int pid, int mid, DateTime StartTime, DateTime EndTime)
        {
            return DBConnector.ExecuteSql("select * from normalreceiveddata where PID = " + pid + " and MID = " + mid +
                                          " and EventTime between '" + StartTime + "' and '" + EndTime +"'");
        }


        private double GetEnergyTotal(DataTable DataSource, DataTable electricityparameter, DataTable electricitytime,
            int line, out int TotalMinutes, out double EnergySpike, out int MinutesSpike, out double EnergyPeak,
            out int MinutesPeak, out double EnergyValley, out int MinutesValley)
        {
            #region 获取数据

            double newWPP = Convert.ToDouble(DataSource.Rows[line]["WPP"]); //电能数据（新）
            DateTime newEventTime = Convert.ToDateTime(DataSource.Rows[line]["EventTime"]); //电能数据时间（新）
            double oldWPP = Convert.ToDouble(DataSource.Rows[line - 1]["WPP"]); //电能数据（旧）
            DateTime oldEventTime = Convert.ToDateTime(DataSource.Rows[line - 1]["EventTime"]); //电能数据（旧）
            TimeSpan ts = newEventTime - oldEventTime; //数据间隔时间

            #endregion

            int TimeSpike = 0; //尖峰时长（分钟）
            int TimePeak = 0; //高峰时长（分钟）    
            int TimeValley = 0; //低谷时长（分钟）

            #region 计算尖峰谷时间

            int myWholeHour;
            bool IsHour = false;
            if (ts.TotalMinutes > (60 - oldEventTime.Minute))
            {
                IsHour = true;
            }

            if (IsHour)
            {
                myWholeHour = ((int) ts.TotalMinutes - (60 - oldEventTime.Minute))/60;


                switch (electricitytime.Rows[oldEventTime.Hour]["ElectricityType"].ToString())
                {
                    case "1":
                        //digu
                        TimeValley = 60 - oldEventTime.Minute;
                        break;
                    case "2":
                        //gaofeng
                        TimePeak = 60 - oldEventTime.Minute;
                        break;
                    case "3":
                        //jianfeng
                        TimeSpike = 60 - oldEventTime.Minute;
                        break;
                }


                for (int i = 0; i < myWholeHour; i++)
                {
                    oldEventTime.AddHours(1);
                    switch (electricitytime.Rows[oldEventTime.Hour]["ElectricityType"].ToString())
                    {
                        case "1":
                            //digu
                            TimeValley = TimeValley + 60;
                            break;
                        case "2":
                            //gaofeng
                            TimePeak = TimePeak + 60;
                            break;
                        case "3":
                            //jianfeng
                            TimeSpike = TimeSpike + 60;
                            break;
                    }
                }

                switch (electricitytime.Rows[oldEventTime.Hour]["ElectricityType"].ToString())
                {
                    case "1":
                        //digu
                        TimeValley = TimeValley + newEventTime.Minute;
                        break;
                    case "2":
                        //gaofeng
                        TimePeak = TimePeak + newEventTime.Minute;
                        break;
                    case "3":
                        //jianfeng
                        TimeSpike = TimeSpike + newEventTime.Minute;
                        break;
                }
            }
            else
            {
                switch (electricitytime.Rows[oldEventTime.Hour]["ElectricityType"].ToString())
                {
                    case "1":
                        //digu
                        TimeValley = (int) ts.TotalMinutes;
                        break;
                    case "2":
                        //gaofeng
                        TimePeak = (int) ts.TotalMinutes;
                        break;
                    case "3":
                        //jianfeng
                        TimeSpike = (int) ts.TotalMinutes;
                        break;
                }
            }

            #endregion

            MinutesSpike = TimeSpike; //尖峰时长（分钟）
            MinutesPeak = TimePeak; //高峰时长（分钟）  
            MinutesValley = TimeValley; //低谷时长（分钟）
            TotalMinutes = TimeSpike + TimePeak + TimeValley; //总共时长（分钟）


            double EnergyTotal = newWPP - oldWPP; //总电量
            //测试数据发送频率< 1min,正常运行无需以下代码--------------------------
            if (TotalMinutes == 0) //-------------------------
            {
                //-------------------------
                EnergySpike = EnergyTotal/3; //尖峰电量-----------------
                EnergyPeak = EnergyTotal/3; //高峰电量-----------------
                EnergyValley = EnergyTotal/3; //低谷电量-----------------                       
            } //--------------------------------------------------------------------
            else
            {
                EnergySpike = EnergyTotal*TimeSpike/TotalMinutes; //尖峰电量
                EnergyPeak = EnergyTotal*TimePeak/TotalMinutes; //高峰电量
                EnergyValley = EnergyTotal*TimeValley/TotalMinutes; //低谷电量
            }


            return EnergyTotal;
        }

        private void CalBaseData()
        {
            StartEnergy = Convert.ToDouble(DataSource.Rows[0]["WPP"]);
            FirstDataTime = Convert.ToDateTime(DataSource.Rows[0]["EventTime"]);

            EndEnergy = Convert.ToDouble(DataSource.Rows[DataSource.Rows.Count - 1]["WPP"]);
            LastDataTime = Convert.ToDateTime(DataSource.Rows[DataSource.Rows.Count - 1]["EventTime"]);

            #region 临时变量

            //临时变量----------------------------------------------------------------------------------------------------------------
            int TotalMinutes = 0;
            int MinutesSpike = 0;
            int MinutesPeak = 0;
            int MinutesValley = 0;

            int AllTotalMinutes = 0;
            int AllMinutesSpike = 0;
            int AllMinutesPeak = 0;
            int AllMinutesValley = 0;

            double EnergyTotal = 0;
            double EnergySpike = 0;
            double EnergyPeak = 0;
            double EnergyValley = 0;

            double AllEnergyTotal = 0;
            double AllEnergySpike = 0;
            double AllEnergyPeak = 0;
            double AllEnergyValley = 0;
            //------------------------------------------------------------------------------------------------------------------------

            #endregion

            int CountLine = DataSource.Rows.Count;
            for (int i = 1; i < CountLine; i++)
            {
                EnergyTotal = GetEnergyTotal(DataSource, electricityparameterTable, electricitytimeTable, i,
                    out TotalMinutes,
                    out EnergySpike, out MinutesSpike, out EnergyPeak, out MinutesPeak, out EnergyValley,
                    out MinutesValley);

                AllTotalMinutes = AllTotalMinutes + TotalMinutes; //总用电时间（分钟）
                AllMinutesSpike = AllMinutesSpike + MinutesSpike; //尖峰用电时间（分钟）
                AllMinutesPeak = AllMinutesPeak + MinutesPeak; //高峰用电时间（分钟）
                AllMinutesValley = AllMinutesValley + MinutesValley; //低谷用电时间（分钟）

                AllEnergyTotal = AllEnergyTotal + EnergyTotal; //总电量
                AllEnergySpike = AllEnergySpike + EnergySpike; //尖峰电量
                AllEnergyPeak = AllEnergyPeak + EnergyPeak; //高峰电量
                AllEnergyValley = AllEnergyValley + EnergyValley; //低谷电量
            }

            TotalHour = AllTotalMinutes/60;
            SpikeHour = AllMinutesSpike/60;
            PeakHour = AllMinutesPeak/60;
            ValleyHour = AllMinutesValley/60;

            TotalEnergy = AllEnergyTotal;
            SpikeEnergy = AllEnergySpike;
            PeakEnergy = AllEnergyPeak;
            ValleyEnergy = AllEnergyValley;
        }

        private void CalAdvanceData(bool IsCouplerLoss)
        {
            PowerRate = Convert.ToDouble(electricityparameterTable.Rows[0]["PowerRate"]); //倍率

            PriceSpike = Convert.ToDouble(electricityparameterTable.Rows[0]["PriceSpike"]); //尖峰电价
            PricePeak = Convert.ToDouble(electricityparameterTable.Rows[0]["PricePeak"]); //高峰电价
            PriceValley = Convert.ToDouble(electricityparameterTable.Rows[0]["PriceValley"]); //低谷电价

            TotalPower = TotalEnergy*PowerRate;
            SpikePower = SpikeEnergy*PowerRate;
            PeakPower = PeakEnergy*PowerRate;
            ValleyPower = ValleyEnergy*PowerRate;

            double newWQP = Convert.ToDouble(DataSource.Rows[DataSource.Rows.Count - 1]["WQP"]); //无功数据（新）
            double oldWQP = Convert.ToDouble(DataSource.Rows[0]["WQP"]); //无功数据（旧）
            double reactiveQI = newWQP - oldWQP; //无功电量QI象限
            ReactiveQI = reactiveQI;

            if (IsCouplerLoss)
            {
                double CoActiveCopperLoss = Convert.ToDouble(electricityparameterTable.Rows[0]["CoActiveCopperLoss"]);
                //有功铜损系数
                double CoReactiveCopperLoss = Convert.ToDouble(electricityparameterTable.Rows[0]["CoReactiveCopperLoss"]);
                //无功铜损系数

                double CoActiveNoloadLoss = Convert.ToDouble(electricityparameterTable.Rows[0]["CoActiveNoloadLoss"]);
                //有功空载损耗
                double CoReactiveNoloadLoss = Convert.ToDouble(electricityparameterTable.Rows[0]["CoReactiveNoloadLoss"]);
                //无功空载损耗
                //----------------------------------------------------------------------------------------------------------------------------
                //                     有功铜损系数       总电能
                ActiveCopperLoss = CoActiveCopperLoss*TotalPower; //有功铜损                  
                //                   有功空载损耗      运行时间
                ActiveCoreLoss = CoActiveNoloadLoss*TotalHour; //有功铁损
                ActiveAll = TotalPower + ActiveCopperLoss + ActiveCoreLoss; //有功合计

                //                         有功铜损           无功铜损系数
                ReactiveCopperLoss = ActiveCopperLoss*CoReactiveCopperLoss; //无功铜损
                //                       无功空载损耗       运行时间
                ReactiveCoreLoss = CoReactiveNoloadLoss*TotalHour; //无功铁损
                ReactiveAll = ReactiveQI + ReactiveCopperLoss + ReactiveCoreLoss; //无功合计 

                #region 计算尖峰谷所占一天的时段

                int HourSpikeOneDay = 0;
                int HourPeakOneDay = 0;
                int HourValleyOneDay = 0;
                for (int i = 0; i < electricitytimeTable.Rows.Count; i++)
                {
                    switch (electricitytimeTable.Rows[i]["ElectricityType"].ToString())
                    {
                        case "1":
                            HourValleyOneDay = HourValleyOneDay + 1;
                            break;
                        case "2":
                            HourPeakOneDay = HourPeakOneDay + 1;
                            break;
                        case "3":
                            HourValleyOneDay = HourValleyOneDay + 1;
                            break;
                    }
                }

                #endregion

                ActiveSpike = SpikePower + (ActiveCopperLoss*SpikePower/TotalPower) +
                              (ActiveCoreLoss*HourSpikeOneDay/24);
                //尖峰电量（计费数量） = 尖峰电量 + ( 有功铜损 * 尖峰电量 / 总电量 ) + ( 有功铁损 * 一天的尖峰时间 / 一天总时间)
                ActivePeak = PeakPower + (ActiveCopperLoss*PeakPower/TotalPower) + (ActiveCoreLoss*HourPeakOneDay/24);
                //高峰电量（计费数量） = 尖峰电量 + ( 有功铜损 * 尖峰电量 / 总电量 ) + ( 有功铁损 * 一天的尖峰时间 / 一天总时间)
                ActiveValley = ValleyPower + (ActiveCopperLoss*ValleyPower/TotalPower) +
                               (ActiveCoreLoss*HourValleyOneDay/24);
                //低谷电量（计费数量）= 尖峰电量 + ( 有功铜损 * 尖峰电量 / 总电量 ) + ( 有功铁损 * 一天的尖峰时间 / 一天总时间)
            }
            else
            {
                ActiveCopperLoss = 0; //有功铜损
                ActiveCoreLoss = 0; //有功铁损
                ActiveAll = TotalPower; //有功合计
                ReactiveCopperLoss = 0; //无功铜损
                ReactiveCoreLoss = 0; //无功铁损
                ReactiveAll = 0; //无功合计 
                ActiveSpike = SpikePower; //尖峰电量（计费数量）
                ActivePeak = PeakPower; //高峰电量（计费数量）
                ActiveValley = ValleyPower; //低谷电量（计费数量）
            }

            CostSpike = ActiveSpike*PriceSpike; //尖峰金额
            CostPeak = ActivePeak*PricePeak; //高峰金额
            CostValley = ActiveValley*PriceValley; //低谷金额
            CostAll = CostSpike + CostPeak + CostValley; //总金额
        }

        public void Calculating(int pid, int mid, DateTime startTime, DateTime endTime, bool couplerLoss)
        {
            if (startTime == endTime) return;
            PID = pid;
            MID = mid;
            StartTime = startTime;
            EndTime = endTime;
            CouplerLoss = couplerLoss;
            DataSource = MeasureData(PID, MID, StartTime, EndTime);

            electricityparameterTable = DBConnector.ExecuteSql(string.Format("select * from electricityparameter"));
            Electricityparameter = new ElectricityParameter(electricityparameterTable.Rows[0]);

            electricitytimeTable = DBConnector.ExecuteSql(string.Format("select * from electricitytime"));
            Electricitytimeprice = new List<ElectricityPeriod>();
            Electricitytimeprice.AddRange(from DataRow row in electricitytimeTable.Rows
                select new ElectricityPeriod(row));

            CalBaseData();
            CalAdvanceData(couplerLoss);
        }
    }
}