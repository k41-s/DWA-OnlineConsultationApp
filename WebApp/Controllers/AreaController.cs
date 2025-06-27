using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineConsultationApp.core.DTOs;
using System.Net.Http.Json;
using WebApp.ViewModels;

namespace WebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AreaController : Controller
    {
        private readonly HttpClient _client;
        private readonly IMapper _mapper;

        public AreaController(IHttpClientFactory factory, IMapper mapper)
        {
            _client = factory.CreateClient("ApiClient");
            _mapper = mapper;
        }

        // GET: Area
        public async Task<IActionResult> Index()
        {
            var response = await _client.GetAsync("api/areas");
            if (!response.IsSuccessStatusCode)
                return View("Error");

            var dtos = await response.Content.ReadFromJsonAsync<List<AreaDTO>>();
            var vms = _mapper.Map<List<AreaViewModel>>(dtos);
            return View(vms);
        }

        // GET: Area/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var response = await _client.GetAsync($"api/areas/{id}");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            var dto = await response.Content.ReadFromJsonAsync<AreaDTO>();
            var vm = _mapper.Map<AreaViewModel>(dto);
            return View(vm);
        }

        // GET: Area/Create
        public IActionResult Create() => View();

        // POST: Area/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AreaViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            // Check for existing TypeOfWork by name before creating
            var allResponse = await _client.GetAsync("/api/areas");

            var allDtos = allResponse.IsSuccessStatusCode
                ? await allResponse.Content.ReadFromJsonAsync<List<AreaDTO>>()
                : new List<AreaDTO>();

            if (allDtos.Any(t => string.Equals(t.Name, vm.Name, StringComparison.OrdinalIgnoreCase)))
            {
                ModelState.AddModelError("", "An area with this name already exists.");
                return View(vm);
            }

            var dto = _mapper.Map<AreaCreateDTO>(vm);
            var response = await _client.PostAsJsonAsync("api/areas", dto);

            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "An area with this name may already exist.");
            return View(vm);
        }

        // GET: Area/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var response = await _client.GetAsync($"api/areas/{id}");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            var dto = await response.Content.ReadFromJsonAsync<AreaDTO>();
            var vm = _mapper.Map<AreaViewModel>(dto);
            return View(vm);
        }

        // POST: Area/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AreaViewModel vm)
        {
            if (id != vm.Id) return BadRequest();
            if (!ModelState.IsValid) return View(vm);

            var dto = _mapper.Map<AreaCreateDTO>(vm);
            var response = await _client.PutAsJsonAsync($"api/areas/{id}", dto);

            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "Another area with this name may already exist.");
            return View(vm);
        }

        // GET: Area/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _client.GetAsync($"api/areas/{id}");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            var dto = await response.Content.ReadFromJsonAsync<AreaDTO>();
            var vm = _mapper.Map<AreaViewModel>(dto);

            return View(vm);
        }

        // POST: Area/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var response = await _client.DeleteAsync($"api/areas/{id}");

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            else if(response.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
                ModelState.AddModelError("", "Failed to delete area (might be linked to a mentor).");

                var reloadResponse = await _client.GetAsync($"/api/areas/{id}");

                if (!reloadResponse.IsSuccessStatusCode)
                    return NotFound();

                var dto = await reloadResponse.Content.ReadFromJsonAsync<AreaDTO>();
                var vm = _mapper.Map<AreaViewModel>(dto);

                return View("Delete", vm);
            }
            else
            {
                ModelState.AddModelError("", "Failed to delete Area via API.");
                var reloadResponse = await _client.GetAsync($"/api/areas/{id}");
                if (!reloadResponse.IsSuccessStatusCode)
                    return NotFound();

                var dto = await reloadResponse.Content.ReadFromJsonAsync<AreaDTO>();
                var vm = _mapper.Map<AreaViewModel>(dto);

                return View("Delete", vm);
            }
        }
    }
}
