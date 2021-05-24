using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using Newtonsoft;
using Newtonsoft.Json;
//using static System.Net.WebClient;

namespace WeatherForecast
{
    class HTTPService
    {
        private string req;
        private WebClient client;
        public HTTPService(string API_req = "https://api.openweathermap.org/data/2.5/weather?")
        {
            req = API_req;
            client = new WebClient();
        }

        public async Task<string> Request()
        {
            string result = await client.DownloadStringTaskAsync(req);
            client.Dispose();
            return result;
        }

        public async Task<WeatherJSON> JSON()
        {   
            // deserialize string req into class, then we can get the info out of the WeatherJSON class
            WeatherJSON JsonData = JsonConvert.DeserializeObject<WeatherJSON>(await Request());
            return JsonData;
        }


        public void CreateQuery(params string[] args)
        {
            string queryparams = "";
            foreach (string arg in args)
            {
                queryparams += arg + "&";
            }
            queryparams = queryparams.Remove(queryparams.Length - 1);
            req += queryparams;
        }

    }
}