using System.ComponentModel.DataAnnotations;

namespace Blog.ViewModels
{
	public class LoginViewModel
	{
        [Required(ErrorMessage = "Email nessario preencher")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Necessario preencher a senha")]
        public string Password { get; set; }
    }
}
