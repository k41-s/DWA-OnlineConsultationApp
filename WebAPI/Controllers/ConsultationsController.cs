using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using OnlineConsultationApp.core.DTOs;
using WebAPI.Models;
using AutoMapper;

namespace WebAPI.Controllers
{
    [Authorize]
    [Route("api/consultations")]
    [ApiController]
    public class ConsultationsController : ControllerBase
    {
        private readonly ConsultationsContext _context;
        private readonly IMapper _mapper;

        public ConsultationsController(ConsultationsContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // POST: api/consultations/create
        [HttpPost("create")]
        public async Task<IActionResult> CreateConsultation([FromBody] ConsultationCreateDTO dto)
        {
            var mentor = await _context.Mentors.FindAsync(dto.MentorId);
            if (mentor == null)
                return NotFound("Mentor not found");

            var consultation = _mapper.Map<Consultation>(dto);

            _context.Consultations.Add(consultation);
            await _context.SaveChangesAsync();

            return Ok("Consultation requested.");
        }

        // GET: api/consultations/user/5
        [HttpGet("user/{userId}")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult<IEnumerable<ConsultationDTO>>> GetUserConsultations(int userId)
        {
            var consultations = await _context.Consultations
                .Include(c => c.Mentor)
                .Include(c => c.User)
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.RequestedAt)
                .Select(c => new ConsultationDTO
                {
                    Id = c.Id,
                    MentorId = c.MentorId,
                    MentorName = $"{c.Mentor.Name} {c.Mentor.Surname}",
                    UserId = c.UserId,
                    UserName = $"{c.User.Name} {c.User.Surname}",
                    RequestedAt = c.RequestedAt,
                    Status = c.Status,
                    Notes = c.Notes
                })
                .ToListAsync();

            return Ok(consultations);
        }

        // GET: api/consultations/admin
        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<ConsultationDTO>>> GetAllConsultations()
        {
            var consultations = await _context.Consultations
                .Include(c => c.Mentor)
                .Include(c => c.User)
                .OrderByDescending(c => c.RequestedAt)
                .Select(c => new ConsultationDTO
                {
                    Id = c.Id,
                    MentorId = c.MentorId,
                    MentorName = $"{c.Mentor.Name} {c.Mentor.Surname}",
                    UserId = c.UserId,
                    UserName = $"{c.User.Name} {c.User.Surname}",
                    RequestedAt = c.RequestedAt,
                    Status = c.Status,
                    Notes = c.Notes
                })
                .ToListAsync();

            return Ok(consultations);
        }
    }
}
