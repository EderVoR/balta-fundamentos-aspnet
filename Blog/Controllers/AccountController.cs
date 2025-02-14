using Blog.Data;
using Blog.Extensions;
using Blog.Models;
using Blog.Services;
using Blog.ViewModels;
using Blog.ViewModels.Accounts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureIdentity.Password;
using System.Text.RegularExpressions;

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
			[FromServices] BlogDataContext context, [FromServices] EmailService emailService)
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

				emailService.Send(user.Name, user.Email, "Bem vindo", $"Sua senha é {password}");


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

		//[Authorize]
		[HttpPost("v1/accounts/upload-image")]
		public async Task<IActionResult> UploadImage([FromBody] UploadViewModel model,
			[FromServices] BlogDataContext context)
		{
			var fileName = $"{Guid.NewGuid().ToString()}.jpg";
			var data = new Regex(@"^data:imag\/[a-z]+;base64,").Replace(model.Base64Image, "");
			var bytes = Convert.FromBase64String(data);

			try
			{
				await System.IO.File.WriteAllBytesAsync($"wwwroot/images/{fileName}", bytes);
			}
			catch(Exception ex)
			{
				return StatusCode(500, new ResultViewModel<string>("Erro interno de servidor"));
			}

			var user = await context.Users
				.FirstOrDefaultAsync(x => x.Email == User.Identity.Name);

			if (user == null)
				return NotFound(new ResultViewModel<string>("Usuário não encontrado"));

			user.Image = $"https:localhost/images/{fileName}";

			try
			{
				context.Users.Update(user);
				await context.SaveChangesAsync();
			}
			catch(Exception ex)
			{
				return StatusCode(500, new ResultViewModel<string>("Erro inteno no servidor"));
			}

			return Ok(new ResultViewModel<string>("Imagem salva com sucesso"));
		}
	}
}
