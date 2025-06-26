using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineConsultationApp.core.DTOs;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using WebApp.ViewModels;

namespace WebApp.Controllers
{
    [Authorize]
    public class ConsultationViewController : Controller
    {
        private readonly HttpClient _client;
        private readonly IMapper _mapper;

        public ConsultationViewController(IHttpClientFactory factory, IMapper mapper)
        {
            _client = factory.CreateClient("ApiClient");

            _mapper = mapper;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UsersWithConsultations()
        {
            var token = User.Claims.FirstOrDefault(c => c.Type == "AccessToken")?.Value;

            if (!string.IsNullOrEmpty(token))
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await _client.GetAsync("api/users/with-consultations");
            if (!response.IsSuccessStatusCode)
                return View("Error");

            var dtoList = await response.Content.ReadFromJsonAsync<List<UserWithConsultationsDTO>>();
            var vmList = _mapper.Map<List<UserWithConsultationsViewModel>>(dtoList);
            return View(vmList);
        }

        [HttpGet]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> MyConsultations()
        {
            var token = User.Claims.FirstOrDefault(c => c.Type == "AccessToken")?.Value;

            if (!string.IsNullOrEmpty(token))
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var email = User.Identity?.Name;
            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            var userResponse = await _client.GetAsync($"api/users/byemail/{email}");
            if (!userResponse.IsSuccessStatusCode)
                return Unauthorized();

            var user = await userResponse.Content.ReadFromJsonAsync<UserDTO>();

            if (user == null)
            {
                return Unauthorized();
            }

            var response = await _client.GetAsync($"api/consultations/user/{user.Id}");

            if (!response.IsSuccessStatusCode)
                return View("Error");

            var dtoList = await response.Content.ReadFromJsonAsync<List<ConsultationDTO>>();
            var vms = _mapper.Map<List<MyConsultationViewModel>>(dtoList);
            return View(vms);
        }
    }
}
