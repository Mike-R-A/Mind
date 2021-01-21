using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mind.Model
{
    public interface IConnection
    {
        ISensor PrimarySensor { get; }
        ISensor SecondarySensor { get; }
        bool PrimaryFired { get; set; }
        double Association { get; }
        void CalculateAssociation();
        void Fire(ISensor sensor);
    }
    public class Connection : IConnection
    {
        public Connection(ISensor primarySensor, ISensor secondarySensor)
        {
            PrimarySensor = primarySensor;
            SecondarySensor = secondarySensor;
        }
        public ISensor PrimarySensor { get; set; }
        public ISensor SecondarySensor { get; set; }
        public bool PrimaryFired { get; set; }
        public double Association { get; private set; }

        public void CalculateAssociation()
        {
            var timeDifference = SecondarySensor.LastFired - PrimarySensor.LastFired;

            var timeDifferenceSeconds = timeDifference.Value.TotalSeconds;

            var strengthFactor = PrimarySensor.LastStrength * SecondarySensor.LastStrength;

            Association += strengthFactor / (1 + timeDifferenceSeconds);
        }

        public void Fire(ISensor firingSensor)
        {
            if(firingSensor == PrimarySensor)
            {
                PrimaryFired = true;
            } else if (firingSensor == SecondarySensor && PrimaryFired == true)
            {
                CalculateAssociation();
            }
        }
    }
}
