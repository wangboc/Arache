using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AracheTest.Data
{
    public class AmmeterInfo
    {
        public AmmeterInfo(DataRow dr)
        {
            if (dr["PID"] != DBNull.Value)
                this.PID = Convert.ToInt32(dr["PID"]);

            if (dr["ParentID"] != DBNull.Value)
                this.ParentID = Convert.ToInt32(dr["ParentID"]);

            if (dr["Name"] != DBNull.Value)
                this.Name = dr["Name"].ToString();

            if (dr["MID"] != DBNull.Value)
                this.MID = Convert.ToInt32(dr["MID"]);

            if (dr["CabinetID"] != DBNull.Value)
                this.CabinetID = Convert.ToInt32(dr["CabinetID"]);
        }

        public int MID { get; set; }

        public int PID { get; set; }

        public int ParentID { get; set; }

        public String Name { get; set; }

        public int CabinetID { get; set; }
    }
}