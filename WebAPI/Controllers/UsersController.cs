using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineConsultationApp.core.DTOs;
using WebAPI.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;

namespace WebAPI.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ConsultationsContext _context;
        private readonly IMapper _mapper;

        public UsersController(ConsultationsContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/users/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> GetUserById(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return NotFound();

            var dto = _mapper.Map<UserDTO>(user);
            return Ok(dto);
        }


        // GET api/users/byemail/user@example.com
        [HttpGet("byemail/{email}")]
        public async Task<ActionResult<UserDTO>> GetUserByEmail(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null) return NotFound();

            var dto = _mapper.Map<UserDTO>(user);
            return Ok(dto);
        }

        // PUT api/users/updateprofile/user@example.com
        [HttpPut("updateprofile/{email}")]  
        public async Task<IActionResult> UpdateUserProfile(string email, [FromBody] UserCreateDTO dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null) return NotFound();

            user.Name = dto.Name;
            user.Surname = dto.Surname;
            user.Email = dto.Email;
            user.Phone = dto.Phone;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch
            {
                return StatusCode(500, "Failed to update user.");
            }
        }

        // Get: api/users/with-consultations
        [HttpGet("with-consultations")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<UserWithConsultationsDTO>>> GetUsersWithConsultations()
        {
            var users = await _context.Users
                .Include(u => u.Consultations)
                    .ThenInclude(c => c.Mentor)
                .ToListAsync();

            var dtos = _mapper.Map<List<UserWithConsultationsDTO>>(users);
            return Ok(dtos);
        }

    }
}
