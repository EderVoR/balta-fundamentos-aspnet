using Blog.Data;
using Blog.ViewModels;
using Blog.ViewModels.Posts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Controllers
{
	[ApiController]
	public class PostController : ControllerBase
	{
		[HttpGet("v1/posts")]
		public async Task<IActionResult> GetPosts(
			[FromServices] BlogDataContext context,
			[FromQuery] int page = 0, 
			[FromQuery] int pageSize = 25)
		{
			try
			{
				var count = await context.Posts.CountAsync();
				var posts = await context
					.Posts
					.AsNoTracking()
					.Include(x => x.Author)
					.Include(x => x.Category)
					.Select(x => new PostViewModel
					{
						Id = x.Id,
						Title = x.Title,
						Slug = x.Slug,
						CreateDate = x.CreateDate,
						Author = x.Author.Name,
						Category = x.Category.Name
					})
					.Skip(page * pageSize)
					.Take(pageSize)
					.OrderBy(x => x.Id)
					.ToListAsync();

				return Ok(new ResultViewModel<dynamic>(new
				{
					total = count,
					page,
					pageSize,
					posts
				}));
			}
			catch (Exception ex)
			{
				return BadRequest(new ResultViewModel<string>("Falha interna no servidor"));
			}
		}
	}
}
