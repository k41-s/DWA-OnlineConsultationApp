using System.ComponentModel.DataAnnotations;

namespace WebApp.ViewModels
{
    public class ConsultationViewModel
    {
        public int MentorId { get; set; }

        public string MentorName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter a note.")]
        [Display(Name = "Consultation Notes")]
        public string? Notes { get; set; }
        
    }
}
