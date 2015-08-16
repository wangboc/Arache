using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AracheTest.Data
{
    public class ElectricityPeriod
    {
        public ElectricityPeriod(DataRow row)
        {
            if (row["ID"] != DBNull.Value)
                this.ID = (int) row["ID"];

            if (row["Hour"] != DBNull.Value)
                this.Hour = (int)row["Hour"];

            if (row["ElectricityType"] != DBNull.Value)
                this.ElectricityType = (int)row["ElectricityType"];

            if (row["Remark"] != DBNull.Value)
                this.Remark = row["Remark"].ToString();
        }


        public int ID { get; set; }
        public int Hour { get; set; }
        public int ElectricityType { get; set; }
        public String Remark { get; set; }

    }
}
