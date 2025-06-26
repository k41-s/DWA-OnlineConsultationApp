using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineConsultationApp.core.DTOs
{
    public class ConsultationInfoDTO
    {
        public DateTime RequestedAt { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public string MentorName { get; set; } = string.Empty;
        public string MentorSurname { get; set; } = string.Empty;
    }
}
