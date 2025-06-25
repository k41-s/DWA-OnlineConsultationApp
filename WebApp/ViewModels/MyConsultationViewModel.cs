namespace WebApp.ViewModels
{
    public class MyConsultationViewModel
    {
        public DateTime RequestedAt { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Notes { get; set; }

        public string MentorName { get; set; } = string.Empty;
        public string MentorSurname { get; set; } = string.Empty;
        public string? MentorImagePath { get; set; }
    }

}
