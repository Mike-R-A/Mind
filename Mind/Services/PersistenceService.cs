using Mind.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mind.Services
{
    public interface IPersistenceService
    {
        List<SensorClump> SensorClumps { get; set; }
    }
    public class PersistenceService : IPersistenceService
    {
        public PersistenceService()
        {
            SensorClumps = new List<SensorClump>();
        }
        public List<SensorClump> SensorClumps { get; set; }
    }
}
