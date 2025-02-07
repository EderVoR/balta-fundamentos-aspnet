using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers
{
	[ApiController]
	[Route("")]
	public class HomeController : ControllerBase
	{
		[HttpGet("")]
		public IActionResult Get()
		{
			return Ok(new
			{
				teste = "Seja bem vindo"
			});
		}

	}
}
