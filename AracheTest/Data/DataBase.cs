using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Threading;
using AracheTest;
using AracheTest.Tools;
using AracheTest.Data;
using DevExpress.Xpo.DB.Helpers;
using DevExpress.XtraPrinting.Native;

namespace AracheTest.Data
{
    internal class DataBase
    {
        public static List<ElectricityParameter> GetElectricityparameter()
        {
            List<ElectricityParameter> list = new List<ElectricityParameter>();

            DataTable dt = DBConnector.ExecuteSql(string.Format("select * from electricityparameter"));
            if (dt != null)
                list.AddRange(from DataRow row in dt.Rows select new ElectricityParameter(row));
            LogHelper.WriteLog(typeof (frmMain), "获取电费参数信息");
            return list;
        }

        public static List<ElectricityPeriod> GetElectricityPeriods()
        {
            List<ElectricityPeriod> list = new List<ElectricityPeriod>();
            DataTable dt = DBConnector.ExecuteSql(string.Format("select * from electricitytime"));
            if (dt != null)
                list.AddRange(from DataRow row in dt.Rows select new ElectricityPeriod(row));
            LogHelper.WriteLog(typeof (frmMain), "获取电费时段信息");
            return list;
        }

        public static List<ElectricityOriginalData> GetRealTimeData(int mid, int pid)
        {
            List<ElectricityOriginalData> list = new List<ElectricityOriginalData>();
            DateTime yesterday = DateTime.Now.AddDays(-1);
            DataTable dt =
                DBConnector.ExecuteSql(
                    string.Format(
                        "select * from normalreceiveddata where Eventtime > '{0:u}' and MID = '{1}' and PID = '{2}' order by Eventtime",
                        yesterday, mid, pid));
            if (dt != null)
                list.AddRange(from DataRow dr in dt.Rows select new ElectricityOriginalData(dr));
            LogHelper.WriteLog(typeof (frmMain), "获取24小时数据");
            return list;
        }

        public static List<ElectricityOriginalData> GetDatetimeFilteredData(DateTime start, DateTime end, int mid,
            int pid)
        {
            List<ElectricityOriginalData> list = new List<ElectricityOriginalData>();
            DataTable dt =
                DBConnector.ExecuteSql(
                    string.Format(
                        "select * from normalreceiveddata where (Eventtime between '{0}' and '{1}') and MID = '{2}' and PID = '{3}' order by Eventtime",
                        start, end, mid, pid));
            if (dt != null)
                list.AddRange(from DataRow dr in dt.Rows select new ElectricityOriginalData(dr));
            LogHelper.WriteLog(typeof (frmMain), "获取" + start + "至" + end + "数据");
            return list;
        }

        public static List<NodeInfo> GetAllNodeInfo(int pid)
        {
            List<NodeInfo> list = new List<NodeInfo>();
            DataTable dt = DBConnector.ExecuteSql(string.Format("select * from nodeinfo where PID = '{0}'", pid));
            if (dt != null)
                list.AddRange(from DataRow dr in dt.Rows select new NodeInfo(dr));
            LogHelper.WriteLog(typeof (frmMain), "获取所有节点信息");
            return list;
        }

        public static List<Correspondnode> GetCorrespondMid(int nodeId)
        {
            List<Correspondnode> list = new List<Correspondnode>();
            DataTable dt =
                DBConnector.ExecuteSql(string.Format("select * from correspondnode where nodeid= '{0}'", nodeId));
            if (dt != null)
                list.AddRange(from DataRow dr in dt.Rows select new Correspondnode(dr));
            LogHelper.WriteLog(typeof (frmMain), string.Format("获取NodeID为'{0}'节点--电表信息", nodeId));
            return list;
        }

        public static List<ElectricityOriginalData> GetElectricityDataByMid(int mid, int pid)
        {
            List<ElectricityOriginalData> list = new List<ElectricityOriginalData>();
            DataTable dt =
                DBConnector.ExecuteSql(
                    string.Format("select * from normalreceiveddata where MID = '{0}' and PID = '{1}'", mid, pid));
            if (dt != null)
                list.AddRange(from DataRow dr in dt.Rows select new ElectricityOriginalData(dr));
            LogHelper.WriteLog(typeof (frmMain), string.Format("获取MID为'{0}'的电表--电量信息", mid));
            return list;
        }

        public static List<ElectricityOriginalData> GetElectricityDataByNodeId(int nodeID)
        {
            List<ElectricityOriginalData> list = new List<ElectricityOriginalData>();
            DataTable dt =
                DBConnector.ExecuteSql(
                    string.Format(
                        "select * from normalreceiveddata where MID in (select MID from nodeinfo where nodeid='{0}')",
                        nodeID));
            if (dt != null)
                list.AddRange(from DataRow dr in dt.Rows select new ElectricityOriginalData(dr));
            LogHelper.WriteLog(typeof (frmMain), string.Format("获取NodeID为'{0}'的节点相关电量信息", nodeID));
            return list;
        }
    }
}