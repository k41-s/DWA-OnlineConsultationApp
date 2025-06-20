using System.ComponentModel.DataAnnotations;

namespace WebApp.ViewModels
{
    public class MentorViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Surname { get; set; }
        [Required]
        public int TypeOfWorkId { get; set; }
        public string TypeOfWorkName { get; set; } = string.Empty;
        [Required]
        public List<int> AreaIds { get; set; } = new();
        public List<string> AreaNames { get; set; } = new();
    }
}
