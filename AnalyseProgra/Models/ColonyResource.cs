using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalyseProgra.Models
{
    public class ColonyResource
    {
        public int ColonyId { get; set; }
        public int ResourceTypeId { get; set; }

        public double Quantity { get; set; }
        public double ProductionRate { get; set; }
        public double ConsumptionRate { get; set; }
    }
}
