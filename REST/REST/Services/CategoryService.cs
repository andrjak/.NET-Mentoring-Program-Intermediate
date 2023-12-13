using REST.Data;
using REST.Models;

namespace REST.Services;

public class CategoryService(ApplicationContext context)
{
    private readonly ApplicationContext _context = context;

    public CategoryViewModel? Get(int id)
    {
        return _context.Categories
            .Where(x => x.Id == id)
            .Select(x => new CategoryViewModel
            {
                Id = x.Id,
                Name = x.Name,
            })
            .FirstOrDefault();
    }

    public IEnumerable<CategoryViewModel>? Get()
    {
        return _context.Categories
            .Select(x => new CategoryViewModel
            {
                Id = x.Id,
                Name = x.Name,
            });
    }

    public int Add(CategoryAddViewModel categoryView)
    {
        Category category = new()
        {
            Name = categoryView.Name,
        };

        _context.Categories.Add(category);
        _context.SaveChanges();

        return category.Id;
    }

    public CategoryViewModel? Update(CategoryUpdateViewModel categoryView)
    {
        var category = _context.Categories
            .Where(x => x.Id == categoryView.Id)
            .FirstOrDefault();

        if (category is null)
        {
            return null;
        }

        category.Name = categoryView.Name;
        _context.Update(category);
        _context.SaveChanges();

        return new CategoryViewModel
        {
            Id = categoryView.Id,
            Name = categoryView.Name,
        };
    }

    public int? Delete(int id)
    {
        var category = _context.Categories
            .Where(x => x.Id == id)
            .FirstOrDefault();

        if (category is null)
        {
            return null;
        }

        var items = _context.Items
            .Where(x => x.CategoryId == category.Id)
            .ToArray();

        _context.Items.RemoveRange(items);
        _context.Categories.Remove(category);
        _context.SaveChanges();

        return category.Id;
    }
}
