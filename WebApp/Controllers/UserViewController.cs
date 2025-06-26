using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineConsultationApp.core.DTOs;
using WebApp.ViewModels;

namespace WebApp.Controllers
{
    [Authorize]
    public class UserViewController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IMapper _mapper;
        private const int PageSize = 10;

        public UserViewController(IHttpClientFactory clientFactory, IMapper mapper)
        {
            _clientFactory = clientFactory;
            _mapper = mapper;
        }

        // GET: UserView
        public async Task<IActionResult> Index(string? name, int? typeOfWorkId, int page = 1)
        {
            var client = _clientFactory.CreateClient("ApiClient");

            var token = User.Claims.FirstOrDefault(c => c.Type == "AccessToken")?.Value;

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var queryParams = new List<string>();

            if (!string.IsNullOrEmpty(name))
                queryParams.Add($"name={Uri.EscapeDataString(name)}");

            if (typeOfWorkId.HasValue && typeOfWorkId > 0)
                queryParams.Add($"typeOfWorkId={typeOfWorkId}");

            queryParams.Add($"page={page}");
            queryParams.Add($"pageSize={PageSize}");

            string queryString = string.Join('&', queryParams);

            var response = await client.GetAsync($"/api/mentors/search?{queryString}");

            if (!response.IsSuccessStatusCode)
            {
                return View("Error", new ErrorViewModel { RequestId = "Failed to load mentors." });
            }

            var mentorDtos = await response.Content.ReadFromJsonAsync<List<MentorDTO>>();
            if (mentorDtos == null)
            {
                return View("Error", new ErrorViewModel { RequestId = "Invalid data from API." });
            }

            var mentors = _mapper.Map<List<MentorViewModel>>(mentorDtos);

            // Read total count from headers for pagination
            int totalMentors = 0;
            if (response.Headers.TryGetValues("X-Total-Count", out var totalValues))
            {
                int.TryParse(totalValues.FirstOrDefault(), out totalMentors);
            }

            int totalPages = (int)Math.Ceiling(totalMentors / (double)PageSize);

            ViewData["CurrentSearch"] = name;
            ViewData["CurrentTypeOfWorkId"] = typeOfWorkId;
            ViewData["CurrentPage"] = page;
            ViewData["TotalPages"] = totalPages;

            // Get TypeOfWork dropdown
            var towResponse = await client.GetAsync("api/typeofwork");
            if (towResponse.IsSuccessStatusCode)
            {
                var typeOfWorkDtos = await towResponse.Content.ReadFromJsonAsync<List<TypeOfWorkDTO>>();

                var typeOfWorks = _mapper.Map<List<TypeOfWorkViewModel>>(typeOfWorkDtos);

                ViewData["TypeOfWorkList"] = new SelectList(typeOfWorks ?? new List<TypeOfWorkViewModel>(), "Id", "Name");
            }
            else
            {
                ViewData["TypeOfWorkList"] = new SelectList(new List<TypeOfWorkViewModel>(), "Id", "Name");
            }

            return View(mentors);
        }


        // GET: UserView/MentorDetails/5
        public async Task<IActionResult> MentorDetails(int id)
        {
            var client = _clientFactory.CreateClient("ApiClient");

            var token = User.Claims.FirstOrDefault(c => c.Type == "AccessToken")?.Value;

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            if (!User.Identity?.IsAuthenticated ?? true)
                return RedirectToAction("Login", "Account");

            var email = User.Identity?.Name;
            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            // Get user by email to find userId
            var userResponse = await client.GetAsync($"api/users/byemail/{email}");
            if (!userResponse.IsSuccessStatusCode)
                return Unauthorized();

            var user = await userResponse.Content.ReadFromJsonAsync<UserDTO>();
            if (user == null)
                return Unauthorized();

            int userId = user.Id;

            // Get mentor details by id
            var mentorResponse = await client.GetAsync($"api/mentors/{id}");
            if (!mentorResponse.IsSuccessStatusCode)
                return NotFound();

            var mentorDto = await mentorResponse.Content.ReadFromJsonAsync<MentorDTO>();
            if (mentorDto == null)
                return NotFound();

            var mentor = _mapper.Map<MentorViewModel>(mentorDto);

            // Check if user has booked a consultation with this mentor
            var consultationResponse = await client.GetAsync($"api/consultations/user/{userId}");
            if (!consultationResponse.IsSuccessStatusCode)
            {
                ViewData["HasBooked"] = false; // Assume false on error
            }
            else
            {
                var consultations = await consultationResponse.Content.ReadFromJsonAsync<List<ConsultationDTO>>();
                bool hasBooked = consultations?.Any(c => c.MentorId == id) ?? false;
                ViewData["HasBooked"] = hasBooked;
            }

            return View(mentor);
        }

        // GET: UserView/BookConsultation/5
        [HttpGet]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> BookConsultation(int id)
        {
            var client = _clientFactory.CreateClient("ApiClient");

            var token = User.Claims.FirstOrDefault(c => c.Type == "AccessToken")?.Value;

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await client.GetAsync($"api/mentors/{id}");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            var mentorDto = await response.Content.ReadFromJsonAsync<MentorDTO>();
            if (mentorDto == null)
                return NotFound();

            var mentor = _mapper.Map<MentorViewModel>(mentorDto);

            var vm = new ConsultationViewModel
            {
                MentorId = mentor.Id,
                MentorName = $"{mentor.Name} {mentor.Surname}"
            };

            return View(vm);
        }

        // POST: UserView/BookConsultation
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> BookConsultation(ConsultationViewModel model)
        {
            var client = _clientFactory.CreateClient("ApiClient");

            var token = User.Claims.FirstOrDefault(c => c.Type == "AccessToken")?.Value;

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            if (!ModelState.IsValid)
                return View(model);

            var email = User.Identity?.Name;
            if (string.IsNullOrEmpty(email))
                return Unauthorized();

            // Get user by email to find userId
            var userResponse = await client.GetAsync($"api/users/byemail/{email}");
            if (!userResponse.IsSuccessStatusCode)
                return Unauthorized();

            var user = await userResponse.Content.ReadFromJsonAsync<UserDTO>();
            if (user == null)
                return Unauthorized();

            // Check if already booked
            var consultsResponse = await client.GetAsync($"api/consultations/user/{user.Id}");
            if (consultsResponse.IsSuccessStatusCode)
            {
                var consultations = await consultsResponse.Content.ReadFromJsonAsync<List<ConsultationDTO>>();
                if (consultations?.Any(c => c.MentorId == model.MentorId) == true)
                {
                    ModelState.AddModelError("", "You have already booked a consultation with this mentor.");
                    return View(model);
                }
            }

            // Prepare create DTO
            var createDto = new ConsultationCreateDTO
            {
                UserId = user.Id,
                MentorId = model.MentorId,
                Notes = model.Notes ?? ""
            };

            var postResponse = await client.PostAsJsonAsync("api/consultations", createDto);

            Console.WriteLine($"testing: {postResponse.StatusCode}");
            if (!postResponse.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Failed to book consultation. Please try again later.");
                
                return View(model);
            }

            return RedirectToAction("MyConsultations", "ConsultationView");
        }
    }

}
