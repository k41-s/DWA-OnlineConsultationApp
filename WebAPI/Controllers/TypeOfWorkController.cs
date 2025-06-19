using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebAPI.Controllers
{
    [Route("api/typeOfWork")]
    [ApiController]
    public class TypeOfWorkController : ControllerBase
    {
        private readonly ConsultationsContext _context;

        public TypeOfWorkController(ConsultationsContext context)
        {
            _context = context;
        }

        // GET: api/typeOfWork
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TypeOfWork>>> GetAll()
        {
            return await _context.TypeOfWorks.ToListAsync();
        }

        // GET api/typeOfWork/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TypeOfWork>> Get(int id)
        {
            try
            {
                var type = await _context.TypeOfWorks.FindAsync(id);

                if (type == null) 
                    return NotFound();

                return Ok(type);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred while retrieving the type of work.");
            }
        }

        // POST api/typeOfWork
        [HttpPost]
        public async Task<ActionResult<TypeOfWork>> Post([FromBody]TypeOfWork type)
        {
            try
            {
                _context.TypeOfWorks.Add(type);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(Get), new { id = type.Id }, type);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Creation failed: {ex.Message}");
            }
        }

        // PUT api/typeOfWork/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody]TypeOfWork type)
        {
            if (id != type.Id) return BadRequest();

            _context.Entry(type).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.TypeOfWorks.Any(e => e.Id == id)) return NotFound();
                else throw;
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Update failed.");
            }
        }

        // DELETE api/typeOfWork/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var type = await _context.TypeOfWorks.FindAsync(id);
            if (type == null) return NotFound();

            _context.TypeOfWorks.Remove(type);
            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateException)
            {
                return Conflict("Cannot delete this TypeOfWork due to related Mentors.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An unexpected error occurred: {ex.Message}");
            }
        }
    }
}
