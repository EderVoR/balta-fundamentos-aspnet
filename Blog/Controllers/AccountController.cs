using Blog.Data;
using Blog.Extensions;
using Blog.Models;
using Blog.Services;
using Blog.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers
{
	[ApiController]
	public class AccountController : ControllerBase
	{
		[HttpPost("v1/account/login")]
		public IActionResult Login([FromServices] TokenService tokenService)
		{
			var token = tokenService.GenerateToken(null);
			return Ok(token);
		}

		[HttpPost("v1/account/register")]
		public async Task<IActionResult> Register([FromRoute] User model,
			[FromServices] BlogDataContext context)
		{
			if(!ModelState.IsValid)
				return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));

			var user = new User
			{
				Name = model.Name,
				Email = model.Email,
				Slug = model.Email.Replace('@', '-').Replace('.', '-')
			};

			await context.Users.AddAsync(user);
			await context.SaveChangesAsync();

			return Created("", new ResultViewModel<User>(user));
		}
	}
}
