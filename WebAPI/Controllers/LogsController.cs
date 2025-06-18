using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [Route("api/logs")]
    [ApiController]
    public class LogsController : ControllerBase
    {
        private readonly ConsultationsContext _context;

        public LogsController(ConsultationsContext context)
        {
            _context = context;
        }

        // GET: api/logs/get/10
        [HttpGet("get/{n}")]
        public async Task<IActionResult> GetLastLogs(int n)
        {
            if (n <= 0) return BadRequest("N must be greater than 0.");

            var logs = await _context.Logs
                .OrderByDescending(l => l.Timestamp)
                .Take(n)
                .ToListAsync();

            return Ok(logs);
        }

        // GET: api/logs/count
        [HttpGet("count")]
        public async Task<IActionResult> GetLogCount()
        {
            var count = await _context.Logs.CountAsync();
            return Ok(count);
        }
    }
}
