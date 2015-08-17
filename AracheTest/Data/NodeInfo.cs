using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AracheTest.Data
{
    public class NodeInfo
    {
        public NodeInfo(DataRow dr)
        {
            this.NodeID = Convert.ToInt32(dr["NodeID"]);

            if (dr["PID"] != DBNull.Value)
                this.PID = Convert.ToInt32(dr["PID"]);
            else PID = 1;


            if (dr["ParentID"] != DBNull.Value)
                this.ParentID = Convert.ToInt32(dr["ParentID"]);

            if (dr["Name"] != DBNull.Value)
                this.Name = dr["Name"].ToString();

            MID = new List<int>();

            IsNode = true;
        }

        public bool IsNode { get; set; }

        public int NodeID { get; set; }

        public int PID { get; set; }

        public int ParentID { get; set; }

        public String Name { get; set; }

        public List<int> MID { get; set; }
    }
}