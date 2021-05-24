using System;
using System.Reflection;
using System.Threading.Tasks;
using Dapper;
using System.Data.SqlClient;
using System.Configuration;

namespace WeatherForecast

{
    class WeatherDatabase
    {

        private string Schema { get; set; }
        private SqlConnection db;

        public WeatherDatabase(string conn_string = "Data Source=LAPTOP-MA7LJOFS;Initial Catalog=WeatherDB;Integrated Security=True;Pooling=False")
        {
            db = new SqlConnection(conn_string);
        }

        private async Task CreateSchema()
        {
            WeatherSchema Weatherschema = new WeatherSchema();
            await Weatherschema.PopulateSchema();
            string attributes = "(";
            string values = "(";
            foreach (PropertyInfo property in Weatherschema.GetType().GetProperties())
            {
                object value = property.GetValue(Weatherschema, new object[] { });
                string name = property.Name;

                if (name == "ID")
                {
                    continue;
                }

                attributes += name + ",";
                values += "'" + value + "'" + ",";
            }

            attributes = attributes.Remove(attributes.Length - 1);
            values = values.Remove(values.Length - 1);
            attributes += ")";
            values += ")";
            Schema = "INSERT INTO Weather " + attributes + " VALUES " + values + ";";
            
        }

        public async Task DBInsertWeatherData()
        {
            await CreateSchema();

            using (db)
            {   
                try
                {
                    await db.ExecuteAsync(Schema);
                    Console.WriteLine("Weather data stored");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

        }

        public async Task<System.Collections.Generic.IEnumerable<WeatherSchema>> DBGetAllWeatherData()
        {
            using (db)
            {
                try
                {
         
                    var data = await db.QueryAsync<WeatherSchema>("SELECT * FROM Weather");
                    Console.WriteLine("Rows returned:" + data.AsList().ToArray().Length);
                    return data;
                }
                catch (SqlException e)
                {
                    throw e;
                }
            }
        }


        public async Task<System.Collections.Generic.IEnumerable<WeatherSchema>> DBGetWeatherDataTimeFrame(string startingDate, string endingDate)
        {

            using (db)
            {

                try
                {
                    var data = await db.QueryAsync<WeatherSchema>("SELECT * FROM Weather WHERE sunrise LIKE '"
                                                            + startingDate + "' OR sunrise LIKE '%"
                                                            + endingDate + "%' OR sunset LIKE'%"
                                                            + startingDate + "%' OR sunset LIKE'%"
                                                            + endingDate + "%'");

                    Console.WriteLine("Rows returned:" + data.AsList().ToArray().Length);

                    return data;


                } catch (Exception e)
                {
                    throw new ArgumentOutOfRangeException("No weather data found in this timeframe", e);
                    //return new System.Collections.Generic.List<WeatherSchema>();
                }
                
            }

        }

        public async Task DBRemoveWeatherDataTimeFrame(string startingDate, string endingDate)
        {
            using(db)
            {
                try
                {
                    int nOfRows = await db.ExecuteAsync("DELETE * FROM Weather WHERE sunrise LIKE '"
                                        + startingDate + "' OR sunrise LIKE '%"
                                        + endingDate + "%' OR sunset LIKE'%"
                                        + startingDate + "%' OR sunset LIKE'%"
                                        + endingDate + "%'");
                    Console.WriteLine("Rows affected:" + nOfRows.ToString());
                } catch (Exception e)
                {
                    throw new ArgumentOutOfRangeException("No weather data found in this timeframe", e);
                }
            }
        }

        public async Task DBRemoveWeatherData()
        {
            int nOfRows = await db.ExecuteAsync("DELETE FROM Weather");
            Console.WriteLine("Rows affected:" + nOfRows.ToString());

        }

    }


    class WeatherSchema
    {

        public int ID { get; set; }
        public double temperature { get; set; }
        public double feels_like { get; set; }
        public double temp_min { get; set; }
        public double temp_max { get; set; }
        public int pressure { get; set; }
        public int humidity { get; set; }
        public string description { get; set; }
        public string main { get; set; }
        public double speed { get; set; }
        public string sunrise { get; set; }
        public string sunset { get; set; }


        public async Task PopulateSchema()
        {
            HTTPService service = new HTTPService();
            service.CreateQuery("lat=43.7102", "lon=7.2620", "units=metric", "appid=a70d1cd8dca6f38d63697c5ac585c18e");
            WeatherJSON weatherJSONClass = await service.JSON();
            if (weatherJSONClass == null)
            {
                throw new NullReferenceException();
            }
            else
            {
                temperature = weatherJSONClass.main.temp;
                feels_like = weatherJSONClass.main.feels_like;
                temp_min = weatherJSONClass.main.temp;
                temp_max = weatherJSONClass.main.temp_max;
                pressure = weatherJSONClass.main.pressure;
                humidity = weatherJSONClass.main.humidity;
                description = weatherJSONClass.weather[0].description;
                main = description = weatherJSONClass.weather[0].main;
                speed = weatherJSONClass.wind.speed;
                DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                sunrise = dateTime.AddSeconds(Convert.ToDouble(weatherJSONClass.sys.sunrise)).ToLocalTime().ToString();
                sunset = dateTime.AddSeconds(Convert.ToDouble(weatherJSONClass.sys.sunset)).ToLocalTime().ToString();

            }

        }
    }
}
