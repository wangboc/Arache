using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AracheTest.Data
{
    internal class ChargeInfo
    {
        public int ChargeID { get; set; }
        public int PID { get; set; }
        public int MID { get; set; }
        public Double ChargeTotal { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public Double ChargeSpike { get; set; }
        public Double ChargePeak { get; set; }
        public Double ChargeValley { get; set; }
        public Double EnergyTotal { get; set; }
        public Double EnergySpike { get; set; }
        public Double EnergyPeak { get; set; }
        public Double EnergyValley { get; set; }
        public Double PFe { get; set; }
        public Double PCu { get; set; }
        public Double QFe { get; set; }
        public Double QCu { get; set; }
        public Double PTotal { get; set; }
        public Double QTotal { get; set; }
        public bool IsSecond { get; set; }

        public ChargeInfo(DataRow dr)
        {
            if (dr["ChargeID"] != DBNull.Value)
                this.ChargeID = Convert.ToInt32(dr["ChargeID"]);

            if (dr["PID"] != DBNull.Value)
                this.PID = Convert.ToInt32(dr["PID"]);

            if (dr["MID"] != DBNull.Value)
                this.MID = Convert.ToInt32(dr["MID"]);

            if (dr["ChargeTotal"] != DBNull.Value)
                this.ChargeTotal = Convert.ToDouble(dr["ChargeTotal"]);

            if (dr["StartTime"] != DBNull.Value)
                this.StartTime = DateTime.Parse(dr["StartTime"].ToString());

            if (dr["EndTime"] != DBNull.Value)
                this.EndTime = DateTime.Parse(dr["EndTime"].ToString());

            if (dr["ChargeSpike"] != DBNull.Value)
                this.ChargeSpike = Double.Parse(dr["ChargeSpike"].ToString());

            if (dr["ChargePeak"] != DBNull.Value)
                this.ChargePeak = Double.Parse(dr["ChargePeak"].ToString());

            if (dr["ChargeValley"] != DBNull.Value)
                this.ChargeValley = Double.Parse(dr["ChargeValley"].ToString());

            if (dr["EnergyTotal"] != DBNull.Value)
                this.EnergyTotal = Double.Parse(dr["EnergyTotal"].ToString());

            if (dr["EnergySpike"] != DBNull.Value)
                this.EnergySpike = Double.Parse(dr["EnergySpike"].ToString());

            if (dr["EnergyPeak"] != DBNull.Value)
                this.EnergyPeak = Double.Parse(dr["EnergyPeak"].ToString());

            if (dr["EnergyValley"] != DBNull.Value)
                this.EnergyValley = Double.Parse(dr["EnergyValley"].ToString());

            if (dr["PFe"] != DBNull.Value)
                this.PFe = Double.Parse(dr["PFe"].ToString());

            if (dr["PCu"] != DBNull.Value)
                this.PCu = Double.Parse(dr["PCu"].ToString());

            if (dr["QFe"] != DBNull.Value)
                this.QFe = Double.Parse(dr["QFe"].ToString());

            if (dr["QCu"] != DBNull.Value)
                this.QCu = Double.Parse(dr["QCu"].ToString());

            if (dr["PTotal"] != DBNull.Value)
                this.PTotal = Double.Parse(dr["PTotal"].ToString());

            if (dr["QTotal"] != DBNull.Value)
                this.QTotal = Double.Parse(dr["QTotal"].ToString());

            if (dr["IsSecond"] != DBNull.Value)
            {
                if (int.Parse(dr["IsSecond"].ToString()) == 1)
                    IsSecond = true;
                else IsSecond = false;
            }
        }
    }
}