using System.ComponentModel.DataAnnotations;

namespace Blog.ViewModels.Accounts
{
	public class UploadViewModel
	{
		[Required(ErrorMessage = "Imagem inválida")]
        public string Base64Image { get; set; }
    }
}
