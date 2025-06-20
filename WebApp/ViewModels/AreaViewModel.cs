using System.ComponentModel.DataAnnotations;

namespace WebApp.ViewModels
{
    public class AreaViewModel
    {
        public int Id { get; set; }


        [Required(ErrorMessage = "Area name is required.")]
        [Display(Name = "Area Name")]
        public string Name { get; set; }

    }
}
