using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AracheTest.Data
{
    public class ElectricityParameter
    {
        public ElectricityParameter(DataRow row)
        {
            this.ID = (int)row["ID"];
            
            if (row["PowerRate"] != DBNull.Value)
                this.PowerRate = (Double)row["PowerRate"];

            if (row["CoActiveCopperLoss"] != DBNull.Value)
                this.CoActiveCopperLoss = (Double)row["CoActiveCopperLoss"];

            if (row["CoActiveNoloadLoss"] != DBNull.Value)
                this.CoActiveNoloadLoss = (Double)row["CoActiveNoloadLoss"];

            if (row["CoReactiveCopperLoss"] != DBNull.Value)
                this.CoReactiveCopperLoss = (Double)row["CoReactiveCopperLoss"];

            if (row["CoReactiveNoloadLoss"] != DBNull.Value)
                this.CoReactiveNoloadLoss = (Double)row["CoReactiveNoloadLoss"];

            if (row["PriceSpike"] != DBNull.Value)
                this.PriceSpike = (Double)row["PriceSpike"];

            if (row["PricePeak"] != DBNull.Value)
                this.PricePeak = (Double)row["PricePeak"];

            if (row["PriceValley"] != DBNull.Value)
                this.PriceValley = (Double)row["PriceValley"];


        }
        public int ID { get; set; }
        public Double PowerRate { get; set; }
        public Double CoActiveCopperLoss { get; set; }
        public Double CoActiveNoloadLoss { get; set; }
        public Double CoReactiveCopperLoss { get; set; }
        public Double CoReactiveNoloadLoss { get; set; }
        public Double PriceSpike { get; set; }
        public Double PricePeak { get; set; }
        public Double PriceValley { get; set; }
     }
}
