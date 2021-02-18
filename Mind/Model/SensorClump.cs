using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mind.Model
{
    public interface ISensorClump
    {
        string Id { get; set; }
        List<Sensor> Sensors { get; set; }
        Dictionary<string, Sensor> SensorDictionary { get; }
        double GetExpectedHappiness(List<SenseInput> senseInputs, string desiredSenseId, string avoidSenseId, bool useSecondaryAssociation = false);
    }
    public class SensorClump : ISensorClump
    {
        public string Id { get; set; }
        public List<Sensor> Sensors { get; set; }
        public Dictionary<string, Sensor> SensorDictionary
        {
            get
            {
                return Sensors.Select(s => new KeyValuePair<string, Sensor>(s.Id, s)).ToDictionary(x => x.Key, x => x.Value);
            }
        }

        public SensorClump(string id, List<Sensor> sensors)
        {
            Id = id;
            Sensors = sensors;

            foreach (var sensor in Sensors)
            {
                if(sensor.Connections == null)
                {
                    sensor.Connections = new List<IConnection>();
                }

                var otherSensors = Sensors.Where(s => s != sensor);

                foreach(var otherSensor in otherSensors)
                {
                    if(otherSensor.Connections == null)
                    {
                        otherSensor.Connections = new List<IConnection>();
                    }

                    var connection = new Connection(sensor, otherSensor);
                    sensor.Connections.Add(connection);
                    otherSensor.Connections.Add(connection);
                }
            }
        }

        public double GetExpectedHappiness(List<SenseInput> senseInputs, string desiredSenseId, string avoidSenseId, bool useSecondaryAssociation = false)
        {
            var desiredAssociationFactor = GetAssociationStrengthFactor(senseInputs, desiredSenseId);

            var secondaryDesiredAssociationFactor = useSecondaryAssociation ? GetSecondaryAssociation(senseInputs, desiredSenseId) : 0;

            var avoidAssociationFactor = GetAssociationStrengthFactor(senseInputs, avoidSenseId);

            var secondaryAvoidAssociationFactor = useSecondaryAssociation ? GetSecondaryAssociation(senseInputs, avoidSenseId) : 0;

            var associationFactor = desiredAssociationFactor + secondaryDesiredAssociationFactor - avoidAssociationFactor - secondaryAvoidAssociationFactor;

            return associationFactor;
        }

        private double GetSecondaryAssociation(List<SenseInput> senseInputs, string desiredSenseId)
        {
            double secondaryDesiredAssociationFactor = 0;

            var otherSenseInputs = senseInputs.Where(s => s.SenseId != desiredSenseId);

            foreach (var senseInput in otherSenseInputs)
            {
                secondaryDesiredAssociationFactor += Math.Sqrt(GetAssociationStrengthFactor(senseInputs, senseInput.SenseId) *
                    GetAssociationBetweenInputAndDesired(desiredSenseId, senseInput));
            }

            return secondaryDesiredAssociationFactor / (otherSenseInputs.Count() * 2);
        }

        private double GetAssociationStrengthFactor(List<SenseInput> senseInputs, string senseId)
        {
            double totalAssociation = 0;
            foreach (var senseInput in senseInputs.Where(s => s.SenseId != senseId))
            {
                double associationToDesired = GetAssociationBetweenInputAndDesired(senseId, senseInput);

                totalAssociation += associationToDesired * senseInput.Strength;
            }
            GetSelfAssociationFactor(senseInputs, senseId);

            return totalAssociation;
        }

        private double GetAssociationBetweenInputAndDesired(string senseId, SenseInput senseInput)
        {
            var inputSensor = Sensors.Single(s => s.Id == senseInput.SenseId);
            var associationToDesired =
                inputSensor.Connections.Single(c => c.PrimarySensor.Id == inputSensor.Id && c.SecondarySensor.Id == senseId).Association;
            return associationToDesired;
        }

        private double GetSelfAssociationFactor(List<SenseInput> senseInputs, string senseId)
        {
            var desiredSenseInput = senseInputs.SingleOrDefault(s => s.SenseId == senseId);
            double selfAssociationFactor = 0;
            if (desiredSenseInput != null)
            {
                selfAssociationFactor = Sensors.Single(s => s.Id == senseId).SelfAssociation * senseInputs.Single(s => s.SenseId == senseId).Strength;
            }

            return selfAssociationFactor;
        }
    }
}
