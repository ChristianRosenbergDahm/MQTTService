using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mqtt;
using Newtonsoft.Json;
using System.Text;
using System.Text.RegularExpressions;

namespace WyzeCamApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WyzeController : ControllerBase
    {
        static string _apiKey = "";



        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values
        // https://localhost:5001/api/wyze/1/motionnotdetected/APIKEY
        [HttpGet("{cameraID}/{command}/{apikey}")]
        public ActionResult<IEnumerable<string>> Get(String cameraID, String command, String apikey)
        {

            Console.WriteLine(" -------- GET API hit -------");
            Console.WriteLine(" ------ CameraID: " + cameraID + " command: " + command + " ApiKey: " + apikey + " ------");
            string returnString = string.Empty;

            //Check api key
            if (!ValidateApiKey(apikey))
            {
                Console.WriteLine("Api key is not correct");
                return new string[] { "API key not valid", "" };
            }

            using (var mqttClient = MqttClient.CreateAsync("10.0.0.50").Result)
            {
                // var mqttClient = MqttClient.CreateAsync("10.0.0.50").Result ;

                var sess = mqttClient.ConnectAsync().Result;
                var camName = "wyze_" + cameraID.ToString();

                string rcvTopic = "wyze/" + camName + "/receive";
                string sendTopic = "wyze/" + camName + "/command";

                mqttClient.SubscribeAsync(rcvTopic, MqttQualityOfService.ExactlyOnce);
                var sendData = String.Empty;

                Task.Run(() =>
                {
                    var line = command;
                    var data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(line));
                // var line = Regex.Unescape("{'command':" + command + "'}");
                // var data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(line.Replace("'","\"").Remove(0,1)));
                sendData = data.ToString();
                    mqttClient.PublishAsync(new MqttApplicationMessage(sendTopic, data), MqttQualityOfService.ExactlyOnce).Wait();
                });
                returnString = "------- MQTT: SENDTOPIC: " + sendTopic + " --------";
            }
        

            return new string[] { returnString, "" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
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

        private bool ValidateApiKey(string apikey)
        {
            if (apikey == _apiKey)
            {
                return true;
            }
            else return false;
        }

    }
}
