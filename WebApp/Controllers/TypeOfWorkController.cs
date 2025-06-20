using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;
using WebApp.ViewModels;

namespace WebApp.Controllers
{
    [Authorize(Roles = "admin")]
    public class TypeOfWorkController : Controller
    {
        private readonly ConsultationsContext _context;

        public TypeOfWorkController(ConsultationsContext context)
        {
            _context = context;
        }

        // GET: TypeOfWork
        public async Task<IActionResult> Index()
        {
            var types = await _context.TypeOfWorks.ToListAsync();

            var vmList = types.Select(t => new TypeOfWorkViewModel
            {
                Id = t.Id,
                Name = t.Name
            }).ToList();

            return View(vmList);
        }

        // GET: TypeOfWork/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var type = await _context.TypeOfWorks.FindAsync(id);
            if (type == null) return NotFound();

            var vm = new TypeOfWorkViewModel
            {
                Id = type.Id,
                Name = type.Name
            };

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

            bool exists = await _context.TypeOfWorks
                .AnyAsync(t => t.Name == vm.Name);

            if (exists)
            {
                ModelState.AddModelError("", "A type of work with this name already exists.");
                return View(vm);
            }

            var type = new TypeOfWork
            {
                Name = vm.Name
            };

            try
            {
                _context.TypeOfWorks.Add(type);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ModelState.AddModelError("", "Failed to create TypeOfWork.");
                return View(vm);
            }
        }

        // GET: TypeOfWork/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var type = await _context.TypeOfWorks.FindAsync(id);
            if (type == null) return NotFound();

            var vm = new TypeOfWorkViewModel
            {
                Id = type.Id,
                Name = type.Name
            };

            return View(vm);
        }

        // POST: TypeOfWork/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TypeOfWorkViewModel vm)
        {
            if (id != vm.Id) return BadRequest();

            if (!ModelState.IsValid)
                return View(vm);

            var duplicate = await _context.TypeOfWorks
                .AnyAsync(t => t.Id != id && t.Name == vm.Name);

            if (duplicate)
            {
                ModelState.AddModelError("", "Another type of work with this name already exists.");
                return View(vm);
            }

            var type = await _context.TypeOfWorks.FindAsync(id);
            if (type == null) return NotFound();

            type.Name = vm.Name;

            try
            {
                _context.Update(type);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ModelState.AddModelError("", "Failed to update TypeOfWork.");
                return View(vm);
            }
        }

        // GET: TypeOfWork/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var type = await _context.TypeOfWorks.FindAsync(id);
            if (type == null) return NotFound();

            var vm = new TypeOfWorkViewModel
            {
                Id = type.Id,
                Name = type.Name
            };

            return View(vm);
        }

        // POST: TypeOfWork/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var type = await _context.TypeOfWorks.FindAsync(id);
            if (type == null) return NotFound();

            try
            {
                _context.TypeOfWorks.Remove(type);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Cannot delete this TypeOfWork because it has related Mentors.");
                var vm = new TypeOfWorkViewModel
                {
                    Id = type.Id,
                    Name = type.Name
                };
                return View("Delete", vm);
            }
            catch
            {
                ModelState.AddModelError("", "Failed to delete TypeOfWork.");
                var vm = new TypeOfWorkViewModel
                {
                    Id = type.Id,
                    Name = type.Name
                };
                return View("Delete", vm);
            }
        }
    }
}
