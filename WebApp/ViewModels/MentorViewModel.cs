using System.ComponentModel.DataAnnotations;

namespace WebApp.ViewModels
{
    public class MentorViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        [Display(Name = "First Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [Display(Name = "Last Name")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "TypeOfWork is required.")]
        [Display(Name = "Type of Work")]
        public int TypeOfWorkId { get; set; }

        [Display(Name = "Type of Work")]
        public string TypeOfWorkName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Areas are required")]
        [Display(Name = "Selected Areas")]
        public List<int> AreaIds { get; set; } = new();

        [Display(Name = "Areas of Expertise")]
        public List<string> AreaNames { get; set; } = new();
    }
}
