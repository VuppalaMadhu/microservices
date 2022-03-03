using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using PlatformService.Dto;

namespace PlatformService.SyncDataServices.Http
{
    public class HttpCommandDataClient : ICommandDataClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public HttpCommandDataClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }
        public async Task SendPlatformToCommand(PlatformReadDto plat)
        {
            var httpContent = new StringContent(JsonSerializer.Serialize(plat),
            Encoding.UTF8,
            "application/json");

            var response = await _httpClient.PostAsync(_configuration["CommandsService"], httpContent);

            if(response.IsSuccessStatusCode)
            {
                Console.WriteLine("sunc Post ==> OK!");
            }
            else
            {
                Console.WriteLine("sunc Post ==> Not OK!");
            }
        }
    }
}