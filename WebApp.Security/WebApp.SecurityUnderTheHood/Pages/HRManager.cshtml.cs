using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Net.Http;
using WebApp.SecurityUnderTheHood.DTO;

namespace WebApp.SecurityUnderTheHood.Pages
{
    [Authorize (Policy = "HRManagerOnly")]
    public class HRManagerModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        [BindProperty]
        public List<WeatherForecastDTO> weatherForecastItems { get; set; } = new List<WeatherForecastDTO>();

        public HRManagerModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task OnGet()
        {
            // Lúc này base của httpClient có dang: https://localhost:7005/
            var httpClient = _httpClientFactory.CreateClient("OurWebAPI");
            weatherForecastItems 
                = await httpClient.GetFromJsonAsync<List<WeatherForecastDTO>>("WeatherForecast") 
                ?? new List<WeatherForecastDTO>();

        }
    }
}
