using System.Net.Http.Json;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineConsultationApp.core.DTOs;
using WebApp.ViewModels;

namespace WebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TypeOfWorkController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IMapper _mapper;

        public TypeOfWorkController(IHttpClientFactory clientFactory, IMapper mapper)
        {
            _clientFactory = clientFactory;
            _mapper = mapper;
        }

        // GET: TypeOfWork
        public async Task<IActionResult> Index()
        {
            var client = _clientFactory.CreateClient("ApiClient");

            var response = await client.GetAsync("/api/typeOfWork");
            if (!response.IsSuccessStatusCode)
            {
                return View(new List<TypeOfWorkViewModel>());
            }

            var dtoList = await response.Content.ReadFromJsonAsync<List<TypeOfWorkDTO>>();
            var vmList = _mapper.Map<List<TypeOfWorkViewModel>>(dtoList);

            return View(vmList);
        }

        // GET: TypeOfWork/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var client = _clientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync($"/api/typeOfWork/{id}");

            if (!response.IsSuccessStatusCode)
                return NotFound();

            var dto = await response.Content.ReadFromJsonAsync<TypeOfWorkDTO>();
            var vm = _mapper.Map<TypeOfWorkViewModel>(dto);

            return View(vm);
        }

        // GET: TypeOfWork/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TypeOfWork/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TypeOfWorkViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var client = _clientFactory.CreateClient("ApiClient");

            // Check for existing TypeOfWork by name before creating
            var allResponse = await client.GetAsync("/api/typeOfWork");

            var allDtos = allResponse.IsSuccessStatusCode
                ? await allResponse.Content.ReadFromJsonAsync<List<TypeOfWorkDTO>>()
                : new List<TypeOfWorkDTO>();

            if (allDtos.Any(t => string.Equals(t.Name, vm.Name, StringComparison.OrdinalIgnoreCase)))
            {
                ModelState.AddModelError("", "A type of work with this name already exists.");
                return View(vm);
            }

            var dto = _mapper.Map<TypeOfWorkDTO>(vm);

            var response = await client.PostAsJsonAsync("/api/typeOfWork", dto);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ModelState.AddModelError("", "Failed to create TypeOfWork via API.");
                return View(vm);
            }
        }

        // GET: TypeOfWork/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var client = _clientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync($"/api/typeOfWork/{id}");

            if (!response.IsSuccessStatusCode)
                return NotFound();

            var dto = await response.Content.ReadFromJsonAsync<TypeOfWorkDTO>();
            var vm = _mapper.Map<TypeOfWorkViewModel>(dto);

            return View(vm);
        }

        // POST: TypeOfWork/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TypeOfWorkViewModel vm)
        {
            if (id != vm.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(vm);

            var client = _clientFactory.CreateClient("ApiClient");

            // Check for duplicates
            var allResponse = await client.GetAsync("/api/typeOfWork");

            var allDtos = allResponse.IsSuccessStatusCode
                ? await allResponse.Content.ReadFromJsonAsync<List<TypeOfWorkDTO>>()
                : new List<TypeOfWorkDTO>();

            if (allDtos.Any(t => t.Id != id && string.Equals(t.Name, vm.Name, StringComparison.OrdinalIgnoreCase)))
            {
                ModelState.AddModelError("", "Another type of work with this name already exists.");
                return View(vm);
            }

            var dto = _mapper.Map<TypeOfWorkDTO>(vm);

            var response = await client.PutAsJsonAsync($"/api/typeOfWork/{id}", dto);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ModelState.AddModelError("", "Failed to update TypeOfWork via API.");
                return View(vm);
            }
        }

        // GET: TypeOfWork/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var client = _clientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync($"/api/typeOfWork/{id}");

            if (!response.IsSuccessStatusCode)
                return NotFound();

            var dto = await response.Content.ReadFromJsonAsync<TypeOfWorkDTO>();
            var vm = _mapper.Map<TypeOfWorkViewModel>(dto);

            return View(vm);
        }

        // POST: TypeOfWork/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = _clientFactory.CreateClient("ApiClient");
            var response = await client.DeleteAsync($"/api/typeOfWork/{id}");

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
                ModelState.AddModelError("", "Cannot delete this TypeOfWork because it has related Mentors.");

                var reloadResponse = await client.GetAsync($"/api/typeOfWork/{id}");

                if (!reloadResponse.IsSuccessStatusCode)
                    return NotFound();

                var dto = await reloadResponse.Content.ReadFromJsonAsync<TypeOfWorkDTO>();
                var vm = _mapper.Map<TypeOfWorkViewModel>(dto);

                return View("Delete", vm);
            }
            else
            {
                ModelState.AddModelError("", "Failed to delete TypeOfWork via API.");
                var reloadResponse = await client.GetAsync($"/api/typeOfWork/{id}");
                if (!reloadResponse.IsSuccessStatusCode)
                    return NotFound();

                var dto = await reloadResponse.Content.ReadFromJsonAsync<TypeOfWorkDTO>();
                var vm = _mapper.Map<TypeOfWorkViewModel>(dto);

                return View("Delete", vm);
            }
        }
    }
}
