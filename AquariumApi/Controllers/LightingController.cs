using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.IO.Ports;
//using System.Web.Http.Cors;
using AquariumApi.Models;

namespace AquariumApi.Controllers
{
    //[EnableCors(origins: "*", headers: "*", methods: "*")] // tune to your needs
    [Route("api/[controller]")]
    [ApiController]
    public class LightingController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post(LightingConfiguration lightConfig)
        {
            //Uhhh set all the leds
            SerialPort port = new SerialPort("COM4", 9600);
            port.Open();
            lightConfig.LedData.ForEach(l =>
            {
                port.WriteLine($"{l.Id} {l.R} {l.G} {l.B}");
            });
            port.Close();
        }
        [HttpGet("Test")]
        public ActionResult<string> Test()
        {
            var config = new LightingConfiguration()
            {
                LedData = new List<LED>()
            };
            Random random = new Random();
            var count = 84;
            for(var i=0;i<count;i++)
            {
                config.LedData.Add(new LED()
                {
                    Id = i,
                    R = random.Next(255),
                    G = random.Next(255),
                    B = random.Next(255),
                });
            }
            SerialPort port = new SerialPort("COM3", 19200);
            port.Open();
            var data = "";
            config.LedData.ForEach(l =>
            {
                data += l.Serialize();
            });
            port.Write(data);
            port.Close();
            return "200";
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
