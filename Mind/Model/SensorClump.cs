﻿using System;
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
    }
}