using Blog.Data;
using Blog.Extensions;
using Blog.Models;
using Blog.Services;
using Blog.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureIdentity.Password;

namespace Blog.Controllers
{
	[ApiController]
	public class AccountController : ControllerBase
	{
		[HttpPost("v1/account/login")]
		public async Task<IActionResult> Login([FromServices] TokenService tokenService,
			[FromBody] LoginViewModel model, [FromServices] BlogDataContext context)
		{
			if (!ModelState.IsValid)
				return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));

			var user = await context.Users
				.AsNoTracking()
				.Include(s => s.Roles)
				.FirstOrDefaultAsync(x => x.Email == model.Email);

			if (user == null)
				return StatusCode(401, new ResultViewModel<string>("Usuário ou senha inválido"));

			if (!PasswordHasher.Verify(user.PasswordHash, model.Password))
				return StatusCode(401, new ResultViewModel<string>("Usuário ou senha inválido"));

			try
			{
				var token = tokenService.GenerateToken(user);
				return Ok(new ResultViewModel<string>(token, null));
			}
			catch
			{
				return StatusCode(500, new ResultViewModel<string>("Falha interna no servidor"));
			}
		}

		[HttpPost("v1/account")]
		public async Task<IActionResult> Register([FromBody] RegisterViewModel model,
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

			var password = PasswordGenerator.Generate(25);
			user.PasswordHash = PasswordHasher.Hash(password);

			try
			{
				await context.Users.AddAsync(user);
				await context.SaveChangesAsync();

				return Ok(new ResultViewModel<dynamic>(new
				{
					email = user.Email, password
				}));
			}
			catch (DbUpdateException)
			{
				return StatusCode(400, new ResultViewModel<string>("Email já cadastrado"));
			}
			catch(Exception)
			{
				return StatusCode(500, new ResultViewModel<string>("Erro interno no banco de dados"));
			}
		}
	}
}
