using System.Net.Http.Headers;
using System.Net.Http;
using System.Net.Http.Json;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineConsultationApp.core.DTOs;
using WebApp.ViewModels;

namespace WebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class MentorController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IMapper _mapper;

        public MentorController(IHttpClientFactory clientFactory, IMapper mapper)
        {
            _clientFactory = clientFactory;
            _mapper = mapper;
        }

        // GET: Mentor
        public async Task<IActionResult> Index(string searchName, int? typeOfWorkId, int page = 1, int pageSize = 10)
        {
            var client = _clientFactory.CreateClient("ApiClient");

            var queryParams = new List<string>();
            if (!string.IsNullOrEmpty(searchName))
                queryParams.Add($"searchName={Uri.EscapeDataString(searchName)}");

            if (typeOfWorkId.HasValue && typeOfWorkId.Value > 0)
                queryParams.Add($"typeOfWorkId={typeOfWorkId.Value}");

            queryParams.Add($"page={page}");
            queryParams.Add($"pageSize={pageSize}");

            var url = "/api/mentors/";
            if (queryParams.Count > 0)
                url += "?" + string.Join("&", queryParams);

            var token = User.Claims.FirstOrDefault(c => c.Type == "AccessToken")?.Value;

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                return View(new List<MentorViewModel>());
            }

            var mentorDtos = await response.Content.ReadFromJsonAsync<List<MentorDTO>>();
            var mentorVMs = _mapper.Map<List<MentorViewModel>>(mentorDtos);

            await PopulateDropdownsAsync();

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return PartialView("_MentorListPartial", mentorVMs);

            return View(mentorVMs);
        }

        // GET: Mentor/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var client = _clientFactory.CreateClient("ApiClient");

            var token = User.Claims.FirstOrDefault(c => c.Type == "AccessToken")?.Value;

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await client.GetAsync($"/api/mentors/{id}");

            if (!response.IsSuccessStatusCode)
                return NotFound();

            var mentorDto = await response.Content.ReadFromJsonAsync<MentorDTO>();
            var vm = _mapper.Map<MentorViewModel>(mentorDto);

            return View(vm);
        }

        // GET: Mentors/Create
        public async Task<IActionResult> Create()
        {
            await PopulateDropdownsAsync();

            return View();
        }

        // POST: Mentors/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MentorViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdownsAsync();
                return View(vm);
            }

            var client = _clientFactory.CreateClient("ApiClient");

            var token = User.Claims.FirstOrDefault(c => c.Type == "AccessToken")?.Value;

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            // Handle image upload here - upload to wwwroot/uploads and get relative URL
            string? imagePath = null;
            if (Request.Form.Files.Count > 0)
            {
                var imageFile = Request.Form.Files[0];
                if (imageFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }

                    imagePath = "/uploads/" + uniqueFileName;
                }
            }

            // Map ViewModel to DTO and add ImagePath
            var mentorDto = _mapper.Map<MentorDTO>(vm);
            mentorDto.ImagePath = imagePath;

            var response = await client.PostAsJsonAsync("/api/mentors", mentorDto);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ModelState.AddModelError("", "Failed to create mentor via API.");
                await PopulateDropdownsAsync();
                return View(vm);
            }
        }

        // GET: Mentors/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var client = _clientFactory.CreateClient("ApiClient");

            var token = User.Claims.FirstOrDefault(c => c.Type == "AccessToken")?.Value;

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await client.GetAsync($"/api/mentors/{id}");

            if (!response.IsSuccessStatusCode)
                return NotFound();

            var mentorDto = await response.Content.ReadFromJsonAsync<MentorDTO>();
            var vm = _mapper.Map<MentorViewModel>(mentorDto);

            await PopulateDropdownsAsync();

            return View(vm);
        }

        // POST: Mentors/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MentorViewModel vm)
        {
            if (id != vm.Id)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                await PopulateDropdownsAsync();
                return View(vm);
            }

            var client = _clientFactory.CreateClient("ApiClient");

            var token = User.Claims.FirstOrDefault(c => c.Type == "AccessToken")?.Value;

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            // Handle image upload if provided
            if (Request.Form.Files.Count > 0)
            {
                var imageFile = Request.Form.Files[0];
                if (imageFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }

                    vm.ImagePath = "/uploads/" + uniqueFileName;
                }
            }

            var mentorDto = _mapper.Map<MentorDTO>(vm);

            var response = await client.PutAsJsonAsync($"/api/mentors/{id}", mentorDto);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ModelState.AddModelError("", "Failed to update mentor via API.");
                await PopulateDropdownsAsync();
                return View(vm);
            }
        }

        // GET: Mentors/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var client = _clientFactory.CreateClient("ApiClient");

            var token = User.Claims.FirstOrDefault(c => c.Type == "AccessToken")?.Value;

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await client.GetAsync($"/api/mentors/{id}");

            if (!response.IsSuccessStatusCode)
                return NotFound();

            var mentorDto = await response.Content.ReadFromJsonAsync<MentorDTO>();
            var vm = _mapper.Map<MentorViewModel>(mentorDto);

            return View(vm);
        }

        // POST: Mentors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = _clientFactory.CreateClient("ApiClient");

            var token = User.Claims.FirstOrDefault(c => c.Type == "AccessToken")?.Value;

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await client.DeleteAsync($"/api/mentors/{id}");

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ModelState.AddModelError("", "Failed to delete mentor via API.");
                return View();
            }
        }

        private async Task PopulateDropdownsAsync()
        {
            var client = _clientFactory.CreateClient("ApiClient");

            var token = User.Claims.FirstOrDefault(c => c.Type == "AccessToken")?.Value;

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var towResponse = await client.GetAsync("/api/typeOfWork");
            var areasResponse = await client.GetAsync("/api/area");

            var typeOfWorkDtos = towResponse.IsSuccessStatusCode
                ? await towResponse.Content.ReadFromJsonAsync<List<TypeOfWorkDTO>>()
                : new List<TypeOfWorkDTO>();

            var areaDtos = areasResponse.IsSuccessStatusCode
                ? await areasResponse.Content.ReadFromJsonAsync<List<AreaDTO>>()
                : new List<AreaDTO>();

            var typeOfWorks = _mapper.Map<List<TypeOfWorkViewModel>>(typeOfWorkDtos);
            var areas = _mapper.Map<List<AreaViewModel>>(areaDtos);

            ViewData["TypeOfWorkId"] = new SelectList(typeOfWorks, "Id", "Name");
            ViewData["AreaIds"] = new MultiSelectList(areas, "Id", "Name");
        }
    }
}
