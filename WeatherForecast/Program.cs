using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;


namespace WeatherForecast
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //HTTPService service = new HTTPService();
            //service.CreateQuery("lat=43.7102", "lon=7.2620", "units=metric" ,"appid=a70d1cd8dca6f38d63697c5ac585c18e");
            //await service.JSON();

            WeatherDatabase db = new WeatherDatabase();
            //await db.DBInsertWeatherData();
            //await db.DBRemoveWeatherData();
            //IEnumerable<WeatherSchema> data = await db.DBGetWeatherDataTimeFrame("2021", "2021");
            // IEnumerable<WeatherSchema> data = await db.DBGetAllWeatherData();

            Console.ReadLine();

        }


    }

}
