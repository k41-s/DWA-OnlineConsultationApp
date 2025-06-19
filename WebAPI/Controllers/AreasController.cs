using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebAPI.Controllers
{
    [Route("api/areas")]
    [ApiController]
    public class AreasController : ControllerBase
    {
        private readonly ConsultationsContext _context;

        public AreasController(ConsultationsContext context)
        {
            _context = context;
        }

        // GET: api/areas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Area>>> GetAll()
        {
            return await _context.Areas.ToListAsync();
        }

        // GET api/areas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Area>> Get(int id)
        {
            try
            {
                var area = await _context.Areas.FindAsync(id);

                if (area == null) 
                    return NotFound();

                return area;
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred while retrieving the area.");
            }
        }

        // POST api/areas
        [HttpPost]
        public async Task<ActionResult<Area>> Post([FromBody]Area area)
        {
            try
            {
                _context.Areas.Add(area);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(Get), new { id = area.Id }, area);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Creation failed.");
            }
        }

        // PUT api/areas/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody]Area area)
        {
            try
            {
                if (id != area.Id)
                    return BadRequest();

                _context.Entry(area).State = EntityState.Modified;

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Areas.Any(e => e.Id == id))
                    return NotFound();
                else 
                    throw;
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Update failed.");
            }
        }

        // DELETE api/areas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var area = await _context.Areas.FindAsync(id);
                if (area == null) 
                    return NotFound($"TypeOfWork with ID {id} not found.");

                _context.Areas.Remove(area);

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateException)
            {
                return Conflict("Cannot delete this Area due to related Mentors.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An unexpected error occurred: {ex.Message}");
            }
        }
    }
}
