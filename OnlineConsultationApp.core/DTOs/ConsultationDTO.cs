using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineConsultationApp.core.DTOs
{
    public class ConsultationDTO
    {
        public int Id { get; set; }
        public int MentorId { get; set; }
        public string MentorName { get; set; } = null!;
        public int UserId { get; set; }
        public string UserName { get; set; } = null!;
        public DateTime RequestedAt { get; set; }
        public string Status { get; set; } = null!;
        public string? Notes { get; set; }
    }
    
}
