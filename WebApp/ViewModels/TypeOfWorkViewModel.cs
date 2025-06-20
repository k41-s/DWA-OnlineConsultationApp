using System.ComponentModel.DataAnnotations;

namespace WebApp.ViewModels
{
    public class TypeOfWorkViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Type of work name is required.")]
        [Display(Name = "Type of Work Name")]
        public string Name { get; set; }
    }
}
