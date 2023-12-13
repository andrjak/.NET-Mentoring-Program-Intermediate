using Microsoft.AspNetCore.Mvc;
using REST.Models;
using REST.Services;

namespace REST.Controllers;

[ApiController]
[Route("category")]
public class CategoryController(CategoryService categoryService) : ControllerBase
{
    private readonly CategoryService _categoryService = categoryService;

    [HttpGet("{id}")]
    public ActionResult<CategoryViewModel> Get(int id)
    {
        if (id < 0)
        {
            return BadRequest("Invalid id");
        }

        var category = _categoryService.Get(id);

        if (category is null)
        {
            return NotFound();
        }

        return Ok(category);
    }

    [HttpGet]
    public ActionResult<IEnumerable<CategoryViewModel>> Get()
    {
        var categories = _categoryService.Get();

        if (categories is null)
        {
            return NotFound();
        }

        return Ok(categories);
    }

    [HttpPost]
    public ActionResult<int> Add(CategoryAddViewModel categoryView)
    {
        if (string.IsNullOrWhiteSpace(categoryView.Name))
        {
            return BadRequest("Not valid name");
        }

        var id = _categoryService.Add(categoryView);

        return Ok(id);
    }

    [HttpPut]
    public ActionResult<CategoryViewModel> Update(CategoryUpdateViewModel categoryView)
    {
        if (categoryView.Id < 0)
        {
            return BadRequest("Invalid id.");
        }

        var category = _categoryService.Update(categoryView);

        if (category is null)
        {
            return NoContent();
        }

        return Ok(category);
    }

    [HttpDelete("{id}")]
    public ActionResult<int> Delete(int id)
    {
        if (id < 0)
        {
            return BadRequest("Invalid id");
        }

        var categoryId = _categoryService.Delete(id);

        if (categoryId is null)
        {
            return NoContent();
        }

        return Ok();
    }
}
