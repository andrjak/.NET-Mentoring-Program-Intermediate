using Microsoft.AspNetCore.Mvc;
using REST.Data;
using REST.Models;
using REST.Services;

namespace REST.Controllers;

[ApiController]
[Route("item")]
public class ItemController(ItemService itemService) : ControllerBase
{
    private readonly ItemService _itemService = itemService;

    [HttpGet("{categoryId}")]
    public ActionResult<IEnumerable<ItemViewModel>> Get(int categoryId, [FromQuery] int pageNumber, [FromQuery] int pageSize)
    {
        if (categoryId < 0 || pageSize < 1 || pageNumber < 1)
        {
            return BadRequest("Invalid query.");
        }

        var items = _itemService.Get(categoryId, pageSize, pageNumber);

        if (items is null)
        {
            NoContent();
        }

        return Ok(items);
    }

    [HttpPost]
    public ActionResult<int> Add(ItemAddViewModel request)
    {
        var id = _itemService.Add(request);

        if (!id.HasValue) 
        {
            return BadRequest("Category not found");
        }

        return Ok(id.Value);
    }

    [HttpPut]
    public ActionResult<ItemViewModel> Update(ItemUpdateViewModel itemView)
    {
        if (itemView.Id < 0)
        {
            return BadRequest("Invalid id");
        }

        var item = _itemService.Update(itemView);

        if (item is null)
        {
            return NoContent();
        }

        return Ok(item);
    }

    [HttpDelete("{id}")]
    public ActionResult<int> Delete(int id)
    {
        if (id < 0)
        {
            return BadRequest("Invalid id.");
        }

        var itemId = _itemService.Delete(id);

        if (itemId is null)
        {
            return NoContent();
        }

        return Ok(itemId);
    }
}
