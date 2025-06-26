using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineConsultationApp.core.DTOs
{
    public class UserWithConsultationsDTO
    {
        public string Email { get; set; } = string.Empty;
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? Role { get; set; }

        public List<ConsultationInfoDTO> Consultations { get; set; } = new();
    }
}
