using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace WyzeCamApi.Controllers
{
    class HAApiManager
    {

        public void SetCamMotion(string wyzeCamNumber)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiIwZTcwNzk2OTJlMzI0ZDk4OTg1MjEyNmQ0NDRjOTcxNSIsImlhdCI6MTU1ODc5Nzk5MywiZXhwIjoxODc0MTU3OTkzfQ.lNU0gC-zZzbexuUNc5xjMjqcl522QxX1iDUw97zbb7c");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));

            var stateOBJ = new HassState();
            stateOBJ.state = "on";
            var response = client.PostAsJsonAsync("http://10.0.0.50:8123/api/states/input_boolean.wyzecamam" + wyzeCamNumber + "_motion", stateOBJ);
            var responseString = response.Result.Content.ReadAsStringAsync();
            Console.WriteLine(System.DateTime.Now.ToString() + " - Response from HA: " + responseString.Status);
        }
    }
    class HassState
    {
        public string state { get; set; }
    }

}
