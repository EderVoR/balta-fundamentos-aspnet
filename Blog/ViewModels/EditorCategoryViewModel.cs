using System.ComponentModel.DataAnnotations;

namespace Blog.ViewModels
{
	public class EditorCategoryViewModel
	{
        [Required]
        [StringLength(40, MinimumLength = 3, ErrorMessage = "O campo deve possuir entre 3 e 40 caracteres")]
        public string Name { get; set; }

        [Required(ErrorMessage = "O campo slug é obrigatorio")]
        public string Slug { get; set; }
    }
}
