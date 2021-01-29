using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mind.Model
{
    public class FireInput
    {
        public string SensorClumpId { get; set; }
        public List<SenseInput> SenseInputs { get; set; }
        public string DesiredSenseId { get; set; }
        public string AvoidSenseId { get; set; }
    }
}
