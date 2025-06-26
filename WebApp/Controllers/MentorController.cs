using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;
using WebApp.ViewModels;

namespace WebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class MentorController : Controller
    {
        private readonly ConsultationsContext _context;

        public MentorController(ConsultationsContext context)
        {
            _context = context;
        }

        // GET: Mentor
        // Done to LO5 desired, so not by Type of Work
        public async Task<IActionResult> Index(string searchName, int? typeOfWorkId, int page = 1, int pageSize = 10)
        {
            var query = _context.Mentors
                .Include(m => m.Areas)
                .Include(m => m.TypeOfWork)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchName))
            {
                query = query.Where(m => m.Name.Contains(searchName) || m.Surname.Contains(searchName));
            }

            if (typeOfWorkId.HasValue && typeOfWorkId.Value > 0)
            {
                query = query.Where(m => m.TypeOfWork.Id == typeOfWorkId.Value);
            }

            var totalItems = await query.CountAsync();

            var mentors = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var mentorVMs = mentors.Select(m => new MentorViewModel
            {
                Id = m.Id,
                Name = m.Name,
                Surname = m.Surname,
                TypeOfWorkId = m.TypeOfWork.Id,
                TypeOfWorkName = m.TypeOfWork.Name,
                AreaIds = m.Areas.Select(a => a.Id).ToList(),
                AreaNames = m.Areas.Select(a => a.Name).ToList()
            }).ToList();

            ViewBag.TotalItems = totalItems;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return PartialView("_MentorListPartial", mentorVMs);


            ViewData["TypeOfWorkIds"] = await _context.TypeOfWorks.ToListAsync();

            return View(mentorVMs);
        }

        // GET: Mentor/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var mentor = await _context.Mentors
                .Include(m => m.Areas)
                .Include(m => m.TypeOfWork)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (mentor == null) 
                return NotFound();

            var vm = new MentorViewModel
            {
                Id = mentor.Id,
                Name = mentor.Name,
                Surname = mentor.Surname,
                TypeOfWorkId = mentor.TypeOfWork.Id,
                TypeOfWorkName = mentor.TypeOfWork.Name,
                AreaIds = mentor.Areas.Select(a => a.Id).ToList(),
                AreaNames = mentor.Areas.Select(a => a.Name).ToList()
            };

            return View(vm);
        }

        // GET: Mentors/Create
        public IActionResult Create()
        {
            ViewData["TypeOfWorkId"] = new SelectList(_context.TypeOfWorks, "Id", "Name");
            ViewData["AreaIds"] = new MultiSelectList(_context.Areas, "Id", "Name");
            return View();
        }

        // POST: Mentors/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MentorViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    ModelState.AddModelError("", error.ErrorMessage);
                }

                ViewData["TypeOfWorkId"] = new SelectList(_context.TypeOfWorks, "Id", "Name");
                ViewData["AreaIds"] = new MultiSelectList(_context.Areas, "Id", "Name");

                return View(vm);
            }

            bool duplicateExists = await _context.Mentors
                .AnyAsync(m => m.Name == vm.Name && m.Surname == vm.Surname);

            if (duplicateExists)
            {
                ModelState.AddModelError("", "A mentor with this name already exists.");
                ViewData["TypeOfWorkId"] = new SelectList(_context.TypeOfWorks, "Id", "Name");
                ViewData["AreaNames"] = new SelectList(_context.Areas, "Id", "Name");
                return View(vm);
            }

            var areas = await _context.Areas.Where(a => vm.AreaIds.Contains(a.Id)).ToListAsync();

            string? imagePath = null;

            // Handle image upload manually from Request.Form.Files
            if (Request.Form.Files.Count > 0)
            {
                IFormFile? imageFile = Request.Form.Files[0];
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

            var mentor = new Mentor
            {
                Name = vm.Name,
                Surname = vm.Surname,
                TypeOfWorkId = vm.TypeOfWorkId,
                Areas = areas,
                ImagePath = imagePath
            };

            try
            {
                _context.Mentors.Add(mentor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Failed to create mentor: {ex.Message}");

                ViewData["TypeOfWorkId"] = new SelectList(_context.TypeOfWorks, "Id", "Name");
                ViewData["AreaIds"] = new MultiSelectList(_context.Areas, "Id", "Name");
                return View(vm);
            }
        }

        // GET: MentorsController/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var mentor = await _context.Mentors
                .Include(m => m.Areas)
                .Include(m => m.TypeOfWork)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (mentor == null) 
                return NotFound();

            var vm = new MentorViewModel
            {
                Id = mentor.Id,
                Name = mentor.Name,
                Surname = mentor.Surname,
                TypeOfWorkId = mentor.TypeOfWork.Id,
                AreaIds = mentor.Areas.Select(a => a.Id).ToList(),
                AreaNames = mentor.Areas.Select(a => a.Name).ToList(),
                ImagePath = mentor.ImagePath
            };

            ViewData["TypeOfWorkId"] = new SelectList(_context.TypeOfWorks, "Id", "Name");
            ViewData["AreaIds"] = new MultiSelectList(_context.Areas, "Id", "Name");

            return View(vm);
        }

        // POST: MentorsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MentorViewModel vm)
        {
            if (id != vm.Id) 
                return BadRequest();

            if (!ModelState.IsValid)
            {
                ViewData["TypeOfWorkId"] = new SelectList(_context.TypeOfWorks, "Id", "Name");
                ViewData["AreaIds"] = new MultiSelectList(_context.Areas, "Id", "Name");

                return View(vm);
            }

            var duplicate = await _context.Mentors
                .AnyAsync(m => m.Id != id && m.Name == vm.Name && m.Surname == vm.Surname);
            
            if (duplicate)
            {
                ModelState.AddModelError("", "Another mentor with this name already exists.");
                
                ViewData["TypeOfWorkId"] = new SelectList(_context.TypeOfWorks, "Id", "Name");
                ViewData["AreaNames"] = new SelectList(_context.Areas, "Id", "Name");
                
                return View(vm);
            }

            var mentor = await _context.Mentors
                .Include(m => m.Areas)
                .FirstOrDefaultAsync(m => m.Id == id);


            if (mentor == null) 
                return NotFound();

            mentor.Name = vm.Name;
            mentor.Surname = vm.Surname;
            mentor.TypeOfWorkId = vm.TypeOfWorkId;
            mentor.Areas = await _context.Areas.Where(a => vm.AreaIds.Contains(a.Id)).ToListAsync();

            // Handle image upload if provided
            var imageFile = Request.Form.Files.FirstOrDefault();
            if (imageFile != null && imageFile.Length > 0)
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

                // Save relative path
                mentor.ImagePath = "/uploads/" + uniqueFileName;
            }

            try
            {
                _context.Update(mentor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                // Log error
                ModelState.AddModelError("", "Failed to update mentor.");

                ViewData["TypeOfWorkId"] = new SelectList(_context.TypeOfWorks, "Id", "Name");
                ViewData["AreaIds"] = new MultiSelectList(_context.Areas, "Id", "Name");
                return View(vm);
            }
        }

        // GET: MentorsController/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var mentor = await _context.Mentors
                .Include(m => m.Areas)
                .Include(m => m.TypeOfWork)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (mentor == null) 
                return NotFound();

            var vm = new MentorViewModel
            {
                Id = mentor.Id,
                Name = mentor.Name,
                Surname = mentor.Surname,
                TypeOfWorkId = mentor.TypeOfWork.Id,
                TypeOfWorkName = mentor.TypeOfWork.Name,
                AreaNames = mentor.Areas.Select(a => a.Name).ToList(),
                AreaIds = mentor.Areas.Select(a => a.Id).ToList()
            };

            return View(vm);
        }


        // POST: MentorsController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var mentor = await _context.Mentors
                .Include(m => m.Areas)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (mentor == null) 
                return NotFound();

            try
            {
                mentor.Areas.Clear();
                await _context.SaveChangesAsync();

                _context.Mentors.Remove(mentor);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ModelState.AddModelError("", "Failed to delete mentor.");
                return View();
            }
        }
    }
}
