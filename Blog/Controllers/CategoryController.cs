using Blog.Data;
using Blog.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Controllers
{
	[ApiController]
	public class CategoryController : ControllerBase
	{
		[HttpGet("v1/categories")]
		public async Task<IActionResult> GetAsync([FromServices] BlogDataContext context)
		{
			var categories = await context.Categories.ToListAsync();
			return Ok(categories);
		}

		[HttpGet("v1/categories/{id:int}")]
		public async Task<IActionResult> GetByIdAsync(
			[FromRoute] int id, 
			[FromServices] BlogDataContext context)
		{
			var category = await context.Categories.FirstOrDefaultAsync(s => s.Id == id);

			if (category == null)
				return NotFound();

			return Ok(category);
		}

		[HttpPost("v1/categories")]
		public async Task<IActionResult> PostAsync(
			[FromBody] Category category, [FromServices] BlogDataContext context
			)
		{
			await context.Categories.AddAsync(category);
			await context.SaveChangesAsync();

			return Ok(category);
		}

		[HttpPut("v1/categories/{id:int}")]
		public async Task<IActionResult> PutAsync(
			[FromRoute] int id, [FromBody] Category category, [FromServices] BlogDataContext context
			)
		{
			var categoria = await context.Categories
				.FirstOrDefaultAsync(x => x.Id == id);

			if (categoria == null)
				return NotFound();

			categoria.Name = category.Name;
			categoria.Slug = category.Slug;

			context.Categories.Update(categoria);
			await context.SaveChangesAsync();

			return Ok(categoria);
		}

		[HttpDelete("v1/categories/{id:int}")]
		public async Task<IActionResult> DeleteAsync(
			[FromRoute] int id, [FromServices] BlogDataContext context
			)
		{
			var category = await context.Categories.FirstOrDefaultAsync(c => c.Id == id);

			if (category == null)
				return NotFound();

			context.Categories.Remove(category);
			await context.SaveChangesAsync();

			return Ok(category);
		}
	}
}
