using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineConsultationApp.core.DTOs;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [Authorize]
    [Route("api/mentors")]
    [ApiController]
    public class MentorsController : ControllerBase
    {
        private readonly ConsultationsContext _context;
        private readonly IMapper _mapper;

        public MentorsController(ConsultationsContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        private async Task AddLogAsync(string level, string message)
        {
            var logEntry = new Log
            {
                Level = level,
                Message = message,
                Timestamp = DateTime.UtcNow
            };
            _context.Logs.Add(logEntry);
            await _context.SaveChangesAsync();
        }

        // GET: api/mentors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MentorDTO>>> GetMentors()
        {
            var mentors = await _context.Mentors
                .Include(m => m.TypeOfWork)
                .Include(m => m.Areas)
                .ToListAsync();

            var mentorsDto = _mapper.Map<List<MentorDTO>>(mentors);

            return Ok(mentorsDto);
        }

        // GET api/mentors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MentorDTO>> GetMentor(int id)
        {
            try
            {
                var mentor = await _context.Mentors
                    .Include(m => m.TypeOfWork)
                    .Include(m => m.Areas)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (mentor == null)
                {
                    await AddLogAsync("Warning", $"Mentor with id={id} not found.");
                    return NotFound();
                }

                var mentorDto = _mapper.Map<MentorDTO>(mentor);

                await AddLogAsync("Information", $"Mentor with id={id} retrieved.");
                return Ok(mentorDto);
            }
            catch (Exception ex)
            {
                await AddLogAsync("Error", $"Exception in GetMentor: {ex.Message}");
                return StatusCode(500, "An unexpected error occurred while retrieving the mentor.");
            }
        }

        // POST api/mentors
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<MentorDTO>> CreateMentor([FromBody] MentorCreateDTO dto)
        {
            try
            {
                var typeOfWork = await _context.TypeOfWorks.FindAsync(dto.TypeOfWorkId);
                if (typeOfWork == null)
                    return BadRequest("Invalid TypeOfWork ID.");

                var areas = await _context.Areas
                    .Where(a => dto.AreaIds.Contains(a.Id))
                    .ToListAsync();

                var mentor = _mapper.Map<Mentor>(dto);
                mentor.Areas = areas;

                _context.Mentors.Add(mentor);
                await _context.SaveChangesAsync();

                await AddLogAsync("Information", $"Mentor with id={mentor.Id} created.");

                var resultDto = _mapper.Map<MentorDTO>(mentor);

                return CreatedAtAction(nameof(GetMentor), new { id = mentor.Id }, resultDto);
            }
            catch (Exception ex)
            {
                await AddLogAsync("Error", $"Error creating mentor: {ex.Message}");
                return StatusCode(500, "Creation failed.");
            }
        }

        // PUT api/mentors/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateMentor(int id, [FromBody] MentorCreateDTO dto)
        {
            if (id <= 0)
                return BadRequest("Invalid mentor ID.");

            var mentor = await _context.Mentors
                .Include(m => m.Areas)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (mentor == null)
            {
                await AddLogAsync("Warning", $"Mentor with id={id} not found during update.");
                return NotFound();
            }

            var typeOfWork = await _context.TypeOfWorks.FindAsync(dto.TypeOfWorkId);
            if (typeOfWork == null)
                return BadRequest("Invalid user or type of work ID.");

            var areas = await _context.Areas
                .Where(a => dto.AreaIds.Contains(a.Id))
                .ToListAsync();

            mentor.Name = dto.Name;
            mentor.Surname = dto.Surname;
            mentor.TypeOfWorkId = dto.TypeOfWorkId;
            mentor.Areas = areas;
            mentor.ImagePath = dto.ImagePath;

            // for some reason this threw errors
            //mentor = _mapper.Map<Mentor>(dto);

            if (id != mentor.Id)
            {
                await AddLogAsync("Warning", $"Update failed: Mentor ID mismatch (url={id}, body={mentor.Id}).");
                return BadRequest();
            }

            _context.Entry(mentor).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                await AddLogAsync("Information", $"Mentor with id={id} updated.");
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Mentors.Any(e => e.Id == id))
                {
                    await AddLogAsync("Warning", $"Mentor with id={id} not found during update.");
                    return NotFound();
                }
                else
                {
                    await AddLogAsync("Error", $"Concurrency error while updating mentor id={id}.");
                    throw;
                }
            }
            catch (Exception ex)
            {
                await AddLogAsync("Error", $"Error updating mentor id={id}: {ex.Message}");
                return StatusCode(500, "Update failed.");
            }
        }

        // DELETE api/mentors/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteMentor(int id)
        {
            var mentor = await _context.Mentors
                .Include(m => m.Areas)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (mentor == null)
            {
                await AddLogAsync("Warning", $"Mentor with id={id} not found for deletion.");
                return NotFound();
            }

            try
            {
                // remove foreign keys first
                mentor.Areas.Clear();
                await _context.SaveChangesAsync();

                _context.Mentors.Remove(mentor);
                await _context.SaveChangesAsync();

                await AddLogAsync("Information", $"Mentor with id={id} deleted.");
                return NoContent();
            }
            catch (Exception ex)
            {
                await AddLogAsync("Error", $"Error deleting mentor");
                return StatusCode(500, $"An unexpected error occurred: {ex.Message}");
            }
        }

        // GET: api/mentors/search?query=John&page=1&count=10
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<MentorDTO>>> SearchMentors(
            string? name = null,
            int? typeOfWorkId = null,
            int page = 1,
            int pageSize = 10)
        {
            try
            {
                if (page <= 0) page = 1;
                if (pageSize <= 0) pageSize = 10;

                if (_context.Mentors == null)
                    return NotFound("Mentor dataset not found.");

                var mentorsQuery = _context.Mentors
                    .Include(m => m.Areas)
                    .Include(m => m.TypeOfWork)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(name))
                {
                    string nameLower = name.ToLower();
                    mentorsQuery = mentorsQuery.Where(m =>
                        (m.Name != null && m.Name.ToLower().Contains(nameLower)) ||
                        (m.Surname != null && m.Surname.ToLower().Contains(nameLower))
                    );
                }

                if (typeOfWorkId.HasValue && typeOfWorkId > 0)
                {
                    mentorsQuery = mentorsQuery.Where(m => m.TypeOfWorkId == typeOfWorkId.Value);
                }

                var total = await mentorsQuery.CountAsync();

                var mentors = await mentorsQuery
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var mentorsDto = _mapper.Map<List<MentorDTO>>(mentors);

                // Log successful search
                await AddLogAsync("Information", $"Mentor search performed with name filter '{name}', page {page}, count {pageSize}.");

                // Return paged result, optionally include total count in response headers or body
                Response.Headers.Append("X-Total-Count", total.ToString());

                return Ok(mentorsDto);
            }
            catch (Exception ex)
            {
                // Log error
                await AddLogAsync("Error", $"Error during mentor search: {ex.Message}");
                return StatusCode(500, "An error occurred while searching mentors.");
            }
        }
    }
}
