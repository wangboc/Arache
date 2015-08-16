using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DevExpress.DashboardCommon.Native;

namespace AracheTest.Data
{
    public class ChargeInfo
    {
        private DateTime startDateTime;
        private DateTime firstDateTime;
        private DateTime secondDatetime;
        
        public bool IsEffective = false;
       
        
     //输入数据---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        //需要计算用的数据源，要求按时间顺序顺序排列（目前只能使用低压表数据进行计算，高压表没有电能数据，后续更换高级表，可以支持高压数据）
        public List<ElectricityOriginalData> DataSourceFirstMeasure; //起始时间到第一次测量计算时间，不计算铜损与铁损
        public List<ElectricityOriginalData> DataSourceSecondMeasure; //第一次测量时间到第二次测量时间，计算铜损与铁损，一次计费可以不用
        public int MID;

        public ElectricityParameter electricityparameter ; //计算用数据参数
        public List<ElectricityPeriod> electricitytimeprice; //计算用尖峰谷时段表

        public ChargeInfo(DateTime startDatetime, DateTime firstDateTime, DateTime secondDatetime, int MID)
        {
            this.startDateTime = startDatetime;
            this.firstDateTime = firstDateTime;
            this.secondDatetime = secondDatetime;
            this.MID = MID;
            GetElectricityPeriodInfo();
            GetElectricityparameter();
            GetDataSourceFirstMeasure();
            GetDataSourceSecondMeasure();
        }

        private void GetDataSourceFirstMeasure()
        {

            List<ElectricityOriginalData> list = DataBase.GetDatetimeFilteredData(startDateTime, firstDateTime, MID);
            if (list != null && list.Count > 0)
                DataSourceFirstMeasure = list;
        }

        private void GetDataSourceSecondMeasure()
        {
            List<ElectricityOriginalData> list = DataBase.GetDatetimeFilteredData(firstDateTime, secondDatetime, MID);
            if (list != null && list.Count > 0)
                DataSourceSecondMeasure = list;
        }

        private void GetElectricityparameter()
        {
            List<ElectricityParameter> list = DataBase.GetElectricityparameter();
            if (list != null && list.Count > 0)
                electricityparameter = list[0];
        }

        private void GetElectricityPeriodInfo()
        {
            List<ElectricityPeriod> list = DataBase.GetElectricityPeriods();
            if (list != null && list.Count > 0)
                electricitytimeprice = list;
        }


        //输出数据---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public double TotalHour; //总时间
        public double SpikeHour; //尖峰时间
        public double PeakHour; //高峰时间
        public double ValleyHour; //低谷时间
        //一次测量值--------------------------------------------------------------------------------------------
        public double EnergyTotal; //一次测量总有功值（电表）
        public double EnergySpike; //一次测量尖峰有功值（电表）
        public double EnergyPeak; //一次测量高峰有功值（电表）
        public double EnergyValley; //一次测量低谷有功值（电表）
        //二次测量值--------------------------------------------------------------------------------------------
        public double EnergyTotal2; //二次测量总有功值（电表）
        public double EnergySpike2; //二次测量尖峰有功值（电表）
        public double EnergyPeak2; //二次测量高峰有功值（电表）
        public double EnergyValley2; //二次测量低谷有功值（电表）
        //------------------------------------------------------------------------------------------------------



        public double CostAll; //总金额
        public double CostSpike; //尖峰金额
        public double CostPeak; //高峰金额
        public double CostValley; //低谷金额
        //一次测量值--------------------------------------------------------------------------------------------
        public double PowerTotal; //总电量（KW.h）
        public double PowerSpike; //尖峰电量（KW.h）
        public double PowerPeak; //高峰电量（KW.h）
        public double PowerValley; //低谷电量（KW.h）
        //二次测量值--------------------------------------------------------------------------------------------
        public double PowerTotal2; //总电量（KW.h）
        public double PowerSpike2; //尖峰电量（KW.h）
        public double PowerPeak2; //高峰电量（KW.h）
        public double PowerValley2; //低谷电量（KW.h）
        //------------------------------------------------------------------------------------------------------

