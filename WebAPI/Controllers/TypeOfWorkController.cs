using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineConsultationApp.core.DTOs;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [Route("api/typeOfWork")]
    [ApiController]
    public class TypeOfWorkController : ControllerBase
    {
        private readonly ConsultationsContext _context;
        private readonly IMapper _mapper;

        public TypeOfWorkController(ConsultationsContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/typeOfWork
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TypeOfWorkDTO>>> GetAll()
        {
            var types = await _context.TypeOfWorks.ToListAsync();
            var dtos = _mapper.Map<List<TypeOfWorkDTO>>(types);
            return Ok(dtos);
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

                var dto = _mapper.Map<TypeOfWorkDTO>(type);

                return Ok(dto);
            }
            catch (Exception)
            {
                return StatusCode(500, "An unexpected error occurred while retrieving the type of work.");
            }
        }

        // POST api/typeOfWork
        [HttpPost]
        public async Task<ActionResult<TypeOfWorkDTO>> Post([FromBody] TypeOfWorkCreateDTO dto)
        {
            try
            {
                var type = _mapper.Map<TypeOfWork>(dto);

                _context.TypeOfWorks.Add(type);
                await _context.SaveChangesAsync();

                var resultDto = _mapper.Map<TypeOfWorkDTO>(type);

                return CreatedAtAction(nameof(Get), new { id = type.Id }, resultDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Creation failed: {ex.Message}");
            }
        }

        // PUT api/typeOfWork/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] TypeOfWorkCreateDTO dto)
        {
            try
            {
                if (id <= 0)
                    return BadRequest("Invalid id provided");

                var type = await _context.TypeOfWorks.FindAsync(id);

                if (type == null)
                    return NotFound();

                _mapper.Map(dto, type);

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
            catch (Exception)
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
