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

        [HttpPost("SensorClump/{sensorClumpId}")]
        public JsonResult CreateSensorClump(string sensorClumpId, [FromBody] string[] senses)
        {
            var sensors = senses.Select(s => new Sensor(s)).ToList();

            persistenceService.SensorClumps.Add(new SensorClump(sensorClumpId, sensors));

            return new JsonResult(true);
        }

        [HttpPost("fire/{sensorClumpId}/{sensorId}/{strength}")]
        public void Fire(string sensorClumpId, string sensorId, double strength = 1)
        {
            var sensorClump = persistenceService.SensorClumps.Single(s => s.Id == sensorClumpId);

            sensorClump.Sensors.Single(s => s.Id == sensorId).Fire(strength);
        }

        [HttpPost("FireMultiple/{sensorClumpId}")]
        public double FireMultiple(string sensorClumpId, [FromBody] List<SenseInput> senseInputs, [FromQuery] string desiredSenseId, [FromQuery] string avoidSenseId)
        {
            var sensorClump = persistenceService.SensorClumps.ToList().Single(s => s.Id == sensorClumpId);
            Helper.Shuffle(senseInputs);
            senseInputs.ForEach(i => sensorClump.Sensors.Single(s => s.Id == i.SenseId).Fire(i.Strength));

            var expected = sensorClump.GetExpectedHappiness(senseInputs, desiredSenseId, avoidSenseId);

            return expected;
        }

        [HttpPost("FireList")]
        public JsonResult FireList([FromBody] List<FireInput> fireInputs)
        {
            var expectedDictionary = new Dictionary<string, double>();
            foreach (var fireInput in fireInputs)
            {
                var expected = FireMultiple(fireInput.SensorClumpId, fireInput.SenseInputs, fireInput.DesiredSenseId, fireInput.AvoidSenseId);

                expectedDictionary.Add(fireInput.SensorClumpId, expected);  
            }

            return new JsonResult(expectedDictionary);
        }
    }
}
