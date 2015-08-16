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
            this.NodeID = (int)dr["NodeID"];

            if (dr["PID"] != DBNull.Value)
                this.PID = (int)dr["PID"];
            else PID = 1;


            if (dr["ParentID"] != DBNull.Value)
                this.ParentID = (int)dr["ParentID"];

            if (dr["Name"] != DBNull.Value)
                this.Name = dr["Name"].ToString();
        }

        public int NodeID { get; set; }
        
        public int PID { get; set; }
        
        public int ParentID { get; set; }
        
        public String Name { get; set; }

        public int MID { get; set; }
        }
}
