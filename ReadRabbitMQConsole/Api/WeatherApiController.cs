using System;
using System.Threading.Tasks;
using RestSharp;

namespace RabbitMQ.Consumer.Api
{
    public class WeatherApiController : IApiController
    {
        public string GetData()
        {
            var client = new RestClient("https://weatherbit-v1-mashape.p.rapidapi.com/current?lon=38.5&lat=-78.5");
            var request = new RestRequest();
            request.AddHeader("X-RapidAPI-Key", "fd51715415msh3292439f79a16edp130b40jsn2a7dde2fd8cf");
            request.AddHeader("X-RapidAPI-Host", "weatherbit-v1-mashape.p.rapidapi.com");
            var response = client.Execute(request);
            return response.Content;
        }
    }
}