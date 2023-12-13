using Microsoft.EntityFrameworkCore;
using REST.Data;
using REST.Models;
using System.Linq;

namespace REST.Services;

public class ItemService(ApplicationContext context)
{
    private readonly ApplicationContext _context = context;

    public IEnumerable<ItemViewModel>? Get(int categoryId, int pageSize, int pageNumber)
    {
        return _context.Items
            .Where(x => x.CategoryId == categoryId)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new ItemViewModel
            {
                Id = x.Id,
                Name = x.Name,
            });
    }

    public int? Add(ItemAddViewModel itemView)
    {
        if (!_context.Categories.Any(x => x.Id == itemView.CategoryId))
        {
            return null;
        }

        Item item = new()
        {
            Name = itemView.Name,
            CategoryId = itemView.CategoryId,
        };

        _context.Items.Add(item);
        _context.SaveChanges();

        return item.Id;
    }

    public ItemViewModel? Update(ItemUpdateViewModel itemView)
    {
        var item = _context.Items
            .Where(x => x.Id == itemView.Id)
            .FirstOrDefault();

        if (item is null)
        {
            return null;
        }

        item.Name = itemView.Name;
        item.CategoryId = itemView.CategoryId;

        _context.Update(item);
        _context.SaveChanges();

        return new ItemViewModel
        {
            Id = itemView.Id,
            Name = itemView.Name,
        };
    }

    public int? Delete(int id)
    {
        var item = _context.Items
            .Where(x => x.Id == id)
            .FirstOrDefault();

        if (item is null)
        {
            return null;
        }

        _context.Remove(item);
        _context.SaveChanges();

        return item.Id;
    }
}
