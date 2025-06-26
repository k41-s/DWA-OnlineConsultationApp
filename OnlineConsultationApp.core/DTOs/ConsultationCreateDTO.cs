using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineConsultationApp.core.DTOs
{
    public class ConsultationCreateDTO
    {
        public int MentorId { get; set; }
        public int UserId { get; set; }
        public DateTime RequestedAt { get; set; }
        public string Status { get; set; } = null!;
        public string? Notes { get; set; }
    }
}
