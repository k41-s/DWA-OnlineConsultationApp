using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;
using WebApp.ViewModels;

namespace WebApp.Controllers
{
    [Authorize]
    public class ConsultationViewController : Controller
    {
        private readonly ConsultationsContext _context;

        public ConsultationViewController(ConsultationsContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UsersWithConsultations()
        {
            var users = await _context.Users
                .Include(u => u.Consultations)
                    .ThenInclude(c => c.Mentor)
                .ToListAsync();

            var vmList = users.Select(u => new UserWithConsultationsViewModel
            {
                Email = u.Email,
                Name = u.Name,
                Surname = u.Surname,
                Role = u.Role,
                Consultations = u.Consultations.Select(c => new ConsultationInfoViewModel
                {
                    RequestedAt = c.RequestedAt,
                    Status = c.Status,
                    Notes = c.Notes,
                    MentorName = c.Mentor.Name,
                    MentorSurname = c.Mentor.Surname
                }).ToList()
            }).ToList();

            return View(vmList);
        }

        [HttpGet]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> MyConsultations()
        {
            var email = User.Identity?.Name;

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
                return Unauthorized();

            var consultations = await _context.Consultations
                .Include(c => c.Mentor)
                .Where(c => c.UserId == user.Id)
                .OrderByDescending(c => c.RequestedAt)
                .ToListAsync();

            var vms = consultations.Select(c => new MyConsultationViewModel
            {
                RequestedAt = c.RequestedAt,
                Status = c.Status,
                Notes = c.Notes,
                MentorName = c.Mentor.Name,
                MentorSurname = c.Mentor.Surname,
                MentorImagePath = c.Mentor.ImagePath
            }).ToList();

            return View(vms);
        }
    }
}
