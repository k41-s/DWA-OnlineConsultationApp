using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineConsultationApp.core.DTOs;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [Route("api/areas")]
    [ApiController]
    public class AreasController : ControllerBase
    {
        private readonly ConsultationsContext _context;
        private readonly IMapper _mapper;

        public AreasController(ConsultationsContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/areas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AreaDTO>>> GetAll()
        {
            var areas = await _context.Areas.ToListAsync();
            var areaDtos = _mapper.Map<List<AreaDTO>>(areas);
            return Ok(areaDtos);
        }

        // GET api/areas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AreaDTO>> Get(int id)
        {
            try
            {
                var area = await _context.Areas.FindAsync(id);
                if (area == null)
                    return NotFound();

                var areaDto = _mapper.Map<AreaDTO>(area);
                return Ok(areaDto);
            }
            catch (Exception)
            {
                return StatusCode(500, "An unexpected error occurred while retrieving the area.");
            }
        }

        // POST api/areas
        [HttpPost]
        public async Task<ActionResult<AreaDTO>> Post([FromBody] AreaCreateDTO areaCreateDto)
        {
            try
            {
                var area = _mapper.Map<Area>(areaCreateDto);
                _context.Areas.Add(area);
                await _context.SaveChangesAsync();

                var resultDto = _mapper.Map<AreaDTO>(area);
                return CreatedAtAction(nameof(Get), new { id = area.Id }, resultDto);
            }
            catch (Exception)
            {
                return StatusCode(500, "Creation failed.");
            }
        }

        // PUT api/areas/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] AreaCreateDTO areaDto)
        {
            try
            {
                if (id <= 0)
                    return BadRequest("Invalid id provided");

                var area = await _context.Areas.FindAsync(id);
                if (area == null)
                    return NotFound();

                // Map changes from DTO to entity
                _mapper.Map(areaDto, area);

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
            catch (Exception)
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
                var area = await _context.Areas
                    .Include(a => a.Mentors)
                    .FirstOrDefaultAsync(a => a.Id == id);

                if (area == null)
                    return NotFound($"Area with ID {id} not found.");

                area.Mentors.Clear();
                await _context.SaveChangesAsync();

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
