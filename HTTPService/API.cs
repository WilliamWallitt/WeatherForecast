using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace HTTPService
{
    class HTTPService
    {
        private string req;
        private HttpClient client;
        public HTTPService(string API_req="https://api.openweathermap.org/data/2.5/weather?q=London&appid=", string API_key="a70d1cd8dca6f38d63697c5ac585c18e", HttpClient http_client)
        {
            req = API_req + API_key;
            client = http_client;
        }

        async Task APICall()
        {
            HttpResponseMessage result = await client.GetAsync(req);
            Console.WriteLine(result.StatusCode);
        }
    }
}
