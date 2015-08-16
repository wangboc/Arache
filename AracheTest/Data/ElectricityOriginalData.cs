using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AracheTest.Data
{
    public class ElectricityOriginalData
    {
        public ElectricityOriginalData(DataRow dr)
        {
            this.ID = (int)dr["ID"];

            if (dr["PID"] != DBNull.Value)
                this.PID = (int)dr["PID"];
            if (dr["MID"] != DBNull.Value)
                this.MID = (int)dr["MID"];

            if (dr["UA"] != DBNull.Value)
                this.UA = (Double)dr["UA"];
            if (dr["UB"] != DBNull.Value)
                this.UB = (Double)dr["UB"];
            if (dr["UC"] != DBNull.Value)
                this.UC = (Double)dr["UC"];

            if (dr["UAB"] != DBNull.Value)
                this.UAB = (Double)dr["UAB"];
            if (dr["UBC"] != DBNull.Value)
                this.UBC = (Double)dr["UBC"];
            if (dr["UCA"] != DBNull.Value)
                this.UCA = (Double)dr["UCA"];

            if (dr["IA"] != DBNull.Value)
                this.IA = (Double)dr["IA"];
            if (dr["IB"] != DBNull.Value)
                this.IB = (Double)dr["IB"];
            if (dr["IC"] != DBNull.Value)
                this.IC = (Double)dr["IC"];

            if (dr["PA"] != DBNull.Value)
                this.PA = (Double)dr["PA"];
            if (dr["PB"] != DBNull.Value)
                this.PB = (Double)dr["PB"];
            if (dr["PC"] != DBNull.Value)
                this.PC = (Double)dr["PC"];
            if (dr["PS"] != DBNull.Value)
                this.PS = (Double)dr["PS"];

            if (dr["QA"] != DBNull.Value)
                this.QA = (Double)dr["QA"];
            if (dr["QB"] != DBNull.Value)
                this.QB = (Double)dr["QB"];
            if (dr["QC"] != DBNull.Value)
                this.QC = (Double)dr["QC"];
            if (dr["QS"] != DBNull.Value)
                this.QS = (Double)dr["QS"];

            if (dr["SA"] != DBNull.Value)
                this.SA = (Double)dr["SA"];
            if (dr["SB"] != DBNull.Value)
                this.SB = (Double)dr["SB"];
            if (dr["SC"] != DBNull.Value)
                this.SC = (Double)dr["SC"];
            if (dr["SS"] != DBNull.Value)
                this.SS = (Double)dr["SS"];

            if (dr["PFA"] != DBNull.Value)
                this.PFA = (Double)dr["PFA"];
            if (dr["PFB"] != DBNull.Value)
                this.PFB = (Double)dr["PFB"];
            if (dr["PFC"] != DBNull.Value)
                this.PFC = (Double)dr["PFC"];
            if (dr["PFS"] != DBNull.Value)
                this.PFS = (Double)dr["PFS"];

            if (dr["FR"] != DBNull.Value)
                this.FR = (Double)dr["FR"];
            if (dr["WPN"] != DBNull.Value)
                this.WPN = (Double)dr["WPN"];
            if (dr["WPP"] != DBNull.Value)
                this.WPP = (Double)dr["WPP"];
            if (dr["WQN"] != DBNull.Value)
                this.WQN = (Double)dr["WQN"];
            if (dr["WQP"] != DBNull.Value)
                this.WQP = (Double)dr["WQP"];

            if (dr["IStatus"] != DBNull.Value)
                this.IStatus = (int)dr["IStatus"];
            if (dr["OStatus"] != DBNull.Value)
                this.OStatus = (int)dr["OStatus"];
            if (dr["EventTime"] != DBNull.Value)
                this.EventTime = Convert.ToDateTime(dr["EventTime"]);
            if (dr["ReceivedTime"] != DBNull.Value)
                this.ReceivedTime = Convert.ToDateTime(dr["ReceivedTime"]);
        }

        public int ID { get; set; }
        /// <summary>
        /// //项目编号
        /// </summary>
        public int PID { get; set; }
        /// <summary>
        /// 表编号
        /// </summary>
        public int MID { get; set; }

        /// <summary>
        /// A相电压
        /// </summary>
        public Double UA { get; set; }
        /// <summary>
        /// B相电压
        /// </summary>
        public Double UB { get; set; }
        /// <summary>
        /// C相电压
        /// </summary>
        public Double UC { get; set; }

        /// <summary>
        /// A-B线电压
        /// </summary>
        public Double UAB { get; set; }
        /// <summary>
        /// B-C线电压
        /// </summary>
        public Double UBC { get; set; }
        /// <summary>
        /// C-A线电压
        /// </summary>
        public Double UCA { get; set; }

        /// <summary>
        /// A相电流
        /// </summary>
        public Double IA { get; set; }
        /// <summary>
        /// B相电流
        /// </summary>
        public Double IB { get; set; }
        /// <summary>
        /// C相电流
        /// </summary>
        public Double IC { get; set; }

        /// <summary>
        /// A相有功功率
        /// </summary>
        public Double PA { get; set; }
        /// <summary>
        /// B相有功功率
        /// </summary>
        public Double PB { get; set; }
        /// <summary>
        /// C相有功功率
        /// </summary>
        public Double PC { get; set; }
        /// <summary>
        /// 合相有功功率
        /// </summary>
        public Double PS { get; set; }

        /// <summary>
        /// A相无功功率
        /// </summary>
        public Double QA { get; set; }
        /// <summary>
        /// B相无功功率
        /// </summary>
        public Double QB { get; set; }
        /// <summary>
        /// C相无功功率
        /// </summary>
        public Double QC { get; set; }
        /// <summary>
        /// 合相无功功率
        /// </summary>
        public Double QS { get; set; }

        /// <summary>
        /// A相视在功率
        /// </summary>
        public Double SA { get; set; }
        /// <summary>
        /// B相视在功率
        /// </summary>
        public Double SB { get; set; }
        /// <summary>
        /// C相视在功率
        /// </summary>
        public Double SC { get; set; }
        /// <summary>
        /// 合相视在功率
        /// </summary>
        public Double SS { get; set; }

        /// <summary>
        /// A相功率因数
        /// </summary>
        public Double PFA { get; set; }
        /// <summary>
        /// B相功率因数
        /// </summary>
        public Double PFB { get; set; }
        /// <summary>
        /// C相功率因数
        /// </summary>
        public Double PFC { get; set; }
        /// <summary>
        /// 合相功率因数
        /// </summary>
        public Double PFS { get; set; }

        /// <summary>
        /// 电网频率
        /// </summary>
        public Double FR { get; set; }

        /// <summary>
        /// 正向有功电能
        /// </summary>
        public Double WPP { get; set; }
        /// <summary>
        /// 负向有功电能
        /// </summary>
        public Double WPN { get; set; }
        /// <summary>
        /// 正向无功电能
        /// </summary>
        public Double WQP { get; set; }
        /// <summary>
        /// 负向无功电能
        /// </summary>
        public Double WQN { get; set; }

        /// <summary>
        /// 开关量输入状态
        /// </summary>
        public int IStatus { get; set; }
        /// <summary>
        /// 开关量输出状态
        /// </summary>
        public int OStatus { get; set; }

        /// <summary>
        /// 发生时间
        /// </summary>
        public DateTime EventTime { get; set; }
        /// <summary>
        /// 接收时间
        /// </summary>
        public DateTime ReceivedTime { get; set; }

    }
}
