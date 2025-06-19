using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using WebAPI.DTOs;
using WebAPI.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebAPI.Controllers
{
    [Route("api/mentors")]
    [ApiController]
    public class MentorsController : ControllerBase
    {
        private readonly ConsultationsContext _context;

        public MentorsController(ConsultationsContext context)
        {
            _context = context;
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
        public async Task<ActionResult<IEnumerable<Mentor>>> GetMentors()
        {
            return await _context.Mentors.Include(m => m.TypeOfWork).ToListAsync();
        }

        // GET api/mentors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Mentor>> GetMentor(int id)
        {
            try
            {
                Mentor? mentor
                        = await _context.Mentors
                        .Include(m => m.User)
                        .Include(m => m.Areas)
                        .Include(m => m.TypeOfWorkId)
                        .FirstOrDefaultAsync(m => m.Id == id);

                if (mentor == null)
                {
                    await AddLogAsync("Warning", $"Mentor with id={id} not found.");
                    return NotFound();
                }

                await AddLogAsync("Information", $"Mentor with id={id} retrieved.");
                return Ok(mentor);
            }
            catch (Exception ex)
            {
                await AddLogAsync("Error", $"Exception in GetMentor: {ex.Message}");
                return StatusCode(500, "An unexpected error occurred while retrieving the mentor.");
            }
        }

        // POST api/mentors
        [HttpPost]
        public async Task<ActionResult<Mentor>> CreateMentor([FromBody]MentorCreateDTO dto)
        {
            try
            {
                var user = await _context.Users.FindAsync(dto.UserId);
                var typeOfWork = await _context.TypeOfWorks.FindAsync(dto.TypeOfWorkId);
                var areas = await _context.Areas.Where(a => dto.AreaIds.Contains(a.Id)).ToListAsync();

                if (user == null || typeOfWork == null)
                    return BadRequest("Invalid user or type of work ID.");

                Mentor mentor = new Mentor
                {
                    Id = dto.UserId, // Assuming UserId = MentorId (1:1)
                    TypeOfWorkId = dto.TypeOfWorkId,
                    Areas = areas
                };

                _context.Mentors.Add(mentor);
                await _context.SaveChangesAsync();

                await AddLogAsync("Information", $"Mentor with id={mentor.Id} created.");
                return CreatedAtAction(nameof(GetMentor), new { id = mentor.Id }, mentor);
            }
            catch (Exception ex)
            {
                await AddLogAsync("Error", $"Error creating mentor: {ex.Message}");
                return StatusCode(500, "Creation failed.");
            }
        }

        // PUT api/mentors/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMentor(int id, [FromBody]MentorCreateDTO dto)
        {
            var user = await _context.Users.FindAsync(dto.UserId);
            var typeOfWork = await _context.TypeOfWorks.FindAsync(dto.TypeOfWorkId);
            var areas = await _context.Areas.Where(a => dto.AreaIds.Contains(a.Id)).ToListAsync();

            if (user == null || typeOfWork == null)
                return BadRequest("Invalid user or type of work ID.");

            Mentor mentor = new Mentor
            {
                Id = dto.UserId, // Assuming UserId = MentorId (1:1)
                TypeOfWorkId = dto.TypeOfWorkId,
                Areas = areas
            };

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
        public async Task<IActionResult> DeleteMentor(int id)
        {
            var mentor = await _context.Mentors.FindAsync(id);
            if (mentor == null)
            {
                await AddLogAsync("Warning", $"Mentor with id={id} not found for deletion.");
                return NotFound();
            }

            try
            {
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
        public async Task<ActionResult<IEnumerable<Mentor>>> SearchMentors(
            string? name = null,
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
                    .Include(m => m.User)
                    .Include(m => m.TypeOfWork)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(name))
                {
                    string nameLower = name.ToLower();
                    mentorsQuery = mentorsQuery.Where(m =>
                        m.User.Name.ToLower().Contains(nameLower) ||
                        m.User.Surname.ToLower().Contains(nameLower)
                    );
                }

                var total = await mentorsQuery.CountAsync();

                List<Mentor> mentors = await mentorsQuery
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                // Log successful search
                await AddLogAsync("Information", $"Mentor search performed with name filter '{name}', page {page}, count {pageSize}.");


                // Return paged result, optionally include total count in response headers or body
                Response.Headers.Append("X-Total-Count", total.ToString());

                return Ok(mentors);
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
