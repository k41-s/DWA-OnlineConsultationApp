using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.DTOs;
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
        public async Task<ActionResult<IEnumerable<TypeOfWorkDTO>>> GetAll()
        {
            var types = await _context.TypeOfWorks
                .Select(t => new TypeOfWorkDTO
                {
                    Id = t.Id,
                    Name = t.Name
                })
                .ToListAsync();

            return Ok(types);
        }

        // GET api/typeOfWork/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TypeOfWorkDTO>> Get(int id)
        {
            try
            {
                var type = await _context.TypeOfWorks.FindAsync(id);

                if (type == null) 
                    return NotFound();

                var dto = new TypeOfWorkDTO
                {
                    Id = type.Id,
                    Name = type.Name
                };

                return Ok(dto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred while retrieving the type of work.");
            }
        }

        // POST api/typeOfWork
        [HttpPost]
        public async Task<ActionResult<TypeOfWorkDTO>> Post([FromBody]TypeOfWorkCreateDTO dto)
        {
            try
            {
                var type = new TypeOfWork
                {
                    Name = dto.Name
                };

                _context.TypeOfWorks.Add(type);
                await _context.SaveChangesAsync();

                var reaultDto = new TypeOfWorkDTO
                {
                    Id = type.Id,
                    Name = type.Name
                };

                return CreatedAtAction(nameof(Get), new { id = type.Id }, reaultDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Creation failed: {ex.Message}");
            }
        }

        // PUT api/typeOfWork/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody]TypeOfWorkCreateDTO dto)
        {
            try
            {
                if (id <= 0) 
                return BadRequest("Invalid id provided");

                var type = await _context.TypeOfWorks.FindAsync(id);

                if (type == null)
                    return NotFound();

                type.Name = dto.Name;

                _context.Entry(type).State = EntityState.Modified;

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.TypeOfWorks.Any(e => e.Id == id)) 
                    return NotFound();
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
