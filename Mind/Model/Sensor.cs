using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mind.Model
{
    public interface ISensor
    {
        string Id { get; }
        List<IConnection> Connections { get; set; }
        DateTime? LastFired { get; set; }
        double LastStrength { get; set; }
        void Fire(double strength);
        double GetConnectionStrength(string secondarySensorId, bool includeSecondLevel);
    }
    public class Sensor : ISensor
    {
        public Sensor(string id)
        {
            Id = id;
        }
        public string Id { get; set; }
        public List<IConnection> Connections { get; set; }
        public double SelfAssociation { get; set; }
        public DateTime? LastFired { get; set; }
        public double LastStrength { get; set; }

        public void Fire(double strength)
        {
            var fired = DateTime.Now;
            SetSelfAssociation(fired, strength);
            LastFired = fired;
            LastStrength = strength;
            foreach (var connection in Connections)
            {
                connection.Fire(this);
            }
        }

        private void SetSelfAssociation(DateTime fired, double strength)
        {
            var strengthFactor = LastStrength * strength;
            if (LastFired.HasValue)
            {
                var timeDifference = fired - LastFired;
                SelfAssociation += strengthFactor / (1 + timeDifference.Value.TotalSeconds);
            }
        }

        public double GetConnectionStrength(string secondarySensorId, bool includeSecondLevel)
        {
            var directConnectionsToSecondary = Connections.Where(c => c.SecondarySensor.Id == secondarySensorId);
            var associationSum = directConnectionsToSecondary.Sum(c => c.Association);

            if (includeSecondLevel)
            {
                var indirectAssociationSum = GetIndirectAssociationSum(secondarySensorId);

                associationSum += indirectAssociationSum;
            }

            return associationSum;
        }

        private double GetIndirectAssociationSum(string secondarySensorId)
        {
            var otherConnections = Connections.Where(c => c.SecondarySensor.Id != secondarySensorId);

            double indirectAssociationSquareSum = 0;
            foreach (var otherConnection in otherConnections)
            {
                var indirectConnectionsToSecondary = otherConnection.SecondarySensor.Connections.Where(c => c.SecondarySensor.Id == secondarySensorId);

                indirectAssociationSquareSum += Math.Pow(otherConnection.Association, 2) + Math.Pow(indirectConnectionsToSecondary.Sum(i => i.Association), 2);
            }
            var indirectAssociationSum = Math.Sqrt(indirectAssociationSquareSum);
            return indirectAssociationSum;
        }
    }
}
