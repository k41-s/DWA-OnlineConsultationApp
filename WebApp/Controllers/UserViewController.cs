using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;
using WebApp.ViewModels;

namespace WebApp.Controllers
{
    [Authorize]
    public class UserViewController : Controller
    {
        private readonly ConsultationsContext _context;

        public UserViewController(ConsultationsContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string search, int? typeOfWorkId, int page = 1)
        {
            int pageSize = 10;

            var query = _context.Mentors
                .Include(m => m.TypeOfWork)
                .Include(m => m.Areas)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(m => m.Name.Contains(search) || m.Surname.Contains(search));
            }

            if (typeOfWorkId.HasValue)
            {
                query = query.Where(m => m.TypeOfWorkId == typeOfWorkId);
            }

            var totalCount = await query.CountAsync();

            var mentors = await query
                .OrderBy(m => m.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var mentorVMs = mentors.Select(m => new MentorViewModel
            {
                Id = m.Id,
                Name = m.Name,
                Surname = m.Surname,
                TypeOfWorkId = m.TypeOfWorkId,
                TypeOfWorkName = m.TypeOfWork?.Name ?? "",
                ImagePath = m.ImagePath,
                AreaNames = m.Areas.Select(a => a.Name).ToList()
            }).ToList();

            ViewData["CurrentSearch"] = search;
            ViewData["CurrentTypeOfWorkId"] = typeOfWorkId;
            ViewData["TotalPages"] = (int)Math.Ceiling((double)totalCount / pageSize);
            ViewData["CurrentPage"] = page;
            ViewData["TypeOfWorkList"] = new SelectList(_context.TypeOfWorks, "Id", "Name");

            return View(mentorVMs);
        }

        public async Task<IActionResult> MentorDetails(int id)
        {
            var mentor = await _context.Mentors
                .Include(m => m.TypeOfWork)
                .Include(m => m.Areas)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (mentor == null)
                return NotFound();

            var vm = new MentorViewModel
            {
                Id = mentor.Id,
                Name = mentor.Name,
                Surname = mentor.Surname,
                TypeOfWorkId = mentor.TypeOfWorkId,
                TypeOfWorkName = mentor.TypeOfWork?.Name ?? "",
                ImagePath = mentor.ImagePath,
                AreaIds = mentor.Areas.Select(a => a.Id).ToList(),
                AreaNames = mentor.Areas.Select(a => a.Name).ToList()
            };

            return View(vm);
        }

        // GET: UserView/BookConsultation/5
        [HttpGet]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> BookConsultation(int id)
        {
            var mentor = await _context.Mentors.FindAsync(id);
            if (mentor == null) return NotFound();

            var vm = new ConsultationViewModel
            {
                MentorId = mentor.Id,
                MentorName = mentor.Name + " " + mentor.Surname
            };

            return View(vm);
        }

        // POST: UserView/BookConsultation
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> BookConsultation(ConsultationViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userEmail = User.Identity?.Name;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            if (user == null) return Unauthorized();

            var consultation = new Consultation
            {
                MentorId = model.MentorId,
                UserId = user.Id,
                RequestedAt = DateTime.Now,
                Status = "Pending",
                Notes = model.Notes
            };

            _context.Consultations.Add(consultation);
            await _context.SaveChangesAsync();

            return RedirectToAction("MentorDetails", new { id = model.MentorId });
        }

        // Hide book button if already booked
        // make a page to view booked consultations
        // implement 7. admin view of users and their bookings
    }
}
