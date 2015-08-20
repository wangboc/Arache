using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalogSend
{
    public class ElectricityOriginalData
    {
        public ElectricityOriginalData()
        {
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