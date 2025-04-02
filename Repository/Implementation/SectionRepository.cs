using Entity.Data;
using Entity.ViewModel;
using Microsoft.EntityFrameworkCore;
using Repository.Interface;

namespace Repository.Implementation;

public class SectionRepository : ISectionRepository
{
    private readonly PizzaShopContext _context;

    public SectionRepository(PizzaShopContext context)
    {
        _context = context;
    }

    public List<SectionViewModel> GetAllSectionsAsync()
    {
        var sections = _context.Sections.Where(s => s.IsDeleted == false)
        .Select(s => new SectionViewModel
        {
            Id = s.Id,
            Name = s.Name,
            Description = s.Description,
            IsDeleted = s.IsDeleted,
            CreatedBy = s.CreatedBy,
            CreatedAt = DateTime.UtcNow,
            ModifiedBy = s.ModifiedBy,
            ModifiedAt = DateTime.UtcNow
        })
        .OrderBy(s => s.Name).ToList();
        return sections;
    }
    public async Task<bool> IsSectionNameUniqueAsync(string name, int? sectionId = null)
    {
        name = name.Trim().ToLower();
        if (sectionId.HasValue)
        {
            return !await _context.Sections.AnyAsync(s => s.Name.ToLower() == name && s.Id != sectionId.Value && s.IsDeleted == false);
        }
        else
        {
            return !await _context.Sections.AnyAsync(s => s.Name.ToLower() == name && s.IsDeleted == false);
        }
    }
    public Section GetSectionById(int sectionId)
    {
        return _context.Sections.FirstOrDefault(s => s.Id == sectionId);
    }

    public async Task<bool> AddSectionAsync(Section section)
    {
        try
        {
            await _context.Sections.AddAsync(section);
            return await _context.SaveChangesAsync() > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception Message: " + ex.Message);
            if (ex.InnerException != null)
            {
                Console.WriteLine("Inner Exception: " + ex.InnerException.Message);
            }
            return false;
        }
    }

    public async Task<bool> UpdateSectionAsync(Section section)
    {
        try
        {
            _context.Sections.Update(section);
            return await _context.SaveChangesAsync() > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception Message: " + ex.Message);
            if (ex.InnerException != null)
            {
                Console.WriteLine("Inner Exception: " + ex.InnerException.Message);
            }
            return false;
        }
    }
    public async Task<string> DeleteSectionAsync(int id, bool softDelete, int userId)
    {
        var section = GetSectionById(id);
        if (section != null)
        {
            var tables = await _context.Tables.Where(t => t.SectionId == id && t.IsDeleted == false).ToListAsync();
            foreach (var table in tables)
            {
                if (table.IsAvailable == false)
                {
                    return "table is occupied";
                }
                table.IsDeleted = true;
                _context.Tables.Update(table);
            }

            section.IsDeleted = true;
            section.ModifiedBy = userId;
            section.ModifiedAt = DateTime.UtcNow;
            _context.Sections.Update(section);
            await _context.SaveChangesAsync();
            return "success";
        }
        else
        {
            return "section not found";
        }
    }
}
