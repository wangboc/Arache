using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AracheTest.Data
{
    class Correspondnode
    {
        public Correspondnode(DataRow dr)
        {
            this.ID = (int) dr["ID"];

            if (dr["PID"] != DBNull.Value)
                this.PID = (int)dr["PID"];

            if (dr["MID"] != DBNull.Value)
                this.MID = (int)dr["MID"];

            if (dr["NodeID"] != DBNull.Value)
                this.NodeID = (int)dr["NodeID"];

            if (dr["TransformerID"] != DBNull.Value)
                this.TransformerID = (int)dr["TransformerID"];
        }

        public int ID { set; get; }
        public int PID { set; get; }
        public int MID { set; get; }
        public int NodeID { set; get; }
        public int TransformerID { set; get; }
    }
}
