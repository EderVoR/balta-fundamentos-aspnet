using Blog.Data;
using Blog.Extensions;
using Blog.Models;
using Blog.ViewModels;
using Blog.ViewModels.Categories;
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
			try
			{
				var categories = await context.Categories.ToListAsync();
				return Ok(new ResultViewModel<List<Category>>(categories));
			}
			catch
			{
				return StatusCode(500, new ResultViewModel<List<Category>>("Falha interna no servidor"));
			}
		}

		[HttpGet("v1/categories/{id:int}")]
		public async Task<IActionResult> GetByIdAsync(
			[FromRoute] int id,
			[FromServices] BlogDataContext context)
		{
			try
			{
				var category = await context.Categories.FirstOrDefaultAsync(s => s.Id == id);

				if (category == null)
					return NotFound(new ResultViewModel<Category>("Não foi possível localizar a categoria"));

				return Ok(new ResultViewModel<Category>(category));
			}
			catch
			{
				return StatusCode(500, new ResultViewModel<Category>("Erro interno no servidor"));
			}
		}

		[HttpPost("v1/categories")]
		public async Task<IActionResult> PostAsync(
			[FromBody] EditorCategoryViewModel model, [FromServices] BlogDataContext context
			)
		{
			try
			{
				if (!ModelState.IsValid)
					return BadRequest(new ResultViewModel<Category>(ModelState.GetErrors()));

				var category = new Category
				{
					Id = 2,
					Name = model.Name,
					Slug = model.Slug,
					Posts = null
				};

				await context.Categories.AddAsync(category);
				await context.SaveChangesAsync();

				return Created($"v1/categories/{category.Id}", new ResultViewModel<Category>(category));

			}
			catch (Exception ex)
			{
				return StatusCode(500, new ResultViewModel<Category>("Erro interno no servidor"));
			}
		}

		[HttpPut("v1/categories/{id:int}")]
		public async Task<IActionResult> PutAsync(
			[FromRoute] int id, [FromBody] EditorCategoryViewModel category, [FromServices] BlogDataContext context
			)
		{
			try
			{
				var categoria = await context.Categories
					.FirstOrDefaultAsync(x => x.Id == id);

				if (categoria == null)
					return NotFound(new ResultViewModel<Category>("Categoria não localizada"));

				categoria.Name = category.Name;
				categoria.Slug = category.Slug;

				context.Categories.Update(categoria);
				await context.SaveChangesAsync();

				return Ok(new ResultViewModel<Category>(categoria));

			}
			catch (Exception ex)
			{
				return StatusCode(500, new ResultViewModel<Category>("05X01 - Não foi possível editar a categoria"));
			}
		}

		[HttpDelete("v1/categories/{id:int}")]
		public async Task<IActionResult> DeleteAsync(
			[FromRoute] int id, [FromServices] BlogDataContext context
			)
		{
			try
			{
				var category = await context.Categories.FirstOrDefaultAsync(c => c.Id == id);

				if (category == null)
					return NotFound(new ResultViewModel<Category>("Categoia não localizada"));

				context.Categories.Remove(category);
				await context.SaveChangesAsync();

				return Ok(new ResultViewModel<Category>(category));
			}
			catch
			{
				return StatusCode(500, new ResultViewModel<Category>("Erro interno no servidor"));
			}
		}
	}
}
