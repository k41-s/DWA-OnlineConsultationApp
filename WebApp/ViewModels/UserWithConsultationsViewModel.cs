namespace WebApp.ViewModels
{
    public class UserWithConsultationsViewModel
    {
        public string Email { get; set; } = string.Empty;
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? Role { get; set; }

        public List<ConsultationInfoViewModel> Consultations { get; set; } = new();
    }

    public class ConsultationInfoViewModel
    {
        public DateTime RequestedAt { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public string MentorName { get; set; } = string.Empty;
        public string MentorSurname { get; set; } = string.Empty;
    }
}
