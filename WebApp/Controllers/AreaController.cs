using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;
using WebApp.ViewModels;

namespace WebApp.Controllers
{
    [Authorize(Roles = "admin")]
    public class AreaController : Controller
    {
        private readonly ConsultationsContext _context;

        public AreaController(ConsultationsContext context)
        {
            _context = context;
        }

        // GET: Area
        public async Task<IActionResult> Index()
        {
            var areas = await _context.Areas
                .Select(a => new AreaViewModel
                {
                    Id = a.Id,
                    Name = a.Name
                })
                .ToListAsync();

            return View(areas);
        }

        // GET: Area/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var area = await _context.Areas
                .FirstOrDefaultAsync(a => a.Id == id);

            if (area == null) return NotFound();

            var vm = new AreaViewModel
            {
                Id = area.Id,
                Name = area.Name
            };

            return View(vm);
        }

        // GET: Area/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Area/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AreaViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var area = new Area
            {
                Name = vm.Name
            };

            _context.Add(area);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Area/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var area = await _context.Areas.FindAsync(id);
            if (area == null) return NotFound();

            var vm = new AreaViewModel
            {
                Id = area.Id,
                Name = area.Name
            };

            return View(vm);
        }

        // POST: Area/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AreaViewModel vm)
        {
            if (id != vm.Id) return BadRequest();
            if (!ModelState.IsValid) return View(vm);

            var area = await _context.Areas.FindAsync(id);
            if (area == null) return NotFound();

            area.Name = vm.Name;

            try
            {
                _context.Update(area);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ModelState.AddModelError("", "Failed to update area.");
                return View(vm);
            }
        }

        // GET: Area/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var area = await _context.Areas.FindAsync(id);
            if (area == null) return NotFound();

            var vm = new AreaViewModel
            {
                Id = area.Id,
                Name = area.Name
            };

            return View(vm);
        }

        // POST: Area/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var area = await _context.Areas.FindAsync(id);
            if (area == null) return NotFound();

            try
            {
                _context.Areas.Remove(area);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ModelState.AddModelError("", "Failed to delete area.");
                return View();
            }
        }
    }
}
