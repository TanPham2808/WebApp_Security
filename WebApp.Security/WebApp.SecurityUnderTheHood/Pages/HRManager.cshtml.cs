using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Net.Http;
using WebApp.SecurityUnderTheHood.DTO;
using WebApp.SecurityUnderTheHood.Pages.Account;
using System.Text.Json.Serialization;
using System.Net.Http.Json;
using Newtonsoft.Json;
using WebApp.SecurityUnderTheHood.Authorization;
using Newtonsoft.Json.Linq;

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
            // get token from session
            JwtToken token = new JwtToken();

            var strTokenObj = HttpContext.Session.GetString("access_token");
            if (string.IsNullOrEmpty(strTokenObj))
            {
                token = await Authenticate();
            }
            else
            {
                token = JsonConvert.DeserializeObject<JwtToken>(strTokenObj) ?? new JwtToken();
            }

            if (token == null ||
                string.IsNullOrWhiteSpace(token.AccessToken) ||
                token.ExpiresAt <= DateTime.UtcNow)
            {
                token = await Authenticate();
            }

            var httpClient = _httpClientFactory.CreateClient("OurWebAPI");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token?.AccessToken ?? string.Empty);
            weatherForecastItems = await httpClient.GetFromJsonAsync<List<WeatherForecastDTO>>("WeatherForecast") ?? new List<WeatherForecastDTO>();
        }

        /// <summary>
        /// Lấy Token từ API > sau đó lưu vào Session
        /// </summary>
        /// <returns></returns>
        private async Task<JwtToken> Authenticate()
        {
            // Lúc này base của httpClient có dang: https://localhost:7005/
            var httpClient = _httpClientFactory.CreateClient("OurWebAPI");

            var res = await httpClient.PostAsJsonAsync("api/Auth/", new Credential { UserName = "admin", Password = "password" });
            res.EnsureSuccessStatusCode();
            string strJwt = await res.Content.ReadAsStringAsync();
            HttpContext.Session.SetString("access_token", strJwt);

            return JsonConvert.DeserializeObject<JwtToken>(strJwt) ?? new JwtToken();
        }
    }
}
