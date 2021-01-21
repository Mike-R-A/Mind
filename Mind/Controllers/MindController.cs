using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Mind.Model;
using Mind.Services;

namespace Mind.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MindController : ControllerBase
    {
        private readonly IPersistenceService persistenceService;

        public MindController(IPersistenceService persistenceService)
        {
            this.persistenceService = persistenceService;
        }

        [HttpGet]
        public void Get()
        {
            if(persistenceService.SensorClumps == null)
            {
                persistenceService.SensorClumps = new List<SensorClump>();

                var sensorsLeft = new List<Sensor>
                {
                    new Sensor("red"),
                    new Sensor("green"),
                    new Sensor("blue"),
                    new Sensor("health"),
                    new Sensor("damage"),
                    new Sensor("open"),
                    new Sensor("close")
                };

                var sensorsRight = new List<Sensor>
                {
                    new Sensor("red"),
                    new Sensor("green"),
                    new Sensor("blue"),
                    new Sensor("health"),
                    new Sensor("damage"),
                    new Sensor("open"),
                    new Sensor("close")
                };

                persistenceService.SensorClumps.Add(new SensorClump("left", sensorsLeft));
                persistenceService.SensorClumps.Add(new SensorClump("right", sensorsRight));
            }
        }

        [HttpPost("SensorClump")]
        public void CreateSensorClump(string id, string senseIds)
        {
            var senseIdsArray = senseIds.Split(',');
            var sensors = senseIdsArray.Select(sid => new Sensor(sid)).ToList();

            persistenceService.SensorClumps.Add(new SensorClump(id, sensors));
        }

        [HttpGet("fire/{sensorClumpId}/{sensorId}")]
        public void Fire(string sensorClumpId, string sensorId, [FromQuery] double strength = 1)
        {
            var sensorClump = persistenceService.SensorClumps.Single(s => s.Id == sensorClumpId);

            sensorClump.Sensors.Single(s => s.Id == sensorId).Fire(strength);
        }
    }
}