        public double ReactiveEnergyQI; //无功（QI象限）

        public double PriceSpike; //尖峰电价
        public double PricePeak; //高峰电价
        public double PriceValley; //低谷电价

        public double WPPNew; //一次测量示数 （有功总）
        public DateTime EventTimeNew; //一次测量时间  datetime

        public double WPPNew2; //二次测量示数 （有功总）
        public DateTime EventTimeNew2; //二次测量时间  datetime

        public double WPPOld; //初始测量示数 （有功总）
        public DateTime EventTimeOld; //初始测量时间  datetime

        public double ActiveCopperLoss; //有功铜损
        public double ActiveCoreLoss; //有功铁损
        public double ActiveAll; //有功合计

        public double ReactiveCopperLoss; //无功铜损
        public double ReactiveCoreLoss; //无功铁损
        public double ReactiveAll; //无功合计
    }

    public class CalculateChargeClass
    {
        private double GetEnergyTotal(List<ElectricityOriginalData> dataSource,
            ElectricityParameter electricityparameter, List<ElectricityPeriod> electricitytime, int line,
            out int TotalMinutes, out double EnergySpike, out int MinutesSpike, out double EnergyPeak,
            out int MinutesPeak, out double EnergyValley, out int MinutesValley)
        {
            #region 获取数据

            double newWPP = dataSource[line].WPP; //电能数据（新）
            DateTime newEventTime = dataSource[line].EventTime; //电能数据时间（新）
            double oldWPP = dataSource[line - 1].WPP; //电能数据（旧）
            DateTime oldEventTime = dataSource[line - 1].EventTime; //电能数据（旧）
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


                switch (electricitytime[oldEventTime.Hour].ElectricityType)
                {
                    case 1:
                        //digu
                        TimeValley = 60 - oldEventTime.Minute;
                        break;
                    case 2:
                        //gaofeng
                        TimePeak = 60 - oldEventTime.Minute;
                        break;
                    case 3:
                        //jianfeng
                        TimeSpike = 60 - oldEventTime.Minute;
                        break;
                }


                for (int i = 0; i < myWholeHour; i++)
                {
                    oldEventTime = oldEventTime.AddHours(1);
                    switch (electricitytime[oldEventTime.Hour].ElectricityType)
                    {
                        case 1:
                            //digu
                            TimeValley = TimeValley + 60;
                            break;
                        case 2:
                            //gaofeng
                            TimePeak = TimePeak + 60;
                            break;
                        case 3:
                            //jianfeng
                            TimeSpike = TimeSpike + 60;
                            break;
                    }
                }

                switch (electricitytime[oldEventTime.Hour].ElectricityType)
                {
                    case 1:
                        //digu
                        TimeValley = TimeValley + newEventTime.Minute;
                        break;
                    case 2:
                        //gaofeng
                        TimePeak = TimePeak + newEventTime.Minute;
                        break;
                    case 3:
                        //jianfeng
                        TimeSpike = TimeSpike + newEventTime.Minute;
                        break;
                }
            }
            else
            {
                switch (electricitytime[oldEventTime.Hour].ElectricityType)
                {
                    case 1:
                        //digu
                        TimeValley = (int) ts.TotalMinutes;
                        break;
                    case 2:
                        //gaofeng
                        TimePeak = (int) ts.TotalMinutes;
                        break;
                    case 3:
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

        public ChargeInfo FirstMeasureData(DateTime startDateTime, DateTime firstDateTime, DateTime secondDateTime,
            int mid)
        {
            #region 变量

            //总体数据-------------------------------------------------


            int AllMinutesSpike = 0;
            int AllMinutesPeak = 0;
            int AllMinutesValley = 0;
            int AllTotalMinutes = 0;

            double AllEnergySpike = 0;
            double AllEnergyPeak = 0;
            double AllEnergyValley = 0;
            double AllEnergyTotal = 0;

            double AllCostSpike = 0;
            double AllCostPeak = 0;
            double AllCostValley = 0;
            double AllCostAll = 0;


            //单次测量数据----------------------------------------------
            double EnergyTotal; //总电能值（电表）
            double EnergySpike; //尖峰电能值（电表）
            double EnergyPeak; //高峰电能值（电表）
            double EnergyValley; //低谷电能值（电表）

            int TotalMinutes; //总时长(分钟)
            int MinutesSpike; //尖峰时长（分钟）
            int MinutesPeak; //高峰时长（分钟）
            int MinutesValley; //低谷时长（分钟）

            #endregion

            #region 计算


            ChargeInfo StructData = new ChargeInfo(startDateTime, firstDateTime, secondDateTime, mid);
            if (StructData.DataSourceFirstMeasure == null)
                return null;
            int CountLine = StructData.DataSourceFirstMeasure.Count;
            for (int i = 1; i < CountLine; i++)
            {
                EnergyTotal = GetEnergyTotal(StructData.DataSourceFirstMeasure, StructData.electricityparameter,
                    StructData.electricitytimeprice, i, out TotalMinutes, out EnergySpike, out MinutesSpike,
                    out EnergyPeak, out MinutesPeak, out EnergyValley, out MinutesValley);
                AllTotalMinutes = AllTotalMinutes + TotalMinutes; //总用电时间（分钟）
                AllMinutesSpike = AllMinutesSpike + MinutesSpike; //尖峰用电时间（分钟）
                AllMinutesPeak = AllMinutesPeak + MinutesPeak; //高峰用电时间（分钟）
                AllMinutesValley = AllMinutesValley + MinutesValley; //低谷用电时间（分钟）

                AllEnergyTotal = AllEnergyTotal + EnergyTotal; //总电量
                AllEnergySpike = AllEnergySpike + EnergySpike; //尖峰电量
                AllEnergyPeak = AllEnergyPeak + EnergyPeak; //高峰电量
                AllEnergyValley = AllEnergyValley + EnergyValley; //低谷电量
            }


            double PowerRate = StructData.electricityparameter.PowerRate; //倍率

            double PriceSpike = StructData.electricityparameter.PriceSpike; //尖峰电价
            double PricePeak = StructData.electricityparameter.PricePeak; //高峰电价
            double PriceValley = StructData.electricityparameter.PriceValley; //低谷电价

            double ActiveCopperLoss = 0; //有功铜损
            double ActiveCoreLoss = 0; //有功铁损

            double ReactiveCopperLoss = 0; //无功铜损
            double ReactiveCoreLoss = 0; //无功铁损


            double newWQP = StructData.DataSourceFirstMeasure[CountLine - 1].WQP; //无功数据（新）
            double oldWQP = StructData.DataSourceFirstMeasure[0].WQP; //无功数据（旧）
            double ReactiveEnergyTotal = newWQP - oldWQP; //总电量

            double ReactiveAll = ReactiveCopperLoss + ReactiveCoreLoss; //无功合计

            double CostSpike = AllEnergySpike*PriceSpike*PowerRate; //尖峰金额
            double CostPeak = AllEnergyPeak*PricePeak*PowerRate; //高峰金额
            double CostValley = AllEnergyValley*PriceValley*PowerRate; //低谷金额
            double CostAll = CostSpike + CostPeak + CostValley; //总金额

            #endregion

            //结构体返回值-----------------------------------------------------------------------------------------------------------
            StructData.TotalHour = (double) AllTotalMinutes/60; //总用电时间（小时）
            StructData.SpikeHour = (double) AllMinutesSpike/60; //尖峰用电时间（小时）
            StructData.PeakHour = (double) AllMinutesPeak/60; //高峰用电时间（小时）
            StructData.ValleyHour = (double) AllMinutesValley/60; //低谷用电时间（小时）

            StructData.EnergyTotal = AllEnergyTotal; //有功（总） = 有功合计
            StructData.EnergySpike = AllEnergySpike; //有功（尖峰）
            StructData.EnergyPeak = AllEnergyPeak; //有功（高峰）
            StructData.EnergyValley = AllEnergyValley; //有功（低谷）

            StructData.EnergyTotal2 = 0; //有功（总） = 有功合计
            StructData.EnergySpike2 = 0; //有功（尖峰）
            StructData.EnergyPeak2 = 0; //有功（高峰）
            StructData.EnergyValley2 = 0; //有功（低谷）


            StructData.CostAll = CostAll; //总金额
            StructData.CostSpike = CostSpike; //尖峰金额
            StructData.CostPeak = CostPeak; //高峰金额
            StructData.CostValley = CostValley; //低谷金额

            StructData.PowerTotal = AllEnergyTotal*PowerRate; //总电量（kw.h）
            StructData.PowerSpike = AllEnergySpike*PowerRate; //尖峰电量（kw.h）
            StructData.PowerPeak = AllEnergyPeak*PowerRate; //高峰电量（kw.h）
            StructData.PowerValley = AllEnergyValley*PowerRate; //低谷电量（kw.h）

            StructData.PriceSpike = StructData.electricityparameter.PriceSpike; //尖峰电价
            StructData.PricePeak = StructData.electricityparameter.PricePeak; //高峰电价
            StructData.PriceValley = StructData.electricityparameter.PriceValley; //低谷电价

            StructData.WPPNew = StructData.DataSourceFirstMeasure[CountLine - 1].WPP; //一次测量示数 （有功总）
            StructData.EventTimeNew = StructData.DataSourceFirstMeasure[CountLine - 1].EventTime; //一次测量时间  datetime
            //二次测量值-------------------------------------------------------------------------------------------------------------------------------------------------------------
            StructData.WPPNew2 = StructData.WPPNew; //二次测量示数 （有功总） 不可用，与一次测量相同
            StructData.EventTimeNew2 = StructData.EventTimeNew; //二次测量时间  datetime  不可用，与一次测量相同

            StructData.PowerTotal2 = StructData.PowerTotal; //总电量（kw.h）
            StructData.PowerSpike2 = StructData.PowerSpike; //尖峰电量（kw.h）
            StructData.PowerPeak2 = StructData.PowerPeak; //高峰电量（kw.h）
            StructData.PowerValley2 = StructData.PowerValley; //低谷电量（kw.h）
            //------------------------------------------------------------------------------------------------------------------------------------------------------------------------

            StructData.WPPOld = StructData.DataSourceFirstMeasure[0].WPP; //初始测量示数 （有功总）
            
            StructData.EventTimeOld = StructData.DataSourceFirstMeasure[0].EventTime; //初始测量时间  datetime

            StructData.ReactiveEnergyQI = ReactiveEnergyTotal; //无功（QI象限）

            StructData.ActiveCopperLoss = ActiveCopperLoss; //有功铜损
            StructData.ActiveCoreLoss = ActiveCoreLoss; //有功铁损
            StructData.ActiveAll = AllEnergyTotal + ActiveCopperLoss + ActiveCoreLoss; //有功合计

            StructData.ReactiveCopperLoss = ReactiveCopperLoss; //无功铜损
            StructData.ReactiveCoreLoss = ReactiveCoreLoss; //无功铁损
            StructData.ReactiveAll = ReactiveAll; //无功合计

            bool hour = false;
            bool energy = false;
            #region

            Double value = StructData.ValleyHour + StructData.SpikeHour + StructData.PeakHour;
            if (StructData.TotalHour == value)
                hour = true;
            if (StructData.EnergyTotal == StructData.EnergyPeak + StructData.EnergySpike + StructData.EnergyValley)
                energy = true;

       
            #endregion


            return StructData;
        }

        public ChargeInfo FirstMeasureData(ChargeInfo structData)
        {
            #region 变量

            //总体数据-------------------------------------------------


            int AllMinutesSpike = 0;
            int AllMinutesPeak = 0;
            int AllMinutesValley = 0;
            int AllTotalMinutes = 0;

            double AllEnergySpike = 0;
            double AllEnergyPeak = 0;
            double AllEnergyValley = 0;
            double AllEnergyTotal = 0;

            double AllCostSpike = 0;
            double AllCostPeak = 0;
            double AllCostValley = 0;
            double AllCostAll = 0;


            //单次测量数据----------------------------------------------
            double EnergyTotal; //总电能值（电表）
            double EnergySpike; //尖峰电能值（电表）
            double EnergyPeak; //高峰电能值（电表）
            double EnergyValley; //低谷电能值（电表）

            int TotalMinutes; //总时长(分钟)
            int MinutesSpike; //尖峰时长（分钟）
            int MinutesPeak; //高峰时长（分钟）
            int MinutesValley; //低谷时长（分钟）

            #endregion

            #region 计算


            ChargeInfo StructData = structData;
            if (StructData.DataSourceFirstMeasure == null)
                return null;
            int CountLine = StructData.DataSourceFirstMeasure.Count;
            for (int i = 1; i < CountLine; i++)
            {
                EnergyTotal = GetEnergyTotal(StructData.DataSourceFirstMeasure, StructData.electricityparameter,
                    StructData.electricitytimeprice, i, out TotalMinutes, out EnergySpike, out MinutesSpike,
                    out EnergyPeak, out MinutesPeak, out EnergyValley, out MinutesValley);
                AllTotalMinutes = AllTotalMinutes + TotalMinutes; //总用电时间（分钟）
                AllMinutesSpike = AllMinutesSpike + MinutesSpike; //尖峰用电时间（分钟）
                AllMinutesPeak = AllMinutesPeak + MinutesPeak; //高峰用电时间（分钟）
                AllMinutesValley = AllMinutesValley + MinutesValley; //低谷用电时间（分钟）

                AllEnergyTotal = AllEnergyTotal + EnergyTotal; //总电量
                AllEnergySpike = AllEnergySpike + EnergySpike; //尖峰电量
                AllEnergyPeak = AllEnergyPeak + EnergyPeak; //高峰电量
                AllEnergyValley = AllEnergyValley + EnergyValley; //低谷电量
            }


            double PowerRate = StructData.electricityparameter.PowerRate; //倍率

            double PriceSpike = StructData.electricityparameter.PriceSpike; //尖峰电价
            double PricePeak = StructData.electricityparameter.PricePeak; //高峰电价
            double PriceValley = StructData.electricityparameter.PriceValley; //低谷电价

            double ActiveCopperLoss = 0; //有功铜损
            double ActiveCoreLoss = 0; //有功铁损

            double ReactiveCopperLoss = 0; //无功铜损
            double ReactiveCoreLoss = 0; //无功铁损


            double newWQP = StructData.DataSourceFirstMeasure[CountLine - 1].WQP; //无功数据（新）
            double oldWQP = StructData.DataSourceFirstMeasure[0].WQP; //无功数据（旧）
            double ReactiveEnergyTotal = newWQP - oldWQP; //总电量

            double ReactiveAll = ReactiveCopperLoss + ReactiveCoreLoss; //无功合计

            double CostSpike = AllEnergySpike * PriceSpike * PowerRate; //尖峰金额
            double CostPeak = AllEnergyPeak * PricePeak * PowerRate; //高峰金额
            double CostValley = AllEnergyValley * PriceValley * PowerRate; //低谷金额
            double CostAll = CostSpike + CostPeak + CostValley; //总金额

            #endregion

            //结构体返回值-----------------------------------------------------------------------------------------------------------
            StructData.TotalHour = (double)AllTotalMinutes / 60; //总用电时间（小时）
            StructData.SpikeHour = (double)AllMinutesSpike / 60; //尖峰用电时间（小时）
            StructData.PeakHour = (double)AllMinutesPeak / 60; //高峰用电时间（小时）
            StructData.ValleyHour = (double)AllMinutesValley / 60; //低谷用电时间（小时）

            StructData.EnergyTotal = AllEnergyTotal; //有功（总） = 有功合计
            StructData.EnergySpike = AllEnergySpike; //有功（尖峰）
            StructData.EnergyPeak = AllEnergyPeak; //有功（高峰）
            StructData.EnergyValley = AllEnergyValley; //有功（低谷）

            StructData.EnergyTotal2 = 0; //有功（总） = 有功合计
            StructData.EnergySpike2 = 0; //有功（尖峰）
            StructData.EnergyPeak2 = 0; //有功（高峰）
            StructData.EnergyValley2 = 0; //有功（低谷）


            StructData.CostAll = CostAll; //总金额
            StructData.CostSpike = CostSpike; //尖峰金额
            StructData.CostPeak = CostPeak; //高峰金额
            StructData.CostValley = CostValley; //低谷金额

            StructData.PowerTotal = AllEnergyTotal * PowerRate; //总电量（kw.h）
            StructData.PowerSpike = AllEnergySpike * PowerRate; //尖峰电量（kw.h）
            StructData.PowerPeak = AllEnergyPeak * PowerRate; //高峰电量（kw.h）
            StructData.PowerValley = AllEnergyValley * PowerRate; //低谷电量（kw.h）

            StructData.PriceSpike = StructData.electricityparameter.PriceSpike; //尖峰电价
            StructData.PricePeak = StructData.electricityparameter.PricePeak; //高峰电价
            StructData.PriceValley = StructData.electricityparameter.PriceValley; //低谷电价

            StructData.WPPNew = StructData.DataSourceFirstMeasure[CountLine - 1].WPP; //一次测量示数 （有功总）
            StructData.EventTimeNew = StructData.DataSourceFirstMeasure[CountLine - 1].EventTime; //一次测量时间  datetime
            //二次测量值-------------------------------------------------------------------------------------------------------------------------------------------------------------
            StructData.WPPNew2 = StructData.WPPNew; //二次测量示数 （有功总） 不可用，与一次测量相同
            StructData.EventTimeNew2 = StructData.EventTimeNew; //二次测量时间  datetime  不可用，与一次测量相同

            StructData.PowerTotal2 = StructData.PowerTotal; //总电量（kw.h）
            StructData.PowerSpike2 = StructData.PowerSpike; //尖峰电量（kw.h）
            StructData.PowerPeak2 = StructData.PowerPeak; //高峰电量（kw.h）
            StructData.PowerValley2 = StructData.PowerValley; //低谷电量（kw.h）
            //------------------------------------------------------------------------------------------------------------------------------------------------------------------------

            StructData.WPPOld = StructData.DataSourceFirstMeasure[0].WPP; //初始测量示数 （有功总）
            StructData.EventTimeOld = StructData.DataSourceFirstMeasure[0].EventTime; //初始测量时间  datetime

            StructData.ReactiveEnergyQI = ReactiveEnergyTotal; //无功（QI象限）

            StructData.ActiveCopperLoss = ActiveCopperLoss; //有功铜损
            StructData.ActiveCoreLoss = ActiveCoreLoss; //有功铁损
            StructData.ActiveAll = AllEnergyTotal + ActiveCopperLoss + ActiveCoreLoss; //有功合计

            StructData.ReactiveCopperLoss = ReactiveCopperLoss; //无功铜损
            StructData.ReactiveCoreLoss = ReactiveCoreLoss; //无功铁损
            StructData.ReactiveAll = ReactiveAll; //无功合计

            bool hour = false;
            bool energy = false;
            #region

            Double value = StructData.ValleyHour + StructData.SpikeHour + StructData.PeakHour;
            if (StructData.TotalHour == value)
                hour = true;
            if (StructData.EnergyTotal == StructData.EnergyPeak + StructData.EnergySpike + StructData.EnergyValley)
                energy = true;


            #endregion


            return StructData;
        }

        public ChargeInfo SecondMeasureData(DateTime startDateTime, DateTime firstDateTime, DateTime secondDateTime,
            int mid)
        {
            ChargeInfo StructData = FirstMeasureData(startDateTime, firstDateTime, secondDateTime, mid);
            if (StructData == null)
                return null;
            ChargeInfo TempStruct1 = FirstMeasureData(startDateTime, firstDateTime, secondDateTime, mid);
            TempStruct1.DataSourceFirstMeasure = TempStruct1.DataSourceFirstMeasure;
       

            ChargeInfo TempStruct2 = FirstMeasureData(startDateTime, firstDateTime, secondDateTime, mid);
            TempStruct2.DataSourceFirstMeasure = TempStruct2.DataSourceSecondMeasure;
            FirstMeasureData(TempStruct2);
    
            double PowerRate = StructData.electricityparameter.PowerRate; //倍率

            double CoActiveCopperLoss = StructData.electricityparameter.CoActiveCopperLoss; //有功铜损系数
            double CoReactiveCopperLoss = StructData.electricityparameter.CoReactiveCopperLoss; //无功铜损系数

            double CoActiveNoloadLoss = StructData.electricityparameter.CoActiveNoloadLoss; //有功空载损耗
            double CoReactiveNoloadLoss = StructData.electricityparameter.CoReactiveNoloadLoss; //无功空载损耗


            //结构体返回值-----------------------------------------------------------------------------------------------------------------------------------
            StructData.TotalHour = TempStruct1.TotalHour + TempStruct2.TotalHour; //总用电时间（小时）
            StructData.SpikeHour = TempStruct1.SpikeHour + TempStruct2.SpikeHour; //尖峰用电时间（小时）
            StructData.PeakHour = TempStruct1.PeakHour + TempStruct2.PeakHour; //高峰用电时间（小时）
            StructData.ValleyHour = TempStruct1.ValleyHour + TempStruct2.ValleyHour; //低谷用电时间（小时）

            StructData.EnergyTotal = TempStruct1.EnergyTotal; //有功（总） = 有功合计
            StructData.EnergySpike = TempStruct1.EnergySpike; //有功（尖峰）
            StructData.EnergyPeak = TempStruct1.EnergyPeak; //有功（高峰）
            StructData.EnergyValley = TempStruct1.EnergyValley; //有功（低谷）

            StructData.EnergyTotal2 = TempStruct1.EnergyTotal + TempStruct2.EnergyTotal; //有功（总） = 有功合计
            StructData.EnergySpike2 = TempStruct1.EnergySpike + TempStruct2.EnergySpike; //有功（尖峰）
            StructData.EnergyPeak2 = TempStruct1.EnergyPeak + TempStruct2.EnergyPeak; //有功（高峰）
            StructData.EnergyValley2 = TempStruct1.EnergyValley + TempStruct2.EnergyValley; //有功（低谷）
            //-----------------------------------------------------------------------------------------------------------------------------------------------

            
            
            
            
            
            StructData.WPPNew = TempStruct1.WPPNew; //一次测量示数 （有功总）
            StructData.EventTimeNew = TempStruct1.EventTimeNew; //一次测量时间  datetime

            StructData.WPPNew2 = TempStruct2.WPPNew; //二次测量示数 （有功总） 
            StructData.EventTimeNew2 = TempStruct2.EventTimeNew; //二次测量时间  datetime  

            StructData.PriceSpike = StructData.electricityparameter.PriceSpike; //尖峰电价
            StructData.PricePeak = StructData.electricityparameter.PricePeak; //高峰电价
            StructData.PriceValley = StructData.electricityparameter.PriceValley; //低谷电价

            StructData.PowerTotal = StructData.EnergyTotal * PowerRate; //总电量（kw.h）
            StructData.PowerSpike = StructData.EnergySpike * PowerRate; //尖峰电量（kw.h）
            StructData.PowerPeak = StructData.EnergyPeak * PowerRate; //高峰电量（kw.h）
            StructData.PowerValley = StructData.EnergyValley * PowerRate; //低谷电量（kw.h）


            //二次测量值-------------------------------------------------------------------------------------------------------------------------------------------------------------


            StructData.PowerTotal2 = StructData.EnergyTotal2*PowerRate; //总电量（kw.h）
            StructData.PowerSpike2 = StructData.EnergySpike2*PowerRate; //尖峰电量（kw.h）
            StructData.PowerPeak2 = StructData.EnergyPeak2*PowerRate; //高峰电量（kw.h）
            StructData.PowerValley2 = StructData.EnergyValley2*PowerRate; //低谷电量（kw.h）
            //------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            StructData.WPPOld = TempStruct1.WPPOld; //初始测量示数 （有功总）
            StructData.EventTimeOld = TempStruct1.EventTimeOld; //初始测量时间  datetime
            //-------------------------------------------------------------------------------------------------------------
            //                            有功铜损系数             总电能
            double ActiveCopperLoss = CoActiveCopperLoss*StructData.PowerTotal2;
            StructData.ActiveCopperLoss = ActiveCopperLoss; //有功铜损
            //                           有功空载损耗           运行时间
            double ActiveCoreLoss = CoActiveNoloadLoss*StructData.TotalHour;
            StructData.ActiveCoreLoss = ActiveCoreLoss; //有功铁损

            StructData.ActiveAll = StructData.PowerTotal2 + ActiveCopperLoss + ActiveCoreLoss; //有功合计
            //--------------------------------------------------------------------------------------------------------------
            //                              有功铜损            无功铜损系数
            double ReactiveCopperLoss = ActiveCopperLoss*CoReactiveCopperLoss;
            StructData.ReactiveCopperLoss = ReactiveCopperLoss; //无功铜损
            //                             无功空载损耗          运行时间
            double ReactiveCoreLoss = CoReactiveNoloadLoss*StructData.TotalHour;
            StructData.ReactiveCoreLoss = ReactiveCoreLoss; //无功铁损
            //--------------------------------------------------------------------------------------------------------------
            StructData.ReactiveEnergyQI = (TempStruct1.ReactiveEnergyQI + TempStruct2.ReactiveEnergyQI)*PowerRate;
                //无功（QI象限）
            StructData.ReactiveAll = StructData.ReactiveEnergyQI + ReactiveCopperLoss + ReactiveCoreLoss; //无功合计

            //                        
            StructData.CostSpike = StructData.PowerSpike2 +
                                   (ActiveCopperLoss*StructData.PowerSpike2/StructData.PowerTotal2) +
                                   (ActiveCoreLoss*2/24);
                //尖峰金额 = 尖峰电量 + ( 有功铜损 * 尖峰电量 / 总电量 ) + ( 有功铁损 * 一天的尖峰时间 / 一天总时间)
            StructData.CostPeak = StructData.PowerPeak2 +
                                  (ActiveCopperLoss*StructData.PowerPeak2/StructData.PowerTotal2) +
                                  (ActiveCoreLoss*10/24);
                //尖峰金额 = 尖峰电量 + ( 有功铜损 * 尖峰电量 / 总电量 ) + ( 有功铁损 * 一天的尖峰时间 / 一天总时间)
            StructData.CostValley = StructData.PowerValley2 +
                                    (ActiveCopperLoss*StructData.PowerValley2/StructData.PowerTotal2) +
                                    (ActiveCoreLoss*12/24);
                //尖峰金额 = 尖峰电量 + ( 有功铜损 * 尖峰电量 / 总电量 ) + ( 有功铁损 * 一天的尖峰时间 / 一天总时间)
            StructData.CostAll = StructData.CostSpike + StructData.CostPeak + StructData.CostValley; //总金额

            return StructData;
        }
    }
}